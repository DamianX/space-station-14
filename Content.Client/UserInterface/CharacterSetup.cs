using System.Collections.Generic;
using Content.Client.GameObjects.Components.Mobs;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Robust.Client.Interfaces.GameObjects.Components;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;

namespace Content.Client.UserInterface
{
    public class CharacterSetup : SS14Window
    {
        private readonly List<IEntity> _previewDummies = new List<IEntity>();

        public CharacterSetup(ILocalizationManager localization,
            IClientPreferencesManager preferencesManager,
            IEntityManager entityManager)
        {
            Title = localization.GetString("Character setup");
            var header = new NanoHeading {Text = localization.GetString("Character setup")};

            var characters = preferencesManager.Preferences.Characters;
            var characterListVBox = new VBoxContainer();
            foreach (var character in characters)
            {
                var characterVBox = new VBoxContainer();
                var characterView = new SpriteView {Scale = (2, 2)};
                if (character?.CharacterAppearance is HumanoidCharacterAppearance appearance)
                {
                    var previewDummy = entityManager.SpawnEntity("HumanMob_Content", GridCoordinates.Nullspace);
                    _previewDummies.Add(previewDummy);
                    previewDummy.GetComponent<LooksComponent>().Appearance = appearance;
                    characterView.Sprite = previewDummy.GetComponent<ISpriteComponent>();
                }

                var characterName = character?.Name ?? "*empty*";
                var characterButton = new Button {Text = characterName};
                characterVBox.AddChild(characterView);
                characterVBox.AddChild(characterButton);
                characterListVBox.AddChild(characterVBox);
            }

            var tabs = new TabContainer();
            tabs.AddChild(new IdentityPage());
            tabs.SetTabTitle(0, "Identity");

            tabs.AddChild(new IdentityPage());
            tabs.SetTabTitle(1, "Appearance");

            tabs.AddChild(new IdentityPage());
            tabs.SetTabTitle(2, "Occupation");

            var vBox = new VBoxContainer();
            vBox.AddChild(header);
            var characterListScroll = new ScrollContainer
            {
                SizeFlagsVertical = SizeFlags.FillExpand
            };
            characterListScroll.AddChild(characterListVBox);
            vBox.AddChild(characterListScroll);
            vBox.AddChild(tabs);

            Contents.AddChild(vBox);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            foreach (var dummy in _previewDummies)
            {
                dummy.Delete();
            }

            _previewDummies.Clear();
        }
    }
}
