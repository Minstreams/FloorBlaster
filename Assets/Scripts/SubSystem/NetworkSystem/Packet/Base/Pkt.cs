/// <summary>
/// 网络包直接继承此类
/// 命名前缀：
///     【UDP】U
/// 上传：
///     【输入】I
///     【请求】R
///     【数据】C
/// 下载：
///     【广播】：S
/// </summary>
public class Pkt<T> : PacketBase
{
    public Pkt()
    {
        this.pktTypeStr = typeof(T).FullName;
    }
}