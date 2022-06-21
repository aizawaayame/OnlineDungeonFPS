using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Models;
using Network;
using Protocol;

namespace GameServer.Entities
{
    /// <summary>
    /// Player data class.
    /// </summary>
    class Character : Entity, IPostResponser
    {

        #region Public Properties
        public int CharacterId { get => TCharacter.ID; }
        public int ConfigId { get => TCharacter.TID; }

        public int Exp { get => NCharacter.Exp; }
        public int Level { get => NCharacter.Level; }
        public long Gold { get => NCharacter.Gold; }
        public string CharacterName { get => TCharacter.Name; }
        public CharacterClass CharacterClass { get => (CharacterClass)TCharacter.Class; }
        public TCharacter TCharacter { get; set; }
        public NCharacter NCharacter { get; set; }
        public CharacterDefine Define { get; private set; }
        
        #endregion

        #region Constructor

        public Character(TCharacter tCharacter) : base(Map.GetMapInitPos(1),Vector3Int.zero)
        {
            this.TCharacter = tCharacter;
            this.NCharacter = new NCharacter(){
                Class = (CharacterClass)TCharacter.Class,
                ConfigId = TCharacter.TID,
                EntityId = 0,
                Entity = this.NEntity,
                Exp = TCharacter.Exp,
                Gold = TCharacter.Gold,
                Id = TCharacter.ID,
                Level = TCharacter.Level,
                mapId = 1,
                Name = TCharacter.Name,
            };
            this.Define = DataManager.Instance.CharacterDefines[this.ConfigId];
        }

        #endregion

        #region Public Methods

        public void PostProcess(NetMessageResponse message)
        {
            
        }

        public void Clear()
        {
            
        }
        #endregion

        #region Private Methods



        #endregion

    }
}
