using Content.Shared.Preferences.Appearance;
using Content.Shared.Preferences.Profiles;
using Robust.Client.GameObjects;
using Robust.Client.Interfaces.GameObjects.Components;

namespace Content.Client.GameObjects.Components.Mobs
{
    public class HumanoidVisualizer2D : AppearanceVisualizer
    {
        public override void OnChangeData(AppearanceComponent component)
        {
            base.OnChangeData(component);

            var sprite = component.Owner.GetComponent<ISpriteComponent>();
            if (component.TryGetData<HumanoidCharacterAppearance>(CharacterVisuals.CharacterAppearance,
                out var state))
            {
                sprite.LayerSetState(HumanoidVisualLayers.Hair, state.HairPrototypeId);
                sprite.LayerSetColor(HumanoidVisualLayers.Hair, state.HairColor);

                sprite.LayerSetState(HumanoidVisualLayers.FacialHair, state.FacialHairPrototypeId);
                sprite.LayerSetColor(HumanoidVisualLayers.FacialHair, state.FacialHairColor);
            }
        }
    }
}
