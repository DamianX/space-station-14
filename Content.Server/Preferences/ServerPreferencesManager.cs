using System.Collections.Generic;
using System.IO;
using Content.Server.GameObjects;
using Content.Server.Interfaces;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Appearance;
using Content.Shared.Preferences.Profiles;
using Robust.Server.Interfaces.Player;
using Robust.Shared.Interfaces.Configuration;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Interfaces.Resources;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Server.Preferences
{
    public class ServerPreferencesManager : SharedPreferencesManager, IServerPreferencesManager, IPostInjectInit
    {
#pragma warning disable 649
        [Dependency] private readonly IServerNetManager _netManager;
        [Dependency] private readonly IConfigurationManager _configuration;
        [Dependency] private readonly IResourceManager _resourceManager;
#pragma warning restore 649

        private const string PrefsPath = "/preferences/";
        private static readonly ResourcePath PrefsDirPath = new ResourcePath(PrefsPath).ToRootedPath();

        void IPostInjectInit.PostInject()
        {
            _configuration.RegisterCVar("game.maxcharacterslots", 10);
            _netManager.RegisterNetMessage<MsgPreferencesPayload>(nameof(MsgPreferencesPayload), HandlePreferencePayload);
        }

        public void SendPayloadToClient(IPlayerSession session)
        {
            var msg = _netManager.CreateNetMessage<MsgPreferencesPayload>();
            msg.Prefs = Get(session.SessionId.Username);
            _netManager.ServerSendMessage(msg, session.ConnectedClient);
        }

        private void HandlePreferencePayload(MsgPreferencesPayload message)
        {
            Save(message.Prefs, message.MsgChannel.SessionId.Username);
        }

        public PlayerPrefs DefaultPlayerPrefs()
        {
            var prefs = new PlayerPrefs();
            prefs.Characters = new List<ICharacterProfile>
            {
                new HumanoidCharacterProfile
                {
                    Age = 18,
                    CharacterAppearance = new HumanoidCharacterAppearance(),
                    Gender = Gender.Male,
                    Name = "John Doe"
                }
            };
            prefs.MaxCharacters = _configuration.GetCVar<int>("game.maxcharacterslots");
            prefs.SelectedCharacter = 0;
            return prefs;
        }

        public PlayerPrefs Get(string username)
        {
            var prefs = GetFromYaml(username);
            if (prefs is null)
            {
                prefs = DefaultPlayerPrefs();
                Save(prefs, username);
            }
            return prefs;
        }

        /// <returns>The requested PlayerPrefs or null if not found.</returns>
        private PlayerPrefs GetFromYaml(string username)
        {
            if (!IsValidUsername(username))
            {
                return null;
            }
            var filePath = PrefsDirPath / (username + ".yml");
            if (!_resourceManager.UserData.Exists(filePath))
            {
                return null;
            }
            using (var file = _resourceManager.UserData.Open(filePath, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    YamlObjectSerializer serializer = YamlObjectSerializer.NewReader(new YamlMappingNode());
                    var yaml = new YamlStream();
                    yaml.Load(reader);
                    if (yaml.Documents.Count == 0)
                    {
                        return null;
                    }
                    return (PlayerPrefs) serializer.NodeToType(typeof(PlayerPrefs), yaml.Documents[0].RootNode);
                }
            }
        }

        public bool IsValidUsername(string username)
        {
            return true;
        }

        public bool IsValidPlayerPrefs(PlayerPrefs prefs)
        {
            var configMaxCharacters = _configuration.GetCVar<int>("game.maxcharacterslots");
            if (prefs.Characters is null)
                return false;
            if (prefs.MaxCharacters != configMaxCharacters)
                return false;
            if (prefs.SelectedCharacter > configMaxCharacters)
                return false;
            if (prefs.SelectedCharacter >= prefs.Characters.Count)
                return false;
            return true;
        }

        public void Save(PlayerPrefs prefs, string username)
        {
            if (!IsValidPlayerPrefs(prefs))
            {
                return;
            }

            _resourceManager.UserData.CreateDir(PrefsDirPath);
            var filePath = PrefsDirPath / (username + ".yml");
            using (var file = _resourceManager.UserData.Open(filePath, FileMode.Create))
            {
                using (var writer = new StreamWriter(file))
                {
                    YamlObjectSerializer serializer = YamlObjectSerializer.NewWriter(new YamlMappingNode());
                    YamlNode serialized = serializer.TypeToNode(prefs);
                    var yaml = new YamlStream(new YamlDocument(serialized));
                    yaml.Save(writer, false);
                }
            }
        }
    }
}
