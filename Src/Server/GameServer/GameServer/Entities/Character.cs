using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using Protocol;

namespace GameServer.Entities
{
    /// <summary>
    /// Character
    /// 玩家角色类
    /// </summary>
    class Character : CharacterBase, IPostResponser
    {

        #region Public Properties
        public TCharacter Data { get; private set; }

        #endregion

        #region Constructor

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Info = new NCharacterInfo{
                Type = type,
                Id = cha.ID,
                Name = cha.Name,
                Level = 1, //cha.Level;
                ConfigId = cha.TID,
                Class = (CharacterClass)cha.Class,
                mapId = cha.MapID,
                Entity = this.EntityData
            };
        }

        #endregion

        #region Public Methods

        public void PostProcess(NetMessageResponse message)
        {
            
        }

        public void Clear()
        {
            
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo(){
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level
            };
        }
        #endregion

    }
}
