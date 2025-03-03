﻿using Uplay.Demux;

namespace UplayKit;

public static class DemuxSocketExt
{
    public static void KeepAlivePush(this DemuxSocket socket)
    {
        Push keepalive = new()
        {
            KeepAlive = new()
        };

        socket.SendPush(keepalive);
    }

    /// <summary>
    /// Checking if the Current version is same as the Latest
    /// </summary>
    /// <returns>True or False</returns>
    public static bool VersionCheck(this DemuxSocket socket)
    {
        socket.RequestId++;
        Req patchreq = new()
        {
            GetPatchInfoReq = new()
            {
                PatchTrackId = "DEFAULT",
                TestConfig = socket.TestConfig,
                TrackType = 0
            },
            RequestId = socket.RequestId
        };
        var patchrsp = socket.SendReq(patchreq);
        if (patchrsp == null)
            return false;
        return DemuxSocket.ClientVersion == patchrsp.GetPatchInfoRsp.LatestVersion;
    }

    public static GetPatchInfoRsp? GetPatch(this DemuxSocket socket, string trackId = "DEFAULT", uint trackType = 0)
    {
        socket.RequestId++;
        Req patchreq = new()
        {
            GetPatchInfoReq = new()
            {
                PatchTrackId = trackId,
                TestConfig = socket.TestConfig,
                TrackType = trackType
            },
            RequestId = socket.RequestId
        };
        var patchrsp = socket.SendReq(patchreq);
        if (patchrsp == null)
            return null;
        return patchrsp.GetPatchInfoRsp;
    }

    /// <summary>
    /// Initially pushing the Current version
    /// </summary>
    public static void PushVersion(this DemuxSocket socket)
    {
        Push versionPush = new()
        {
            ClientVersion = new()
            {
                Version = DemuxSocket.ClientVersion
            }
        };

        socket.SendPush(versionPush);
    }

    /// <summary>
    /// Trying to Authenticate with UbiTicket
    /// </summary>
    /// <param name="token">Ubi Ticket</param>
    /// <param name="KeepAlive">Keeping the Sokcet Alive</param>
    /// <returns>True or False</returns>
    public static bool Authenticate(this DemuxSocket socket, string token, bool KeepAlive = false)
    {
        socket.RequestId++;
        Req authReq = new()
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
        };
        var authRsp = socket.SendReq(authReq);
        if (authRsp == null)
            return false;
        return authRsp.AuthenticateRsp.Success;
    }
}
