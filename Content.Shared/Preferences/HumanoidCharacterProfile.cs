#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Content.Shared.GameTicking;
using Content.Shared.Roles;
using Content.Shared.Text;
using Robust.Shared.Interfaces.Random;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Localization.Macros;
using Robust.Shared.Localization;

namespace Content.Shared.Preferences
{
    /// <summary>
    /// Character profile. Looks immutable, but uses non-immutable semantics internally for serialization/code sanity purposes.
    /// </summary>
    [Serializable, NetSerializable]
    public record HumanoidCharacterProfile(
        string Name,
        int Age,
        Sex Sex,
        Gender Gender,
        HumanoidCharacterAppearance HumanoidAppearance,
        ClothingPreference Clothing,
        BackpackPreference Backpack,
        IImmutableDictionary<string, JobPriority> JobPriorities,
        PreferenceUnavailableMode PreferenceUnavailable,
        IImmutableList<string> AntagPreferences) : ICharacterProfile, IGenderable
    {
        public ICharacterAppearance CharacterAppearance => HumanoidAppearance;
        public static int MinimumAge = 18;
        public static int MaximumAge = 120;
        public static int MaxNameLength = 32;

        public static HumanoidCharacterProfile Default()
        {
            return Random();
        }

        public static HumanoidCharacterProfile Random()
        {
            var random = IoCManager.Resolve<IRobustRandom>();
            var sex = random.Prob(0.5f) ? Sex.Male : Sex.Female;
            var gender = sex == Sex.Male ? Gender.Male : Gender.Female;

            var firstName = random.Pick(sex == Sex.Male
                ? Names.MaleFirstNames
                : Names.FemaleFirstNames);
            var lastName = random.Pick(Names.LastNames);
            var name = $"{firstName} {lastName}";
            var age = random.Next(MinimumAge, MaximumAge);

            return new HumanoidCharacterProfile(name, age, sex, gender, HumanoidCharacterAppearance.Random(sex), ClothingPreference.Jumpsuit, BackpackPreference.Backpack,
                new Dictionary<string, JobPriority>
                {
                    {SharedGameTicker.OverflowJob, JobPriority.High}
                }.ToImmutableDictionary(), PreferenceUnavailableMode.StayInLobby, new List<string>().ToImmutableList());
        }

        /// <summary>
        ///     Makes this profile valid so there's no bad data like negative ages.
        /// </summary>
        public static HumanoidCharacterProfile EnsureValid(
            HumanoidCharacterProfile profile,
            IPrototypeManager prototypeManager)
        {
            var age = Math.Clamp(profile.Age, MinimumAge, MaximumAge);
            var sex = profile.Sex switch
            {
                Sex.Male => Sex.Male,
                Sex.Female => Sex.Female,
                _ => Sex.Male // Invalid enum values.
            };
            var gender = profile.Gender switch
            {
                Gender.Epicene => Gender.Epicene,
                Gender.Female => Gender.Female,
                Gender.Male => Gender.Male,
                Gender.Neuter => Gender.Neuter,
                _ => Gender.Epicene // Invalid enum values.
            };

            string name;
            if (string.IsNullOrEmpty(profile.Name))
            {
                name = "John Doe";
            }
            else if (profile.Name.Length > MaxNameLength)
            {
                name = profile.Name[..MaxNameLength];
            }
            else
            {
                name = profile.Name;
            }

            // TODO: Avoid Z̨͇̙͉͎̭͔̼̿͋A͚̖̞̗̞͈̓̾̀ͩͩ̔L̟ͮ̈͝G̙O͍͎̗̺̺ͫ̀̽͊̓͝ͅ tier shenanigans.
            // And other stuff like RTL overrides and such.
            // Probably also emojis...

            name = name.Trim();

            var appearance = HumanoidCharacterAppearance.EnsureValid(profile.CharacterAppearance as HumanoidCharacterAppearance);

            var prefsUnavailableMode = profile.PreferenceUnavailable switch
            {
                PreferenceUnavailableMode.StayInLobby => PreferenceUnavailableMode.StayInLobby,
                PreferenceUnavailableMode.SpawnAsOverflow => PreferenceUnavailableMode.SpawnAsOverflow,
                _ => PreferenceUnavailableMode.StayInLobby // Invalid enum values.
            };

            var clothing = profile.Clothing switch
            {
                ClothingPreference.Jumpsuit => ClothingPreference.Jumpsuit,
                ClothingPreference.Jumpskirt => ClothingPreference.Jumpskirt,
                _ => ClothingPreference.Jumpsuit // Invalid enum values.
            };

            var backpack = profile.Backpack switch
            {
                BackpackPreference.Backpack => BackpackPreference.Backpack,
                BackpackPreference.Satchel => BackpackPreference.Satchel,
                BackpackPreference.Duffelbag => BackpackPreference.Duffelbag,
                _ => BackpackPreference.Backpack // Invalid enum values.
            };

            var priorities = new Dictionary<string, JobPriority>(profile.JobPriorities
                .Where(p => prototypeManager.HasIndex<JobPrototype>(p.Key) && p.Value switch
                {
                    JobPriority.Never => false, // Drop never since that's assumed default.
                    JobPriority.Low => true,
                    JobPriority.Medium => true,
                    JobPriority.High => true,
                    _ => false
                })).ToImmutableDictionary();

            var antags = profile.AntagPreferences
                .Where(prototypeManager.HasIndex<AntagPrototype>)
                .ToImmutableList();

            return new HumanoidCharacterProfile(name, age, sex, gender, appearance, clothing, backpack, priorities, prefsUnavailableMode, antags);
        }

        public HumanoidCharacterProfile WithAntagPreference(string antagId, bool pref)
        {
            if(pref)
            {
                if(!AntagPreferences.Contains(antagId))
                {
                    return this with { AntagPreferences = AntagPreferences.Add(antagId)};
                }
            }
            else
            {
                return this with {AntagPreferences = AntagPreferences.Remove(antagId)};
            }

            return this;
        }


        public string Summary =>
            Loc.GetString("{0}, {1} years old human. {2:Their} pronouns are {2:they}/{2:them}.", Name, Age, this);

        public bool MemberwiseEquals(ICharacterProfile maybeOther)
        {
            if (maybeOther is not HumanoidCharacterProfile other) return false;
            if (Name != other.Name) return false;
            if (Age != other.Age) return false;
            if (Sex != other.Sex) return false;
            if (Gender != other.Gender) return false;
            if (PreferenceUnavailable != other.PreferenceUnavailable) return false;
            if (Clothing != other.Clothing) return false;
            if (Backpack != other.Backpack) return false;
            if (!JobPriorities.SequenceEqual(other.JobPriorities)) return false;
            if (!AntagPreferences.SequenceEqual(other.AntagPreferences)) return false;
            return CharacterAppearance.MemberwiseEquals(other.CharacterAppearance);
        }
    }
}
