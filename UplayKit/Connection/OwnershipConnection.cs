using Google.Protobuf;
using System.Text.Json;
using Uplay.Ownership;

namespace UplayKit.Connection;

public class OwnershipConnection(DemuxSocket demuxSocket, string ticket, string sessionId) : CustomConnection("ownership_service", demuxSocket)
{
    #region Base
    public event EventHandler<Push>? PushEvent;
    public string Ticket => ticket;
    public string SessionId => sessionId;
    public override void OnConnected()
    {
        socket.NewMessage += Socket_NewMessage;
    }

    public override void OnDisconnected()
    {
        socket.NewMessage -= Socket_NewMessage;
    }
    private void Socket_NewMessage(object? sender, DemuxEventArgs e)
    {
        if (e.Data.ConnectionId == ConnectionId)
        {
            var down = Formatters.FormatData<Downstream>(e.Data.Data.ToArray());
            if (down != null && down.Push != null)
            {
                Logs.FileLogger.Information("Ownership push: {pushMessage}", down.Push.ToString());
                PushEvent?.Invoke(this, down.Push);
            }
        }
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.InitializeRsp;
    }

    /// <summary>
    /// Initializer and getting our games.
    /// </summary>
    /// <param name="writeToFile">Should write "Ownership" as proto and "Ownership.json" as parsed json file</param>
    /// <returns></returns>
    public List<OwnedGame> GetOwnedGames(bool writeToFile = false)
    {
        var rsp = Initialize();
        if (rsp == null)
            return [];
        if (writeToFile)
        {
            File.WriteAllText("Ownership.json", JsonSerializer.Serialize(rsp, new JsonSerializerOptions() { WriteIndented = true }));
            using MemoryStream ms = new();
            rsp.WriteTo(ms);
            File.WriteAllBytes("Ownership", ms.ToArray());
        }
        //  This only exist here to ensure there is NO crash when someone doesnt own anything.
        if (rsp.OwnedGames == null)
            return [];
        if (rsp.OwnedGames.OwnedGames_ == null)
            return [];
        return [.. rsp.OwnedGames.OwnedGames_];
    }

    public (string Token, ulong Expiration) GetOwnershipToken(uint productId)
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return (string.Empty, ulong.MinValue);
        return (rsp.Response.OwnershipTokenRsp.Token, rsp.Response.OwnershipTokenRsp.Expiration);
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.RegisterTemporaryOwnershipRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.ConsumeOwnershipRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.UnlockProductBranchRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.GetUplayPcTicketRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.ClaimKeystorageKeyRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.RegisterOwnershipRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.RegisterOwnershipRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.DeprecatedGetProductFromCdKeyRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null || rsp.Response.GetProductConfigRsp.Result != GetProductConfigRsp.Types.Result.Success)
            return string.Empty;
        return rsp.Response.GetProductConfigRsp.Configuration;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.SwitchProductBranchRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.SwitchProductBranchRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.SignOwnershipRsp;
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
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.RegisterOwnershipRsp;
    }
    #endregion
}
