using Uplay.SteamService;

namespace UplayKit.Services;

public class SteamService(DemuxSocket demuxSocket) : CustomService("steam_service", demuxSocket)
{
    #region Function
    public List<SteamUserInfo> GetSteamFriends(string SteamID)
    {
        Req req = new()
        {
            GetSteamFriendsReq = new()
            {
                SteamId = SteamID
            }
        };
        var rsp = SendRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null || !rsp.Response.GetSteamFriendsRsp.Success)
            return [];
        return [..rsp.Response.GetSteamFriendsRsp.SteamFriends];
    }

    public SteamUserInfo GetSteamUserInfo(string SteamID)
    {
        Req req = new()
        {
            GetSteamUserInfoReq = new()
            {
                SteamId = SteamID
            }
        };
        var rsp = SendRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return new();
        return rsp.Response.GetSteamUserInfoRsp.SteamUserInfo;
    }
    #endregion
}
