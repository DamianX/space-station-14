using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences.Appearance
{
    [Serializable, NetSerializable]
    public class HumanoidCharacterAppearance : ICharacterAppearance
    {
        public Color EyeColor;
        public Color HairColor;
        public Color FacialHairColor;
        public Color SkinColor;
        public string HairPrototypeId;
        public string FacialHairPrototypeId;
        public void ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref EyeColor, "eyeColor", Color.Black);
            serializer.DataField(ref HairColor, "hairColor", Color.Black);
            serializer.DataField(ref FacialHairColor, "facialHairColor", Color.Black);
            serializer.DataField(ref SkinColor, "skinColor", Color.Pink);
            serializer.DataField(ref HairPrototypeId, "hairStyle", string.Empty);
            serializer.DataField(ref FacialHairPrototypeId, "facialHairStyle", string.Empty);
        }
    }
}
