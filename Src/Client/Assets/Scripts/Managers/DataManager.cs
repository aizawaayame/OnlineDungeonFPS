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
        public Dictionary<int, MapDefine> MapDefines { get; set; }= null;
        public Dictionary<int, CharacterDefine> CharacterDefines { get; set; }= null;
        public Dictionary<int, TeleporterDefine> TeleporterDefines { get; set; }= null;
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPointDefines { get; set; }= null;

        public DataManager()
        {
            this.DataPath = "Data/";
            Debug.LogFormat("DataManager > DataManager()");
        }

        public void Load()
        {
            string json = File.ReadAllText(DataPath + "MapDefine.txt");
            this.MapDefines = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);
            
            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.CharacterDefines = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.TeleporterDefines = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "SpawnRuleDefinee.txt");
            this.SpawnPointDefines = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>> (json);
        }

        public IEnumerator LoadData()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.MapDefines = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.CharacterDefines = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.TeleporterDefines = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "SpawnRuleDefine.txt");
            this.SpawnPointDefines = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

            yield return null;
        }
        #if UNITY_EDITOR
        public void SaveTeleporters()
        {
            string json = JsonConvert.SerializeObject(this.TeleporterDefines, Formatting.Indented);
            File.WriteAllText(this.DataPath + "TeleporterDefine.txt", json);
        }

        public void SaveSpawnPoints()
        {
            string json = JsonConvert.SerializeObject(this.SpawnPointDefines, Formatting.Indented);
            File.WriteAllText(this.DataPath + "SpawnRuleDefine.txt", json);
        }
        
        #endif
        
    }
}
