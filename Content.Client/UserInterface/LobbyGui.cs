using Content.Client.Chat;
using Content.Client.Interfaces;
using Content.Client.Utility;
using Content.Shared.Preferences;
using Robust.Client.Graphics.Drawing;
using Robust.Client.Interfaces.GameObjects.Components;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    internal sealed class LobbyGui : Control
    {
        public Label ServerName { get; }
        public Label StartTime { get; }
        public Button ReadyButton { get; }
        public Button ObserveButton { get; }
        public Button LeaveButton { get; }
        public ChatBox Chat { get; }
        public ItemList OnlinePlayerItemList { get; }
        public ServerInfo ServerInfo { get; }
        private readonly HumanoidProfileEditor _humanoidProfileEditor;

        public LobbyGui(IEntityManager entityManager,
            ILocalizationManager localization,
            IResourceCache resourceCache,
            IClientPreferencesManager preferencesManager)
        {
            _humanoidProfileEditor = new HumanoidProfileEditor(localization, resourceCache, preferencesManager);
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
                                Text = localization.GetString("Lobby"),
                                StyleClasses = {NanoStyle.StyleClassLabelHeadingBigger},
                                /*MarginBottom = 40,
                                MarginLeft = 8,*/
                                VAlign = Label.VAlignMode.Center
                            }
                        }
                    },
                    (ServerName = new Label
                    {
                        StyleClasses = {NanoStyle.StyleClassLabelHeadingBigger},
                        /*MarginBottom = 40,
                        GrowHorizontal = GrowDirection.Both,*/
                        VAlign = Label.VAlignMode.Center,
                        SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkCenter
                    }),
                    (LeaveButton = new Button
                    {
                        SizeFlagsHorizontal = SizeFlags.ShrinkEnd,
                        Text = localization.GetString("Leave"),
                        StyleClasses = {NanoStyle.StyleClassButtonBig},
                        //GrowHorizontal = GrowDirection.Begin
                    })
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

            var previewDummy = entityManager.SpawnEntity("HumanMob_Content", GridCoordinates.Nullspace);
            var characterEditButton = new Button {Text = localization.GetString("Edit")};
            characterEditButton.OnPressed += args => { _humanoidProfileEditor.Open(); };
            var characterLoadButton = new Button {Text = localization.GetString("Load")};
            hBox.AddChild(new VBoxContainer
            {
                SizeFlagsHorizontal = SizeFlags.FillExpand,
                SeparationOverride = 0,
                Children =
                {
                    new VBoxContainer()
                    {
                        SizeFlagsVertical = SizeFlags.FillExpand,
                        Children =
                        {
                            characterEditButton,
                            characterLoadButton,
                            new SpriteView{Sprite = previewDummy.GetComponent<ISpriteComponent>(), OverrideDirection = Direction.South, Scale = (2, 2)},
                            new SpriteView{Sprite = previewDummy.GetComponent<ISpriteComponent>(), OverrideDirection = Direction.North, Scale = (2, 2)},
                            new SpriteView{Sprite = previewDummy.GetComponent<ISpriteComponent>(), OverrideDirection = Direction.West, Scale = (2, 2)},
                            new SpriteView{Sprite = previewDummy.GetComponent<ISpriteComponent>(), OverrideDirection = Direction.East, Scale = (2, 2)},
                        }
                    },

                    new StripeBack
                    {
                        Children =
                        {
                            new MarginContainer
                            {
                                MarginRightOverride = 3,
                                MarginLeftOverride = 3,
                                MarginTopOverride = 3,
                                MarginBottomOverride = 3,
                                Children =
                                {
                                    new HBoxContainer
                                    {
                                        SeparationOverride = 6,
                                        Children =
                                        {
                                            (ObserveButton = new Button
                                            {
                                                Text = localization.GetString("Observe"),
                                                StyleClasses = {NanoStyle.StyleClassButtonBig}
                                            }),
                                            (StartTime = new Label
                                            {
                                                SizeFlagsHorizontal = SizeFlags.FillExpand,
                                                Align = Label.AlignMode.Right,
                                                FontColorOverride = Color.DarkGray,
                                                StyleClasses = {NanoStyle.StyleClassLabelBig}
                                            }),
                                            (ReadyButton = new Button
                                            {
                                                ToggleMode = true,
                                                Text = localization.GetString("Ready Up"),
                                                StyleClasses = {NanoStyle.StyleClassButtonBig}
                                            }),
                                        }
                                    }
                                }
                            }
                        }
                    },

                    new MarginContainer
                    {
                        MarginRightOverride = 3,
                        MarginLeftOverride = 3,
                        MarginTopOverride = 3,
                        MarginBottomOverride = 3,
                        SizeFlagsVertical = SizeFlags.FillExpand,
                        Children =
                        {
                            (Chat = new ChatBox
                            {
                                Input = {PlaceHolder = localization.GetString("Say something!")}
                            })
                        }
                    },
                }
            });

            hBox.AddChild(new PanelContainer
            {
                PanelOverride = new StyleBoxFlat {BackgroundColor = NanoStyle.NanoGold}, CustomMinimumSize = (2, 0)
            });

            {
                hBox.AddChild(new VBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand,
                    Children =
                    {
                        new NanoHeading
                        {
                            Text = localization.GetString("Online Players"),
                        },
                        new MarginContainer
                        {
                            SizeFlagsVertical = SizeFlags.FillExpand,
                            MarginRightOverride = 3,
                            MarginLeftOverride = 3,
                            MarginTopOverride = 3,
                            MarginBottomOverride = 3,
                            Children =
                            {
                                (OnlinePlayerItemList = new ItemList())
                            }
                        },
                        new NanoHeading
                        {
                            Text = localization.GetString("Server Info"),
                        },
                        new MarginContainer
                        {
                            SizeFlagsVertical = SizeFlags.FillExpand,
                            MarginRightOverride = 3,
                            MarginLeftOverride = 3,
                            MarginTopOverride = 3,
                            MarginBottomOverride = 2,
                            Children =
                            {
                                (ServerInfo = new ServerInfo(localization))
                            }
                        },
                    }
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _humanoidProfileEditor.Dispose();
            }
        }
    }
}
