using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : MonoBehaviour
{
    [MinsHeader("摄像机物体", SummaryType.TitleGray, -1)]
    [Label]
    public Vector3 offset;
    private void Update()
    {
        if (PlayerAvater.local != null)
        {
            transform.position = Vector3.Lerp(transform.position, PlayerAvater.local.transform.position + offset, 0.02f);
        }
    }
}
