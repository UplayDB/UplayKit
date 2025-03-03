using Uplay.Friends;

namespace UplayKit.Connection;

public class FriendsConnection(DemuxSocket demuxSocket) : CustomConnection("friends_service", demuxSocket)
{
    #region Base
    public List<Friend> Friends = [];
    public List<Friend> Friends_Send = [];
    public List<Friend> Friends_Received = [];
    public event EventHandler<Push>? PushEvent;

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
                Logs.FileLogger.Verbose("Friends Service Push: {push}", down.Push.ToString());
                PushEvent?.Invoke(this, down.Push);
            }
        }
    }
    #endregion
    #region Functions
    public bool Auth(string Token, string SessionId, Status.Types.ActivityStatus status = Status.Types.ActivityStatus.Invisible)
    {
        Req req = new()
        {
            RequestId = ReqId,
            InitializeReq = new()
            {
                ProtoVersion = 1,
                IsStaging = false,
                Localization = "en-US",
                ActivityStatus = status,
                UbiTicket = Token,
                SessiondId = SessionId
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return false;
        foreach (var rel in rsp.Response.InitializeRsp.Relationship)
        {
            if (rel.Blacklisted)
                continue;

            switch (rel.Relation)
            {
                case Relationship.Types.Relation.Friends:
                    Friends.Add(rel.Friend);
                    break;
                case Relationship.Types.Relation.PendingReceivedInvite:
                    Friends_Received.Add(rel.Friend);
                    break;
                case Relationship.Types.Relation.PendingSentInvite:
                    Friends_Send.Add(rel.Friend);
                    break;
                case Relationship.Types.Relation.NoRelationship:
                default:
                    break;
            }
        }
        return rsp.Response.InitializeRsp.Success;
    }

    public Dictionary<string, bool> AcceptAll()
    {
        Dictionary<string, bool> accepted = [];
        foreach (var friend in Friends_Received)
        {
            Req req = new()
            {
                RequestId = ReqId,
                AcceptFriendshipReq = new()
                {
                    User = friend
                }
            };
            ReqId++;
            var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
            if (rsp != null && rsp.Response != null)
            {
                accepted.Add(friend.NameOnPlatform, rsp.Response.AcceptFriendshipRsp.Ok);
            }
        }
        return accepted;
    }

    public bool SetActivity(Status.Types.ActivityStatus status)
    {
        Req req = new()
        {
            RequestId = ReqId,
            SetActivityStatusReq = new()
            {
                ActivityStatus = status
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return false;
        return rsp.Response.SetActivityStatusRsp.Success;
    }

    public bool SetGame(uint productId, string productname)
    {
        Req req = new()
        {
            RequestId = ReqId,
            SetGameReq = new()
            {
                Game = new()
                {
                    UplayId = productId,
                    ProductName = productname
                }
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return false;
        return rsp.Response.SetGameRsp.Success;
    }
    public bool SetEmptyGame()
    {
        Req req = new()
        {
            RequestId = ReqId,
            SetGameReq = new()
            {
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return false;
        return rsp.Response.SetGameRsp.Success;
    }
    public bool SetRichPresence(uint productId, string key, string val)
    {
        Req req = new()
        {
            RequestId = ReqId,
            SetRichPresenceReq = new()
            {
                PresenceState = new()
                {
                    PresenceId = 1,
                    ProductId = productId,
                    PresenceTokens = 
                    {
                        new RichPresenceTokenPair()
                        {
                            Key = key,
                            Val = val
                        }
                    }
                }
            }
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return false;
        return rsp.Response.SetRichPresenceRsp.Success;
    }
    #endregion
}
