using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Flow/Tree", fileName = "FlowTree")]
    public class FlowTree : Blueprint
    {
        private FlowRootNode rootNode; //根流程节点
        private BlueprintExecutePort curExecutePort; //当前执行端口

        protected override void OnInit()
        {
            base.OnInit();
            InitRoot();
        }

        private void InitRoot()
        {
            foreach (BlueprintNode node in Nodes)
            {
                if (node is FlowRootNode root)
                {
                    rootNode = root;
                    break;
                }
            }

            if (rootNode == null)
            {
                rootNode = new FlowRootNode();
                rootNode.OnCreate();
                AddNode(rootNode);
            }
        }

        public void Update()
        {
            curExecutePort ??= rootNode.OutputExePort;
            BeginExecuted(curExecutePort);
        }
    }
}