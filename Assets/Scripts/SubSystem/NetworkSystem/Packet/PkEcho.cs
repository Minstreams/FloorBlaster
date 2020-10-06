using System.Net;
/// <summary>
/// IP 回声定位器
/// </summary>
public class PkEcho : Pkt<PkEcho>
{
    public string addressStr;
    public IPAddress address
    {
        get => IPAddress.Parse(addressStr);
        set => addressStr = value.ToString();
    }
    public PkEcho(IPAddress address) : base()
    {
        this.address = address;
    }
}