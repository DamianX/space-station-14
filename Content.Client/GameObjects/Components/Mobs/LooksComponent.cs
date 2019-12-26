using Content.Shared.GameObjects.Components.Mobs;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Appearance;
using Robust.Client.GameObjects;
using Robust.Client.Graphics.Shaders;
using Robust.Client.Interfaces.GameObjects.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.GameObjects.Components.Mobs
{
    [RegisterComponent]
    public sealed class LooksComponent : SharedLooksComponent
    {
        private const string HairShaderName = "hair";
        private const string HairColorParameter = "hairColor";

#pragma warning disable 649
        [Dependency] private readonly IPrototypeManager _prototypeManager;
#pragma warning restore 649

        private ShaderInstance _facialHairShader;
        private ShaderInstance _hairShader;

        public override void Initialize()
        {
            base.Initialize();

            var shaderProto = _prototypeManager.Index<ShaderPrototype>(HairShaderName);

            _facialHairShader = shaderProto.InstanceUnique();
            _hairShader = shaderProto.InstanceUnique();
        }

        protected override void Startup()
        {
            base.Startup();

            if (Owner.TryGetComponent(out ISpriteComponent sprite))
            {
                sprite.LayerSetShader(HumanoidVisualLayers.Hair, _hairShader);
                sprite.LayerSetShader(HumanoidVisualLayers.FacialHair, _facialHairShader);
            }
        }

        public override HumanoidCharacterAppearance Appearance
        {
            get => base.Appearance;
            set
            {
                base.Appearance = value;
                UpdateLooks();
            }
        }

        private void UpdateLooks()
        {
            var sprite = Owner.GetComponent<SpriteComponent>();

            _hairShader?.SetParameter(HairColorParameter, Appearance.HairColor);
            _facialHairShader?.SetParameter(HairColorParameter, Appearance.FacialHairColor);

            sprite.LayerSetState(HumanoidVisualLayers.Hair,
                HairStyles.HairStylesMap[Appearance.HairStyleName ?? HairStyles.DefaultHairStyle]);
            sprite.LayerSetState(HumanoidVisualLayers.FacialHair,
                HairStyles.FacialHairStylesMap[Appearance.FacialHairStyleName ?? HairStyles.DefaultFacialHairStyle]);
        }
    }
}
