using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Preferences
{
    public class PreferencesPage : BasePreferencesPage
    {
        public PreferencesPage(ILocalizationManager localization)
        {
            var vBox = new VBoxContainer
            {
                MarginTop = 10,
                MarginLeft = 10
            };
            AddChild(vBox);

            var hearMusicHBox = new HBoxContainer
            {
                SeparationOverride = 8
            };
            vBox.AddChild(hearMusicHBox);

            var hearMusicLabel = new Label
            {
                Text = localization.GetString("Hear music:")
            };
            hearMusicHBox.AddChild(hearMusicLabel);

            var hearMusicYes = new Button
            {
                Text = localization.GetString("Yes"),
                ToggleMode = true,
            };
            hearMusicHBox.AddChild(hearMusicYes);

            var hearMusicNo = new Button
            {
                Text = localization.GetString("No"),
                ToggleMode = true,
            };
            hearMusicHBox.AddChild(hearMusicNo);

        }
    }
}
