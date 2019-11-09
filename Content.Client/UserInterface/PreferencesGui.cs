using System.Collections.Immutable;
using Content.Client.UserInterface.Preferences;
using Content.Client.Utility;
using Robust.Client.Graphics.Drawing;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    public class PreferencesGui : Control
    {
        public Button CloseButton { get; }

        public PreferencesGui(ILocalizationManager localization, IResourceCache resourceCache)
        {
            var panelTex = resourceCache.GetTexture("/Nano/button.svg.96dpi.png");
            var back = new StyleBoxTexture
            {
                Texture = panelTex,
                Modulate = new Color(37, 37, 42),
            };
            back.SetPatchMargin(StyleBox.Margin.All, 10);

            var panel = new Panel
            {
                PanelOverride = back
            };
            panel.SetAnchorAndMarginPreset(LayoutPreset.Wide);
            AddChild(panel);

            var vBox = new VBoxContainer();
            vBox.SetAnchorAndMarginPreset(LayoutPreset.Wide);
            vBox.MarginTop = 40;
            AddChild(vBox);

            AddChild(new Label
            {
                Text = localization.GetString("Preferences"),
                StyleClasses = {NanoStyle.StyleClassLabelHeadingBigger},
                MarginBottom = 40,
                MarginLeft = 8,
                VAlign = Label.VAlignMode.Center
            });

            AddChild(CloseButton = new Button
            {
                SizeFlagsHorizontal = SizeFlags.ShrinkEnd,
                Text = localization.GetString("Close"),
                StyleClasses = {NanoStyle.StyleClassButtonBig},
                GrowHorizontal = GrowDirection.Begin
            });

            CloseButton.SetAnchorAndMarginPreset(LayoutPreset.TopRight);

            TabContainer tabs = new TabContainer();

            vBox.AddChild(tabs);

            tabs.AddChild(new PreferencesPage(localization));
            tabs.SetTabTitle(0, "Settings");

            tabs.AddChild(new CharacterPage());
            tabs.SetTabTitle(1, "Character");
        }
    }
}
