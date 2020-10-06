using UnityEngine;

/// <summary>
/// Packet Input Move
/// </summary>
public class PkiMove : Pkt<PkiMove>
{
    public Vector2 input;
    public PkiMove(Vector2 input) : base()
    {
        this.input = input;
    }
}

/// <summary>
/// 玩家位置
/// </summary>
public class PkPos : Pktid<PkPos>
{
    public Vector2 position;
    public PkPos(string id, Vector2 position) : base(id)
    {
        this.position = position;
    }
}