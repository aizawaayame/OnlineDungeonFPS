using System;
using Models;
using TMPro;
using Utilities;

namespace UI
{
    public class UIMain : MonoSingleton<UIMain>
    {

        #region Public Fields

        public TMP_Text charInfo;

        #endregion
        
        #region Private Mothods

        protected override void OnStart()
        {
            this.UpdateAvatar();
        }
        
        void UpdateAvatar()
        {
            string level = User.Instance.NCharacter.Level.ToString();
            string name = User.Instance.NCharacter.Name;
            this.charInfo.text = String.Format($"{name}:{level}");
        }

        #endregion
    }
}
