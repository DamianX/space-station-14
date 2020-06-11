using System;
using Content.Server.GameObjects.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameObjects.Components.Security
{
    [RegisterComponent]
    public class SecurityCameraComponent : Component
    {
        public override string Name => "SecurityCamera";

        private SecurityCameraConsoleEntitySystem _entitySystem;

        private string _category;
        private string _tag;
        [ViewVariables]
        private Guid _guid;

        [ViewVariables(VVAccess.ReadWrite)]
        public string Category
        {
            get => _category;
            set
            {
                if (_category == value)
                {
                    return;
                }

                _category = value;
                _entitySystem.ChangeCameraCategory(_guid, _category);
            }
        }

        [ViewVariables(VVAccess.ReadWrite)]
        public string Tag
        {
            get => _tag;
            set
            {
                if (_tag == value)
                {
                    return;
                }

                _tag = value;
                _entitySystem.ChangeCameraTag(_guid, _tag);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _entitySystem = EntitySystem.Get<SecurityCameraConsoleEntitySystem>();
            _guid = Guid.NewGuid();
        }

        protected override void Startup()
        {
            base.Startup();
            _entitySystem.AddCamera(_guid, _category, _tag);
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            _entitySystem.RemoveCamera(_guid);
        }

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref _tag, "tag", string.Empty);
            serializer.DataField(ref _category, "category", string.Empty);
        }
    }
}
