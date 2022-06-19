using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Entities;
using Protocol;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {

        #region Fields
        
        readonly Dictionary<int, Character> characters = new Dictionary<int, Character>();
        
        #endregion

        #region Public Methods

        public void Init()
        {

        }

        public void Clear()
        {
            this.characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            this.characters[cha.ID] = character;
            return character;
        }


        public void RemoveCharacter(int characterId)
        {
            var cha = this.characters[characterId];
            EntityManager.Instance.RemoveEntity(cha.Data.MapID, cha);
            this.characters.Remove(characterId);
        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.characters.TryGetValue(characterId, out character);
            return character;
        }
        #endregion
    }
}
