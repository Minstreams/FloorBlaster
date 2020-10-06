public class PkRoomInfo : Pkt<PkRoomInfo>
{
    public string roomTitle;
    public PkRoomInfo(string title) : base()
    {
        roomTitle = title;
    }
}