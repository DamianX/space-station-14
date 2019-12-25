using Content.Client.GameObjects.Components;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface
{
    public class HumanoidProfileEditor : SS14Window
    {
        private readonly IClientPreferencesManager _preferencesManager;
        private readonly HumanoidCharacterProfile _profile;
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

        public HumanoidProfileEditor(ILocalizationManager localization,
            IResourceCache resourceCache,
            IClientPreferencesManager preferencesManager)
        {
            Title = "Character editor";
            _profile = (HumanoidCharacterProfile) preferencesManager.Preferences.SelectedCharacter;
            _characterSlot = preferencesManager.Preferences.SelectedCharacterIndex;
            _preferencesManager = preferencesManager;
            _hairPickerWindow = new HairPickerWindow(resourceCache, localization);
            _hairPickerWindow.Populate();
            _hairPickerWindow.OnHairStylePicked += newStyle =>
            {
                _profile.CharacterAppearance.HairStyleName = newStyle;
                IsDirty = true;
            };
            _hairPickerWindow.OnHairColorPicked += newColor =>
            {
                _profile.CharacterAppearance.HairColor = newColor;
                IsDirty = true;
            };
            _facialHairPickerWindow = new FacialHairPickerWindow(resourceCache, localization);
            _facialHairPickerWindow.Populate();
            _facialHairPickerWindow.OnHairStylePicked += newStyle =>
            {
                _profile.CharacterAppearance.FacialHairStyleName = newStyle;
                IsDirty = true;
            };
            _facialHairPickerWindow.OnHairColorPicked += newColor =>
            {
                _profile.CharacterAppearance.FacialHairColor = newColor;
                IsDirty = true;
            };

            var vBox = new VBoxContainer();
            Contents.AddChild(vBox);

            #region Name

            {
                var hBox = new HBoxContainer();
                var nameLabel = new Label {Text = localization.GetString("Name:")};
                _nameEdit = new LineEdit {SizeFlagsHorizontal = SizeFlags.FillExpand};
                _nameEdit.OnTextChanged += args =>
                {
                    _profile.Name = args.Text;
                    IsDirty = true;
                };
                hBox.AddChild(nameLabel);
                hBox.AddChild(_nameEdit);
                vBox.AddChild(hBox);
            }

            #endregion Name

            #region Sex

            {
                var hBox = new HBoxContainer();
                var sexLabel = new Label {Text = localization.GetString("Sex:")};
                _sexMaleButton = new Button {Text = localization.GetString("Male")};
                _sexMaleButton.OnPressed += args =>
                {
                    _profile.Sex = Sex.Male;
                    IsDirty = true;
                    UpdateSexControls();
                };
                _sexFemaleButton = new Button {Text = localization.GetString("Female")};
                _sexFemaleButton.OnPressed += args =>
                {
                    _profile.Sex = Sex.Female;
                    IsDirty = true;
                    UpdateSexControls();
                };
                hBox.AddChild(sexLabel);
                hBox.AddChild(_sexMaleButton);
                hBox.AddChild(_sexFemaleButton);
                vBox.AddChild(hBox);
            }

            #endregion Sex

            #region Age

            {
                var hBox = new HBoxContainer();
                var ageLabel = new Label {Text = localization.GetString("Age:")};
                _ageEdit = new LineEdit {SizeFlagsHorizontal = SizeFlags.FillExpand};
                _ageEdit.OnTextChanged += args =>
                {
                    if (!int.TryParse(args.Text, out var newAge))
                        return;
                    _profile.Age = newAge;
                    IsDirty = true;
                };
                hBox.AddChild(ageLabel);
                hBox.AddChild(_ageEdit);
                vBox.AddChild(hBox);
            }

            #endregion Age

            #region Hair

            {
                var hairButton = new Button {Text = localization.GetString("Customize hair")};
                hairButton.OnPressed += args => { _hairPickerWindow.Open(); };
                var facialHairButton = new Button {Text = localization.GetString("Customize facial hair")};
                facialHairButton.OnPressed += args => { _facialHairPickerWindow.Open(); };
                vBox.AddChild(hairButton);
                vBox.AddChild(facialHairButton);
            }

            #endregion Hair

            #region Save

            {
                _saveButton = new Button {Text = localization.GetString("Save")};
                _saveButton.OnPressed += args =>
                {
                    IsDirty = false;
                    _preferencesManager.UpdateCharacter(_profile, _characterSlot);
                };
                vBox.AddChild(_saveButton);
            }

            UpdateControls();

            #endregion Save
        }

        private void UpdateSexControls()
        {
            _sexMaleButton.Disabled = _profile.Sex == Sex.Male;
            _sexFemaleButton.Disabled = _profile.Sex == Sex.Female;
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
    }
}
