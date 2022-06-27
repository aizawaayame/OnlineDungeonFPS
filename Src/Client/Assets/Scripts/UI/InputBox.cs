using UnityEngine;

class InputBox
{
    static Object cacheObject = null;

    public static UIInputBox Show(string message, string title = "", string btnOK = "", string btnCancel = "",string emptyTips="")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIInputBox inputbox = go.GetComponent<UIInputBox>();
        inputbox.Init(title,message,btnOK,btnCancel,emptyTips);
        return inputbox;
    }
}
