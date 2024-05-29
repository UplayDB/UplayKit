using Google.Protobuf;
using Uplay.Party;

namespace UplayKit.Connection
{
    public class PartyConnection
    {
        #region Base
        public uint connectionId;
        public DemuxSocket socket;
        public bool IsConnectionClosed = false;
        public static readonly string ServiceName = "party_service";
        private uint Cookie = 0;
        public event EventHandler<Push>? PushEvent;
        private uint ReqId { get; set; } = 1;
        public PartyConnection(DemuxSocket demuxSocket)
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
                Console.WriteLine("Party Connection cancelled.");
                Close();
            }
            else
            {
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (rsp.OpenConnectionRsp.Success)
                {
                    Console.WriteLine("Party Connection successful");
                    socket.AddToObj(connectionId, this);
                    socket.AddToDict(connectionId, ServiceName);
                    socket.NewMessage += Socket_NewMessage;
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
            socket.NewMessage -= Socket_NewMessage;
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
                Debug.WriteDebug(ds.ToString(), "party.txt");
                return ds.Response;
            }  
            return null;
        }
        private void Socket_NewMessage(object? sender, DemuxEventArgs e)
        {
            if (e.Data.ConnectionId == connectionId)
            {
                var down = Formatters.FormatData<Downstream>(e.Data.Data.ToArray());
                if (down != null && down.Push != null)
                {
                    Debug.WriteDebug(down.Push.ToString(), "party_push.txt");
                    PushEvent?.Invoke(this, down.Push);
                }
            }
        }
        #endregion
        #region Functions
        public StartSessionRsp? StartSession(uint cookie = 0)
        {
            StartSessionReq session = new();

            if (cookie != 0)
            {
                session.Cookie = cookie;
            }

            Req req = new()
            {
                RequestId = ReqId,
                StartSessionReq = session
            };
            ReqId++;
            var rsp = SendRequest(req);
            if (rsp != null)
            {
                Cookie = rsp.StartSessionRsp.Cookie;
                return rsp.StartSessionRsp;
            }
            return null;
        }
        #endregion
    }
}
