using Google.Protobuf;
using System.Net.Sockets;
using Uplay.Demux;
using NetCoreServer;
using System.Net;
using System.Security.Authentication;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Buffers;

namespace UplayKit;

#region DemuxEventArgs
public class DemuxEventArgs(DataMessage data) : EventArgs
{
    public DataMessage Data { get; set; } = data;
}
#endregion
public class DemuxSocket : SslClient
{
    #region Fields
    public event EventHandler<DemuxEventArgs>? NewMessage;
    public uint RequestId { get; internal set; }
    public static string ConnectionHost { get; internal set; } = "dmx.upc.ubisoft.com";
    public static int ConnectionPort { get; internal set; } = 443;
    public int WaitInTimeMS = 10;
    public const uint ClientVersion = 11646;
    public bool TestConfig { get; set; } = false;
    public uint TerminateConnectionId { get; internal set; } = 0;
    /// <summary>
    /// Connection Dictionary for the Service Names.
    /// </summary>
    public Dictionary<uint, string> ConnectionDict = [];
    /// <summary>
    /// Connection Dictionary for the whole Connection (this)
    /// </summary>
    public Dictionary<uint, object> ConnectionObject = [];

    private bool IsUserRequest;
    private ArraySegment<byte> Buffer;
    //private ArraySegment<byte> NonWaitingBuffer; // currently unused, soon
    private bool IsWaitingForMore;
    private bool IsBufferReady;
    private uint WaitingLen;
    private readonly object Locker = new();
    #endregion
    #region Basic

    public DemuxSocket() : base(new SslContext(SslProtocols.Tls12), new DnsEndPoint(ConnectionHost, ConnectionPort, AddressFamily.InterNetwork))
    {
        Start();
    }

    public DemuxSocket(SslContext context) : base(context, new DnsEndPoint(ConnectionHost, ConnectionPort, AddressFamily.InterNetwork))
    {
        Start();
    }
    public void Start()
    {
        // These made for Getting our receive size bigget
        this.OptionSendBufferLimit = 8192 * 4;
        this.OptionSendBufferSize = 8192 * 4;
        this.OptionReceiveBufferLimit = 8192 * 4;
        this.OptionReceiveBufferSize = 8192 * 4;
        if (Logs.File_Log_Switch.MinimumLevel <= Serilog.Events.LogEventLevel.Debug)
        {
            Logs.File_Log_Switch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
            Context.CertificateValidationCallback = (object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true;
        }
        NewMessage += DemuxSocket_NewMessage;
        Connect();
        ReceiveAsync();
        Logs.MixedLogger.Information("DemuxSocket Connected? {IsConnected}", this.IsConnected);
    }
    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        Logs.FileLogger.Verbose("OnReceived!");
        Logs.FileLogger.Verbose($"STATS: Bytes Sent: {BytesSent} | Bytes sending: {BytesSending} | Bytes Pending {BytesPending} | Bytes Recieved {BytesReceived}");
        Logs.FileLogger.Verbose($"Buffer L: {buffer.Length} | Offset : {offset} | Size: {size}");

        ArraySegment<byte> buff = buffer.Take((int)size).ToArray();

        // check here if we wants the connection.
        if (!IsUserRequest)
        {
            //  We are not waiting any data! Therefore must be a push!
            //  First byte is a Push byte!
            if (buff[0] == 0x12)
            {
                NonUserRequest(Formatters.FormatDataNoLength<Downstream>(buff));

            }
            else if (buff.Count > 4) // this means we have a length
            {
                NonUserRequest(Formatters.FormatData<Downstream>(buff));

                Logs.FileLogger.Verbose("Remaining buffer count: " + buff.Count);
                File.WriteAllBytes("Error_Remaining_Buff", [.. buff]);
                File.WriteAllBytes("Error_Remaining_Bytes", buffer);
            }
            else
            {
                Logs.FileLogger.Verbose($"Unknown byte! {buff[0]}");
                //Debug.PWDebug($"Unknown byte! {_InternalReaded[0]}   ", "ERROR");
                File.WriteAllBytes("Error_Received_Buff", [.. buff]);
                File.WriteAllBytes("Error_Received_Bytes", buffer);
            }
        }

        if (!IsWaitingForMore)
        {
            var length_to_read = Formatters.FormatLength(BitConverter.ToUInt32(buff.Slice(0, 4)));
            Logs.FileLogger.Verbose($"Should Read Length of {length_to_read}");
            Buffer = buff.Slice(4);
            if (Buffer.Count == length_to_read)
            {
                IsBufferReady = true;
                IsWaitingForMore = false;
                ReceiveAsync();
                return;
            }
            IsWaitingForMore = true;
            WaitingLen = length_to_read - (uint)Buffer.Count;
            Logs.FileLogger.Verbose($"More to read! {WaitingLen}");
            ReceiveAsync();
            return;
        }

        if (buff.Count == WaitingLen)
        {
            Buffer = Buffer.ToArray().Concat(buff).ToArray();
            IsBufferReady = true;
            IsWaitingForMore = false;
            WaitingLen = 0;
            ReceiveAsync();
            return;
        }
        else
        {
            Logs.FileLogger.Verbose($"Waiting Len: {WaitingLen}");
            var remain_buff = buff.Take((int)WaitingLen).ToArray();
            Buffer = Buffer.ToArray().Concat(remain_buff).ToArray();
            var lasted = buff.Skip((int)WaitingLen).ToArray();
            WaitingLen -= (uint)remain_buff.Length;
            Logs.FileLogger.Verbose($"Waiting to read: {WaitingLen}");
            if (lasted.Length > 0)
            {
                File.WriteAllBytes($"RemainingBytes_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", lasted);
                Logs.FileLogger.Verbose("REMAININGBYTES: " + lasted.Length);
            }

            ReceiveAsync();
            return;
        }
    }


