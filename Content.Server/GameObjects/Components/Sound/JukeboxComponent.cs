using System.Collections.Generic;
using Content.Server.GameObjects.EntitySystems;
using Content.Shared.GameObjects.Components.Sound;
using Robust.Server.GameObjects;
using Robust.Server.GameObjects.Components.UserInterface;
using Robust.Server.Interfaces.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;

namespace Content.Server.GameObjects.Components.Sound
{
    [RegisterComponent]
    [ComponentReference(typeof(IActivate))]
    public class JukeboxComponent : SharedJukeboxComponent, IActivate
    {
        private AppearanceComponent _appearance;
        private SoundComponent _sound;
        private BoundUserInterface _userInterface;
        private string _currentSong;
        private List<string> _availableSongs = new List<string>();

        public override void Initialize()
        {
            base.Initialize();
            _appearance = Owner.GetComponent<AppearanceComponent>();
            _sound = Owner.GetComponent<SoundComponent>();
            _userInterface = Owner.GetComponent<ServerUserInterfaceComponent>()
                .GetBoundUserInterface(JukeboxUiKey.Key);
            _userInterface.OnReceiveMessage += UiMessageReceived;
            InitializePlaylist();
        }

        private void UiMessageReceived(ServerBoundUserInterfaceMessage obj)
        {
            switch (obj.Message)
            {
                case JukeboxPlayRequest msg:
                    if (msg.SongIndex < 0 || msg.SongIndex > _availableSongs.Count)
                    {
                        Logger.Warning($"Tried to play an invalid song index {msg.SongIndex}");
                        return;
                    }

                    var songName = _availableSongs[msg.SongIndex];
                    Play(songName);
                    break;
                case JukeboxStopRequest _:
                    Stop();
                    break;
            }
        }

        private void Play(string song)
        {
            _sound.StopAllSounds();
            _currentSong = song;
            _sound.Play($"/Audio/Music/{song}");
            UpdateUserInterface();
        }

        private void Stop()
        {
            _currentSong = null;
            _sound.StopAllSounds();
            UpdateUserInterface();
        }

        private void InitializePlaylist()
        {
            _availableSongs.Add("title1.ogg");
            UpdateUserInterface();
        }

        private void UpdateUserInterface()
        {
            _userInterface.SetState(new JukeboxBoundUserInterfaceState(_currentSong, _availableSongs.ToArray()));
        }

        public void Activate(ActivateEventArgs eventArgs)
        {
            if(!eventArgs.User.TryGetComponent(out IActorComponent actor))
            {
                return;
            }
            _userInterface.Open(actor.playerSession);
        }
    }
}
