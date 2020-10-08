using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperTester : MonoBehaviour
{
    public string ip;
    [ContextMenu("PrintIp")]
    public void PrintIp()
    {
        Debug.Log(NetworkSystem.PacketToString(new HServerReg(ip)));
    }
    [ContextMenu("PrintReqList")]
    public void PrintReqList()
    {
        Debug.Log(NetworkSystem.PacketToString(new HReqList()));
    }
    public List<string> sList = new List<string>();

    [ContextMenu("PrintSList")]
    public void PrintSList()
    {
        Debug.Log(NetworkSystem.PacketToString(new HServerList(sList)));
    }

}
