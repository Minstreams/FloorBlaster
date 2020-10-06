﻿using GameSystem;
using GameSystem.Networking;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
        transform.position = Vector3.Lerp(targetPosition, transform.position, Mathf.Pow(0.001f, Time.deltaTime / Setting.lerpTime));
        if (IsServer) ServerUpdate();
    }

    [TCPReceive]
    void PacketPositionReceive(PkPos packet)
    {
        if (IsServer) return;
        targetPosition = packet.position;
    }


    // 在服务器端 ========================================
    public Vector2 inputVec;
    public Vector2 inputLerped;
    void ServerUpdate()
    {
        inputLerped = Vector2.Lerp(inputVec, inputLerped, Mathf.Pow(0.001f, Time.deltaTime / Setting.lerpTime));
        targetPosition.x += inputLerped.x * Time.deltaTime * info.speed;
        targetPosition.z += inputLerped.y * Time.deltaTime * info.speed;
    }
    [TCPProcess]
    void PacketInputMoveProcess(PkiMove packet, Server.Connection connection)
    {
        inputVec = packet.input;
        ServerBoardcastPacket(new PkPos(connection.netId, targetPosition));
    }
}