    private void NonUserRequest(Downstream? downstream)
    {
        CheckTheConnection(downstream);
        if (downstream != null && downstream.Push.Data != null)
            NewMessage?.Invoke(this, new DemuxEventArgs(downstream.Push.Data));
        if (downstream != null && downstream.Push.KeepAlive != null)
            this.KeepAlivePush();
    }

    protected override void OnDisconnected()
    {
        NewMessage -= DemuxSocket_NewMessage;
    }

    protected override void OnError(SocketError error)
    {
        Logs.MixedLogger.Error("ERROR: " + error);
    }

    protected override void OnDisconnecting()
    {
        Logs.MixedLogger.Information("DISCONNECTING!");
    }
    #endregion
    #region Event
    private void DemuxSocket_NewMessage(object? sender, DemuxEventArgs e)
    {
        Logs.FileLogger.Verbose("New Message! {Data}",e.Data.ToString());
    }
    #endregion
    #region Connection Handling
    /// <summary>
    /// Adding to ConnectionDict
    /// </summary>
    /// <param name="connectionID">Id</param>
    /// <param name="ConnectionName">Name</param>
    public void AddToDict(uint connectionID, string ConnectionName)
    {
        ConnectionDict.Add(connectionID, ConnectionName);
        Logs.MixedLogger.Verbose("Connection added {ConnectionName} as ID {ConnectionId}", ConnectionName, connectionID);
    }

    /// <summary>
    /// Adding to ConnectionObject
    /// </summary>
    /// <param name="connectionID">Id</param>
    /// <param name="ConnectionObj">this</param>
    public void AddToObj(uint connectionID, object ConnectionObj)
    {
        ConnectionObject.Add(connectionID, ConnectionObj);
        Logs.MixedLogger.Verbose("Connection added {ConnectionObj} as ID {ConnectionId}", ConnectionObj, connectionID);
    }

    /// <summary>
    /// Remove Connection from Dictionaries
    /// </summary>
    /// <param name="connectionID">ID to remove</param>
    public void RemoveConnection(uint connectionID)
    {
        ConnectionDict.Remove(connectionID);
        ConnectionObject.Remove(connectionID);
    }

