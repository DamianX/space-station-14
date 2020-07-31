using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database
{
    public class PrefsDb
    {
        private readonly IDatabaseConfiguration _dbConfig;

        private PreferencesDbContext PrefsCtx =>
            _dbConfig switch
            {
                SqliteConfiguration sqlite => new SqlitePreferencesDbContext(
                    sqlite.Options),
                PostgresConfiguration postgres => new PostgresPreferencesDbContext(postgres.Options),
                _ => throw new NotImplementedException()
            };

        public PrefsDb(IDatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig;
            PrefsCtx.Database.Migrate();
        }

        public Prefs GetPlayerPreferences(string username)
        {
            return PrefsCtx
                .Preferences
                .Include(p => p.HumanoidProfiles)
                .ThenInclude(h => h.Jobs)
                .SingleOrDefault(p => p.Username == username);
        }

        public void SaveSelectedCharacterIndex(string username, int slot)
        {
            var prefs = PrefsCtx.Preferences.SingleOrDefault(p => p.Username == username);
            if (prefs is null)
            {
                PrefsCtx.Preferences.Add(new Prefs
                {
                    Username = username,
                    SelectedCharacterSlot = slot
                });
            }
            else
            {
                prefs.SelectedCharacterSlot = slot;
            }

            PrefsCtx.SaveChanges();
        }

        public void SaveCharacterSlot(string username, HumanoidProfile newProfile)
        {
            var prefs = PrefsCtx
                .Preferences
                .Single(p => p.Username == username);
            var oldProfile = prefs
                .HumanoidProfiles
                .SingleOrDefault(h => h.Slot == newProfile.Slot);
            if (!(oldProfile is null)) prefs.HumanoidProfiles.Remove(oldProfile);
            prefs.HumanoidProfiles.Add(newProfile);
            PrefsCtx.SaveChanges();
        }

        public void DeleteCharacterSlot(string username, int slot)
        {
            PrefsCtx
                .Preferences
                .Single(p => p.Username == username)
                .HumanoidProfiles
                .RemoveAll(h => h.Slot == slot);
            PrefsCtx.SaveChanges();
        }
    }
}
