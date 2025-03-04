using Google.Protobuf;
using Serilog;

namespace UplayKit.Connection;

public class CustomConnection
{
    #region Base
    public uint ConnectionId { get; protected set; }
    public readonly DemuxSocket socket;
    public bool IsConnectionClosed { get; protected set; } = false;
    public string ServiceName = string.Empty;
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
        Uplay.Demux.Req openConnectionReq = new()
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
            Log.Information("{ServiceName} Connection cancelled.", ServiceName);
            Close();
        }
        else
        {
            ConnectionId = rsp.OpenConnectionRsp.ConnectionId;
            if (rsp.OpenConnectionRsp.Success)
            {
                Log.Information("{ServiceName} Connection successful.", ServiceName);
                socket.AddToObj(ConnectionId, this);
                socket.AddToDict(ConnectionId, ServiceName);
                IsConnectionClosed = false;
                OnConnected();
            }
        }
    }

    /// <summary>
    /// Closing CustomConnection
    /// </summary>
    public void Close()
    {
        if (socket.TerminateConnectionId == ConnectionId)
        {
            Log.Information("Connection terminated via Socket {ServiceName} {Id}", ServiceName, ConnectionId);
        }
        socket.RemoveConnection(ConnectionId);
        ConnectionId = uint.MaxValue;
        IsConnectionClosed = true;
        OnDisconnected();
    }
    #endregion
    #region Virtuals
    public virtual void OnConnected()
    {

    }
    public virtual void OnDisconnected()
    {

    }
    #endregion
    #region Request
    public V? SendPostRequest<T, V>(T post) 
        where V : IMessage<V>, new()
        where T : IMessage<T>, new()
    {
        if (IsConnectionClosed)
            return default;

        Log.Verbose("{ServiceName} Post: {post}", ServiceName, post.ToString());
        Uplay.Demux.Upstream up = new()
        {
            Push = new()
            {
                Data = new()
                {
                    ConnectionId = ConnectionId,
                    Data = ByteString.CopyFrom(Formatters.FormatUpstream(post.ToByteArray()))
                }
            }
        };

        Uplay.Demux.Downstream? down = socket.SendUpstream(up);
        if (IsConnectionClosed || down == null || !down.Push.Data.HasData)
            return default;

        V? ds = Formatters.FormatData<V>(down.Push.Data.Data.ToByteArray());

        if (ds == null)
            return default;
        Log.Verbose("{ServiceName} Service Response: {Response}", ServiceName, ds.ToString());
        return ds;
    }
    #endregion
}
