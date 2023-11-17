namespace PartsKit
{
    [BlueprintNodeType(typeof(BlueprintNodeInOutPutTest))]
    public class BlueprintNodeInOutPutViewTest : BlueprintNodeView
    {
        public override void Init(BlueprintNode blueprintNodeVal, BlueprintView ownerViewVal)
        {
            base.Init(blueprintNodeVal, ownerViewVal);
            title = "InOutPutViewTest";
        }
    }
}