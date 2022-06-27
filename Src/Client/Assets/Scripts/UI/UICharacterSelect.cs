using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using Protocol.Message;
using UnityEngine.Events;
using Common.Data;
using TMPro;


    public class UICharacterSelect : MonoBehaviour {
    
    #region Fields

    [SerializeField]
    GameObject panelCreate;
    [SerializeField]
    GameObject panelSelect;
    
    [SerializeField]
    TMP_Text descs;
    [SerializeField]
    TMP_InputField inputUserName;
    
    [SerializeField]
    GameObject uiCharInfo;
    [SerializeField]
    Transform uiCharList;

    List<GameObject> uiChars = new List<GameObject>();
    
    CharacterClass charClass;
    int selectCharacterIdx = -1;

    #endregion
    public string UserName
    {
        get { return inputUserName.text; }
        set
        {
            inputUserName.text = value;
        }
    }
    
    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
    }
    void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);
        if (init)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();



            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                GameObject go = Instantiate(uiCharInfo,this.uiCharList);
                UICharInfo charInfo = go.GetComponent<UICharInfo>();
                charInfo.Info = User.Instance.Info.Player.Characters[i];
                Button button = go.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() => {
                    OnSelectCharacter(idx);
                });
                uiChars.Add(go);
                go.SetActive(true);
            }

            if (User.Instance.Info.Player.Characters.Count > 0)
            {
                OnSelectCharacter(0);
            }
        }
    }
    public void InitCharacterCreate()
    {
        panelCreate.SetActive(true);
        panelSelect.SetActive(false);
        OnSelectClass(1);
    }

    void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]{3}", cha.Id, cha.Name, cha.Class,cha.Level);
        
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Selected = idx == i;
        }
    }

    void OnCharacterCreate(Result result,string msg)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(msg, "错误",MessageBoxType.Error);
        }
    }

    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass) charClass;
        descs.text = DataManager.Instance.Characters[charClass].Description;
    }
    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(this.inputUserName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.inputUserName.text, this.charClass);
    }
    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {

            UserService.Instance.SendGameEnter(this.selectCharacterIdx);
        }
    }

    public void OnClickBack()
    {
        InitCharacterSelect(false);
    }
}


