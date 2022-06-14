﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Managers;
using Modules;
using Services;
using SkillBridge.Message;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.CharacterSelect
{
    /// <summary>
    /// Manage the Character Select Scene's UI
    /// </summary>
    public class UICharacterSelect : MonoBehaviour
    {

        #region Fields&Properties
        public GameObject panelCreate;
        public GameObject panelSelect;
        
        /// <summary>
        /// Create character
        /// </summary>
        public TMP_Text classDesc;
        CharacterClass charClass;   // the character class which will be created
        public TMP_InputField inputUserName;
        int nameKsy;
        public string UserName
        {
            get { return inputUserName.text; }
            set
            {
                inputUserName.text = value;
            }
        }
        
        /// <summary>
        /// Select character
        /// </summary>
        public GameObject uiCharacter;
        public Transform uiCharacterScroll;
        List<GameObject> uiCharacters = new List<GameObject>(); // the user's all characters
        
        private int selectCharacterIdx = -1;
        #endregion
        
        #region Private Methods
        void Start()
        {
            InitCharacterSelect(true);
            EventManager.AddListener<UserCreateCharacterEvent>(OnCharacterCreate);
        }
        
        /// <summary>
        /// Init or refresh the UI
        /// </summary>
        /// <param name="init"></param>
        public void InitCharacterSelect(bool init)
        {
            panelCreate.SetActive(false);
            panelSelect.SetActive(true);

            if (init)
            {
                foreach (var old in uiCharacters)
                {
                    Destroy(old);
                }
            }
            uiCharacters.Clear();

            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {   
                Debug.LogFormat("创建角色滚动条{0}",User.Instance.Info.Player.Characters.Count);
                GameObject obj = Instantiate(this.uiCharacter, this.uiCharacterScroll);
                UICharacterInfo charInfo = obj.GetComponent<UICharacterInfo>();
                charInfo.Info = User.Instance.Info.Player.Characters[i];
                Button button = obj.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() =>
                {
                    OnSelectCharacter(idx);
                });
                uiCharacters.Add(obj);
                obj.SetActive(true);
            }

            if (User.Instance.Info.Player.Characters.Count > 0)
            {
                OnSelectCharacter(0);
            }
        }
        void OnDestroy()
        {
            EventManager.RemoveListener<UserCreateCharacterEvent>(OnCharacterCreate);
        }
    
        #endregion

        #region UICallBack
        
        public void OnClickEnterCreateCharacterPanel()
        {
            panelCreate.SetActive(true);
            panelSelect.SetActive(false);
            OnSelectedClass((int)CharacterClass.Warrior);
        }
        
        /// <summary>
        /// select a character of user to enter the game.
        /// </summary>
        /// <param name="idx">the character idx.</param>
        public void OnSelectCharacter(int idx)
        {
            this.selectCharacterIdx = idx;
            var currentChar = User.Instance.Info.Player.Characters[idx];
            Debug.LogFormat("Select Char:[{0}]{1}[{2}]{3}", currentChar.Id, currentChar.Name, currentChar.Class,currentChar.Level);
            User.Instance.CurrentCharacterInfo = currentChar;
            
            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                UICharacterInfo ci = this.uiCharacters[i].GetComponent<UICharacterInfo>();
                ci.IsSelected = idx == i;
            }
        }
        
        /// <summary>
        /// Selecte character class to create
        /// </summary>
        /// <param name="charClass"> the character class that will be created</param>
        public void OnSelectedClass(int charClass)
        {
            this.charClass = (CharacterClass)charClass;
            //TODO class desc is doing
            /*classDesc.text = DataManager.Instance.characters[(int)charClass].Name;*/
        }

        public void OnClickCreateCharacter()
        {
            if (string.IsNullOrEmpty(this.inputUserName.text))
            {
                MessageBox.Show("请输入角色名称");
                return;
            }
            UserService.Instance.SendUserCreateCharacter(this.inputUserName.text, this.charClass);
        }
        
        
        #endregion
        
        #region Events

        void OnCharacterCreate(UserCreateCharacterEvent evt)
        {
            if (evt.result == Result.Success)
            {
                Debug.Log("角色创建成功");
                InitCharacterSelect(true);
            }
            else
            {
                MessageBox.Show(evt.msg, "错误",MessageBoxType.Error);
            }

        }

        #endregion
    }
}
