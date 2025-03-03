using Uplay.DownloadService;

namespace UplayKit.Connection;

public class DownloadConnection(DemuxSocket demuxSocket) : CustomConnection("download_service", demuxSocket)
{
    #region Base
    public bool InitDone { get; private set; } = false;   //  These only exist if you need auth or something to can call other things.
    public string NetworkId = string.Empty;
    #endregion
    #region Functions
    public bool InitDownloadToken(string ownershipToken)
    {
        Req initializeReqDownload = new()
        {
            InitializeReq = new()
            {
                OwnershipToken = ownershipToken,
                NetworkId = NetworkId
            },
            RequestId = ReqId
        };
        ReqId++;
        var initializeRspDownload = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = initializeReqDownload });
        if (initializeRspDownload != null)
        {
            InitDone = true;
            return initializeRspDownload.Response.InitializeRsp.Ok;
        }
        else
        {
            InitDone = false;
            return false;
        }
    }

    public bool InitDownloadSignature(string signature, uint branchId, ulong exp, uint productId)
    {
        Req initializeReqDownload = new()
        {
            InitializeReq = new()
            {
                Signature = signature,
                BranchId = branchId,
                Expiration = exp,
                ProductId = productId,
                NetworkId = NetworkId
            },
            RequestId = ReqId
        };
        ReqId++;
        var initializeRspDownload = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = initializeReqDownload });
        if (initializeRspDownload != null)
        {
            InitDone = true;
            return initializeRspDownload.Response.InitializeRsp.Ok;
        }
        else
        {
            InitDone = false;
            return false;
        }
    }

    /// <summary>
    /// Getting url for the ManifestType (Dont use on fileChunks!)
    /// </summary>
    /// <param name="manifest">Manifest Id</param>
    /// <param name="productId">Product Id</param>
    /// <param name="manifestType">manifest,metadata,licenses</param>
    /// <returns>Urls or ""</returns>
    public List<string> GetUrl(string manifest, uint productId, string manifestType = "manifest")
    {
        if (!InitDone)
            return [];

        Req urlReq = new()
        {
            RequestId = ReqId,
            UrlReq = new()
            {
                UrlRequests =
                {
                    new UrlReq.Types.Request()
                    {
                        ProductId = productId,
                        RelativeFilePath =
                        {
                                $"manifests/{manifest}.{manifestType}"
                        }
                    }
                }
            }
        };
        ReqId++;
        var urlRsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = urlReq });
        if (urlRsp != null)
        {
            if (UrlRsp.Types.Result.Success == urlRsp.Response.UrlRsp.UrlResponses[0].Result && urlRsp.Response.UrlRsp.UrlResponses[0].DownloadUrls.Count > 0)
                return [.. urlRsp.Response.UrlRsp.UrlResponses[0].DownloadUrls[0].Urls];
        }
        return [];
    }

    public List<UrlRsp.Types.DownloadUrls> GetUrlList(uint productId, List<string> ToRelPath)
    {
        if (!InitDone)
            return [];

        Req urlReq = new()
        {
            RequestId = ReqId,
            UrlReq = new()
            {
                UrlRequests =
                {
                    new UrlReq.Types.Request()
                    {
                        ProductId = productId,
                        RelativeFilePath = { ToRelPath }
                    }
                }
            }
        };
        ReqId++;
        var downloadUrls = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = urlReq });
        if (downloadUrls != null)
        {
            if (UrlRsp.Types.Result.Success == downloadUrls.Response.UrlRsp.UrlResponses[0].Result && downloadUrls.Response.UrlRsp.UrlResponses[0].DownloadUrls.Count > 0)
                return [.. downloadUrls.Response.UrlRsp.UrlResponses[0].DownloadUrls];
        }
        return [];
    }
    #endregion
}
