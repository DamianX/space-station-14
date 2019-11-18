using System;
using System.Collections.Generic;
using Content.Shared.Preferences.Profiles;
using Robust.Shared.Interfaces.Serialization;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
    [Serializable, NetSerializable]
    public sealed class PlayerPrefs : IExposeData
    {
        public List<ICharacterProfile> Characters;
        public int MaxCharacters;
        public int SelectedCharacter;

        public void ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref Characters, "characters", new List<ICharacterProfile>());
            serializer.DataField(ref MaxCharacters, "maxCharacters", 10);
            serializer.DataField(ref SelectedCharacter, "selectedCharacter", 0);
        }
    }
}
