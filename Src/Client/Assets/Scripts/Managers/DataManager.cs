using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common.Data;
using Newtonsoft.Json;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class DataManager : Utilities.Singleton<DataManager>
    {
        public string dataPath;
        public Dictionary<int, MapDefine> maps = null;
        public Dictionary<int, CharacterDefine> characters = null;
        public Dictionary<int, TeleporterDefine> teleporters = null;
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> spawnPoints = null;

        public DataManager()
        {
            this.dataPath = "Data/";
            Debug.LogFormat("DataManager > DataManager()");
        }

        public void Load()
        {
            string json = File.ReadAllText(dataPath + "MapDefine.txt");
            this.maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
            
            json = File.ReadAllText(this.dataPath + "CharacterDefine.txt");
            this.characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(this.dataPath + "TeleporterDefine.txt");
            this.teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(this.dataPath + "SpawnPointDefine.txt");
            this.spawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>> (json);
        }

        public IEnumerator LoadData()
        {
            string json = File.ReadAllText(this.dataPath + "MapDefine.txt");
            this.maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.dataPath + "CharacterDefine.txt");
            this.characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.dataPath + "TeleporterDefine.txt");
            this.teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.dataPath + "SpawnPointDefine.txt");
            this.spawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

            yield return null;
        }
        #if UNITY_EDITOR
        public void SaveTeleporters()
        {
            string json = JsonConvert.SerializeObject(this.teleporters, Formatting.Indented);
            File.WriteAllText(this.dataPath + "TeleporterDefine.txt", json);
        }

        public void SaveSpawnPoints()
        {
            string json = JsonConvert.SerializeObject(this.spawnPoints, Formatting.Indented);
            File.WriteAllText(this.dataPath + "SpawnPointDefine.txt", json);
        }
        
        #endif
        
    }
}
