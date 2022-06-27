using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollButton : MonoBehaviour
{
    public delegate void PageChanged(bool Isadd);
    public event PageChanged OnChange;

    public void OnClick(bool IsAdd)
    {
        if (OnChange!=null)
        {
            OnChange(IsAdd);
        }
    }
}
