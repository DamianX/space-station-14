using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.UserInterface;
using Robust.Shared.Serialization;

namespace Content.Shared.GameObjects.Components.Sound
{
    public class SharedJukeboxComponent : Component
    {
        public override string Name => "Jukebox";

        [Serializable, NetSerializable]
        public class JukeboxPlayRequest : BoundUserInterfaceMessage
        {
            public readonly int SongIndex;

            public JukeboxPlayRequest(int songIndex)
            {
                SongIndex = songIndex;
            }
        }

        [Serializable, NetSerializable]
        public class JukeboxStopRequest : BoundUserInterfaceMessage
        {
        }

        [Serializable, NetSerializable]
        public class JukeboxBoundUserInterfaceState : BoundUserInterfaceState
        {
            public readonly string CurrentSong;
            public readonly string[] AvailableSongs;

            public JukeboxBoundUserInterfaceState(string currentSong, string[] availableSongs)
            {
                CurrentSong = currentSong;
                AvailableSongs = availableSongs;
            }
        }

        [Serializable, NetSerializable]
        public enum JukeboxUiKey
        {
            Key
        }

        [Serializable, NetSerializable]
        public enum JukeboxVisuals
        {
            Status
        }

        [Serializable, NetSerializable]
        public enum JukeboxStatus
        {
            Normal,
            Off,
            Broken,
            Running,
            Emagged,
        }

    }
}
