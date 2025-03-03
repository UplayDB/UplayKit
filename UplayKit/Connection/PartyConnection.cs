using Uplay.Party;

namespace UplayKit.Connection;

public class PartyConnection(DemuxSocket demuxSocket) : CustomConnection("party_service", demuxSocket)
{
    #region Base
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
                Logs.FileLogger.Verbose("Party Service Push: {rsp}", down.Push.ToString());
                PushEvent?.Invoke(this, down.Push);
            }
        }
    }
    #endregion
    #region Functions
    public StartSessionRsp? StartSession(uint cookie = 0)
    {
        StartSessionReq session = new();

        if (cookie != 0)
        {
            session.Cookie = cookie;
        }

        Req req = new()
        {
            RequestId = ReqId,
            StartSessionReq = session
        };
        ReqId++;
        var rsp = SendPostRequest<Upstream, Downstream>(new Upstream() { Request = req });
        if (rsp == null || rsp.Response == null)
            return null;
        return rsp.Response.StartSessionRsp;
    }
    #endregion
}
