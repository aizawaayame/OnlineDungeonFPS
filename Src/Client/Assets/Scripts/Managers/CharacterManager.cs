using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Common;
using Entities;
using Models;
using Protocol;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    /// <summary>
    /// Manage all the characters in the user's map.
    /// Manage character enter map and leave map event.
    /// </summary>
    public class CharacterManager : Singleton<CharacterManager>, IDisposable    
    {
        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        #region Fields&Properties
        /// <summary>
        /// Int is the entity idx, character is the client entity.
        /// </summary>
        public Dictionary<int, Character> Characters { get; private set; }= new Dictionary<int, Character>();
        public Character CurrentCharacter
        {
            get
            {
                Character character = GetCharacter(User.Instance.NCharacter.Id);
                return character;
            }
        }
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
            int[] keys = this.Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        public void AddCharacter(NCharacter cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2}", cha.Id, cha.Name, cha.mapId);
            Character character = new Character(cha);
            this.Characters[cha.Id] = character;
            EntityManager.Instance.AddEntity(character);
            OnCharacterEnter?.Invoke(character);
            if (character.EntityID == User.Instance.NCharacter.EntityId)
            {
                User.Instance.Character = character;
            }
        }

        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", characterId);
            if (this.Characters.ContainsKey(characterId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[characterId].EntityID);
                OnCharacterLeave?.Invoke(this.Characters[characterId]);
                this.Characters.Remove(characterId);
            }
        }

        public Character GetCharacter(int entityId)
        {
            Character character;
            this.Characters.TryGetValue(entityId, out character);
            return character;
        }
        #endregion

        #region Indexer

        public Character this[int characterId]
        {
            get => Characters[characterId];
            set => Characters[characterId] = value;
        }

        #endregion
    }
}
