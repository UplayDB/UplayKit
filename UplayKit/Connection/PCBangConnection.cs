using Google.Protobuf;
using Uplay.Pcbang;

namespace UplayKit.Connection
{
    public class PCBangConnection
    {
        #region Base
        public uint connectionId;
        public DemuxSocket socket;
        public bool IsConnectionClosed = false;
        public static string ServiceName = "pcbang_service";
        private uint ReqId { get; set; } = 1;
        public PCBangConnection(DemuxSocket demuxSocket)
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
                OpenConnectionReq = new()
                {
                    ServiceName = ServiceName
                },
                RequestId = socket.RequestId
            };
            socket.RequestId++;

            var rsp = socket.SendReq(openConnectionReq);
            if (rsp == null)
            {
                Console.WriteLine("Test Connection cancelled.");
                Close();
            }
            else
            {
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (rsp.OpenConnectionRsp.Success)
                {
                    Console.WriteLine("Test Connection successful");
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
            if (IsConnectionClosed || down == null || !down.Push.Data.HasData)
                return null;

            var ds = Formatters.FormatData<Downstream>(down.Push.Data.Data.ToByteArray());

            if (ds != null || ds?.Response != null)
            {
                Debug.WriteDebug(ds.ToString(), "pcbang.txt");
                return ds.Response;
            }      
            return null;
        }
        #endregion
        #region Functions
        public InitializeRsp? Init(string provider)
        {
            Req req = new()
            {
                RequestId = ReqId,
                InitializeReq = new()
                {
                    Provider = provider
                }
            };
            ReqId++;
            var rsp = SendRequest(req);
            if (rsp != null)
            {
                return rsp.InitializeRsp;
            }
            return null;
        }
        #endregion
    }
}
