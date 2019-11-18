using System.Linq;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Profiles;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Preferences.Character
{
    public class IdentityPage : Panel
    {
        private LineEdit _nameEdit;
        private Button _genderMaleButton;
        private Button _genderFemaleButton;

        public IdentityPage(ILocalizationManager localization, IClientPreferencesManager prefsMgr)
        {
            var vBox = new VBoxContainer
            {
                MarginTop = 10,
                MarginLeft = 10
            };
            AddChild(vBox);

            var nameHBox = new HBoxContainer
            {
                SizeFlagsHorizontal = SizeFlags.FillExpand
            };
            vBox.AddChild(nameHBox);

            var nameLabel = new Label{Text = localization.GetString("Name: ")};
            nameHBox.AddChild(nameLabel);

            _nameEdit = new LineEdit
            {
                SizeFlagsHorizontal = SizeFlags.FillExpand
            };
            nameHBox.AddChild(_nameEdit);

            var genderHBox = new HBoxContainer();
            vBox.AddChild(genderHBox);

            var genderLabel = new Label {Text = localization.GetString("Gender: ")};
            genderHBox.AddChild(genderLabel);

            _genderMaleButton = new Button{Text = localization.GetString("Male")};
            genderHBox.AddChild(_genderMaleButton);

            _genderFemaleButton = new Button {Text = localization.GetString("Female")};
            genderHBox.AddChild(_genderFemaleButton);

            var saveButton = new Button
            {
                Text = localization.GetString("Save")
            };
            vBox.AddChild(saveButton);

            var prefs = prefsMgr.Get();
            if (prefs is null)
            {
                return;
            }

            var profile = prefs.Characters.ElementAtOrDefault(prefs.SelectedCharacter);
            UpdateControls(profile);
        }

        private void UpdateControls(ICharacterProfile profile)
        {
            if (profile is null)
            {
                return;
            }

            _nameEdit.Text = profile.Name();
            _genderMaleButton.Disabled = profile.Gender() == Gender.Male;
            _genderFemaleButton.Disabled = profile.Gender() == Gender.Female;
        }
    }
}
