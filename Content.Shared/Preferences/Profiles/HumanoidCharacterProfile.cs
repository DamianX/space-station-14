using System.Security.Cryptography;
using Content.Shared.Preferences.Appearance;

namespace Content.Shared.Preferences.Profiles
{
    public class HumanoidCharacterProfile : ICharacterProfile
    {
        public string Name { get; set; }
        public ICharacterAppearance CharacterAppearance { get; set; }

        public int Age { get; set; }

        string ICharacterProfile.Name() => Name;
        ICharacterAppearance ICharacterProfile.CharacterAppearance() => CharacterAppearance;
    }
}
