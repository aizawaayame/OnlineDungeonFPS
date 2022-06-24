using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

using Common;
using Common.Data;

using Newtonsoft.Json;
namespace GameServer.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        internal string DataPath;
        internal Dictionary<int, MapDefine> MapDefines = null;
        internal Dictionary<int, CharacterDefine> CharacterDefines = null;
        internal Dictionary<int, TeleporterDefine> TeleporterDefines = null;
        public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPointDefines = null;
        public Dictionary<int, Dictionary<int,SpawnRuleDefine>> SpawnRuleDefines = null;
        public Dictionary<int, WeaponDefine> WeaponDefines = null;

        public DataManager()
        {
            this.DataPath = "Data/";
            Log.Info("DataManager > DataManager()");
        }

        internal void Load()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.MapDefines = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.CharacterDefines = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
            this.TeleporterDefines = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

            json = File.ReadAllText(this.DataPath + "WeaponDefine.txt");
            this.WeaponDefines = JsonConvert.DeserializeObject<Dictionary<int, WeaponDefine>>(json);

        }
    }
}