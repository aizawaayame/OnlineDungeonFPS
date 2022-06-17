using System;
using System.Collections.Generic;
using Common;
using Entities;
using Protocol;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class CharacterManager : Singleton<CharacterManager>, IDisposable    
    {

        #region Fields&Properties
        /// <summary>
        /// int is the character idx, character is the client entity.
        /// </summary>
        public Dictionary<int, Character> Characters { get; set; } = new Dictionary<int, Character>();
        
        #endregion

        #region Constructor&Deconstructor

        public CharacterManager()
        {
            
        }
        
        public void Dispose()
        {
        }

        #endregion
        
        #region Public Methods

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public void AddCharacter(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;
            
            
        }

        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characterId);
            this.Characters.Remove(characterId);
        }
        #endregion

        #region Actions

        public UnityAction<Character> OnCharacterEnter;

        #endregion

    }
}
