
using Common.Data;
using Utilities;
using Protocol;
using UnityEngine;

namespace Modules
{
    public class User : Singleton<User>
    {
        public NUserInfo Info { get; set; }
        public NCharacterInfo CurrentCharacterInfo { get; set; }
        public GameObject CurrentCharacterObject { get; set; }
        
        public MapDefine CurrentMapData { get; set; }
    }
}
