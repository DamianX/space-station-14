using System;
using Content.Client.GameObjects.Components;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Robust.Client.Graphics.Drawing;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    public class HumanoidProfileEditorPanel : Control
    {
        private readonly IClientPreferencesManager _preferencesManager;
        private HumanoidCharacterProfile _profile;
        private readonly int _characterSlot;

        private readonly HairPickerWindow _hairPickerWindow;
        private readonly FacialHairPickerWindow _facialHairPickerWindow;

        private readonly LineEdit _nameEdit;
        private readonly Button _sexMaleButton;
        private readonly Button _sexFemaleButton;
        private readonly LineEdit _ageEdit;
        private readonly Button _saveButton;

        private bool _isDirty;

        private bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                UpdateSaveButton();
            }
        }

        private static readonly StyleBoxFlat HighlightedStyle = new StyleBoxFlat
        {
            BackgroundColor = new Color(47, 47, 53),
            ContentMarginTopOverride = 10,
            ContentMarginBottomOverride = 10,
            ContentMarginLeftOverride = 10,
            ContentMarginRightOverride = 10
        };

        private static Control HighlightedContainer()
        {
            return new PanelContainer
            {
                PanelOverride = HighlightedStyle
            };
        }

        public HumanoidProfileEditorPanel(ILocalizationManager localization,
            IResourceCache resourceCache,
            IClientPreferencesManager preferencesManager)
        {
            _profile = (HumanoidCharacterProfile) preferencesManager.Preferences.SelectedCharacter;
            _characterSlot = preferencesManager.Preferences.SelectedCharacterIndex;
            _preferencesManager = preferencesManager;
            _hairPickerWindow = new HairPickerWindow(resourceCache, localization);
            _hairPickerWindow.Populate();
            _hairPickerWindow.OnHairStylePicked += newStyle =>
            {
                _profile = _profile.WithCharacterAppearance(
                    _profile.Appearance.WithHairStyleName(newStyle));
                IsDirty = true;
            };
            _hairPickerWindow.OnHairColorPicked += newColor =>
            {
                _profile = _profile.WithCharacterAppearance(
                    _profile.Appearance.WithHairColor(newColor));
                IsDirty = true;
            };
            _facialHairPickerWindow = new FacialHairPickerWindow(resourceCache, localization);
            _facialHairPickerWindow.Populate();
            _facialHairPickerWindow.OnHairStylePicked += newStyle =>
            {
                _profile = _profile.WithCharacterAppearance(
                    _profile.Appearance.WithFacialHairStyleName(newStyle));
                IsDirty = true;
            };
            _facialHairPickerWindow.OnHairColorPicked += newColor =>
            {
                _profile = _profile.WithCharacterAppearance(
                    _profile.Appearance.WithFacialHairColor(newColor));
                IsDirty = true;
            };

            var margin = new MarginContainer
            {
                MarginTopOverride = 10,
                MarginBottomOverride = 10,
                MarginLeftOverride = 10,
                MarginRightOverride = 10
            };
            AddChild(margin);

            var vBox = new VBoxContainer();
            margin.AddChild(vBox);

            var middleContainer = new HBoxContainer
            {
                SeparationOverride = 10
            };
            vBox.AddChild(middleContainer);

            var leftColumn = new VBoxContainer();
            middleContainer.AddChild(leftColumn);

            #region Randomize

            {
                var panel = HighlightedContainer();
                var randomizeEverythingButton = new Button
                {
                    Text = localization.GetString("Randomize everything"),
                    Disabled = true,
                    ToolTip = "Not yet implemented!"
                };
                panel.AddChild(randomizeEverythingButton);
                leftColumn.AddChild(panel);
            }

            #endregion Randomize

            var middleColumn = new VBoxContainer();
            middleContainer.AddChild(middleColumn);

            #region Name

            {
                var panel = HighlightedContainer();
                var hBox = new HBoxContainer
                {
                    SizeFlagsVertical = SizeFlags.FillExpand
                };
                var nameLabel = new Label {Text = localization.GetString("Name:")};
                _nameEdit = new LineEdit
                {
                    CustomMinimumSize = (270, 0),
                    SizeFlagsVertical = SizeFlags.ShrinkCenter
                };
                _nameEdit.OnTextChanged += args =>
                {
                    _profile = _profile.WithName(args.Text);
                    IsDirty = true;
                };
                var nameRandomButton = new Button {Text = localization.GetString("Randomize")};
                hBox.AddChild(nameLabel);
                hBox.AddChild(_nameEdit);
                hBox.AddChild(nameRandomButton);
                panel.AddChild(hBox);
                middleColumn.AddChild(panel);
            }

            #endregion Name

            var sexAndAgeRow = new HBoxContainer
            {
                SeparationOverride = 10
            };
            middleColumn.AddChild(sexAndAgeRow);

            #region Sex

            {
                var panel = HighlightedContainer();
                var hBox = new HBoxContainer();
                var sexLabel = new Label {Text = localization.GetString("Sex:")};

                var sexButtonGroup = new ButtonGroup();

                _sexMaleButton = new Button
                {
                    Text = localization.GetString("Male"),
                    Group = sexButtonGroup
                };
                _sexMaleButton.OnPressed += args =>
                {
                    _profile = _profile.WithSex(Sex.Male);
                    IsDirty = true;
                };
                _sexFemaleButton = new Button
                {
                    Text = localization.GetString("Female"),
                    Group = sexButtonGroup
                };
                _sexFemaleButton.OnPressed += args =>
                {
                    _profile = _profile.WithSex(Sex.Female);
                    IsDirty = true;
                };
                hBox.AddChild(sexLabel);
                hBox.AddChild(_sexMaleButton);
                hBox.AddChild(_sexFemaleButton);
                panel.AddChild(hBox);
                sexAndAgeRow.AddChild(panel);
            }
            #endregion Sex

            #region Age

            {
                var panel = HighlightedContainer();
                var hBox = new HBoxContainer();
                var ageLabel = new Label {Text = localization.GetString("Age:")};
                _ageEdit = new LineEdit {CustomMinimumSize = (40, 0)};
                _ageEdit.OnTextChanged += args =>
                {
                    if (!int.TryParse(args.Text, out var newAge))
                        return;
                    _profile = _profile.WithAge(newAge);
                    IsDirty = true;
                };
                hBox.AddChild(ageLabel);
                hBox.AddChild(_ageEdit);
                panel.AddChild(hBox);
                sexAndAgeRow.AddChild(panel);
            }

            #endregion Age

            var rightColumn = new VBoxContainer();
            middleContainer.AddChild(rightColumn);

            #region Import/Export

            {
                var panelContainer = HighlightedContainer();
                var hBox = new HBoxContainer();
                var importButton = new Button
                {
                    Text = localization.GetString("Import"),
                    Disabled = true,
                    ToolTip = "Not yet implemented!"
                };
                var exportButton = new Button
                {
                    Text = localization.GetString("Export"),
                    Disabled = true,
                    ToolTip = "Not yet implemented!"
                };
                hBox.AddChild(importButton);
                hBox.AddChild(exportButton);
                panelContainer.AddChild(hBox);
                rightColumn.AddChild(panelContainer);
            }
            #endregion Import/Export

            #region Save

            {
                var panel = HighlightedContainer();
                _saveButton = new Button {
                    Text = localization.GetString("Save"),
                    SizeFlagsHorizontal = SizeFlags.ShrinkCenter
                };
                _saveButton.OnPressed += args =>
                {
                    IsDirty = false;
                    _preferencesManager.UpdateCharacter(_profile, _characterSlot);
                    OnProfileChanged?.Invoke(_profile);
                };
                panel.AddChild(_saveButton);
                rightColumn.AddChild(panel);
            }

            #endregion Save

            #region Hair

            {
                var panel = HighlightedContainer();
                panel.SizeFlagsHorizontal = SizeFlags.None;
                var hairVBox = new VBoxContainer();
                var hairButtonGroup = new ButtonGroup();
                var hairButtonHBox = new HBoxContainer();
                var hairButton = new Button
                {
                    Text = localization.GetString("Hair"),
                    Group = hairButtonGroup,
                    Pressed = true
                };
                var facialHairButton = new Button
                {
                    Text = localization.GetString("Facial Hair"),
                    Group = hairButtonGroup
                };

                var hairPanel = new HairPickerPanel(resourceCache, localization);
                hairPanel.Populate(HairType.Hair);

                hairButton.OnPressed += args => { hairPanel.Populate(HairType.Hair); };
                facialHairButton.OnPressed += args => { hairPanel.Populate(HairType.FacialHair); };

                hairButtonHBox.AddChild(hairButton);
                hairButtonHBox.AddChild(facialHairButton);

                hairVBox.AddChild(hairButtonHBox);

                hairVBox.AddChild(hairPanel);

                panel.AddChild(hairVBox);
                vBox.AddChild(panel);
            }

            #endregion Hair

            UpdateControls();
        }

        private void UpdateSexControls()
        {
            _sexMaleButton.Pressed = _profile.Sex == Sex.Male;
            _sexFemaleButton.Pressed = _profile.Sex == Sex.Female;
        }

        private void UpdateSaveButton()
        {
            _saveButton.Disabled = !IsDirty;
        }

        private void UpdateControls()
        {
            _nameEdit.Text = _profile.Name;
            UpdateSexControls();
            _ageEdit.Text = _profile.Age.ToString();
            UpdateSaveButton();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _hairPickerWindow.Dispose();
            _facialHairPickerWindow.Dispose();
        }

        public event Action<HumanoidCharacterProfile> OnProfileChanged;
    }
}
