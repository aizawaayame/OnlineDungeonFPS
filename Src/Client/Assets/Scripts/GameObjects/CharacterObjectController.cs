using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Entities;
using Managers;
using Models;
using UnityEngine;
using Utilities;
using Object = UnityEngine.Object;

namespace GameObjects
{
    /// <summary>
    /// Manage all the character in the scene for the client.
    /// All the game scene should place one.
    /// </summary>
    public class CharacterObjectController : MonoSingleton<CharacterObjectController>
    {

        #region Fields&Properties

        Dictionary<int, GameObject> characterGameObjects = new Dictionary<int, GameObject>();

        #endregion

        #region Private Methods

        protected override void OnStart()
        {
            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
            CharacterManager.Instance.OnChracterLeave += OnCharacterLeave;
        }

        void OnDestroy()
        {
            CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
            CharacterManager.Instance.OnChracterLeave -= OnCharacterLeave;
        }
        
        /// <summary>
        /// Create the new character game object in the scene.
        /// Init the game object's transform
        /// Init the entity controller.
        /// init the player controller.
        /// </summary>
        void CreateCharacterObject(Character character)
        {
            if (!characterGameObjects.ContainsKey(character.NCharacter.Id) || characterGameObjects[character.NCharacter.Id] == null)
            {
                Debug.LogFormat("开始创建实体");
                Object obj = Resloader.Load<Object>(character.Define.Resource);
                if (obj == null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",character.Define.TID, character.Define.Resource);
                    return;
                }
                GameObject go = (GameObject)Instantiate(obj);
                go.name = "Character_" + character.NCharacter.Id + "_" + character.NCharacter.Name;

                go.transform.position = character.Position;
                go.transform.forward = character.Direction;
                characterGameObjects.Add(character.NCharacter.Id, go);
                //characterGameObjects[character.NCharacter.Id] = go;

                EntityController ec = go.GetComponent<EntityController>();
                if (ec != null)
                {
                    ec.Entity = character;
                    ec.IsPlayer = character.IsPlayer;
                }

                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc != null)
                {
                    if (character.NCharacter.Id == User.Instance.NCharacter.Id)
                    {
                        User.Instance.CurrentCharacterObject = go;
                        pc.mainCamera.enabled = true;
                        pc.weaponCamera.enabled = true;
                        pc.enabled = true;
                        pc.Character = character;
                        pc.EntityController = ec;
                    }
                    else
                    {
                        pc.mainCamera.enabled = false;
                        pc.weaponCamera.enabled = false;
                        pc.enabled = false;
                    }
                }

            }
        }
        #endregion

        #region Events

        void OnCharacterEnter(Character cha)
        {
            CreateCharacterObject(cha);
        }

        void OnCharacterLeave(Character cha)
        {
            if (!characterGameObjects.ContainsKey(cha.EntityID))
            {
                return;
            }
            if (characterGameObjects[cha.EntityID] != null)
            {
                Destroy(characterGameObjects[cha.EntityID]);
                this.characterGameObjects.Remove(cha.EntityID);
            }
        }
        #endregion
        
        #region IEnumerator
        
        IEnumerator InitGameObjects()
        {
            Debug.LogFormat($"开始创建角色对象，其中CharacterManager.Instance.Characters.Count为{CharacterManager.Instance.Characters.Count}");
            foreach (var cha in CharacterManager.Instance.Characters.Values)
            {
                CreateCharacterObject(cha);
                yield return null;
            }
        }
        
        #endregion
           
    }
}
