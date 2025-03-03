using Uplay.Store;

namespace UplayKit.Connection;

public class StoreConnection(DemuxSocket demuxSocket) : CustomConnection("store_service", demuxSocket)
{
    #region Base
    public Storefront storefront = new();
    public event EventHandler<Push>? PushEvent;
    public override void OnConnected()
    {
        socket.NewMessage += Socket_NewMessage;
    }

    public override void OnDisconnected()
    {
        socket.NewMessage -= Socket_NewMessage;
    }
    private void Socket_NewMessage(object? sender, DemuxEventArgs e)
    {
        if (e.Data.ConnectionId == ConnectionId)
        {
            var down = Formatters.FormatData<Downstream>(e.Data.Data.ToArray());
            if (down != null && down.Push != null)
            {
                Logs.FileLogger.Verbose("Store Service Push: {push}", down.Push.ToString());
                PushEvent?.Invoke(this, down.Push);
            }
        }
    }
    #endregion
    #region Functions
    public bool Init()
    {
        Req req = new()
        {
            InitializeReq = new()
            {
                UseStaging = socket.TestConfig,
                ProtoVersion = 7
            },
            RequestId = ReqId
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
        {
            storefront = new();
            return false;
        }
        storefront = rsp.Response.InitializeRsp.Storefront;
        return rsp.Response.InitializeRsp.Success;
    }

    public GetStoreRsp? GetStore()
    {
        Req req = new()
        {
            RequestId = ReqId,
            GetStoreReq = new()
            {
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.GetStoreRsp;
    }

    public GetDataRsp? GetData(StoreType storeType, List<uint> prodIds)
    {
        Req req = new()
        {
            RequestId = ReqId,
            GetDataReq = new()
            {
                StoreDataType = storeType,
                ProductId =
                {
                    prodIds
                }
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.GetDataRsp;
    }
    #endregion
}
