using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Models;


namespace Managers
{
    public class GameObjectManager : MonoSingleton<GameObjectManager>
    {

        Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
        // Use this for initialization
        protected override void OnStart()
        {
            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
            CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
        }

        private void OnDestroy()
        {
            CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
            CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
        }
        void OnCharacterEnter(Character cha)
        {
            CreateCharacterObject(cha);
        }

        void OnCharacterLeave(Character character)
        {
            if (!Characters.ContainsKey(character.entityId))
                return;

            if (Characters[character.entityId] != null)
            {
                Destroy(Characters[character.entityId]);
                this.Characters.Remove(character.entityId);
            }
        }

        IEnumerator InitGameObjects()
        {
            foreach (var cha in CharacterManager.Instance.Characters.Values)
            {
                CreateCharacterObject(cha);
                yield return null;
            }
        }

        private void CreateCharacterObject(Character character)
        {
            User.Instance.CurrentCharacterObject = GameObject.FindGameObjectWithTag("Player");
            /*if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
            {
                Object obj = Resloader.Load<Object>(character.Define.Resource);
                if (obj == null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                    return;
                }
                GameObject go = (GameObject)Instantiate(obj, this.transform);
                go.name = "Character_" + character.Id + "_" + character.Name;
                Characters[character.entityId] = go;

            }
            this.InitGameObject(Characters[character.entityId], character);*/
        }

        void InitGameObject(GameObject go, Character character)
        {
            go.transform.position = GameObjectTool.LogicToWorld(character.position);
            go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
            Characters[character.entityId] = go;
            EntityController ec = go.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.Entity = character;
                ec.IsPlayer = character.IsCurrentPlayer;
            }
            PlayerController pc = go.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (character.IsCurrentPlayer)
                {
                    User.Instance.CurrentCharacterObject = go;
                    Debug.Log("指定角色成功");
                    pc.enabled = true;
                    pc.MainCamera.enabled = true;
                    pc.WeaponCamera.enabled = true;

                    HealthController health = pc.GetComponent<HealthController>();
                    health.MaxHealth = 995 + User.Instance.CurrentCharacter.Level * 10;
                }
                else
                {
                    pc.enabled = false;
                    pc.MainCamera.enabled = false;
                    pc.WeaponCamera.enabled = false;
                }
            }
        }

    }
}

