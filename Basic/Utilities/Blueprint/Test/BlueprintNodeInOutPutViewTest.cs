namespace PartsKit
{
    [BlueprintNodeType(typeof(BlueprintNodeInOutPutTest))]
    public class BlueprintNodeInOutPutViewTest : BlueprintNodeView
    {
        public override void Init(BlueprintNode blueprintNodeVal)
        {
            base.Init(blueprintNodeVal);
            title = "InOutPutViewTest";
        }
    }
}