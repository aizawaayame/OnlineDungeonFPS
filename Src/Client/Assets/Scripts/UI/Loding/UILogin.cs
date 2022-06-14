using System;
using Services;
using SkillBridge.Message;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;


namespace UI.Loding
{
    public class UILogin : MonoBehaviour
    {
        public TMP_InputField userName;
        public TMP_InputField password;

        public Button buttonLogin;

        #region Private Methods
        void Start()
        {
            EventManager.AddListener<UserLoginEvent>(OnLogin);
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<UserLoginEvent>(OnLogin);
        }
        #endregion
    
        #region Events
        void OnLogin(UserLoginEvent evt)
        {
            if (evt.result == Result.Success)
            {   
                Debug.LogFormat("登录成功，返回SUCCESS");
                SceneManager.LoadScene("Scenes/CharacterSelect", LoadSceneMode.Single);
            }
            else
            {
                MessageBox.Show("登录失败","错误提示",MessageBoxType.Information,btnOK:"",btnCancel:"");
            }
        }
        #endregion

        #region UICallBack

        public void OnClickLogin()
        {
            if (string.IsNullOrEmpty(this.userName.text))
            {
                MessageBox.Show("请输入用户名");
                return;
            }
            if (string.IsNullOrEmpty(this.password.text))
            {
                MessageBox.Show("请输入密码");
                return;
            }
            UserService.Instance.SendLogin(userName.text, password.text);
        }

        #endregion
        

    }
}
