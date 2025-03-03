using Google.Protobuf;
using Uplay.CloudsaveService;

namespace UplayKit.Connection;

public class CloudSaveConnection(DemuxSocket demuxSocket) : CustomConnection("cloudsave_service", demuxSocket)
{
    #region Functions
    public CloudsaveRsp List(uint uplayid, string Username, string ownershipToken)
    {
        Req getuserinfo = new()
        {
            RequestId = ReqId + 1,
            CloudsaveReq = new()
            {
                UplayId = uplayid,
                Username = Username,
                OwnershipToken = ownershipToken,
                ListItems = new()
            }
        };
        var userinforsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = getuserinfo });
        if (userinforsp == null)
            return new() { Status = CloudsaveRsp.Types.Status.InternalError };
        return userinforsp.Response.CloudsaveRsp;
    }

    public CloudsaveRsp GetItem(uint uplayid, string Username, string ownershipToken, uint ItemId)
    {
        Req getuserinfo = new()
        {
            RequestId = ReqId + 1,
            CloudsaveReq = new()
            {
                UplayId = uplayid,
                Username = Username,
                OwnershipToken = ownershipToken,
                GetItem = new()
                {
                    ItemId = ItemId
                }
            }
        };
        var userinforsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = getuserinfo });
        if (userinforsp == null)
            return new() { Status = CloudsaveRsp.Types.Status.InternalError };
        return userinforsp.Response.CloudsaveRsp;
    }

    public CloudsaveUrlRsp Send(uint uplayid, string ownershipToken, string ItemName, string md5B64, uint lenght)
    {
        Item item = new()
        {
            ItemName = ItemName,
            OptionalArgs = new()
            {
                Md5Base64 = md5B64,
                WriteLength = lenght
            }
        };
        Req getuserinfo = new()
        {
            RequestId = ReqId + 1,
            CloudsaveUrlReq = new()
            {
                UplayId = uplayid,
                OwnershipToken = ownershipToken,
                Method = CloudsaveUrlReq.Types.Method.Put,
                Items = { item }
            }
        };
        var userinforsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = getuserinfo });
        if (userinforsp == null)
            return new() { Status = CloudsaveUrlRsp.Types.Status.InternalError };
        return userinforsp.Response.CloudsaveUrlRsp;
    }

    public CloudsaveUrlRsp Get(uint uplayid, string ownershipToken, string ItemName)
    {
        Item item = new()
        {
            ItemName = ItemName
        };
        Req getuserinfo = new()
        {
            RequestId = ReqId + 1,
            CloudsaveUrlReq = new()
            {
                UplayId = uplayid,
                OwnershipToken = ownershipToken,
                Method = CloudsaveUrlReq.Types.Method.Get,
                Items = { item }
            }
        };
        var userinforsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = getuserinfo });
        if (userinforsp == null)
            return new() { Status = CloudsaveUrlRsp.Types.Status.InternalError };
        return userinforsp.Response.CloudsaveUrlRsp;
    }

    public CloudsaveRspV2 GetV2(uint uplayid, string ownershipToken, string ItemName, uint ItemId)
    {
        Req getuserinfo = new()
        {
            RequestId = ReqId + 1,
            CloudsaveReqV2 = new()
            {
                UplayId = uplayid,
                OwnershipToken = ownershipToken,
                GetItems =
                { 
                    Items =
                    {
                        new CloudsaveReqV2.Types.GetItems.Types.Item()
                        {
                            ItemId = ItemId,
                            ItemName = ItemName
                        },
                    }
                },
            }
        }; 
        var userinforsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = getuserinfo });
        if (userinforsp == null)
            return new() { Status = CloudsaveRspV2.Types.Status.InternalError };
        return userinforsp.Response.CloudsaveRspV2;
    }
    #endregion
}
