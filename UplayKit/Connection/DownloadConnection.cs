using Google.Protobuf;
using Uplay.DownloadService;

namespace UplayKit.Connection
{
    public class DownloadConnection
    {
        #region Base
        private uint connectionId;
        private DemuxSocket socket;
        public bool IsConnectionClosed = false;
        public bool InitDone = false;   //  These only exist if you need auth or something to can call other things.
        public static readonly string ServiceName = "download_service";
        public string NetworkId = string.Empty;
        private uint ReqId { get; set; } = 1;
        public DownloadConnection(DemuxSocket demuxSocket)
        {
            socket = demuxSocket;

            Connect();
        }
        /// <summary>
        /// Reconnect the DownloadConnection
        /// </summary>
        public void Reconnect()
        {
            if (IsConnectionClosed)
                Connect();
        }
        internal void Connect()
        {
            var openConnectionReq = new Uplay.Demux.Req
            {
                RequestId = socket.RequestId,
                OpenConnectionReq = new()
                {
                    ServiceName = ServiceName
                }
            };
            socket.RequestId++;
            var rsp = socket.SendReq(openConnectionReq);
            if (rsp == null)
            {
                Console.WriteLine("Download Connection cancelled.");
                Close();
            }
            else
            {
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (rsp.OpenConnectionRsp.Success)
                {
                    Console.WriteLine("Download Connection successful.");
                    socket.AddToObj(connectionId, this);
                    socket.AddToDict(connectionId, ServiceName);
                    IsConnectionClosed = false;
                }
            }
        }
        /// <summary>
        /// Closing DownloadConnection
        /// </summary>
        public void Close()
        {
            if (socket.TerminateConnectionId == connectionId)
            {
                Console.WriteLine($"Connection terminated via Socket {ServiceName}");
            }
            socket.RemoveConnection(connectionId);
            connectionId = uint.MaxValue;
            IsConnectionClosed = true;
        }
        #endregion
        #region Request
        public Rsp? SendRequest(Req req)
        {
            if (IsConnectionClosed)
                return null;
            Debug.WriteDebug(req.ToString(), "DebugConnections/download_req.txt");
            Upstream post = new() { Request = req };
            Uplay.Demux.Upstream up = new()
            {
                Push = new()
                {
                    Data = new()
                    {
                        ConnectionId = connectionId,
                        Data = ByteString.CopyFrom(Formatters.FormatUpstream(post.ToByteArray()))
                    }
                }
            };

            var down = socket.SendUpstream(up);
            if (IsConnectionClosed || down == null || !down.Push.Data.HasData)
                return null;

            var ds = Formatters.FormatData<Downstream>(down.Push.Data.Data.ToByteArray());

            if (ds != null || ds?.Response != null)
            {
                Debug.WriteDebug(ds.ToString(), "DebugConnections/download_rsp.txt");
                return ds.Response;
            }
            return null;
        }
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
            var initializeRspDownload = SendRequest(initializeReqDownload);
            if (initializeRspDownload != null)
            {
                InitDone = true;
                return initializeRspDownload.InitializeRsp.Ok;
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
            var initializeRspDownload = SendRequest(initializeReqDownload);
            if (initializeRspDownload != null)
            {
                InitDone = true;
                return initializeRspDownload.InitializeRsp.Ok;
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
        public string GetUrl(string manifest, uint productId, string manifestType = "manifest")
        {
            if (!InitDone)
                return string.Empty;

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
            var urlRsp = SendRequest(urlReq);
            if (urlRsp != null)
            {
                if (UrlRsp.Types.Result.Success == urlRsp.UrlRsp.UrlResponses[0].Result && urlRsp.UrlRsp.UrlResponses[0].DownloadUrls.Count > 0)
                    return urlRsp.UrlRsp.UrlResponses[0].DownloadUrls[0].Urls[0].ToString();
            }
            return string.Empty;
        }

        public List<string> GetUrlList(uint productId, List<string> ToRelPath)
        {
            if (!InitDone)
                return new();

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
            var downloadUrls = SendRequest(urlReq);
            if (downloadUrls != null)
            {
                if (UrlRsp.Types.Result.Success == downloadUrls.UrlRsp.UrlResponses[0].Result && downloadUrls.UrlRsp.UrlResponses[0].DownloadUrls.Count > 0)
                    return downloadUrls.UrlRsp.UrlResponses[0].DownloadUrls.ToList().Select(a => a.Urls[0]).ToList();
            }
            return new();
        }
        #endregion
    }
}
