using Common;
using System.Collections.Generic;
using GameServer.Entities;

namespace GameServer.Managers
{
    class CharacterManager : Singleton<CharacterManager>
    {

        #region Fields

        public Dictionary<int, Character> Characters { get; set; }= new Dictionary<int, Character>();
        
        #endregion

        #region Public Methods

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter tCharacter)
        {
            Character character = new Character(tCharacter);
            EntityManager.Instance.AddEntity(character.MapId,character);
            this.Characters[character.CharacterId] = character;
            return character;
        }


        public void RemoveCharacter(int characterId)
        {
            var cha = this.Characters[characterId];
            EntityManager.Instance.RemoveEntity(cha.MapId, cha);
            this.Characters.Remove(characterId);
        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Characters.TryGetValue(characterId, out character);
            return character;
        }
        #endregion
    }
}
