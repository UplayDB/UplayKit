using Uplay.Demux;

namespace UplayKit;

public static class DemuxSocketExt
{
    public static void KeepAlivePush(this DemuxSocket socket)
    {
        socket.SendPush(new()
        {
            KeepAlive = new()
        });
    }

    public static void IncrementRequestId(this DemuxSocket socket)
    {
        lock (socket.Locker)
        {
            socket.RequestId++;
        }
    }

    /// <summary>
    /// Checking if the Current version is same as the Latest
    /// </summary>
    /// <returns>True or False</returns>
    public static bool VersionCheck(this DemuxSocket socket)
    {
        var patchrsp = socket.GetPatch();
        if (patchrsp == null)
            return false;
        return DemuxSocket.ClientVersion == patchrsp.LatestVersion;
    }

    public static GetPatchInfoRsp? GetPatch(this DemuxSocket socket, string trackId = "DEFAULT", uint trackType = 0)
    {
        var patchrsp = socket.SendReq(new()
        {
            GetPatchInfoReq = new()
            {
                PatchTrackId = trackId,
                TestConfig = socket.TestConfig,
                TrackType = trackType
            },
            RequestId = socket.RequestId
        });
        socket.RequestId++;
        if (patchrsp == null)
            return null;
        return patchrsp.GetPatchInfoRsp;
    }

    /// <summary>
    /// Initially pushing the Current version
    /// </summary>
    public static void PushVersion(this DemuxSocket socket)
    {
        socket.SendPush(new()
        {
            ClientVersion = new()
            {
                Version = DemuxSocket.ClientVersion
            }
        });
    }

    /// <summary>
    /// Trying to Authenticate with UbiTicket
    /// </summary>
    /// <param name="token">Ubi Ticket</param>
    /// <param name="KeepAlive">Keeping the Sokcet Alive</param>
    /// <returns>True or False</returns>
    public static bool Authenticate(this DemuxSocket socket, string token, bool KeepAlive = false)
    {
        var authRsp = socket.SendReq(new()
        {
            RequestId = socket.RequestId,
            AuthenticateReq = new()
            {
                ClientId = "uplay_pc",
                SendKeepAlive = KeepAlive,
                Token = new()
                {
                    UbiTicket = token
                }
            }
        });
        socket.RequestId++;
        if (authRsp == null)
            return false;
        return authRsp.AuthenticateRsp.Success;
    }
}
