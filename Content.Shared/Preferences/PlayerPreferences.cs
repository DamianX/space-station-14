using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Robust.Shared.Interfaces.Serialization;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
    /// <summary>
    /// Contains all player characters and the index of the currently selected character.
    /// Serialized both over the network and to disk.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class PlayerPreferences
    {
        public static PlayerPreferences Default()
        {
            return new PlayerPreferences(new List<ICharacterProfile>
                {
                    HumanoidCharacterProfile.Default()
                },
                0);
        }

        public PlayerPreferences(IEnumerable<ICharacterProfile> characters, int selectedCharacterIndex)
        {
            _characters = characters.ToList();
            SelectedCharacterIndex = selectedCharacterIndex;
        }

        private List<ICharacterProfile> _characters;

        /// <summary>
        /// All player characters.
        /// </summary>
        public IEnumerable<ICharacterProfile> Characters => _characters.AsEnumerable();

        /// <summary>
        /// Index of the currently selected character.
        /// </summary>
        public int SelectedCharacterIndex { get; }

        /// <summary>
        /// Retrieves the currently selected character.
        /// </summary>
        public ICharacterProfile SelectedCharacter => Characters.ElementAtOrDefault(SelectedCharacterIndex);
    }
}
