using Robust.Shared.GameObjects;

namespace Content.Server.GameObjects.Components.DCPU
{
    [RegisterComponent]
    public class DcpuComponent : Component
    {
        public override string Name => "Dcpu";
        private Processor _vm;
        private const int InstructionsPerTick = 1;

        public override void Initialize()
        {
            base.Initialize();
            _vm = new Processor(InstructionLoader.Load("Resources/Increment.bin"), new IHardware[0]);
        }

        public void Update(float frameTime)
        {
            for (var i = 0; i < InstructionsPerTick; i++)
                _vm.ProcessNextInstruction();
        }
    }
}
