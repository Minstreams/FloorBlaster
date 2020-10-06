using GameSystem.Networking;
using UnityEngine;

public class PlayerAvater : GameSystem.Networking.NetworkPlayer
{
    // 参数
    [System.Serializable]
    public class PlayerInfo
    {
        public float speed = 5;
        public float weight = 5;
    }
    public PlayerInfo info;

    // 本地处理 ==========================================
    public Vector3 targetPosition;
    float lerpRate = 1;

    private void Start()
    {
        lerpRate = 1 - Mathf.Pow(0.001f, Time.fixedDeltaTime / Setting.lerpTime);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpRate);
        if (IsServer) ServerUpdate();
    }

    [TCPReceive]
    void PacketPositionReceive(SiPos packet)
    {
        if (IsServer) return;
        targetPosition = packet.position;
    }


    // 在服务器端 ========================================
    public Vector2 inputVec;
    public Vector2 inputLerped;

    float posSyncTimer = 0;
    void ServerUpdate()
    {
        inputLerped = Vector2.Lerp(inputLerped, inputVec, Mathf.Pow(0.001f, Time.deltaTime / Setting.lerpTime));
        targetPosition.x += inputLerped.x * Time.deltaTime * info.speed;
        targetPosition.z += inputLerped.y * Time.deltaTime * info.speed;

        posSyncTimer += Time.fixedDeltaTime;
        if (posSyncTimer > Setting.posSyncInterval)
        {
            posSyncTimer = 0;
            ServerBoardcastPacket(new SiPos(netId, targetPosition));
        }
    }
    [TCPProcess]
    void PacketInputMoveProcess(IMove packet, Server.Connection connection)
    {
        inputVec = packet.input;
    }
}
