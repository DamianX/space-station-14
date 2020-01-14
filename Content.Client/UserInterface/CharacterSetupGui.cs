using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Content.Client.GameObjects.Components.Mobs;
using Content.Client.Interfaces;
using Content.Client.Utility;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Client.Graphics.Drawing;
using Robust.Client.Interfaces.GameObjects.Components;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    public class CharacterSetupGui : Control
    {
        public readonly Button CloseButton;

        public CharacterSetupGui(IEntityManager entityManager,
            ILocalizationManager localization,
            IResourceCache resourceCache,
            IClientPreferencesManager preferencesManager)
        {
            var margin = new MarginContainer
            {
                MarginBottomOverride = 20,
                MarginLeftOverride = 20,
                MarginRightOverride = 20,
                MarginTopOverride = 20,
            };

            AddChild(margin);

            var panelTex = resourceCache.GetTexture("/Nano/button.svg.96dpi.png");
            var back = new StyleBoxTexture
            {
                Texture = panelTex,
                Modulate = new Color(37, 37, 42),
            };
            back.SetPatchMargin(StyleBox.Margin.All, 10);

            var panel = new PanelContainer
            {
                PanelOverride = back
            };

            margin.AddChild(panel);

            var vBox = new VBoxContainer {SeparationOverride = 0};

            margin.AddChild(vBox);

            CloseButton = new Button
            {
                SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkEnd,
                Text = localization.GetString("Save and close"),
                StyleClasses = {NanoStyle.StyleClassButtonBig},
            };

            var topHBox = new HBoxContainer
            {
                CustomMinimumSize = (0, 40),
                Children =
                {
                    new MarginContainer
                    {
                        MarginLeftOverride = 8,
                        Children =
                        {
                            new Label
                            {
                                Text = localization.GetString("Character Setup"),
                                StyleClasses = {NanoStyle.StyleClassLabelHeadingBigger},
                                VAlign = Label.VAlignMode.Center,
                                SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkCenter
                            }
                        }
                    },
                    CloseButton
                }
            };

            vBox.AddChild(topHBox);

            vBox.AddChild(new PanelContainer
            {
                PanelOverride = new StyleBoxFlat
                {
                    BackgroundColor = NanoStyle.NanoGold,
                    ContentMarginTopOverride = 2
                },
            });

            var hBox = new HBoxContainer
            {
                SizeFlagsVertical = SizeFlags.FillExpand,
                SeparationOverride = 0
            };
            vBox.AddChild(hBox);

            var charactersVBox = new VBoxContainer();

            hBox.AddChild(new MarginContainer
            {
                CustomMinimumSize = (350, 0),
                SizeFlagsHorizontal = SizeFlags.Fill,
                MarginTopOverride = 5,
                MarginLeftOverride = 5,
                Children =
                {
                    new ScrollContainer
                    {
                        SizeFlagsVertical = SizeFlags.FillExpand,
                        Children =
                        {
                            charactersVBox
                        }
                    }
                }
            });

            var numberOfFullSlots = 0;
            foreach (var character in preferencesManager.Preferences.Characters)
            {
                if (character is null)
                {
                    continue;
                }

                numberOfFullSlots++;
                var previewDummy = entityManager.SpawnEntity("HumanMob_Content", GridCoordinates.Nullspace);
                previewDummy.GetComponent<LooksComponent>().Appearance =
                    (HumanoidCharacterAppearance) character.CharacterAppearance;
                charactersVBox.AddChild(new CharacterPickerButton(previewDummy.GetComponent<SpriteComponent>(),
                    character.Name,
                    character.Name,
                    "Assistant"));
            }

            if (numberOfFullSlots < preferencesManager.Settings.MaxCharacterSlots)
            {
                charactersVBox.AddChild(new Button{Text = "Create new slot..."});
            }

            hBox.AddChild(new PanelContainer
            {
                PanelOverride = new StyleBoxFlat {BackgroundColor = NanoStyle.NanoGold}, CustomMinimumSize = (2, 0)
            });
            hBox.AddChild(new HumanoidProfileEditorPanel(localization, resourceCache, preferencesManager));
        }

        private class CharacterPickerButton : Control
        {
            public CharacterPickerButton(ISpriteComponent spriteComponent,
                string slotName,
                string characterName,
                string jobTitle)
            {
                if (slotName != characterName)
                {
                    slotName = $"({slotName}) {characterName}";
                }

                AddChild(new Button
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand,
                    SizeFlagsVertical = SizeFlags.FillExpand,
                    ToggleMode = true
                });

                AddChild(new HBoxContainer
                {
                    MouseFilter = MouseFilterMode.Ignore,
                    Children =
                    {
                        new SpriteView
                        {
                            Sprite = spriteComponent,
                            Scale = (2, 2),
                            MouseFilter = MouseFilterMode.Ignore,
                            OverrideDirection = Direction.South
                        },
                        new Label
                        {
                            Text = spriteComponent == null ? "empty" : $"{slotName}\n{jobTitle}"
                        }
                    }
                });
            }
        }
    }
}
