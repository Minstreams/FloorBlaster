using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class GetIPTest : MonoBehaviour
{
    [ContextMenu("Test")]
    public void Test()
    {
        string _myHostName = Dns.GetHostName();
        Debug.LogError(_myHostName);
        //获取本机IP 
        string _myHostIP = Dns.GetHostEntry(_myHostName).AddressList[0].ToString();
        Debug.LogError(_myHostIP);
    }
}
