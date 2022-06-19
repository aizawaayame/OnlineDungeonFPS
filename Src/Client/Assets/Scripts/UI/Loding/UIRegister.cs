using Protocol;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class UIRegister : MonoBehaviour
    {

        #region Public Fields to editor

        public TMP_InputField userName;
        public TMP_InputField password;
        public TMP_InputField passwordConfirm;
        public Button buttonRegister;
        
        #endregion
        
        #region Private Mothods

        void Awake()
        {
            UserService.Instance.OnRegister += OnRegister;
        }
    

        void Update () {
            Debug.Log(userName.text);
            Debug.Log(password.text);
            Debug.Log(passwordConfirm.text);
        }

        void OnDestroy()
        {
            UserService.Instance.OnRegister -= OnRegister;
        }

        #endregion

        #region Events
        /// <summary>
        /// UI Click register button callback
        /// </summary>
        public void OnClickRegister()
        {
            if (string.IsNullOrEmpty(this.userName.text))
            {
                MessageBox.Show("请输入账号");
                return;
            }
            if (string.IsNullOrEmpty(this.password.text))
            {
                MessageBox.Show("请输入密码");
                return;
            }
            if (string.IsNullOrEmpty(this.passwordConfirm.text))
            {
                MessageBox.Show("请输入确认密码");
                return;
            }
            if (this.password.text != this.passwordConfirm.text)
            {
                MessageBox.Show("两次输入的密码不一致");
                return;
            }
            UserService.Instance.SendRegister(this.userName.text, this.password.text);
        }
        
        void OnRegister(Result result,string msg)
        {
            MessageBox.Show(string.Format("结果：{0} msg:{1}",result,msg));
        }

        #endregion
    }
}

