using System.Text;
using Robust.Client.GameObjects.Components.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects.Components.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using static Content.Shared.GameObjects.Components.Sound.SharedJukeboxComponent;

namespace Content.Client.GameObjects.Components.Sound
{
    public class JukeboxBoundUserInterface : BoundUserInterface
    {
#pragma warning disable 649
        [Dependency] private readonly ILocalizationManager _localizationManager;
#pragma warning restore 649
        public JukeboxBoundUserInterface(ClientUserInterfaceComponent owner, object uiKey) : base(owner, uiKey)
        {
        }

        private JukeboxWindow _window;

        protected override void Open()
        {
            base.Open();

            _window = new JukeboxWindow(this, _localizationManager)
            {
                Title = Owner.Owner.Name,
            };
            _window.OnClose += Close;
            _window.OpenCentered();
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            _window.Populate((JukeboxBoundUserInterfaceState) state);
        }

        public void RequestSong(int song)
        {
            SendMessage(new JukeboxPlayRequest(song));
        }

        public void RequestStop()
        {
            SendMessage(new JukeboxStopRequest());
        }
    }

    public class JukeboxWindow : SS14Window
    {
        private JukeboxBoundUserInterface _owner;
        private readonly ILocalizationManager _localizationManager;
        private readonly Label _currentSongLabel;
        private readonly Button _stopButton;
        private readonly ItemList _availableSongsItemList;
        public JukeboxWindow(JukeboxBoundUserInterface owner,
            ILocalizationManager localizationManager)
        {
            _owner = owner;
            _localizationManager = localizationManager;
            var vBox = new VBoxContainer()
            {
                SizeFlagsVertical = SizeFlags.FillExpand,
                SeparationOverride = 5,
            };
            vBox.SetAnchorAndMarginPreset(LayoutPreset.Wide);


            vBox.AddChild(_currentSongLabel = new Label());

            vBox.AddChild(_stopButton = new Button()
            {
                Text = "Stop",
                SizeFlagsHorizontal = SizeFlags.None,
            });

            _stopButton.OnPressed += _ => owner.RequestStop();

            vBox.AddChild(_availableSongsItemList = new ItemList
            {
                SizeFlagsVertical = SizeFlags.FillExpand,
                SizeFlagsStretchRatio = 3
            });

            Contents.AddChild(vBox);
            _availableSongsItemList.OnItemSelected += SongSelected;
        }

        private void SongSelected(ItemList.ItemListSelectedEventArgs obj)
        {
            _owner.RequestSong(obj.ItemIndex);
        }

        public void Populate(JukeboxBoundUserInterfaceState state)
        {
            var currentSongText = new StringBuilder(_localizationManager.GetString("Current song: "));
            currentSongText.Append(state?.CurrentSong ?? _localizationManager.GetString("NONE"));
            _currentSongLabel.Text = currentSongText.ToString();

            _availableSongsItemList.Clear();
            foreach (var song in state.AvailableSongs)
                _availableSongsItemList.AddItem(song);
        }
    }
}
