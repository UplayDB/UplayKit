using Google.Protobuf;
using Uplay.Pcbang;

namespace UplayKit.Connection;

public class PCBangConnection(DemuxSocket demuxSocket) : CustomConnection("pcbang_service", demuxSocket)
{
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.InitializeRsp;
    }
    #endregion
}
