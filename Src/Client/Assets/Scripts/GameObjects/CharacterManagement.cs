using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Entities;
using Managers;
using Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameObjects
{
    /// <summary>
    /// this controller control the scene's characters, it should be placed at the scene
    /// </summary>
    public class CharacterManagement : MonoBehaviour
    {

        #region Fields&Properties

        Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

        #endregion

        #region Private Methods

        void Start()
        {
            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        }

        void OnDestroy()
        {
            CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        }
        void CreateCharacterObject(Character cha)
        {
            if (!Characters.ContainsKey(cha.NInfo.Id) || Characters[cha.NInfo.Id] == null)
            {
                Object obj = Resloader.Load<Object>(cha.Define.Resource);
                if (obj == null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",cha.Define.TID, cha.Define.Resource);
                    return;
                }
                GameObject go = (GameObject)Instantiate(obj);
                go.name = "Character_" + cha.NInfo.Id + "_" + cha.NInfo.Name;

                go.transform.position = GameObjectTool.LogicV3IntToWorldV3(cha.Position);
                go.transform.forward = GameObjectTool.LogicV3IntToWorldV3(cha.Direction);
                Characters[cha.NInfo.Id] = go;

                EntityController ec = go.GetComponent<EntityController>();
                if (ec != null)
                {
                    ec.Entity = cha;
                    ec.IsPlayer = cha.IsPlayer;
                }

                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc != null)
                {
                    if (cha.NInfo.Id == User.Instance.CurrentCharacterInfo.Id)
                    {
                        User.Instance.CurrentCharacterObject = go;
                        pc.mainCamera.enabled = true;
                        pc.weaponCamera.enabled = true;
                        pc.enabled = true;
                        pc.Character = cha;
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

        #region Subscriber

        void OnCharacterEnter(Character cha)
        {
            CreateCharacterObject(cha);
        }

        #endregion
        
        #region IEnumerator
        
        IEnumerator InitGameObjects()
        {
            foreach (var cha in CharacterManager.Instance.Characters.Values)
            {
                CreateCharacterObject(cha);
                yield return null;
            }
        }
        
        #endregion
           
    }
}
