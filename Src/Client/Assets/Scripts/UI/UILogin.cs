using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using Protocol.Message;
using System;
using TMPro;

public class UILogin : MonoBehaviour {

    #region Fields
    [SerializeField]
    TMP_InputField username;
    [SerializeField]
    TMP_InputField password;
    [SerializeField]
    GameObject uiRegister;
    #endregion
    
    void Start()
    {
        UserService.Instance.OnLogin = OnLogin;
    }

    void OnLogin(Result result, string msg)
    {
        if (result==Result.Success)
        {
            MessageBox.Show("登陆成功");
            SceneManager.Instance.LoadScene("CharacterSelect");
        }
        else
        {
            MessageBox.Show("登录失败？","登录失败",MessageBoxType.Information,btnOK:"",btnCancel:"");
        }
    }

    #region Events

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入用户名");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        UserService.Instance.SendLogin(this.username.text,this.password.text);
    }

    public void OnClickRegister()
    {
        uiRegister.SetActive(true);
        this.gameObject.SetActive(false);
    }
    #endregion

}
