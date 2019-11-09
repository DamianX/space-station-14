using System.Collections.Generic;
using Content.Shared.Preferences.Profiles;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
    public sealed class PlayerPrefs
    {
        public List<ICharacterProfile> Characters { get; set; }
    }
}
