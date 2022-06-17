using Managers;
using Protocol;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {

        #region Fields&Properties

        public NCharacterInfo NInfo { get; set; }
        public Common.Data.CharacterDefine Define { get; set; }

        public string Name
        {
            get
            {
                if (this.NInfo.Type == CharacterType.Player)
                {
                    return this.NInfo.Name;
                }
                else
                {
                    return this.Define.Name;
                }
            }
        }

        public bool IsPlayer
        {
            get
            {
                return this.NInfo.Id == Modules.User.Instance.CurrentCharacterInfo.Id;
            }
        }

        #endregion

        #region Constructors

        public Character(NCharacterInfo nInfo) : base(nInfo.Entity)
        {
            this.NInfo = nInfo;
            this.Define = DataManager.Instance.Characters[nInfo.Tid];
        }

        #endregion
        
        #region Public Methods

        public void Move()
        {
            this.Speed = this.Define.Speed;
        }

        public void Stop()
        {
            this.Speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            this.Direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            this.Position = position;
        }
        #endregion
    }
}
