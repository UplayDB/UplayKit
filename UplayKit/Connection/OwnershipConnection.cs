using Google.Protobuf;
using Uplay.Ownership;

namespace UplayKit.Connection
{
    public class OwnershipConnection
    {
        #region Base
        private uint connectionId;
        private DemuxSocket socket;
        public bool isServiceSuccess = false;
        public bool isConnectionClosed = false;
        public static readonly string ServiceName = "ownership_service";
        public event EventHandler<Push> PushEvent;
        private uint ReqId { get; set; } = 1;
        public OwnershipConnection(DemuxSocket demuxSocket)
        {
            socket = demuxSocket;

            Connect();
        }

        public void Reconnect()
        {
            if (isConnectionClosed)
                Connect();
        }
        internal void Connect()
        {
            var openConnectionReq = new Uplay.Demux.Req
            {
                RequestId = socket.RequestId,
                OpenConnectionReq = new()
                {
                    ServiceName = ServiceName
                }
            };
            socket.RequestId++;
            var rsp = socket.SendReq(openConnectionReq);
            if (rsp == null)
            {
                Console.WriteLine("Ownership Connection cancelled.");
                Close();
            }
            else
            {
                isServiceSuccess = rsp.OpenConnectionRsp.Success;
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (isServiceSuccess == true)
                {
                    Console.WriteLine("Ownership Connection successful");
                    socket.AddToObj(connectionId, this);
                    socket.AddToDict(connectionId, ServiceName);
                    socket.NewMessage += Socket_NewMessage;
                }
            }
        }

        public void Close()
        {
            if (socket.TerminateConnectionId == connectionId)
            {
                Console.WriteLine($"Connection terminated via Socket {ServiceName}");
            }
            socket.RemoveConnection(connectionId);
            isServiceSuccess = false;
            connectionId = uint.MaxValue;
            isConnectionClosed = true;
            socket.NewMessage -= Socket_NewMessage;
        }
        #endregion
        #region Request/Message        
        private void Socket_NewMessage(object? sender, DemuxEventArgs e)
        {
            if (e.Data.ConnectionId == connectionId)
            {
                var down = Formatters.FormatData<Downstream>(e.Data.Data.ToArray());
                if (down != null && down.Push != null)
                {
                    Debug.WriteDebug(down.Push.ToString(), "ownership_push.txt");
                    PushEvent?.Invoke(this, down.Push);
                }
            }
        }
        public Rsp? SendRequest(Req req)
        {
            if (isConnectionClosed)
                return null;

            Upstream post = new() { Request = req };
            Uplay.Demux.Upstream up = new()
            {
                Push = new()
                {
                    Data = new()
                    {
                        ConnectionId = connectionId,
                        Data = ByteString.CopyFrom(Formatters.FormatUpstream(post.ToByteArray()))
                    }
                }
            };

            var down = socket.SendUpstream(up);
            if (isConnectionClosed || down == null || !down.Push.Data.HasData)
                return null;

            var ds = Formatters.FormatData<Downstream>(down.Push.Data.Data.ToByteArray());
            Debug.WriteDebug(ds.ToString(), "ownership.txt");

            if (ds != null || ds?.Response != null)
                return ds.Response;
            return null;
        }
        #endregion
        #region Functions
        /// <summary>
        /// Initializer
        /// </summary>
        /// <returns></returns>
        public List<OwnedGame>? GetOwnedGames(bool writeToFile = false, List<InitializeReq.Types.ProductBranchData> branchdatas = null)
        {
            if (branchdatas == null)
            {
                branchdatas = new();
            }

            Req initializeReqOwnership = new()
            {
                RequestId = ReqId,
                InitializeReq = new()
                {
                    GetAssociations = true,
                    ProtoVersion = 7,
                    UseStaging = socket.TestConfig,
                    Branches = { branchdatas }
                }
            };
            ReqId += 1;
            var initializeRspOwnership = SendRequest(initializeReqOwnership);
            if (initializeRspOwnership != null)
            {
                isServiceSuccess = initializeRspOwnership.InitializeRsp.Success;

                if (writeToFile)
                {
                    File.WriteAllText("Ownership", initializeRspOwnership.InitializeRsp.ToString());
                }

                return initializeRspOwnership.InitializeRsp.OwnedGames.OwnedGames_.ToList();
            }
            else
            {
                isServiceSuccess = false;
                return null;
            }
        }

        public string GetOwnershipToken(uint productId)
        {
            Req ownershipTokenReq = new()
            {
                RequestId = ReqId,
                OwnershipTokenReq = new()
                {
                    ProductId = productId
                }
            };
            ReqId += 1;
            var ownershipTokenRps = SendRequest(ownershipTokenReq);
            if (ownershipTokenRps != null)
            {
                isServiceSuccess = ownershipTokenRps.OwnershipTokenRsp.Success;
                return ownershipTokenRps.OwnershipTokenRsp.Token;
            }
            else
            {
                isServiceSuccess = false;
                return "";
            }
        }

        public string GetUplayPCTicket(uint productId)
        {
            Req ownershipTokenReq = new()
            {
                RequestId = ReqId,
                GetUplayPcTicketReq = new()
                {
                    UplayId = productId
                }
            };
            ReqId += 1;
            var ownershipTokenRps = SendRequest(ownershipTokenReq);
            if (ownershipTokenRps != null)
            {
                isServiceSuccess = ownershipTokenRps.GetUplayPcTicketRsp.Success;
                return ownershipTokenRps.GetUplayPcTicketRsp.UplayPcTicket;
            }
            else
            {
                isServiceSuccess = false;
                return "";
            }
        }

        public RegisterOwnershipRsp? RegisterOwnership(uint productId, string cdkey)
        {
            Req ownershipTokenReq = new()
            {
                RequestId = ReqId,
                RegisterOwnershipReq = new()
                {
                    ProductId = productId,
                    CdKey = cdkey
                }
            };
            ReqId += 1;
            var ownershipTokenRps = SendRequest(ownershipTokenReq);
            if (ownershipTokenRps != null)
            {
                isServiceSuccess = (ownershipTokenRps.RegisterOwnershipRsp.Result == RegisterOwnershipRsp.Types.Result.Success);
                return ownershipTokenRps.RegisterOwnershipRsp;
            }
            else
            {
                isServiceSuccess = false;
                return null;
            }
        }

        public string GetProductConfig(uint productId)
        {
            Req appconfig = new()
            {
                RequestId = ReqId,
                GetProductConfigReq = new()
                {
                    ProductId = productId,
                    DeprecatedTestConfig = socket.TestConfig
                }
            };
            ReqId += 1;
            var ownershipTokenRps = SendRequest(appconfig);
            if (ownershipTokenRps != null)
            {
                isServiceSuccess = (ownershipTokenRps.GetProductConfigRsp.Result == GetProductConfigRsp.Types.Result.Success);
                return ownershipTokenRps.GetProductConfigRsp.Configuration;
            }
            else
            {
                isServiceSuccess = false;
                return "";
            }
        }
        #endregion
    }
}
