using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System;
using System.IO;

using Common.Data;

using Newtonsoft.Json;

namespace Managers
{
    public class DataManager : Singleton<DataManager>
    {
        public string DataPath;
        public Dictionary<int, MapDefine> Maps = null;
        public Dictionary<int, CharacterDefine> Characters = null;




        public DataManager()
        {
            this.DataPath = "Data/";
            Debug.LogFormat("DataManager > DataManager()");
        }

        public void Load()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);



        }


        public IEnumerator LoadData()
        {
            string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
            this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

            yield return null;

            json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
            this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

            yield return null;

        }

    }
}
