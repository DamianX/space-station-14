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
            serializer.DataField(ref EyeColor, "eyeColor", Color.Black, true);
            serializer.DataField(ref HairColor, "hairColor", Color.Black, true);
            serializer.DataField(ref FacialHairColor, "facialHairColor", Color.Black, true);
            serializer.DataField(ref SkinColor, "skinColor", Color.Pink, true);
            serializer.DataField(ref HairPrototypeId, "hairStyle", string.Empty, true);
            serializer.DataField(ref FacialHairPrototypeId, "facialHairStyle", string.Empty, true);
        }
    }
}
