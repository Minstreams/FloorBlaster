using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    namespace Operator
    {
        [AddComponentMenu("[TheMatrix]/Operator/SavableObjectOperator")]
        public class SavableObjectOperator : MonoBehaviour
        {
#if UNITY_EDITOR
            [MinsHeader("Operator of TheMatrix", SummaryType.PreTitleOperator, -1)]
            [MinsHeader("Savable Object Operator", SummaryType.TitleOrange, 0)]
            [MinsHeader("此操作节点用来测试游戏可保存的持久化数据", SummaryType.CommentCenter, 1)]
            [ConditionalShow, SerializeField] private bool useless;
#endif

            //Data
            [MinsHeader("Data", SummaryType.Header, 2)]
            [Label]
            public Savable.SavableObject target;
            [Label(true)]
            public bool saveOnDestroy;
            [Label(true)]
            public bool loadOnStart;

            private void OnDestroy()
            {
                if (saveOnDestroy) Save();
            }
            private void Start()
            {
                if (loadOnStart) Load();
            }

            //Input
            [ContextMenu("Save")]
            public void Save()
            {
                TheMatrix.Save(target);
            }
            [ContextMenu("Load")]
            public void Load()
            {
                TheMatrix.Load(target);
            }
        }
    }
}
