using UnityEditor.Experimental.GraphView;

namespace PartsKit
{
    public class BlueprintBlackboardView : Blackboard
    {
        private BlueprintView ownerView;
        private BlueprintBlackboard blackboard;

        public virtual void Init(BlueprintView ownerViewVal, BlueprintBlackboard blackboardVal)
        {
            blackboard = blackboardVal;
            ownerView = ownerViewVal;
        }
    }
}