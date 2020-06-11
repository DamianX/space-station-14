using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.UserInterface;
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

        [Serializable, NetSerializable]
        public class SecurityCameraEntry
        {
            public string Name;
            public readonly Guid Identifier;

            public SecurityCameraEntry(string name, Guid identifier)
            {
                Name = name;
                Identifier = identifier;
            }
        }

        [Serializable, NetSerializable]
        public class SecurityCameraConsoleInterfaceState : BoundUserInterfaceState
        {
            public readonly Dictionary<string, List<SecurityCameraEntry>> Categories;

            public SecurityCameraConsoleInterfaceState(Dictionary<string, List<SecurityCameraEntry>> categories)
            {
                Categories = categories;
            }
        }

        [Serializable, NetSerializable]
        public class SecurityCameraSelectedMessage : BoundUserInterfaceMessage
        {
            public readonly Guid Identifier;

            public SecurityCameraSelectedMessage(Guid identifier)
            {
                Identifier = identifier;
            }
        }
    }
}
