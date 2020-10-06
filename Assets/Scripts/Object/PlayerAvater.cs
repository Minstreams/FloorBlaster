using GameSystem;
using GameSystem.Networking;
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
    float lerpRate = 1;

    private void Start()
    {
        lerpRate = 1 - Mathf.Pow(0.001f, Time.fixedDeltaTime / Setting.lerpTime);
        if (isLocal) local = this;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpRate);

        if (!IsConnected)
        {
            //离线操作
            inputLerped = Vector2.Lerp(inputLerped, InputSystem.movement, lerpRate);
        }
        else
        {
            inputLerped = Vector2.Lerp(inputLerped, inputVec, lerpRate);
        }

        targetPosition.x += inputLerped.x * Time.deltaTime * speed;
        targetPosition.z += inputLerped.y * Time.deltaTime * speed;

        if (IsServer) ServerUpdate();
    }

    [TCPReceive]
    void SiPosReceive(SiPos packet)
    {
        if (IsServer) return;
        targetPosition = packet.position;
    }
    [TCPReceive]
    void SiMoveReceive(SiMove packet)
    {
        if (!IsServer)
        {
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
    public Vector2 inputVec;
    public Vector2 inputLerped;

    float posSyncTimer = 0;
    void ServerUpdate()
    {
        posSyncTimer += Time.fixedDeltaTime;
        if (posSyncTimer > Setting.posSyncInterval)
        {
            posSyncTimer = 0;
            ServerBoardcastPacket(new SiPos(netId, targetPosition));
        }
    }
    [TCPProcess]
    void IMoveProcess(IMove packet, Server.Connection connection)
    {
        inputVec = packet.input;
        ServerBoardcastPacket(new SiMove(connection.netId, packet.input));
    }
}
