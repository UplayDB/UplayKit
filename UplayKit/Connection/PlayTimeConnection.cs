using Google.Protobuf;
using Uplay.Playtime;

namespace UplayKit.Connection;

public class PlayTimeConnection(DemuxSocket demuxSocket) : CustomConnection("playtime_service", demuxSocket)
{
    #region Functions
    public Result UpdatePlayTime(uint prodId, uint Seconds, bool IsActive)
    {
        Req req = new()
        {
            UpdatePlaytimeReq = new()
            {
                ProductIds = { prodId },
                SecondsToAdd = Seconds,
                IsActivePlaySession = IsActive,
                GameId = prodId

            },
            RequestId = ReqId,
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return Result.ServerError;
        return rsp.Response.UpdatePlaytimeRsp.Result;
    }

    public GetPlaytimeRsp GetPlayTime(uint prodId, string AccountId)
    {
        Req req = new()
        {
            GetPlaytimeReq = new()
            {
                AccountId = AccountId,
                ProductId = prodId
            },
            RequestId = ReqId,
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return new() { Result = Result.ServerError };
        return rsp.Response.GetPlaytimeRsp;
    }

    public GetFriendsPlaytimeRsp GetFriendsPlayTime(uint prodId, List<string> AccountIds)
    {
        Req req = new()
        {
            GetFriendsPlaytimeReq = new()
            {
                GameId = prodId,
                MyFriends = { AccountIds }
            },
            RequestId = ReqId,
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return new() { Result = Result.ServerError };
        return rsp.Response.GetFriendsPlaytimeRsp;
    }
    #endregion
}
