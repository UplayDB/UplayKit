using Google.Protobuf;

namespace UplayKit.Connection
{
    public class CustomConnection
    {
        #region Base
        private uint connectionId;
        private DemuxSocket socket;
        public bool IsConnectionClosed = false;
        public string ServiceName = "";
        public uint ReqId { get; set; } = 1;
        public CustomConnection(string serviceName, DemuxSocket demuxSocket)
        {
            ServiceName = serviceName;
            socket = demuxSocket;
            Connect();
        }

        /// <summary>
        /// Reconnect the CustomConnection
        /// </summary>
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
                Console.WriteLine("Custom Connection cancelled.");
                Close();
            }
            else
            {
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (rsp.OpenConnectionRsp.Success)
                {
                    Console.WriteLine("Custom Connection successful.");
                    socket.AddToObj(connectionId, this);
                    socket.AddToDict(connectionId, ServiceName);
                    IsConnectionClosed = false;
                }
            }
        }

        /// <summary>
        /// Closing CustomConnection
        /// </summary>
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
        public V? SendPostRequest<T, V>(T post) 
            where V : IMessage<V>, new()
            where T : IMessage<T>, new()
        {
            if (IsConnectionClosed)
                return default(V);

            Debug.WriteDebug(post.ToString(), $"DebugConnections/custom_{ServiceName}_req.txt");
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
                return default(V);

            var ds = Formatters.FormatData<V>(down.Push.Data.Data.ToByteArray());

            if (ds != null)
            {
                Debug.WriteDebug(ds.ToString(), $"DebugConnections/custom_{ServiceName}_rsp.txt");
                return ds;
            }
            return default(V);
        }
        #endregion
    }
}
