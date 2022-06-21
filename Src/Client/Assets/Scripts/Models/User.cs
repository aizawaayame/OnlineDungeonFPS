
using Common.Data;
using Managers;
using Utilities;
using Protocol;
using UnityEngine;

namespace Models
{
    /// <summary>
    /// contains user's info
    /// </summary>
    public class User : Singleton<User>
    {
        public NUser NUser { get; set; }
        public NCharacter NCharacter { get; set; }
        public GameObject CurrentCharacterObject { get; set; }

        public MapDefine CurrentMapDefine
        {
            get
            {
                MapDefine mapDefine = DataManager.Instance.MapDefines[NCharacter.mapId];
                return mapDefine;
            }
        }
    }
}
