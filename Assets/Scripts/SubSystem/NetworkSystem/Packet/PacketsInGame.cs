using UnityEngine;

/// <summary>
/// Packet Input Move
/// </summary>
public class IMove : Pkt<IMove>
{
    public Vector2 input;
    public IMove(Vector2 input) : base()
    {
        this.input = input;
    }
}

/// <summary>
/// 同步
/// </summary>
public class SiMove : Pktid<SiMove>
{
    public Vector2 input;

    public SiMove(string id, Vector2 input) : base(id)
    {
        this.input = input;
    }
}

/// <summary>
/// 玩家位置
/// </summary>
public class SiPos : Pktid<SiPos>
{
    public Vector3 position;
    public SiPos(string id, Vector3 position) : base(id)
    {
        this.position = position;
    }
}