using Google.Protobuf;
using System.Net.Sockets;
using Uplay.Demux;
using NetCoreServer;
using System.Net;
using System.Security.Authentication;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace UplayKit
{
    #region DemuxEventArgs
    public class DemuxEventArgs : EventArgs
    {
        public DemuxEventArgs(DataMessage data)
        {
            Data = data;
        }
        public DataMessage Data { get; set; }
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
        public uint ClientVersion { get; internal set; } = 11124;
        public bool TestConfig { get; set; } = false;
        public uint TerminateConnectionId { get; set; } = 0;
        /// <summary>
        /// Connection Dictionary for the Service Names.
        /// </summary>
        public Dictionary<uint, string> ConnectionDict = new();
        /// <summary>
        /// Connection Dictionary for the whole Connection (this)
        /// </summary>
        public Dictionary<uint, object> ConnectionObject = new();
        bool IsWaitingData;
        byte[]? InternalReaded = null;
        uint LengthRemaining = 0;
        bool LengthWaiting;
        byte[]? ReadedOnRec = null;
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
            if (Debug.isDebug)
            {
                Directory.CreateDirectory("DebugConnections");
                Directory.CreateDirectory("SendReqRsp");
                Directory.CreateDirectory("SendUpDownstream");
                Context.CertificateValidationCallback = (object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true;
            }
            NewMessage += DemuxSocket_NewMessage;
            Connect();
            ReceiveAsync();
            Debug.PWDebug("DemuxSocket Connected? "  + this.IsConnected);
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            Debug.WriteDebug("\nOnReceived!");
            Debug.WriteDebug($"Buffer L: {buffer.Length} | Offset : {offset} | Size: {size}");
            Debug.WriteDebug($"STATS: Bytes Sent: {BytesSent} | Bytes sending: {BytesSending} | Bytes Pending {BytesPending} | Bytes Recieved {BytesReceived}");

            byte[] buff = buffer.Take((int)size).ToArray();

            // check here if we wants the connection.
            if (!IsWaitingData)
            {
                //  We are not waiting any data! Therefore must be a push!
                //  First byte is a Push byte!
                if (buff[0] == 0x12)
                {
                    var downstream = Formatters.FormatDataNoLength<Downstream>(buff);
                    CheckTheConnection(downstream);
                    if (downstream != null && downstream.Push.Data != null)
                        NewMessage?.Invoke(this, new DemuxEventArgs(downstream.Push.Data));
                    if (downstream != null && downstream.Push.KeepAlive != null)
                        this.KeepAlivePush();
                }
                else
                {
                    Debug.PWDebug($"Unknown byte! {buff[0]}");
                    //Debug.PWDebug($"Unknown byte! {_InternalReaded[0]}   ", "ERROR");
                    File.WriteAllBytes("Error_Received_Bytes", buffer);
                }
            }

            if (!LengthWaiting)
            {
                var length_to_read = Formatters.FormatLength(BitConverter.ToUInt32(buff[..4], 0));
                Debug.WriteDebug($"Should Read Length of {length_to_read}");
                var data = buff.Skip(4).ToArray();

                // The length is same as our data length
                if (data.Length == length_to_read)
                {
                    InternalReaded = data;
                    ReceiveAsync();
                    return;
                }
                LengthRemaining = length_to_read - (uint)data.Length;
                ReadedOnRec = data;
                LengthWaiting = true;
                Debug.WriteDebug($"More to read! {LengthRemaining}");
                ReceiveAsync();
                return;
            }

            // if length is same as remaining we just return with it
            if (buff.Length == LengthRemaining)
            {
                InternalReaded = ReadedOnRec?.Concat(buff).ToArray();
                LengthWaiting = false;
                LengthRemaining = 0;
                ReceiveAsync();
                return;
            }
            else
            {
                // ...

                Debug.WriteDebug($"Remaining Currently: {LengthRemaining}");
                var remain_buff = buff.Take((int)LengthRemaining).ToArray();
                ReadedOnRec = ReadedOnRec?.Concat(remain_buff).ToArray();
                var lasted = buff.Skip((int)LengthRemaining).ToArray();
                LengthRemaining -= (uint)remain_buff.Length;
                Debug.WriteDebug($"Remaining to read: {LengthRemaining}");        
                if (lasted.Length > 0)
                {
                    File.WriteAllBytes($"RemainingBytes_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", lasted);
                    Debug.WriteDebug("REMAININGBYTES: " + lasted.Length);
                }
                
                ReceiveAsync();
                return;
            }
        }

        protected override void OnDisconnected()
        {
            NewMessage -= DemuxSocket_NewMessage;
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine("ERROR: " + error);
        }

        protected override void OnDisconnecting()
        {
            Debug.PWDebug("DISCONNECTING!!");
        }
        #endregion
        #region Event
        private void DemuxSocket_NewMessage(object? sender, DemuxEventArgs e)
        {
            Debug.WriteDebug(e.Data.ToString(), "NewMessage.txt");
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
            Debug.PWDebug($"Connection added {ConnectionName} as ID {connectionID}");
        }

        /// <summary>
        /// Adding to ConnectionObject
        /// </summary>
        /// <param name="connectionID">Id</param>
        /// <param name="ConnectionObj">this</param>
        public void AddToObj(uint connectionID, object ConnectionObj)
        {
            ConnectionObject.Add(connectionID, ConnectionObj);
            Debug.PWDebug($"Connection added {ConnectionObj} as ID {connectionID}");
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
            Debug.PWDebug(connectionID);
            Debug.PWDebug($"Connection Terminated ID: {connectionID}, Reason: {errorCode}");
            if (connectionID == 0)
            {
                Disconnect();
                return;
            }
            var CloseMethod = ConnectionObject[connectionID].GetType().GetMethod("Close");
            if (CloseMethod != null)
            {
                CloseMethod.Invoke(ConnectionObject[connectionID], null);
            }
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
            Debug.WriteText(req.ToString(), $"SendReqRsp/{req.RequestId}_req.txt");
            Upstream post = new() { Request = req };
            var up = Formatters.FormatUpstream(post.ToByteArray());
            IsWaitingData = true;
            long sentBytes = Send(up);
            if (sentBytes == up.Length)
            {
                while (InternalReaded == null)
                {
                    Thread.Sleep(WaitInTimeMS);
                }
                IsWaitingData = false;
                var downstream = Formatters.FormatDataNoLength<Downstream>(InternalReaded);
                InternalReaded = null;
                if (downstream?.Response != null && !CheckTheConnection(downstream))
                {
                    Debug.WriteText(downstream.Response.ToString(), $"SendReqRsp/{req.RequestId}_rsp.txt");
                    return downstream.Response;
                }
            }
            IsWaitingData = false;
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
            Debug.WriteText(up.ToString(), $"SendUpDownstream/{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_up.txt");
            var upbytes = Formatters.FormatUpstream(up.ToByteArray());
            Debug.PWDebug("[SendUpstream] We sent our Upstream!");
            long sentBytes = Send(upbytes);
            IsWaitingData = true;
            if (sentBytes == upbytes.Length)
            {
                while (InternalReaded == null)
                {
                    Thread.Sleep(WaitInTimeMS);
                }
                IsWaitingData = false;
                var downstream = Formatters.FormatDataNoLength<Downstream>(InternalReaded);
                InternalReaded = null;
                if (downstream != null && !CheckTheConnection(downstream))
                {
                    
                    Debug.WriteText(downstream.ToString(), $"SendUpDownstream/{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_down.txt");
                    return downstream;
                }
            }
            IsWaitingData = false;
            return null;
        }

        /// <summary>
        /// Sending Bytes Request
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public byte[]? SendBytes(byte[] post)
        {
            long sentBytes = Send(Formatters.FormatUpstream(post));
            IsWaitingData = true;
            if (sentBytes == post.Length)
            {
                while (InternalReaded == null)
                {
                    Thread.Sleep(WaitInTimeMS);
                }
                IsWaitingData = false;
                var returner = InternalReaded;
                InternalReaded = null;
                return returner;
            }
            IsWaitingData = false;
            return null;
        }

        /// <summary>
        /// Send push to the socket.
        /// </summary>
        /// <param name="push">Pushed object</param>
        public void SendPush(Push push)
        {
            try
            {
                Upstream up = new() { Push = push };
                Send(Formatters.FormatUpstream(up.ToByteArray()));
                Debug.PWDebug("Write was successful!");
            }
            catch (Exception ex)
            {
                InternalEx.WriteEx(ex);
            }
        }

        public bool CheckTheConnection(Downstream? downstream)
        {
            if (downstream?.Push != null)
            {
                if (downstream.Push?.ConnectionClosed != null)
                {
                    if (downstream.Push.ConnectionClosed.HasConnectionId)
                    {
                        Console.WriteLine("Connection closed");
                        TerminateConnection(downstream.Push.ConnectionClosed.ConnectionId, downstream.Push.ConnectionClosed.ErrorCode);
                        return true;
                    }
                    if (downstream.Push.ClientOutdated != null)
                    {
                        Console.WriteLine("Your Client is Outdated!");
                        TerminateConnection(0);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
