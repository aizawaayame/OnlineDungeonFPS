using Managers;
using UnityEngine;

namespace UI
{
    public abstract class UIWindow : MonoBehaviour
    {   public enum WindowResult
        {
            None = 0,
            Yes,
            No
        }
        public delegate void CloseHandler(UIWindow sender, WindowResult result);
        public event CloseHandler OnClose;
        
        public virtual System.Type Type { get => this.GetType(); }

        public void Close(WindowResult result = WindowResult.None)
        {
            UIManager.Instance.Close(this.Type);
            this.OnClose?.Invoke(this,result);
            this.OnClose = null;
        }

        public virtual void OnCloseClick()
        {
            this.Close();
        }

        public virtual void OnYesClick()
        {
            this.Close(WindowResult.Yes);
        }

        public virtual void OnNoClick()
        {
            this.Close(WindowResult.No);
        }


    }
}
