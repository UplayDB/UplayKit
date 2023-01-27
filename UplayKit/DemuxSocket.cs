using Google.Protobuf;
using System.Net.Security;
using System.Net.Sockets;
using Uplay.Demux;

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
    public class DemuxSocket
    {
        #region Fields
        public event EventHandler<DemuxEventArgs> NewMessage;

        public SslStream sslStream;
        public TcpClient tcpClient;
        public NetworkStream network;
        public uint RequestId { get; internal set; }
        public static string ConnectionHost { get; internal set; } = "dmx.upc.ubisoft.com";
        public static int ConnectionPort { get; internal set; } = 443;
        public uint ClientVersion { get; internal set; } = 10799;
        public bool TestConfig { get; set; } = false;
        public uint TerminateConnectionId { get; set; } = 0;
        public bool IsClosed { get; internal set; } = false;
        /// <summary>
        /// Connection Dictionary for the Service Names.
        /// </summary>
        public Dictionary<uint, string> ConnectionDict = new();
        /// <summary>
        /// Connection Dictionary for the whole Connection (this)
        /// </summary>
        public Dictionary<uint, object> ConnectionObject = new();

        public bool StopTheCheck = false;
        private bool PauseRead = false;
        #endregion
        #region Basic
        public DemuxSocket(bool UseLocal = false)
        {
            try
            {
                if (UseLocal)
                {
                    ConnectionHost = "dmx.local.upc.ubisoft.com";
                    ConnectionPort = 6000;
                }
                tcpClient = new TcpClient();
                tcpClient.Connect(ConnectionHost, ConnectionPort);
                sslStream = new SslStream(tcpClient.GetStream());
                SslClientAuthenticationOptions sslClientAuthenticationOptions = new()
                {
                    TargetHost = ConnectionHost
                };
                sslStream.AuthenticateAsClient(sslClientAuthenticationOptions);
                RequestId = 1;
                network = tcpClient.GetStream();
                new Thread(TDataCheck).Start();
                NewMessage += DemuxSocket_NewMessage;
                Debug.PWDebug("[DemuxSocket] Started.");
            }
            catch (Exception ex)
            { 
                InternalEx.WriteEx(ex);
                Console.WriteLine("Connection to demux has been failed!");
            }

        }

        /// <summary>
        /// Closing the Connection safely
        /// </summary>
        public void Close()
        {
            if (IsClosed)
                return;

            try
            {
                sslStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                sslStream.Dispose();
                sslStream.Close();
                network.Dispose();
                network.Close();
                tcpClient.Dispose();
                tcpClient.Close();
                RequestId = 0;
                IsClosed = true;
                NewMessage -= DemuxSocket_NewMessage;
                Debug.PWDebug("[DemuxSocket] Closed.");
            }
            catch (Exception ex)
            {
                InternalEx.WriteEx(ex);
            }
        }
        #endregion
        #region Event
        private void TDataCheck(object? obj)
        {
            while (!IsClosed && network != null)
            {
                try
                {
                    if (PauseRead == false && StopTheCheck == false)
                    {
                        // Thanks for SteamRE [TcpConnection.cs]
                        // waiting 1 ms and reading again
                        if (network.Socket.Poll(100000, SelectMode.SelectRead) && network.DataAvailable)
                        {
                            byte[] buffer = new byte[4];
                            sslStream.Read(buffer, 0, 4);
                            uint responseLength = Formatters.FormatLength(BitConverter.ToUInt32(buffer, 0));
                            if (responseLength == 0)
                            {
                                Debug.PWDebug("Response Length is 0!");
                                return;
                            }
                            if (responseLength > 0)
                            {
                                buffer = new byte[responseLength];
                                sslStream.Read(buffer, 0, (int)responseLength);
                            }
                            //Clearing buffer
                            //sslStream.Flush();
                            var downstream = Formatters.FormatDataNoLength<Downstream>(buffer);
                            Debug.WriteDebug(downstream.ToString(), "DownStream.txt");
                            if (downstream.Push.ClientOutdated != null)
                            {
                                Console.WriteLine("Your Client is Outdated!");
                                TerminateConnection(0);
                            }
                            if (downstream.Push.ConnectionClosed != null)
                            {
                                TerminateConnection(downstream.Push.ConnectionClosed.ConnectionId, downstream.Push.ConnectionClosed.ErrorCode);
                            }
                            if (downstream.Push.Data != null)
                            {
                                NewMessage?.Invoke(this, new(downstream.Push.Data));
                            }
                            if (downstream.Response!.HasRequestId)
                            {
                                Console.WriteLine("Sadly we read the Response :(!");
                            }
                        }
                    }
                }
                catch (Exception ex) { InternalEx.WriteEx(ex); }
            }

        }
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
                Close();
                return;
            }
            var CloseMethod = ConnectionObject[connectionID].GetType().GetMethod("Close");
            if (CloseMethod != null)
            {
                CloseMethod.Invoke(ConnectionObject[connectionID], null);
            }
        }

        #endregion
        #region Communicating with Current Demux
        /// <summary>
        /// Checking if the Current version is same as the Latest
        /// </summary>
        /// <returns>True or False</returns>
        public bool VersionCheck()
        {
            RequestId++;
            Req patchreq = new()
            {
                GetPatchInfoReq = new()
                {
                    PatchTrackId = "DEFAULT",
                    TestConfig = TestConfig,
                    TrackType = 0
                },
                RequestId = RequestId
            };
            var patchrsp = SendReq(patchreq);
            if (patchrsp != null)
            {
                return (ClientVersion == patchrsp.GetPatchInfoRsp.LatestVersion);
            }
            return false;
        }

        /// <summary>
        /// Initially pushing the Current version
        /// </summary>
        public void PushVersion()
        {
            Push versionPush = new()
            {
                ClientVersion = new()
                {
                    Version = ClientVersion
                }
            };

            SendPush(versionPush);
        }

        /// <summary>
        /// Trying to Authenticate with UbiTicket
        /// </summary>
        /// <param name="token">Ubi Ticket</param>
        /// <returns>True or False</returns>
        public bool Authenticate(string token)
        {
            RequestId++;
            Req authReq = new()
            {
                RequestId = RequestId,
                AuthenticateReq = new()
                {
                    ClientId = "uplay_pc",
                    SendKeepAlive = false,
                    Token = new()
                    {
                        UbiTicket = token
                    }
                }
            };
            var authRsp = SendReq(authReq);
            if (authRsp == null)
            {
                return false;
            }
            return authRsp.AuthenticateRsp.Success;
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
            try
            {
                Upstream post = new() { Request = req };

                byte[] buffer = new byte[4];
                uint responseLength;
                sslStream.Write(Formatters.FormatUpstream(post.ToByteArray()));
                Debug.WriteDebug(post.ToString(), "DemuxReq.txt");
                PauseRead = true;
                while (true)
                {
                    if (network.DataAvailable)
                    {
                        sslStream.Read(buffer, 0, 4);
                        responseLength = Formatters.FormatLength(BitConverter.ToUInt32(buffer, 0));
                        Debug.PWDebug("[SendReq] Response Length: " + responseLength);
                        if (responseLength == 0) { throw new Exception("Response Length from Demux is 0, something is not right"); }
                        if (responseLength > 0)
                        {
                            buffer = new byte[responseLength];
                            sslStream.Read(buffer, 0, (int)responseLength);
                            break;
                        }
                    }
                }
                PauseRead = false;
                Debug.PWDebug("[SendReq] Final Response Length: " + responseLength);
                //Fail save!
                if (responseLength == 0)
                {
                    Debug.PWDebug("RSP 0???");
                    return null;
                }


                var downstream = Formatters.FormatDataNoLength<Downstream>(buffer);
                Debug.WriteDebug(downstream.ToString(), "SendReq.txt");
                if (downstream?.Push != null)
                {
                    Debug.WriteDebug(downstream.Push.ToString(), "DemuxPush.txt");
                    if (downstream.Push.ClientOutdated != null)
                    {
                        Console.WriteLine("Your Client is Outdated!");
                        TerminateConnection(0);
                    }
                    if (downstream.Push.ConnectionClosed != null)
                    {
                        TerminateConnection(downstream.Push.ConnectionClosed.ConnectionId, downstream.Push.ConnectionClosed.ErrorCode);
                    }
                }
                if (downstream?.Response != null)
                {
                    Debug.WriteDebug(downstream.Response.ToString(), "DemuxRsp.txt");
                    return downstream.Response;
                }
                return null;
            }
            catch (Exception ex)
            {
                InternalEx.WriteEx(ex);
                return null;
            }
        }

        /// <summary>
        /// Sending Request outside of Demux
        /// </summary>
        /// <param name="upstream">Any Upstream</param>
        /// <returns>Any Downstream or Null</returns>
        /// <exception cref="Exception">If the Response Length from Response is 0</exception>
        public Downstream? SendUpstream(Upstream upstream)
        {
            try
            {
                byte[] buffer = new byte[4];
                uint responseLength;
                sslStream.Write(Formatters.FormatUpstream(upstream.ToByteArray()));
                Debug.WriteDebug(upstream.ToString(), "DemuxUp.txt");
                PauseRead = true;
                while (true)
                {
                    sslStream.Read(buffer, 0, 4);
                    responseLength = Formatters.FormatLength(BitConverter.ToUInt32(buffer, 0));
                    Debug.PWDebug("[SendUpstream] Response Length: " + responseLength);
                    if (responseLength == 0) { throw new Exception("Response Length from Demux is 0, something is not right"); }
                    if (responseLength > 0)
                    {
                        buffer = new byte[responseLength];
                        for (int i = 0; i < responseLength; i++)
                        {
                            sslStream.Read(buffer, i, 1);
                        }
                        break;
                    }
                }
                PauseRead = false;
                sslStream.Flush();
                Debug.PWDebug("[SendUpstream] Final Response Length: " + responseLength);
                //Fail save!
                if (responseLength == 0)
                    return null;

                var downstream = Formatters.FormatDataNoLength<Downstream>(buffer);
                Debug.WriteDebug(downstream.ToString(), "DemuxDown.txt");
                if (downstream?.Push != null)
                {
                    if (downstream.Push?.ConnectionClosed != null)
                    {
                        if (downstream.Push.ConnectionClosed.HasConnectionId)
                        {
                            Console.WriteLine("Connection closed");
                            TerminateConnection(downstream.Push.ConnectionClosed.ConnectionId, downstream.Push.ConnectionClosed.ErrorCode);
                        }
                    }
                }
                return downstream;
            }
            catch (Exception ex)
            {
                InternalEx.WriteEx(ex);
                return null;
            }
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
                sslStream.Write(Formatters.FormatUpstream(up.ToByteArray()));
                sslStream.Flush();
                Debug.PWDebug("Write was successful!");
            }
            catch (Exception ex)
            {
                InternalEx.WriteEx(ex);
            }
        }
        #endregion
    }
}
