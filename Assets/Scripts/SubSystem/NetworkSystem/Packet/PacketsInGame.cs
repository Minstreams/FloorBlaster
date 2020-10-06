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
/// 玩家位置
/// </summary>
public class SiPos : Pktid<SiPos>
{
    public Vector2 position;
    public SiPos(string id, Vector2 position) : base(id)
    {
        this.position = position;
    }
}