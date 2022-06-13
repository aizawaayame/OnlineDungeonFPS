using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
namespace UI
{
    public class UIMessageBox : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text message;
        public Button buttonYes;
        public Button buttonNo;
        public Button buttonClose;

        public TMP_Text buttonYesTitle;
        public TMP_Text buttonNoTitle;

        public UnityAction OnYes;
        public UnityAction OnNo;

        public void Init(string title, string message, MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
        {
            if (!string.IsNullOrEmpty(title)) this.title.text = title;
            this.message.text = message;

            if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = title;
            if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = title;
            
            this.buttonYes.onClick.AddListener(OnClickYes);
            this.buttonNo.onClick.AddListener(OnClickNo);
            
            this.buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);
        }

        void OnClickYes()
        {
            Destroy(this.gameObject);
            this.OnYes?.Invoke();
        }
        void OnClickNo()
        {
            Destroy(this.gameObject);
            this.OnNo?.Invoke();
        }
    }

}
