using System;
using Content.Shared.Preferences.Appearance;
using Robust.Shared.Serialization;
using ObjectSerializer = Robust.Shared.Serialization.ObjectSerializer;

namespace Content.Shared.Preferences.Profiles
{
    [Serializable, NetSerializable]
    public class HumanoidCharacterProfile : ICharacterProfile
    {
        public string Name;
        public ICharacterAppearance CharacterAppearance;

        public int Age;

        public Gender Gender;

        string ICharacterProfile.Name() => Name;
        int ICharacterProfile.Age() => Age;
        Gender ICharacterProfile.Gender() => Gender;

        ICharacterAppearance ICharacterProfile.CharacterAppearance() => CharacterAppearance;
        public void ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref Name, "name", string.Empty);
            serializer.DataField(ref CharacterAppearance, "appearance", null);
            serializer.DataField(ref Age, "age", 18);
        }
    }
}
