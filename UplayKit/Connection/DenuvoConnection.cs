using Google.Protobuf;
using Uplay.DenuvoService;

namespace UplayKit.Connection;

public class DenuvoConnection(DemuxSocket demuxSocket) : CustomConnection("denuvo_service", demuxSocket)
{
    public (GetGameTokenRsp response, Rsp.Types.Result result)? GetGameToken(string ownership_token, ByteString requestToken)
    {
        Downstream? downstream = SendPostRequest<Upstream, Downstream>(new Upstream()
        { 
            Request = new()
            { 
                RequestId = ReqId,
                GetGameTokenReq = new()
                {
                    OwnershipToken = ownership_token,
                    RequestToken = requestToken
                }
            }
        });
        ReqId++;
        if (downstream == null || downstream.Response == null)
            return null;
        return (downstream.Response.GetGameTokenRsp, downstream.Response.Result);
    }

    public (GetGameTimeTokenRsp response, Rsp.Types.Result result)? GetTimeToken(string ownership_token, ByteString requestToken)
    {
        Downstream? downstream = SendPostRequest<Upstream, Downstream>(new Upstream()
        {
            Request = new()
            {
                RequestId = ReqId,
                GetGameTimeTokenReq = new()
                {
                    OwnershipToken = ownership_token,
                    RequestToken = requestToken
                }
            }
        });
        ReqId++;
        if (downstream == null || downstream.Response == null)
            return null;
        return (downstream.Response.GetGameTimeTokenRsp, downstream.Response.Result);
    }
}
