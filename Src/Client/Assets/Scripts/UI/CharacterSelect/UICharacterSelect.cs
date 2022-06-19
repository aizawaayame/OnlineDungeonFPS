using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Managers;
using Modules;
using Protocol;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Utilities;

namespace UI
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
        Protocol.CharacterClass charClass;   // the character class which will be created
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
            UserService.Instance.OnCharacterCreate += OnCharacterCreate;
        }
        
        /// <summary>
        /// Init or refresh the UI
        /// </summary>
        /// <param name="init"></param>
        void InitCharacterSelect(bool init)
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
            UserService.Instance.OnCharacterCreate -= OnCharacterCreate;
        }
    
        #endregion

        #region UICallBack
        
        public void OnClickEnterCreateCharacterPanel()
        {
            panelCreate.SetActive(true);
            panelSelect.SetActive(false);
            OnSelectedClass((int)Protocol.CharacterClass.Warrior);
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
            this.charClass = (Protocol.CharacterClass)charClass;
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

        public void OnClickPlay()
        {
            if (selectCharacterIdx >= 0 )
            {
                UserService.Instance.SendGameEnter(selectCharacterIdx);
            }
        }
        #endregion
        
        #region Events

        void OnCharacterCreate(Result result, string msg)
        {
            if (result == Protocol.Result.Success)
            {
                Debug.Log("角色创建成功");
                InitCharacterSelect(true);
            }
            else
            {
                MessageBox.Show(msg, "错误",MessageBoxType.Error);
            }

        }

        #endregion
    }
}
