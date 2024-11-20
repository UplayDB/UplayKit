using Google.Protobuf;
using Uplay.CloudsaveService;

namespace UplayKit.Connection;

public class CloudSaveConnection
{
    #region Base
    public uint connectionId;
    public DemuxSocket socket;
    public bool IsConnectionClosed = false;
    public static readonly string ServiceName = "cloudsave_service";
    private uint ReqId { get; set; } = 1;
    public CloudSaveConnection(DemuxSocket demuxSocket)
    {
        socket = demuxSocket;
        Connect();
    }

    public void Reconnect()
    {
        if (IsConnectionClosed)
            Connect();
    }
    internal void Connect()
    {
        var openConnectionReq = new Uplay.Demux.Req
        {
            OpenConnectionReq = new()
            {
                ServiceName = ServiceName
            },
            RequestId = socket.RequestId
        };
        socket.RequestId++;
        var rsp = socket.SendReq(openConnectionReq);
        if (rsp == null)
        {
            Console.WriteLine("CloudSave Connection cancelled.");
            Close();
        }
        else
        {
            connectionId = rsp.OpenConnectionRsp.ConnectionId;
            if (rsp.OpenConnectionRsp.Success)
            {
                Console.WriteLine("CloudSave Connection successful");
                socket.AddToObj(connectionId, this);
                socket.AddToDict(connectionId, ServiceName);
                IsConnectionClosed = false;
            }
        }
    }

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

        Debug.WriteDebug(req.ToString(), "DebugConnections/cloudsave_req.txt");
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
            Debug.WriteDebug(ds.ToString(), "DebugConnections/cloudsave_rsp.txt");
            return ds.Response;
        }
        return null;
    }
    #endregion
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
        var userinforsp = SendRequest(getuserinfo);
        if (userinforsp != null)
        {
            return userinforsp.CloudsaveRsp;
        }
        else
        {
            return new() { Status = CloudsaveRsp.Types.Status.InternalError };
        }
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
        var userinforsp = SendRequest(getuserinfo);
        if (userinforsp != null)
        {
            return userinforsp.CloudsaveRsp;
        }
        else
        {
            return new() { Status = CloudsaveRsp.Types.Status.InternalError };
        }
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
        var userinforsp = SendRequest(getuserinfo);
        if (userinforsp != null)
        {
            return userinforsp.CloudsaveUrlRsp;
        }
        else
        {
            return new() { Status = CloudsaveUrlRsp.Types.Status.InternalError };
        }
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
        var userinforsp = SendRequest(getuserinfo);
        if (userinforsp != null)
        {
            return userinforsp.CloudsaveUrlRsp;
        }
        else
        {
            return new() { Status = CloudsaveUrlRsp.Types.Status.InternalError };
        }
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
        var userinforsp = SendRequest(getuserinfo);
        if (userinforsp != null)
        {
            return userinforsp.CloudsaveRspV2;
        }
        else
        {
            return new() { Status = CloudsaveRspV2.Types.Status.InternalError };
        }
    }
    #endregion
}
