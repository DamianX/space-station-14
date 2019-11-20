using System.Linq;
using Content.Client.Interfaces;
using Content.Shared.Preferences.Appearance;
using Content.Shared.Preferences.Profiles;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Preferences.Character
{
    public class IdentityPage : Panel
    {
        private readonly LineEdit _nameEdit;
        private readonly Button _genderMaleButton;
        private readonly Button _genderFemaleButton;

        private bool _isDirty;
        private readonly Button _saveButton;
        private readonly LineEdit _ageEdit;
        private readonly LineEdit _eyeColorEdit;
        private readonly LineEdit _hairColorEdit;
        private readonly LineEdit _facialHairColorEdit;
        private readonly LineEdit _hairStyleEdit;

        public IdentityPage(ILocalizationManager localization, IClientPreferencesManager prefsMgr)
        {
            var prefs = prefsMgr.Get();

            var vBox = new VBoxContainer
            {
                MarginTop = 10,
                MarginLeft = 10
            };
            AddChild(vBox);

            #region Name

            {
                var nameHBox = new HBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                vBox.AddChild(nameHBox);

                var nameLabel = new Label {Text = localization.GetString("Name: ")};
                nameHBox.AddChild(nameLabel);

                _nameEdit = new LineEdit
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                _nameEdit.OnTextChanged += args =>
                {
                    prefs.SelectedCharacter.Name = args.Text;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                nameHBox.AddChild(_nameEdit);
            }

            #endregion Name

            #region Age

            {
                var ageHBox = new HBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                vBox.AddChild(ageHBox);

                var ageLabel = new Label {Text = localization.GetString("Age: ")};
                ageHBox.AddChild(ageLabel);

                _ageEdit = new LineEdit
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                _ageEdit.OnTextChanged += args =>
                {
                    if (!int.TryParse(args.Text, out var integer))
                        return;
                    prefs.SelectedCharacter.Age = integer;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                ageHBox.AddChild(_ageEdit);
            }

            #endregion Age

            #region Gender

            {
                var genderHBox = new HBoxContainer();
                vBox.AddChild(genderHBox);

                var genderLabel = new Label {Text = localization.GetString("Gender: ")};
                genderHBox.AddChild(genderLabel);

                _genderMaleButton = new Button {Text = localization.GetString("Male")};
                _genderMaleButton.OnPressed += args =>
                {
                    prefs.SelectedCharacter.Gender = Gender.Male;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                genderHBox.AddChild(_genderMaleButton);

                _genderFemaleButton = new Button {Text = localization.GetString("Female")};
                _genderFemaleButton.OnPressed += args =>
                {
                    prefs.SelectedCharacter.Gender = Gender.Female;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                genderHBox.AddChild(_genderFemaleButton);
            }

            #endregion Gender

            #region EyeColor

            {
                var eyeColorHBox = new HBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                vBox.AddChild(eyeColorHBox);

                var eyeColorLabel = new Label {Text = localization.GetString("Eye Color: ")};
                eyeColorHBox.AddChild(eyeColorLabel);

                _eyeColorEdit = new LineEdit
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                _eyeColorEdit.OnTextChanged += args =>
                {
                    var color = Color.TryFromHex(args.Text);
                    if (!color.HasValue)
                        return;
                    var appearance = (HumanoidCharacterAppearance) prefs.SelectedCharacter.CharacterAppearance;
                    appearance.EyeColor = color.Value;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                eyeColorHBox.AddChild(_eyeColorEdit);
            }

            #endregion

            #region HairStyle

            {
                var hairStyleHBox = new HBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                vBox.AddChild(hairStyleHBox);

                var hairColorLabel = new Label {Text = localization.GetString("Hair style: ")};
                hairStyleHBox.AddChild(hairColorLabel);

                _hairStyleEdit = new LineEdit
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                _hairStyleEdit.OnTextChanged += args =>
                {
                    var appearance = (HumanoidCharacterAppearance) prefs.SelectedCharacter.CharacterAppearance;
                    appearance.HairPrototypeId = args.Text;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                hairStyleHBox.AddChild(_hairStyleEdit);
            }

            #endregion

            #region HairColor

            {
                var hairColorHBox = new HBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                vBox.AddChild(hairColorHBox);

                var hairColorLabel = new Label {Text = localization.GetString("Eye Color: ")};
                hairColorHBox.AddChild(hairColorLabel);

                _hairColorEdit = new LineEdit
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                _hairColorEdit.OnTextChanged += args =>
                {
                    var color = Color.TryFromHex(args.Text);
                    if (!color.HasValue)
                        return;
                    var appearance = (HumanoidCharacterAppearance) prefs.SelectedCharacter.CharacterAppearance;
                    appearance.HairColor = color.Value;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                hairColorHBox.AddChild(_hairColorEdit);
            }

            #endregion

            #region FacialHairColor

            {
                var facialHairColorHBox = new HBoxContainer
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                vBox.AddChild(facialHairColorHBox);

                var facialHairColorLabel = new Label {Text = localization.GetString("Eye Color: ")};
                facialHairColorHBox.AddChild(facialHairColorLabel);

                _facialHairColorEdit = new LineEdit
                {
                    SizeFlagsHorizontal = SizeFlags.FillExpand
                };
                _facialHairColorEdit.OnTextChanged += args =>
                {
                    var color = Color.TryFromHex(args.Text);
                    if (!color.HasValue)
                        return;
                    var appearance = (HumanoidCharacterAppearance) prefs.SelectedCharacter.CharacterAppearance;
                    appearance.FacialHairColor = color.Value;
                    _isDirty = true;
                    UpdateControls(prefs.SelectedCharacter);
                };
                facialHairColorHBox.AddChild(_facialHairColorEdit);
            }

            #endregion

            _saveButton = new Button
            {
                Text = localization.GetString("Save")
            };
            _saveButton.OnPressed += args =>
            {
                prefsMgr.Save();
                _isDirty = false;
                UpdateControls(prefs.SelectedCharacter);
            };
            vBox.AddChild(_saveButton);

            if (prefs is null)
            {
                return;
            }

            var profile = prefs.Characters.ElementAtOrDefault(prefs.SelectedCharacterIndex);
            UpdateControls(profile);
        }

        private void UpdateControls(ICharacterProfile profile)
        {
            if (profile is null)
            {
                return;
            }

            _nameEdit.Text = profile.Name;
            _ageEdit.Text = profile.Age.ToString();
            _genderMaleButton.Disabled = profile.Gender == Gender.Male;
            _genderFemaleButton.Disabled = profile.Gender == Gender.Female;
            var appearance = (HumanoidCharacterAppearance) profile.CharacterAppearance;
            _eyeColorEdit.Text = appearance.EyeColor.ToHex();
            _hairStyleEdit.Text = appearance.HairPrototypeId;
            _hairColorEdit.Text = appearance.EyeColor.ToHex();
            _facialHairColorEdit.Text = appearance.EyeColor.ToHex();
            _saveButton.Disabled = !_isDirty;
        }
    }
}
