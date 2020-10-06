using UnityEngine;

/// <summary>
/// 请求查询服务器信息
/// </summary>
public class RInfo : Pkt<RInfo> { }

/// <summary>
/// 上传玩家信息
/// </summary>
public class CPlayerInfo : Pkt<CPlayerInfo>
{
    public PlayerAvater.PlayerInfo info;

    public CPlayerInfo(PlayerAvater.PlayerInfo info) : base()
    {
        this.info = info;
    }
}

/// <summary>
/// 初始化房间的所有信息
/// </summary>
public class SInfo : Pkt<SInfo>
{
    public SInfo() : base()
    {
    }
}
