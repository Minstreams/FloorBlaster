using GameSystem;
using GameSystem.Networking;
using GameSystem.Operator;
using UnityEngine;

public class PlayerAvater : GameSystem.Networking.NetworkPlayer
{
    public static PlayerAvater local;

    // 参数
    public bool isLocal = false;
    /// <summary>
    /// 个性化数据
    /// </summary>
    public PersonalizationSystem.PlayerInfo info;

    public float speed = 8;

    // 本地处理 ==========================================
    public Vector3 targetPosition;
    public Vector2 velocityRaw;
    Vector3 rawToV(Vector2 raw) => new Vector3(raw.x, 0, raw.y) * speed;
    Vector3 velocity => rawToV(velocityRaw);
    public Vector2 inputVec;

    float lerpRate = 1;

    private void Start()
    {
        lerpRate = 1 - Mathf.Pow(0.001f, Time.fixedDeltaTime / Setting.lerpTime);
        if (isLocal) local = this;
    }
    private void FixedUpdate()
    {
        if (IsServer || !isLocal)
        {
            velocityRaw = Vector2.Lerp(velocityRaw, inputVec, lerpRate);
        }
        else
        {
            velocityRaw = Vector2.Lerp(velocityRaw, InputSystem.movement, lerpRate);
        }
        targetPosition += velocity * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpRate / 2f);
    }

    [TCPReceive]
    void SiMoveReceive(SiMove packet)
    {
        if (IsServer) return;
        float t = (ServerTimer - packet.t);
        Vector2 nowVRaw = Vector2.Lerp(packet.vRaw, packet.input, (1 - Mathf.Pow(0.001f, t / Setting.lerpTime)));
        targetPosition = packet.pos + rawToV(0.215f * (packet.vRaw + nowVRaw)) * t;
        if (!isLocal)
        {
            velocityRaw = nowVRaw;
            inputVec = packet.input;
        }
    }

    [TCPConnection]
    void OnConnection()
    {
        if (isLocal)
        {
            ActivateId(NetworkSystem.netId);
        }
    }
    [TCPDisconnection]
    void OnDisconnection()
    {
        if (isLocal)
        {
            DeactivateId();
        }
    }


    // 在服务器端 ========================================

    [TCPProcess]
    void IMoveProcess(IMove packet, Server.Connection connection)
    {
        inputVec = packet.input;
        ServerBoardcastPacket(new SiMove(connection.netId, targetPosition, velocityRaw, inputVec));
        connection.Send(new STimer(packet.t));
    }

    private void OnDrawGizmos()
    {
        var c = Gizmos.color;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(targetPosition, Vector3.one);
        Gizmos.color = c;
    }
}
