using GameSystem;
/// <summary>
/// 由服务器广播房间的简要信息
/// </summary>
public class URoomBrief : Pkt<URoomBrief>
{
    public string title;
    public string hello;
    public URoomBrief(string title) : base()
    {
        this.title = title;
        this.hello = NetworkSystem.clientHello;
    }
}