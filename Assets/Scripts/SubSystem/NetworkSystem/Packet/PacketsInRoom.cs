using GameSystem;
using GameSystem.Operator;
using System.Collections.Generic;

/// <summary>
/// 请求查询服务器信息
/// </summary>
public class RInfo : Pkt<RInfo> { }

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
/// 所有玩家的信息
/// </summary>
public class SPlayerInfo : Pkt<SPlayerInfo>
{
    public List<RoomManager.PlayerRecordUnit> records;

    public SPlayerInfo(Dictionary<string, PlayerAvater> playersDatabase) : base()
    {
        records = new List<RoomManager.PlayerRecordUnit>();
        var i = playersDatabase.GetEnumerator();
        while (i.MoveNext())
        {
            var pair = i.Current;
            records.Add(new RoomManager.PlayerRecordUnit(pair.Key, pair.Value.info, pair.Value.transform.position));
        }
    }
}
