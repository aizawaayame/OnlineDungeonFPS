using System;
using Entities;
using Managers;
using Protocol;
using UnityEngine;

namespace GameObjects
{
    public class PlayerController : MonoBehaviour
    {
        
        #region Fields&Properties

        public Camera mainCamera;
        public Camera weaponCamera;
        public Character Character { get; set; }
        public int Speed { get; set; }
        public EntityController EntityController { get; set; }
        CharacterState state;
        #endregion

        void Start()
        {
            state = CharacterState.Idle;
            if (this.Character == null)
            {
                Debug.LogError("the PlayerController's Character is null");
            }
        }
    }
}
