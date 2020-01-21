using System;
using System.IO;
using Content.Server.Database;
using Content.Server.Interfaces;
using Content.Shared.Preferences;
using Robust.Server.Interfaces.Player;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Interfaces.Resources;
using Robust.Shared.IoC;

namespace Content.Server.Preferences
{
    /// <summary>
    ///     Sends <see cref="SharedPreferencesManager.MsgPreferencesAndSettings" /> before the client joins the lobby.
    ///     Receives <see cref="SharedPreferencesManager.MsgSelectCharacter" /> and
    ///     <see cref="SharedPreferencesManager.MsgUpdateCharacter" /> at any time.
    /// </summary>
    public class ServerPreferencesManager : SharedPreferencesManager, IServerPreferencesManager
    {
        private PreferencesDatabase _preferencesDb;

        public void Initialize()
        {
            _netManager.RegisterNetMessage<MsgPreferencesAndSettings>(nameof(MsgPreferencesAndSettings));
            _netManager.RegisterNetMessage<MsgSelectCharacter>(nameof(MsgSelectCharacter),
                HandleSelectCharacterMessage);
            _netManager.RegisterNetMessage<MsgUpdateCharacter>(nameof(MsgUpdateCharacter),
                HandleUpdateCharacterMessage);

            _configuration.RegisterCVar("game.maxcharacterslots", 10);
            _configuration.RegisterCVar("database.prefs_engine", "sqlite");
            _configuration.RegisterCVar("database.prefs_sqlite_dbpath", "preferences.db");
            _configuration.RegisterCVar("database.prefs_pg_host", "localhost");
            _configuration.RegisterCVar("database.prefs_pg_port", 5432);
            _configuration.RegisterCVar("database.prefs_pg_database", "ss14_prefs");
            _configuration.RegisterCVar("database.prefs_pg_username", string.Empty);
            _configuration.RegisterCVar("database.prefs_pg_password", string.Empty);

            var engine = _configuration.GetCVar<string>("database.prefs_engine").ToLower();
            IDatabaseConfiguration dbConfig;
            switch (engine)
            {
                case "sqlite":
                    var configPreferencesDbPath = _configuration.GetCVar<string>("database.prefs_sqlite_dbpath");
                    var finalPreferencesDbPath =
                        Path.Combine(_resourceManager.UserData.RootDir, configPreferencesDbPath);
                    dbConfig = new SqliteConfiguration(
                        finalPreferencesDbPath
                    );
                    break;
                case "postgres":
                    dbConfig = new PostgresConfiguration(
                        _configuration.GetCVar<string>("database.prefs_pg_host"),
                        _configuration.GetCVar<int>("database.prefs_pg_port"),
                        _configuration.GetCVar<string>("database.prefs_pg_database"),
                        _configuration.GetCVar<string>("database.prefs_pg_username"),
                        _configuration.GetCVar<string>("database.prefs_pg_password")
                    );
                    break;
                default:
                    throw new NotImplementedException("Unknown database engine {engine}.");
            }

            var maxCharacterSlots = _configuration.GetCVar<int>("game.maxcharacterslots");

            _preferencesDb = new PreferencesDatabase(dbConfig, maxCharacterSlots);
        }

        public void OnClientConnected(IPlayerSession session)
        {
            var msg = _netManager.CreateNetMessage<MsgPreferencesAndSettings>();
            msg.Preferences = GetPreferences(session.SessionId.Username);
            msg.Settings = new GameSettings
            {
                MaxCharacterSlots = _configuration.GetCVar<int>("game.maxcharacterslots")
            };
            _netManager.ServerSendMessage(msg, session.ConnectedClient);
        }

        /// <summary>
        ///     Retrieves preferences for the given username from storage.
        ///     Creates and saves default preferences if they are not found, then returns them.
        /// </summary>
        public PlayerPreferences GetPreferences(string username)
        {
            var prefs = GetFromSql(username);
            if (prefs is null)
            {
                prefs = PlayerPreferences.Default(); // TODO: Create random character instead
                SavePreferences(prefs, username);
            }

            return prefs;
        }

        /// <summary>
        ///     Saves the given preferences to storage.
        /// </summary>
        public void SavePreferences(PlayerPreferences prefs, string username)
        {
            _preferencesDb.SaveSelectedCharacterIndex(username, prefs.SelectedCharacterIndex);
            var index = 0;
            foreach (var character in prefs.Characters)
            {
                _preferencesDb.SaveCharacterSlot(username, character, index);
                index++;
            }
        }

        private void HandleSelectCharacterMessage(MsgSelectCharacter message)
        {
            _preferencesDb.SaveSelectedCharacterIndex(message.MsgChannel.SessionId.Username,
                message.SelectedCharacterIndex);
        }

        private void HandleUpdateCharacterMessage(MsgUpdateCharacter message)
        {
            _preferencesDb.SaveCharacterSlot(message.MsgChannel.SessionId.Username, message.Profile, message.Slot);
        }

        /// <summary>
        ///     Returns the requested <see cref="PlayerPreferences" /> or null if not found.
        /// </summary>
        private PlayerPreferences GetFromSql(string username)
        {
            return _preferencesDb.GetPlayerPreferences(username);
        }
#pragma warning disable 649
        [Dependency] private readonly IServerNetManager _netManager;
        [Dependency] private readonly IConfigurationManager _configuration;
        [Dependency] private readonly IResourceManager _resourceManager;
#pragma warning restore 649
    }
}
