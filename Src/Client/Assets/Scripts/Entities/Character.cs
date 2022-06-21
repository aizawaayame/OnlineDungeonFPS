using Managers;
using Protocol;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {

        #region Fields&Properties
        public string Name
        {
            get
            {
                if (this.NCharacter.Type == CharacterType.Player)
                {
                    return NCharacter.Name;
                }
                else
                {
                    return this.Define.Name;
                }
            }
        }
        public int CharacterId { get => NCharacter.Id; }
        public bool IsPlayer { get => this.NCharacter.Id == Models.User.Instance.NCharacter.Id; }
        public NCharacter NCharacter { get; set; }

        public Common.Data.CharacterDefine Define
        {
            get => DataManager.Instance.CharacterDefines[(int)NCharacter.Class];
        }
        
        #endregion

        #region Constructors

        public Character(NCharacter nCharacter) : base(nCharacter.Entity)
        {
            NCharacter = nCharacter;
        }

        #endregion
    }
}
