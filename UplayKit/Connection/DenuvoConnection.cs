using Google.Protobuf;
using Uplay.DenuvoService;

namespace UplayKit.Connection;

public class DenuvoConnection
{
    #region Base
    private uint connectionId;
    private readonly DemuxSocket socket;
    public bool IsConnectionClosed = false;
    public string ServiceName = "denuvo_service";
    public uint ReqId { get; set; } = 1;
    public DenuvoConnection(DemuxSocket demuxSocket)
    {
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
            Console.WriteLine("Denuvo Connection cancelled.");
            Close();
        }
        else
        {
            connectionId = rsp.OpenConnectionRsp.ConnectionId;
            if (rsp.OpenConnectionRsp.Success)
            {
                Console.WriteLine("Denuvo Connection successful.");
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
            return default;

        Logs.FileLogger.Verbose("Denuvo Service Post: {post}", post.ToString());
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
            return default;

        var ds = Formatters.FormatData<V>(down.Push.Data.Data.ToByteArray());

        if (ds != null)
        {
            Logs.FileLogger.Verbose("Custom Service Response: {Response}", ds.ToString());
            return ds;
        }
        return default;
    }
    #endregion

    public (GetGameTokenRsp? response, Rsp.Types.Result result)? GetGameToken(string ownership_token, string requestToken)
    {
        Downstream? downstream = SendPostRequest<Upstream, Downstream>(new Upstream()
        { 
            Request = new()
            { 
                RequestId = ReqId,
                GetGameTokenReq = new()
                {
                    OwnershipToken = ownership_token,
                    RequestToken = ByteString.FromBase64(requestToken)
                }
            }
        });
        ReqId++;
        if (downstream == null || downstream.Response == null)
            return null;
        return (downstream.Response.GetGameTokenRsp, downstream.Response.Result);
    }
}
