using System;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.GameObjects.Components.Mobs
{
    public abstract class SharedLooksComponent : Component
    {
        private HumanoidCharacterAppearance _appearance;

        public sealed override string Name => "Hair";
        public sealed override uint? NetID => ContentNetIDs.HAIR;
        public sealed override Type StateType => typeof(LooksComponentState);

        [ViewVariables(VVAccess.ReadWrite)]
        public virtual HumanoidCharacterAppearance Appearance
        {
            get => _appearance;
            set
            {
                _appearance = value;
                Dirty();
            }
        }

        public override ComponentState GetComponentState()
        {
            return new LooksComponentState(Appearance);
        }

        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            var cast = (LooksComponentState) curState;
            Appearance = cast.Appearance;
        }

        [Serializable, NetSerializable]
        private sealed class LooksComponentState : ComponentState
        {
            public HumanoidCharacterAppearance Appearance { get; }
            public LooksComponentState(HumanoidCharacterAppearance appearance) : base(ContentNetIDs.HAIR)
            {
                Appearance = appearance;
            }
        }
    }
}
