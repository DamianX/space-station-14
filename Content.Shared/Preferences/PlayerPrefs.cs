using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Preferences.Profiles;
using Robust.Shared.Interfaces.Serialization;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
    [Serializable, NetSerializable]
    public sealed class PlayerPrefs : IExposeData
    {
        public List<ICharacterProfile> Characters;
        public int SelectedCharacterIndex;

        public ICharacterProfile SelectedCharacter => Characters.ElementAtOrDefault(SelectedCharacterIndex);

        public void ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref Characters, "characters", new List<ICharacterProfile>(), true);
            serializer.DataField(ref SelectedCharacterIndex, "selectedCharacter", 0, true);
        }
    }
}
