using Google.Protobuf;
using System.Text.Json;
using Uplay.Ownership;

namespace UplayKit.Connection;

public class OwnershipConnection
{
    #region Base
    private uint connectionId;
    private readonly DemuxSocket socket;
    public bool IsConnectionClosed = false;
    public static readonly string ServiceName = "ownership_service";
    public event EventHandler<Push>? PushEvent;
    public string Ticket;
    public string SessionId;
    private uint ReqId { get; set; } = 1;
    public OwnershipConnection(DemuxSocket demuxSocket, string ticket, string sessionId)
    {
        socket = demuxSocket;
        Ticket = ticket;
        SessionId = sessionId;
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
            Console.WriteLine("Ownership Connection cancelled.");
            Close();
        }
        else
        {
            connectionId = rsp.OpenConnectionRsp.ConnectionId;
            if (rsp.OpenConnectionRsp.Success)
            {
                Console.WriteLine("Ownership Connection successful");
                socket.AddToObj(connectionId, this);
                socket.AddToDict(connectionId, ServiceName);
                socket.NewMessage += Socket_NewMessage;
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
        socket.NewMessage -= Socket_NewMessage;
    }

    #endregion
    #region Request/Message        
    private void Socket_NewMessage(object? sender, DemuxEventArgs e)
    {
        if (e.Data.ConnectionId == connectionId)
        {
            var down = Formatters.FormatData<Downstream>(e.Data.Data.ToArray());
            if (down != null && down.Push != null)
            {
                Debug.WriteDebug(down.Push.ToString(), "ownership_push.txt");
                PushEvent?.Invoke(this, down.Push);
            }
        }
    }
    public Rsp? SendRequest(Req req)
    {
        if (IsConnectionClosed)
            return null;
        Debug.WriteDebug(req.ToString(), "DebugConnections/ownership_req.txt");
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
            Debug.WriteDebug(ds.ToString(), "DebugConnections/ownership_rsp.txt");
            return ds.Response;
        }            
        return null;
    }
    #endregion
    #region Functions
    public InitializeRsp? Initialize()
    {
        Req req = new()
        {
            RequestId = ReqId,
            InitializeReq = new()
            {
                GetAssociations = true,
                ProtoVersion = 7,
                UseStaging = socket.TestConfig
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.InitializeRsp;
        return null;
    }

    /// <summary>
    /// Initializer and getting our games.
    /// </summary>
    /// <param name="writeToFile">Should write "Ownership" as proto and "Ownership.json" as parsed json file</param>
    /// <returns></returns>
    public List<OwnedGame> GetOwnedGames(bool writeToFile = false)
    {
        var rsp = Initialize();
        if (rsp != null)
        {
            if (writeToFile)
            {
                File.WriteAllText("Ownership.json", JsonSerializer.Serialize(rsp, new JsonSerializerOptions() { WriteIndented = true }));
                MemoryStream ms = new();
                rsp.WriteTo(ms);
                File.WriteAllBytes("Ownership", ms.ToArray());
            }
            //  This only exist here to ensure there is NO crash when someone doesnt own anything.
            if (rsp.OwnedGames == null)
                return new();
            if (rsp.OwnedGames.OwnedGames_ == null)
                return new();
            return rsp.OwnedGames.OwnedGames_.ToList();
        }
        return new();
    }

    public (string, ulong) GetOwnershipToken(uint productId)
    {
        Req req = new()
        {
            RequestId = ReqId,
            OwnershipTokenReq = new()
            {
                ProductId = productId
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null && rsp.OwnershipTokenRsp.Success)
        {
            return (rsp.OwnershipTokenRsp.Token, rsp.OwnershipTokenRsp.Expiration);
        }
        return (string.Empty, ulong.MinValue);
    }

    [Obsolete("Ubisoft no longer using this")]
    public RegisterTemporaryOwnershipRsp? RegisterTempOwnershipToken(string token)
    {
        Req req = new()
        {
            RequestId = ReqId,
            RegisterTemporaryOwnershipReq = new()
            {
                Token = token
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.RegisterTemporaryOwnershipRsp;
        return null;
    }

    public ConsumeOwnershipRsp? ConsumeOwnership(uint productId, uint Quantity, string TransactionId, uint GameProductId, string Signature)
    {
        Req req = new()
        {
            RequestId = ReqId,
            ConsumeOwnershipReq = new()
            {
                ProductId = productId,
                Quantity = Quantity,
                TransactionId = TransactionId,
                GameProductId = GameProductId,
                Signature = Signature
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.ConsumeOwnershipRsp;
        return null;
    }

    public UnlockProductBranchRsp? UnlockProductBranch(uint productId, string password)
    {
        Req req = new()
        {
            RequestId = ReqId,
            UnlockProductBranchReq = new()
            {
                Branch = new()
                {
                    ProductId = productId,
                    Password = password
                }
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.UnlockProductBranchRsp;
        return null;
    }

    public GetUplayPCTicketRsp? GetUplayPCTicket(uint productId, GetUplayPCTicketReq.Types.Platform platform = GetUplayPCTicketReq.Types.Platform.Normal)
    {
        Req req = new()
        {
            RequestId = ReqId,
            GetUplayPcTicketReq = new()
            {
                UplayId = productId,
                Platform = platform
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.GetUplayPcTicketRsp;
        return null;
    }

    public ClaimKeystorageKeyRsp? ClaimKeystorageKeys(List<uint> productIds)
    {
        Req req = new()
        {
            RequestId = ReqId,
            ClaimKeystorageKeyReq = new()
            {
                ProductIds = { productIds }
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.ClaimKeystorageKeyRsp;
        return null;
    }

    public RegisterOwnershipRsp? RegisterOwnership(uint productId, string cdkey)
    {
        Req req = new()
        {
            RequestId = ReqId,
            RegisterOwnershipReq = new()
            {
                ProductId = productId,
                CdKey = cdkey
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.RegisterOwnershipRsp;
        return null;
    }

    public RegisterOwnershipRsp? RegisterOwnershipByCdKey(string cdkey)
    {
        Req req = new()
        {
            RequestId = ReqId,
            RegisterOwnershipByCdKeyReq = new()
            {
                CdKey = cdkey
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.RegisterOwnershipRsp;
        return null;
    }

    public DeprecatedGetProductFromCdKeyRsp? DeprecatedGetProductFromCdKey(string cdkey)
    {
        Req req = new()
        {
            RequestId = ReqId,
            DeprecatedGetProductFromCdKeyReq = new()
            {
                CdKey = cdkey
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.DeprecatedGetProductFromCdKeyRsp;
        return null;
    }

    public string GetProductConfig(uint productId)
    {
        Req req = new()
        {
            RequestId = ReqId,
            GetProductConfigReq = new()
            {
                ProductId = productId,
                DeprecatedTestConfig = socket.TestConfig
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null && rsp.GetProductConfigRsp.Result == GetProductConfigRsp.Types.Result.Success)
            return rsp.GetProductConfigRsp.Configuration;
        return string.Empty;
    }

    public SwitchProductBranchRsp? SwitchProductBranch(uint productId, uint branchId, string? password)
    {
        Req req = new()
        {
            RequestId = ReqId,
            SwitchProductBranchReq = new()
            {
               SpecifiedBranch = new()
               {
                   ProductId = productId,
                   BranchId = branchId,
               },
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        if (password != null)
            req.SwitchProductBranchReq.SpecifiedBranch.Password = password;
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.SwitchProductBranchRsp;
        return null;
    }

    public SwitchProductBranchRsp? SwitchBranchToDefault(uint productId)
    {
        Req req = new()
        {
            RequestId = ReqId,
            SwitchProductBranchReq = new()
            {
                DefaultBranch = new()
                {
                    ProductId = productId,
                },
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.SwitchProductBranchRsp;
        return null;
    }


    public SignOwnershipRsp? SignOwnership(uint productId)
    {
        Req req = new()
        {
            RequestId = ReqId,
            SignOwnershipReq = new()
            {
                ProductId = productId
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.SignOwnershipRsp;
        return null;
    }


    public RegisterOwnershipRsp? RegisterOwnership(uint productId, string cdKey, RegisterOwnershipReq.Types.ActivationMode activationMode = RegisterOwnershipReq.Types.ActivationMode.None)
    {
        Req req = new()
        {
            RequestId = ReqId,
            RegisterOwnershipReq = new()
            {
                ProductId = productId,
                CdKey = cdKey,
                ActivationMode = activationMode,
            },
            UbiSessionId = SessionId,
            UbiTicket = Ticket
        };
        ReqId += 1;
        var rsp = SendRequest(req);
        if (rsp != null)
            return rsp.RegisterOwnershipRsp;
        return null;
    }
    #endregion
}
