using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{

    public Text uiName;
    public Text uiLevel;
    protected override void OnStart()
    {
        this.UpdateAvatar();
    }

    private void UpdateAvatar()
    {
        this.uiName.text = string.Format("{0}", User.Instance.CurrentCharacter.Name);
        this.uiLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }
    public void OnClicBackSelectCharacter()
    {
        
        SceneManager.Instance.LoadScene("CharSelect");
        UserService.Instance.SendGameLeave();
    }

    public void OnClickTest()
    {
        //UIManager.Instance.Show<UITest>();
    }
    
}
