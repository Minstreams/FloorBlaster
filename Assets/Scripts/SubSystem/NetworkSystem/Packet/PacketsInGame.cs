using GameSystem;
using UnityEngine;

/// <summary>
/// Packet Input Move
/// </summary>
public class IMove : Pkt<IMove>
{
    public float t;
    public Vector2 input;
    public IMove(Vector2 input) : base()
    {
        this.t = NetworkSystem.timer;
        this.input = input;
    }
}

/// <summary>
/// 位置同步信息
/// </summary>
public class SiMove : Pktid<SiMove>
{
    public float t;
    public Vector3 pos;
    public Vector2 vRaw;
    public Vector2 input;

    public SiMove(string id, Vector3 pos, Vector2 vRaw, Vector2 input) : base(id)
    {
        t = NetworkSystem.timer;
        this.pos = pos;
        this.vRaw = vRaw;
        this.input = input;
    }
}