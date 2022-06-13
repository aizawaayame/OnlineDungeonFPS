using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Managers;
using Models;
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
        public TMP_Text descs;
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
        public GameObject UICharacterInfo;
        public Transform UICharacterList;
        List<GameObject> UICharacters = new List<GameObject>();
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
                foreach (var old in UICharacters)
                {
                    Destroy(old);
                }
            }
            UICharacters.Clear();

            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                GameObject obj = Instantiate(UICharacterInfo, this.UICharacterList);
                UICharacterInfo charInfo = obj.GetComponent<UICharacterInfo>();
                charInfo.info = User.Instance.Info.Player.Characters[i];
                Button button = obj.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() =>
                {
                    OnSelectCharacter(idx);
                });
                UICharacters.Add(obj);
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
        public void OnSelectCharacter(int idx)
        {
            this.selectCharacterIdx = idx;
            var currentChar = User.Instance.Info.Player.Characters[idx];
            Debug.LogFormat("Select Char:[{0}]{1}[{2}]{3}", currentChar.Id, currentChar.Name, currentChar.Class,currentChar.Level);

            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                UICharacterInfo ci = this.UICharacters[i].GetComponent<UICharacterInfo>();
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
            descs.text = DataManager.Instance.characters[(int)charClass].Name;
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
