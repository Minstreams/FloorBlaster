using GameSystem;
using GameSystem.Operator;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// 申请IP
/// </summary>
public class UIp : Pkt<UIp> { }

/// <summary>
/// IP 回声定位器
/// </summary>
public class UEcho : Pkt<UEcho>
{
    public string addressStr;
    public IPAddress address
    {
        get => IPAddress.Parse(addressStr);
        set => addressStr = value.ToString();
    }
    public UEcho(IPAddress address) : base()
    {
        this.address = address;
    }
}

public class UHello : Pkt<UHello>
{
    public string hello;
    public UHello() : base()
    {
        hello = NetworkSystem.clientHello;
    }
}

/// <summary>
/// 由服务器发送的房间的简要信息
/// </summary>
public class URoomBrief : Pkt<URoomBrief>
{
    public string title;
    public URoomBrief(string title) : base()
    {
        this.title = title;
    }
}

/// <summary>
/// 请求同步时间
/// </summary>
public class RTimer : Pkt<RTimer>
{
    public float t;

    public RTimer() : base()
    {
        this.t = NetworkSystem.timer;
    }
}

/// <summary>
/// 同步
/// </summary>
public class STimer : Pkt<STimer>
{
    /// <summary>
    /// 收到的时间
    /// </summary>
    public float t;
    public float tSent;

    public STimer(float tSent) : base()
    {
        this.t = NetworkSystem.timer;
        this.tSent = tSent;
    }
}

/// <summary>
/// 请求查询房间信息
/// </summary>
public class RInfo : Pkt<RInfo> { }

/// <summary>
/// 房间的信息
/// </summary>
public class SInfo : Pkt<SInfo>
{
    public PersonalizationSystem.RoomInfo info;
    public SInfo(PersonalizationSystem.RoomInfo info) : base()
    {
        this.info = info;
    }
}

/// <summary>
/// 上传玩家信息
/// </summary>
public class CPlayerInfo : Pkt<CPlayerInfo>
{
    public PersonalizationSystem.PlayerInfo info;

    public CPlayerInfo(PersonalizationSystem.PlayerInfo info) : base()
    {
        this.info = info;
    }
}

/// <summary>
/// 所有玩家的信息
/// </summary>
public class SPlayerInfo : Pkt<SPlayerInfo>
{
    [System.Serializable]
    public struct PlayerRecordUnit
    {
        public string id;
        public PersonalizationSystem.PlayerInfo info;
        public Vector3 pos;

        public PlayerRecordUnit(string id, PersonalizationSystem.PlayerInfo info, Vector3 pos)
        {
            this.id = id;
            this.info = info;
            this.pos = pos;
        }
    }
    public List<PlayerRecordUnit> records;

    public SPlayerInfo() : base()
    {
        records = new List<PlayerRecordUnit>();
        var i = GameplaySystem.playersDatabase.GetEnumerator();
        while (i.MoveNext())
        {
            var pair = i.Current;
            records.Add(new PlayerRecordUnit(pair.Key, pair.Value.info, pair.Value.targetPosition));
        }
    }
}
