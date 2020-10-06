using System.Net;
/// <summary>
/// IP 回声定位器
/// </summary>
public class UEcho : Pkt<UEcho>
{
    public string addressStr;
    public IPAddress address
    {
        get => IPAddress.Parse(addressStr);
        set => addressStr = value.ToString();
    }
    public UEcho(IPAddress address) : base()
    {
        this.address = address;
    }
}