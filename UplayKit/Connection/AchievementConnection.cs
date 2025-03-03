using Uplay.Uplay;

namespace UplayKit.Connection;

public class AchievementConnection(DemuxSocket demuxSocket) : CustomConnection("ach_frontend", demuxSocket)
{
    #region Functions
    public bool Auth(string token)
    {
        Req req = new()
        {
            RequestId = ReqId,
            AuthenticateReq = new()
            {
                OrbitToken = token
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Req, Rsp>(req);
        if (rsp == null)
            return false;
        return rsp.AuthenticateRsp.Success;
    }

    public List<ProductAchievements> GetAchievements(string userId, string ProductId, string PlatformId)
    {
        Req req = new()
        {
            RequestId = ReqId,
            ReadAchievementsReq = new()
            {
                UserId = userId,
                Product = new()
                {
                    ProductId = ProductId,
                    PlatformId = PlatformId
                }
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Req, Rsp>(req);
        if (rsp == null)
            return [];
        return [.. rsp.ReadAchievementsRsp.AchievementBlob.ProductAchievements];
    }

    public bool WriteAchievement(List<ProductAchievements> productAchievements)
    {
        Req req = new()
        {
            RequestId = ReqId,
            WriteAchievementsReq = new()
            {
                AchievementBlob = new()
                {
                    ProductAchievements = { productAchievements }
                }
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Req, Rsp>(req);
        return rsp != null;
    }
    #endregion
}
