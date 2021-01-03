using System;
using System.Linq;
using Content.Shared.Preferences.Appearance;
using Robust.Shared.Interfaces.Random;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences
{
    [Serializable, NetSerializable]
    public record HumanoidCharacterAppearance(
        string HairStyleName,
        Color HairColor,
        string FacialHairStyleName,
        Color FacialHairColor,
        Color EyeColor,
        Color SkinColor)
        : ICharacterAppearance
    {

        public static HumanoidCharacterAppearance Default()
        {
            return new(
                "Bald",
                Color.Black,
                "Shaved",
                Color.Black,
                Color.Black,
                Color.FromHex("#C0967F")
            );
        }

        public static HumanoidCharacterAppearance Random(Sex sex)
        {
            var random = IoCManager.Resolve<IRobustRandom>();

            var newHairStyle = random.Pick(HairStyles.HairStylesMap.Keys.ToList());

            var newFacialHairStyle = sex == Sex.Female
                ? HairStyles.DefaultFacialHairStyle
                : random.Pick(HairStyles.FacialHairStylesMap.Keys.ToList());

            var newHairColor = random.Pick(HairStyles.RealisticHairColors);
            newHairColor = newHairColor
                .WithRed(RandomizeColor(newHairColor.R))
                .WithGreen(RandomizeColor(newHairColor.G))
                .WithBlue(RandomizeColor(newHairColor.B));

            // TODO: Add random eye and skin color
            return new HumanoidCharacterAppearance(newHairStyle, newHairColor, newFacialHairStyle, newHairColor, Color.Black, Color.FromHex("#C0967F"));

            float RandomizeColor(float channel)
            {
                return MathHelper.Clamp01(channel + random.Next(-25, 25) / 100f);
            }
        }

        public static Color ClampColor(Color color)
        {
            return new(color.RByte, color.GByte, color.BByte);
        }

        public static HumanoidCharacterAppearance EnsureValid(HumanoidCharacterAppearance appearance)
        {
            string hairStyleName;
            if (!HairStyles.HairStylesMap.ContainsKey(appearance.HairStyleName))
            {
                hairStyleName = HairStyles.DefaultHairStyle;
            }
            else
            {
                hairStyleName = appearance.HairStyleName;
            }

            string facialHairStyleName;
            if (!HairStyles.FacialHairStylesMap.ContainsKey(appearance.FacialHairStyleName))
            {
                facialHairStyleName = HairStyles.DefaultFacialHairStyle;
            }
            else
            {
                facialHairStyleName = appearance.FacialHairStyleName;
            }

            var hairColor = ClampColor(appearance.HairColor);
            var facialHairColor = ClampColor(appearance.FacialHairColor);
            var eyeColor = ClampColor(appearance.EyeColor);
            var skinColor = ClampColor(appearance.SkinColor);

            return new HumanoidCharacterAppearance(
                hairStyleName,
                hairColor,
                facialHairStyleName,
                facialHairColor,
                eyeColor,
                skinColor);
        }

        public bool MemberwiseEquals(ICharacterAppearance maybeOther)
        {
            if (maybeOther is not HumanoidCharacterAppearance other) return false;
            if (HairStyleName != other.HairStyleName) return false;
            if (!HairColor.Equals(other.HairColor)) return false;
            if (FacialHairStyleName != other.FacialHairStyleName) return false;
            if (!FacialHairColor.Equals(other.FacialHairColor)) return false;
            if (!EyeColor.Equals(other.EyeColor)) return false;
            if (!SkinColor.Equals(other.SkinColor)) return false;
            return true;
        }
    }
}
