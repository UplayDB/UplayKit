using Google.Protobuf;
using Uplay.Utility;

namespace UplayKit.Services;

public class UtilityService
{
    #region Base
    private DemuxSocket socket;
    public UtilityService(DemuxSocket demuxSocket)
    {
        socket = demuxSocket;
        Console.WriteLine("UtilityService is Ready");
    }
    #endregion
    #region Request
    public Rsp? SendRequest(Req req)
    {
        Upstream post = new() { Request = req };
        var ServiceRequest = new Uplay.Demux.Req
        {
            ServiceRequest = new()
            {
                Service = "utility_service",
                Data = ByteString.CopyFrom(post.ToByteArray())
            },
            RequestId = socket.RequestId
        };
        socket.RequestId++;

        var rsp = socket.SendReq(ServiceRequest);
        if (rsp == null || !rsp.ServiceRsp.HasData)
            return null;

        var ds = Formatters.FormatDataNoLength<Downstream>(rsp.ServiceRsp.Data.ToByteArray());
        if (ds != null || ds?.Response != null)
        {
            return ds.Response;
        }
        return null;
    }
    #endregion
    #region Function
    public GeoIpRsp GetGeoIp()
    {
        Req req = new()
        {
            GeoipReq = new() { }
        };
        var rsp = SendRequest(req);
        if (rsp != null)
        {
            return rsp.GeoipRsp;
        }
        else
        {
            return new() { ContinentCode = "", CountryCode = "" };
        }
    }
    #endregion
}
