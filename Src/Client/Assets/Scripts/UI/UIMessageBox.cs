using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMessageBox : MonoBehaviour {



    public UnityAction OnYes;
    public UnityAction OnNo;

    #region Fields
    [SerializeField]
    TMP_Text title;
    [SerializeField]
    TMP_Text message;

    [SerializeField]
    Button buttonYes;
    [SerializeField]
    Button buttonNo;
    [SerializeField]
    Button buttonClose;

    [SerializeField]
    TMP_Text buttonYesTitle;
    [SerializeField]
    TMP_Text buttonNoTitle;

    #endregion

    public void Init(string title, string message, MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;

        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = btnCancel;

        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);

        this.buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);
    }

    void OnClickYes()
    {
        Destroy(this.gameObject);
        if (this.OnYes != null)
            this.OnYes();
    }

    void OnClickNo()
    {
        Destroy(this.gameObject);
        if (this.OnNo != null)
            this.OnNo();
    }
}
