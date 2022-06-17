using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common.Data;
using Newtonsoft.Json;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class DataManager : Singleton<DataManager>
    {
        public string DataPath { get; set; } = null;
        public Dictionary<int, MapDefine> Maps { get; set; }= null;
        public Dictionary<int, CharacterDefine> Characters { get; set; }= null;
        public Dictionary<int, TeleporterDefine> Teleporters { get; set; }= null;
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints { get; set; }= null;

        public DataManager()
        {
            this.DataPath = "Data/";
            Debug.LogFormat("DataManager > DataManager()");
        }

        public void Load()
        {
            string json = File.ReadAllText(DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
            
            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
            this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>> (json);
        }

        public IEnumerator LoadData()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
            this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

            yield return null;
        }
        #if UNITY_EDITOR
        public void SaveTeleporters()
        {
            string json = JsonConvert.SerializeObject(this.Teleporters, Formatting.Indented);
            File.WriteAllText(this.DataPath + "TeleporterDefine.txt", json);
        }

        public void SaveSpawnPoints()
        {
            string json = JsonConvert.SerializeObject(this.SpawnPoints, Formatting.Indented);
            File.WriteAllText(this.DataPath + "SpawnPointDefine.txt", json);
        }
        
        #endif
        
    }
}
