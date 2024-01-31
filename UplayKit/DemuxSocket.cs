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
        public event EventHandler<DemuxEventArgs> NewMessage;
        public uint RequestId { get; internal set; }
        public static string ConnectionHost { get; internal set; } = "dmx.upc.ubisoft.com";
        public static int ConnectionPort { get; internal set; } = 443;
        public int WaitInTimeMS = 10;
        public uint ClientVersion { get; internal set; } = 11028;
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
        bool Overflow = false;
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
            if (Debug.isDebug)
            {
                Directory.CreateDirectory("SendReq");
                Directory.CreateDirectory("SendUpstream");
                Context.CertificateValidationCallback = (object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true;
            }
            NewMessage += DemuxSocket_NewMessage;
            var Connected = Connect();
            ReceiveAsync();
            Debug.PWDebug("DemuxSocket is "  + Connected);
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            Debug.WriteDebug("OnReceived!\n");
            Debug.WriteDebug($"Buffer L: {buffer.Length} | Offset : {offset} | Size: {size}");
            Debug.WriteDebug($"Bytes Sent: {BytesSent} | Bytes sending: {BytesSending} | Bytes Pending {BytesPending} | Bytes Recieved {BytesReceived}");
            byte[]? _InternalReaded;
            if (LengthWaiting)
            {
                byte[] preReadedOnRec = ReadedOnRec;
                _InternalReaded = buffer.Take((int)size).ToArray();
                /*
                if (LengthRemaining < size)
                {
                    Debug.WriteDebug("LengthRemaining < size! Parsing only the remaining!");
                    _InternalReaded = _InternalReaded.Take((int)LengthRemaining).ToArray();
                    File.WriteAllBytes("push_msg",buffer[(int)LengthRemaining..size]);
                }*/
                ReadedOnRec = ReadedOnRec.Concat(_InternalReaded).ToArray();
                if (LengthRemaining != _InternalReaded.Length)
                {
                    Debug.WriteDebug($"PRE: {LengthRemaining}");
                    uint pre = LengthRemaining;
                    LengthWaiting = true;
                    LengthRemaining -= (uint)_InternalReaded.Length;
                    Debug.WriteDebug($"POST: {LengthRemaining}");
                    if (pre < LengthRemaining)
                    {
                        Console.WriteLine((int)pre - size);
                        Console.WriteLine((int)LengthRemaining - _InternalReaded.Length);
                        Console.WriteLine(Math.Abs((int)LengthRemaining - _InternalReaded.Length));
                        Console.WriteLine((uint)Math.Abs((int)LengthRemaining - _InternalReaded.Length));
                        LengthRemaining = (uint)Math.Abs((int)LengthRemaining - _InternalReaded.Length);
                        Debug.WriteDebug("PRE bytes are BIGGER than remaining! overflow on!");
                        Debug.WriteDebug($"New: {LengthRemaining}");

                        File.WriteAllBytes("error_pre_bytes", preReadedOnRec);
                        File.WriteAllBytes("error_readed_bytes", _InternalReaded);
                        File.WriteAllBytes("error_post_bytes", ReadedOnRec);
                        File.WriteAllBytes("error_buffer_rem_size", buffer[(int)pre..(int)size]);

                        LengthWaiting = false;
                        LengthRemaining = 0;
                        _InternalReaded = ReadedOnRec;
                        ReadedOnRec = null;
                    }
                }
                else
                {
                    Debug.WriteDebug($"Finally readed!");
                    LengthWaiting = false;
                    LengthRemaining = 0;
                    _InternalReaded = ReadedOnRec;
                    ReadedOnRec = null;
                }
            }
            else
            {
                var _InternalReadedLenght = Formatters.FormatLength(BitConverter.ToUInt32(buffer[..4], 0));
                Debug.WriteDebug($"Should Read: {_InternalReadedLenght} | {BitConverter.ToString(buffer[..4])}");
                _InternalReaded = buffer.Skip(4).Take((int)_InternalReadedLenght).ToArray();
                Debug.WriteDebug($"Readed Bytes: {_InternalReaded.Length}");
                if (_InternalReadedLenght != _InternalReaded.Length)
                {
                    LengthWaiting = true;
                    LengthRemaining = _InternalReadedLenght - (uint)_InternalReaded.Length;
                    ReadedOnRec = _InternalReaded;
                }
                else
                {
                    Debug.WriteDebug("No further reading required!");
                    LengthWaiting = false;
                    LengthRemaining = 0;
                    ReadedOnRec = null;
                }
            }
            


            if (!IsWaitingData)
            { 
                //  We are not waiting any data! Therefore must be a push!
                //  First byte is a Push byte!
                if (_InternalReaded[0] == 0x12)
                {
                    var downstream = Formatters.FormatDataNoLength<Downstream>(_InternalReaded);
                    CheckTheConnection(downstream);
                    if (downstream != null && downstream.Push.Data != null)
                        NewMessage?.Invoke(this, new DemuxEventArgs(downstream.Push.Data));
                    if (downstream != null && downstream.Push.KeepAlive != null)
                        this.KeepAlivePush();
                }
                else
                {
                    Debug.PWDebug($"Unknown byte! {_InternalReaded[0]}");
                    //Debug.PWDebug($"Unknown byte! {_InternalReaded[0]}   ", "ERROR");
                    File.WriteAllBytes("Error_Received_Bytes", _InternalReaded);
                }
            }
            else if (!LengthWaiting)
            {
                InternalReaded = _InternalReaded;
            }
            ReceiveAsync();
            Debug.PWDebug("Ask to recieve sent!");
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
            Debug.WriteText(req.ToString(), $"SendReq/{req.RequestId}_req.txt");
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
                if (downstream?.Response != null)
                {
                    Debug.WriteText(downstream.Response.ToString(), $"SendReq/{req.RequestId}_rsp.txt");
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
            Debug.WriteText(up.ToString(), $"SendUpstream/{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_up.txt");
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
                if (downstream != null)
                {
                    Debug.WriteText(downstream.ToString(), $"SendUpstream/{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_down.txt");
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

        public void CheckTheConnection(Downstream? downstream)
        {
            if (downstream?.Push != null)
            {
                if (downstream.Push?.ConnectionClosed != null)
                {
                    if (downstream.Push.ConnectionClosed.HasConnectionId)
                    {
                        Console.WriteLine("Connection closed");
                        TerminateConnection(downstream.Push.ConnectionClosed.ConnectionId, downstream.Push.ConnectionClosed.ErrorCode);
                    }
                    if (downstream.Push.ClientOutdated != null)
                    {
                        Console.WriteLine("Your Client is Outdated!");
                        TerminateConnection(0);
                    }
                }
            }
        }
        #endregion
    }
}