    /// <summary>
    /// Terminating connection (Closing) from the Dictionaries
    /// </summary>
    /// <param name="connectionID">Connection Id</param>
    /// <param name="errorCode">Error Code</param>
    public void TerminateConnection(uint connectionID, ConnectionClosedPush.Types.Connection_ErrorCode errorCode = ConnectionClosedPush.Types.Connection_ErrorCode.ConnectionForceQuit)
    {
        TerminateConnectionId = connectionID;
        Logs.MixedLogger.Information("Connection Terminated ID: {connectionID}, Reason: {errorCode}", connectionID, errorCode);
        if (connectionID == 0)
        {
            Disconnect();
            return;
        }
        ConnectionObject[connectionID].GetType().GetMethod("Close")?.Invoke(ConnectionObject[connectionID], null);
    }
    #endregion
    #region Ssl Communication with Demux

    /// <summary>
    /// Sending Request in Demux
    /// </summary>
    /// <param name="req">Demux Req</param>
    /// <returns>Demux Rsp</returns>
    /// <exception cref="Exception">If the Response Length from Response is 0</exception>
    public Rsp? SendReq(Req req)
    {
        Logs.FileLogger.Verbose("Request: {Req}",req.ToString());
        Upstream post = new() { Request = req };
        Logs.MixedLogger.Verbose("Sending Request!");
        var downstream = SendUpstream(post);
        if (downstream != null && downstream.Response != null && !CheckTheConnection(downstream))
        {
            Logs.FileLogger.Verbose("Response: {Rsp}", downstream.Response.ToString());
            return downstream.Response;
        }
        return null;
    }

    /// <summary>
    /// Sending Request outside of Demux
    /// </summary>
    /// <param name="upstream">Any Upstream</param>
    /// <returns>Any Downstream or Null</returns>
    /// <exception cref="Exception">If the Response Length from Response is 0</exception>
    public Downstream? SendUpstream(Upstream up)
    {
        Logs.FileLogger.Verbose("Upstream: {up}", up.ToString());
        var upbytes = Formatters.FormatUpstream(up.ToByteArray());
        Logs.MixedLogger.Verbose("Sending Upstream!");
        var response = SendBytes(upbytes);
        if (response != null)
        {
            var downstream = Formatters.FormatDataNoLength<Downstream>(response);
            if (downstream != null && !CheckTheConnection(downstream))
            {
                Logs.FileLogger.Verbose("Downstream: {down}",downstream.ToString());
                return downstream;
            }
        }
        return null;
    }

    /// <summary>
    /// Sending Bytes Request
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    public byte[]? SendBytes(ReadOnlySpan<byte> bytes)
    {
        lock (Locker)
        {
            IsUserRequest = true;
            Logs.MixedLogger.Verbose("Sending Bytes!");
            if (Send(bytes) == bytes.Length)
            {
                Logs.MixedLogger.Verbose("Sent success!");
                while (!IsBufferReady)
                {
                    Thread.Sleep(WaitInTimeMS);
                }
                IsBufferReady = false;
                IsUserRequest = false;
                var returner = Buffer.ToArray();
                Buffer = ArraySegment<byte>.Empty;
                return returner;
            }
            Logs.MixedLogger.Verbose("We could not sent the bytes!");
            IsUserRequest = false;
            return null;
        }
    }


    /// <summary>
    /// Send push to the socket.
    /// </summary>
    /// <param name="push">Pushed object</param>
    public void SendPush(Push push)
    {
        lock (Locker)
        {
            Upstream up = new() { Push = push };
            Send(Formatters.FormatUpstream(up.ToByteArray()));
            Logs.MixedLogger.Verbose("Write was successful!");
        }
    }

    public bool CheckTheConnection(Downstream? downstream)
    {
        if (downstream == null)
            return false;
        if (downstream.Push == null)
            return false;
        if (downstream.Push.ClientOutdated != null)
        {
            Logs.MixedLogger.Verbose("Your Client is Outdated!");
            TerminateConnection(0);
            return true;
        }
        if (downstream.Push.ConnectionClosed != null && downstream.Push.ConnectionClosed.HasConnectionId)
        {
            Logs.MixedLogger.Verbose("Connection closed");
            TerminateConnection(downstream.Push.ConnectionClosed.ConnectionId, downstream.Push.ConnectionClosed.ErrorCode);
            return true;
        }
        return false;
    }
    #endregion
}
