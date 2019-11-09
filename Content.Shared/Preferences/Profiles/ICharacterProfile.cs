using Content.Shared.Preferences.Appearance;

namespace Content.Shared.Preferences.Profiles
{
    public interface ICharacterProfile
    {
        string Name();
        ICharacterAppearance CharacterAppearance();
    }
}
