using System;

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

        protected override void OnInit()
        {
        }

        protected override void OnDeInit()
        {
        }

        public void SetData(FlowData dataVal)
        {
            data = dataVal;
        }

        private void Update()
        {
            if (data == null)
            {
                return;
            }

            data.Tree.Update();
        }
    }
}