using Google.Protobuf;
using Uplay.Friends;

namespace UplayKit.Connection
{
    public class FriendsConnection
    {
        #region Base
        public uint connectionId;
        public DemuxSocket socket;
        public bool IsConnectionClosed = false;
        public List<Friend> Friends;
        public List<Friend> Friends_Send;
        public List<Friend> Friends_Received;
        public static readonly string ServiceName = "friends_service";
        public event EventHandler<Push>? PushEvent;
        private uint ReqId { get; set; } = 1;
        public FriendsConnection(DemuxSocket demuxSocket)
        {
            socket = demuxSocket;
            Friends = new();
            Friends_Send = new();
            Friends_Received = new();
            Connect();
        }

        public void Reconnect()
        {
            if (IsConnectionClosed)
                Connect();
        }
        internal void Connect()
        {
            Friends = new();
            Friends_Send = new();
            Friends_Received = new();
            var openConnectionReq = new Uplay.Demux.Req
            {
                OpenConnectionReq = new()
                {
                    ServiceName = ServiceName
                },
                RequestId = socket.RequestId,
            };
            socket.RequestId++;
            var rsp = socket.SendReq(openConnectionReq);
            if (rsp == null)
            {
                Console.WriteLine("Friends Connection cancelled.");
                Close();
            }
            else
            {
                connectionId = rsp.OpenConnectionRsp.ConnectionId;
                if (rsp.OpenConnectionRsp.Success)
                {
                    Console.WriteLine("Friends Connection successful");
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
        public Rsp? SendRequest(Req req)
        {
            if (IsConnectionClosed)
                return null;

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
                Debug.WriteDebug(ds.ToString(), "friends.txt");
                return ds.Response;
            }
            return null;
        }

        private void Socket_NewMessage(object? sender, DemuxEventArgs e)
        {
            if (e.Data.ConnectionId == connectionId)
            {
                var down = Formatters.FormatData<Downstream>(e.Data.Data.ToArray());
                if (down != null && down.Push != null)
                {
                    Debug.WriteDebug(down.Push.ToString(), "friends_push.txt");
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
            var rsp = SendRequest(req);
            if (rsp != null)
            {
                Console.WriteLine("Init rsp: " + rsp.InitializeRsp.Success);
                foreach (var rel in rsp.InitializeRsp.Relationship)
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
                return rsp.InitializeRsp.Success;
            }
            return false;
        }

        public void AcceptAll()
        {
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
                var rsp = SendRequest(req);
                if (rsp != null)
                {
                    Console.WriteLine(friend.NameOnPlatform + " Is Accepted? " + rsp.AcceptFriendshipRsp.Ok);
                }
            }
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
            var rsp = SendRequest(req);
            if (rsp != null)
            {
                return rsp.SetActivityStatusRsp.Success;
            }
            return false;
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
            var rsp = SendRequest(req);
            if (rsp != null)
            {
                return rsp.SetGameRsp.Success;
            }
            return false;
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
            var rsp = SendRequest(req);
            if (rsp != null)
            {
                return rsp.SetGameRsp.Success;
            }
            return false;
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

            var rsp = SendRequest(req);
            if (rsp != null)
            {
                return rsp.SetRichPresenceRsp.Success;
            }
            return false;
        }
        #endregion
    }
}
