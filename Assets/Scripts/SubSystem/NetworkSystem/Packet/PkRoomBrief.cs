/// <summary>
/// 由服务器广播房间的简要信息
/// </summary>
public class PkRoomBrief : Pkt<PkRoomBrief>
{
    public string title;
    public PkRoomBrief(string title) : base()
    {
        this.title = title;
    }
}