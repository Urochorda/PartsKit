using System;
using UnityEngine;

namespace PartsKit
{
    /// <summary>
    /// 流程树数据
    /// </summary>
    [Serializable]
    public class FlowData
    {
        public FlowTree Tree { get; set; }
    }

    /// <summary>
    /// 流程控制器
    /// </summary>
    public class FlowController : PartsKitBehaviour
    {
        private FlowData data; //流程树数据
        [SerializeField] private FlowTree tree;

        protected override void OnInit()
        {
            SetData(new FlowData() { Tree = tree.GetRunBlueprint<FlowTree>() });
        }

        protected override void OnDeInit()
        {
            data.Tree.EndRun();
        }

        public void SetData(FlowData dataVal)
        {
            data = dataVal;
            data.Tree.BeginRun();
        }

        private void Update()
        {
            if (data == null)
            {
                return;
            }

            data.Tree.UpdateRun();
        }
    }
}