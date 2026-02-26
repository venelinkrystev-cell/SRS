using System;
using System.IO;
using UnityEngine;

namespace SRS.Progression
{
    public sealed class PlayerProfileStore
    {
        private const string SaveFileName = "profile_v1.json";

        private readonly string _savePath;
        private readonly string _tempPath;

        public PlayerProfileStore(string rootPath = null)
        {
            var directory = string.IsNullOrWhiteSpace(rootPath)
                ? Application.persistentDataPath
                : rootPath;

            _savePath = Path.Combine(directory, SaveFileName);
            _tempPath = _savePath + ".tmp";
        }

        public string SavePath => _savePath;

        public PlayerProfile LoadOrCreate()
        {
            if (!File.Exists(_savePath))
            {
                Debug.Log($"[ProfileStore] No profile found. Creating a new profile at {_savePath}.");
                return PlayerProfile.CreateDefault();
            }

            try
            {
                var json = File.ReadAllText(_savePath);
                var data = JsonUtility.FromJson<PlayerProfileSaveData>(json);

                if (!Validate(data))
                {
                    Debug.LogWarning("[ProfileStore] Invalid profile data. Falling back to a new profile.");
                    return PlayerProfile.CreateDefault();
                }

                return PlayerProfile.FromSaveData(data);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ProfileStore] Failed to load profile. Creating default profile. Error: {ex.Message}");
                return PlayerProfile.CreateDefault();
            }
        }

        public void Save(PlayerProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var data = profile.ToSaveData();
            data.schemaVersion = PlayerProfile.CurrentSchemaVersion;

            var directory = Path.GetDirectoryName(_savePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_tempPath, json);

            if (File.Exists(_savePath))
            {
                try
                {
                    File.Replace(_tempPath, _savePath, null);
                }
                catch (PlatformNotSupportedException)
                {
                    File.Delete(_savePath);
                    File.Move(_tempPath, _savePath);
                }
            }
            else
            {
                File.Move(_tempPath, _savePath);
            }
        }

        private static bool Validate(PlayerProfileSaveData data)
        {
            if (data == null)
            {
                return false;
            }

            if (data.schemaVersion <= 0 || data.schemaVersion > PlayerProfile.CurrentSchemaVersion)
            {
                return false;
            }

            return true;
        }
    }
}
