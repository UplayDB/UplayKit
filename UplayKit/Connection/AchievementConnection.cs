using Google.Protobuf;
using Uplay.Uplay;

namespace UplayKit.Connection
{
    public class AchievementConnection
    {
        #region Base
        private uint connectionId;
        private DemuxSocket socket;
        public bool IsConnectionClosed = false;
        public static readonly string ServiceName = "ach_frontend";
        private uint ReqId { get; set; } = 1;
        public AchievementConnection(DemuxSocket demuxSocket)
        {
            socket = demuxSocket;
            Connect();
        }
        public void Reconnect()
        {
            if (IsConnectionClosed)
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
                Console.WriteLine("Achievement Connection cancelled.");
                Close();
            }
            else
            {
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (rsp.OpenConnectionRsp.Success)
                {
                    Console.WriteLine("Achievement Connection successful.");
                    socket.AddToObj(connectionId, this);
                    socket.AddToDict(connectionId, ServiceName);
                    IsConnectionClosed = false;
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
            connectionId = uint.MaxValue;
            IsConnectionClosed = true;
        }
        #endregion
        #region Request
        public Rsp? SendRequest(Req req)
        {
            if (IsConnectionClosed)
                return null;

            Debug.WriteDebug(req.ToString(), "DebugConnections/ach_req.txt");
            Uplay.Demux.Upstream up = new()
            {
                Push = new()
                {
                    Data = new()
                    {
                        ConnectionId = connectionId,
                        Data = ByteString.CopyFrom(Formatters.FormatUpstream(req.ToByteArray()))
                    }
                }
            };

            var down = socket.SendUpstream(up);
            if (IsConnectionClosed || down == null || !down.Push.Data.HasData)
                return null;

            var ds = Formatters.FormatData<Rsp>(down.Push.Data.Data.ToByteArray());


            if (ds != null)
            {
                Debug.WriteDebug(ds.ToString(), "DebugConnections/ach_rsp.txt");
                return ds;
            }  
            return null;
        }
        #endregion
        #region Functions
        public bool Auth(string token)
        {
            Req req = new()
            {
                RequestId = ReqId,
                AuthenticateReq = new()
                {
                    OrbitToken = token
                }
            };
            ReqId++;
            var rsp = SendRequest(req);
            if (rsp != null)
                return rsp.AuthenticateRsp.Success;
            return false;
        }

        public List<ProductAchievements> GetAchievements(string userId, string ProductId, string PlatformId)
        {
            Req req = new()
            {
                RequestId = ReqId,
                ReadAchievementsReq = new()
                {
                    UserId = userId,
                    Product = new()
                    {
                        ProductId = ProductId,
                        PlatformId = PlatformId
                    }
                }
            };
            ReqId++;
            var rsp = SendRequest(req);
            if (rsp != null)
                return rsp.ReadAchievementsRsp.AchievementBlob.ProductAchievements.ToList();
            return new();
        }

        public bool WriteAchievement(List<ProductAchievements> productAchievements)
        {
            Req req = new()
            {
                RequestId = ReqId,
                WriteAchievementsReq = new()
                {
                    AchievementBlob = new()
                    {
                        ProductAchievements = { productAchievements }
                    }
                }
            };
            ReqId++;
            var rsp = SendRequest(req);
            return rsp != null;
        }
        #endregion
    }
}
