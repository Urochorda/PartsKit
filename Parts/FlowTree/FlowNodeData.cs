namespace PartsKit
{
    public class FlowNodeData : BlueprintNode
    {
        public int Input { get; set; }

        protected override void RegisterPort()
        {
            AddPort(new BlueprintPort<int>("Input", IBlueprintPort.Orientation.Horizontal,
                IBlueprintPort.Direction.Input, IBlueprintPort.Capacity.Single,
                (value) => Input = value,
                () => Input, () => Input = 0));
        }
    }
}