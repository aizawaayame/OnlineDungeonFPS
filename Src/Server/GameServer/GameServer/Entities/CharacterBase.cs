using Common.Data;
using GameServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace GameServer.Entities
{
    class CharacterBase : Entity
    {

        #region Public Properties

        public int Id
        {
            get
            {
                return this.entityId;
            }
        }
        public NCharacterInfo Info { get; set; }
        public CharacterDefine Define { get; set; }


        #endregion

        #region Constructors

        public CharacterBase(Vector3Int pos, Vector3Int dir):base(pos,dir)
        {

        }

        public CharacterBase(CharacterType type, int tid, int level, Vector3Int pos, Vector3Int dir) :
            base(pos, dir)
        {
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = tid;
            this.Info.Entity = this.EntityData;

            this.Info.Name = this.Define.Name;
        }

        #endregion
    }
}
