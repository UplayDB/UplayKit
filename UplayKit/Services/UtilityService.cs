using Uplay.Utility;

namespace UplayKit.Services;

public class UtilityService(DemuxSocket demuxSocket) : CustomService("utility_service", demuxSocket)
{
    #region Function
    public GeoIpRsp GetGeoIp()
    {
        Req req = new()
        {
            GeoipReq = new() { }
        };
        var rsp = SendRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return new() { ContinentCode = "", CountryCode = "" };
        return rsp.Response.GeoipRsp;
    }
    #endregion
}
