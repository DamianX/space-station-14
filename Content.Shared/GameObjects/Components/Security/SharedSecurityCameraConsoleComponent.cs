using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameObjects.Components.Security
{
    public class SharedSecurityCameraConsoleComponent : Component
    {
        public override string Name => "SecurityCameraConsole";

        [Serializable, NetSerializable]
        public enum SecurityCameraConsoleUiKey
        {
            Key
        }
    }
}
