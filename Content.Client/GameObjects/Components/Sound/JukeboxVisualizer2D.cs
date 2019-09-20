using System;
using Robust.Client.GameObjects;
using Robust.Client.Interfaces.GameObjects.Components;
using static Content.Shared.GameObjects.Components.Sound.SharedJukeboxComponent;

namespace Content.Client.GameObjects.Components.Sound
{
    public class JukeboxVisualizer2D : AppearanceVisualizer
    {
        public override void OnChangeData(AppearanceComponent component)
        {
            base.OnChangeData(component);

            var sprite = component.Owner.GetComponent<ISpriteComponent>();
            if (!component.TryGetData(JukeboxVisuals.Status, out JukeboxStatus status)) return;
            switch (status)
            {
                case JukeboxStatus.Broken:
                    sprite.LayerSetState(JukeboxVisualLayers.Base, "broken");
                    sprite.LayerSetVisible(JukeboxVisualLayers.Overlay, false);
                    break;
                case JukeboxStatus.Normal:
                    sprite.LayerSetState(JukeboxVisualLayers.Base, "normal");
                    sprite.LayerSetVisible(JukeboxVisualLayers.Overlay, false);
                    break;
                case JukeboxStatus.Off:
                    sprite.LayerSetState(JukeboxVisualLayers.Base, "nopower");
                    sprite.LayerSetVisible(JukeboxVisualLayers.Overlay, false);
                    break;
                case JukeboxStatus.Running:
                    sprite.LayerSetState(JukeboxVisualLayers.Base, "normal");
                    sprite.LayerSetState(JukeboxVisualLayers.Overlay, "running-overlay");
                    sprite.LayerSetVisible(JukeboxVisualLayers.Overlay, true);
                    break;
                case JukeboxStatus.Emagged:
                    sprite.LayerSetState(JukeboxVisualLayers.Base, "normal");
                    sprite.LayerSetState(JukeboxVisualLayers.Overlay, "emagged-overlay");
                    sprite.LayerSetVisible(JukeboxVisualLayers.Overlay, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum JukeboxVisualLayers
        {
            Base,
            Overlay,
        }
    }
}
