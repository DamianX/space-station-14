using System;
using Content.Shared.Preferences.Appearance;
using Robust.Shared.Interfaces.Serialization;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences.Profiles
{
    [Serializable, NetSerializable]
    public enum CharacterVisuals
    {
        CharacterAppearance
    }

    public interface ICharacterProfile : IExposeData
    {
        string Name { get; set; }
        int Age { get; set; }
        Gender Gender { get; set; }
        ICharacterAppearance CharacterAppearance { get; set; }
    }
}
