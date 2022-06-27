using Common.Data;
using Protocol.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        NUserInfo userInfo;


        public NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }

        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }


        public NCharacterInfo CurrentCharacter { get; set; }
        public MapDefine CurrentMapData { get; set; }
        public UnityEngine.GameObject CurrentCharacterObject { get; set; }

    }
}
