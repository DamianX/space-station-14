using Content.Shared.Preferences.Appearance;
using Robust.Shared.Interfaces.Serialization;

namespace Content.Shared.Preferences.Profiles
{
    public interface ICharacterProfile : IExposeData
    {
        string Name();
        int Age();
        Gender Gender();
        ICharacterAppearance CharacterAppearance();
    }
}
