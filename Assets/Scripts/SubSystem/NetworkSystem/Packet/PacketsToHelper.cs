// 和Helper服务器通信的特殊Packet,H开头
using System.Collections.Generic;

/// <summary>
/// 服务器自注册
/// </summary>
public class HServerReg : Pkt<HServerReg>
{
    public string ip;

    public HServerReg(string ip) : base()
    {
        this.ip = ip;
    }
}

/// <summary>
/// 请求获取List
/// </summary>
public class HReqList : Pkt<HReqList>
{

}

/// <summary>
/// 下发的List
/// </summary>
public class HServerList : Pkt<HServerList>
{
    public List<string> sList;

    public HServerList(List<string> sList) : base()
    {
        this.sList = sList;
    }
}