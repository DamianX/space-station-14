using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Preferences.Character
{
    public class SlotsPage : Panel
    {
        private List<(Label, Button, Button)> _buttons = new List<(Label, Button, Button)>();
        private VBoxContainer _vBox;
        private ILocalizationManager _localization;
        private IClientPreferencesManager _prefsMgr;
        public SlotsPage(ILocalizationManager localization, IClientPreferencesManager prefsMgr)
        {
            _localization = localization;
            _prefsMgr = prefsMgr;

            _vBox = new VBoxContainer
            {
                MarginTop = 10,
                MarginLeft = 10
            };
            AddChild(_vBox);

            var prefs = prefsMgr.Get();

            var index = 0;
            for (var i = 0; i < _prefsMgr.MaxCharacterSlots(); i++)
            {
                var characterHBox = new HBoxContainer();
                _vBox.AddChild(characterHBox);

                var nameLabel = new Label();
                characterHBox.AddChild(nameLabel);

                var loadButton = new Button
                {
                    Text = _localization.GetString("Select")
                };

                var indexCopy = index;
                loadButton.OnPressed += args =>
                {
                    prefs.SelectedCharacterIndex = indexCopy;
                    _prefsMgr.Save();
                    UpdateButtons(prefs);
                };
                characterHBox.AddChild(loadButton);

                var deleteButton = new Button
                {
                    Text = _localization.GetString("Delete")
                };
                characterHBox.AddChild(deleteButton);
                _buttons.Add((nameLabel, loadButton, deleteButton));
                index++;
            }

            UpdateButtons(prefs);
        }

        private void UpdateButtons(PlayerPrefs prefs)
        {
            var index = 0;
            foreach (var (label, load, delete) in _buttons)
            {
                var character = prefs.Characters.ElementAtOrDefault(index);
                label.Text = character?.Name ??
                             string.Format(_localization.GetString("SLOT {0} EMPTY"), index + 1);
                load.Visible = prefs.SelectedCharacterIndex != index;
                delete.Visible = character != null;
                index++;
            }
        }
    }
}
