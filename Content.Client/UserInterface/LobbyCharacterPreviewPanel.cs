using Content.Client.GameObjects.Components.Mobs;
using Content.Client.Interfaces;
using Content.Shared.Preferences;
using Robust.Client.Interfaces.GameObjects.Components;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface
{
    public class LobbyCharacterPreviewPanel : Control
    {
        private readonly HumanoidProfileEditor _humanoidProfileEditor;
        private readonly IEntity _previewDummy;

        public LobbyCharacterPreviewPanel(IEntityManager entityManager,
            ILocalizationManager localization,
            IResourceCache resourceCache,
            IClientPreferencesManager preferencesManager)
        {
            _humanoidProfileEditor = new HumanoidProfileEditor(localization, resourceCache, preferencesManager);
            _humanoidProfileEditor.OnProfileChanged += profile =>
            {
                _previewDummy.GetComponent<LooksComponent>().Appearance = profile.Appearance;
            };

            _previewDummy = entityManager.SpawnEntity("HumanMob_Content", GridCoordinates.Nullspace);

            var header = new NanoHeading {Text = localization.GetString("Character setup")};

            var characterEditButton = new Button {Text = localization.GetString("Edit"), SizeFlagsHorizontal = SizeFlags.None};
            characterEditButton.OnPressed += args => { _humanoidProfileEditor.Open(); };
            var characterLoadButton = new Button {Text = localization.GetString("Load"), SizeFlagsHorizontal = SizeFlags.None};
            characterEditButton.OnPressed += args => { new CharacterSetup(localization, preferencesManager, entityManager).OpenCentered(); };

            var summaryLabel = new Label
                {Text = ((HumanoidCharacterProfile) preferencesManager.Preferences.SelectedCharacter).Summary};

            var viewSouth = MakeSpriteView(_previewDummy, Direction.South);
            var viewNorth = MakeSpriteView(_previewDummy, Direction.North);
            var viewWest = MakeSpriteView(_previewDummy, Direction.West);
            var viewEast = MakeSpriteView(_previewDummy, Direction.East);

            var vBox = new VBoxContainer();

            vBox.AddChild(header);

            vBox.AddChild(characterEditButton);
            vBox.AddChild(characterLoadButton);

            vBox.AddChild(summaryLabel);

            var hBox = new HBoxContainer();
            hBox.AddChild(viewSouth);
            hBox.AddChild(viewNorth);
            hBox.AddChild(viewWest);
            hBox.AddChild(viewEast);

            vBox.AddChild(hBox);

            AddChild(vBox);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _humanoidProfileEditor.Dispose();
            }
        }

        private static SpriteView MakeSpriteView(IEntity entity, Direction direction)
        {
            return new SpriteView
            {
                Sprite = entity.GetComponent<ISpriteComponent>(),
                OverrideDirection = direction,
                Scale = (2, 2)
            };
        }
    }
}
