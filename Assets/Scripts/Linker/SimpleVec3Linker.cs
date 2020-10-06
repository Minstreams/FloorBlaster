using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/SimpleVec3Linker")]
    public class SimpleVec3Linker : MonoBehaviour
    {
        [MinsHeader("Simple Vec3 Linker", SummaryType.TitleCyan, 0)]

        //Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label]
        public Vector3 data;
        [Label(true)]
        public bool invokeOnStart;

        private void Start()
        {
            if (invokeOnStart) Invoke();
        }

        //Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public Vec3Event output;

        //Input
        [ContextMenu("Invoke")]
        public void Invoke()
        {
            output?.Invoke(data);
        }
        public void SetX(float x)
        {
            data.x = x;
        }
        public void SetY(float y)
        {
            data.y = y;
        }
        public void SetZ(float z)
        {
            data.z = z;
        }
    }
}