
using System;
using Models;
using TMPro;
using UnityEngine;

public class UIGold : MonoBehaviour
{

    #region Fields

    public TMP_Text GoldText;

    #endregion

    void Start()
    {
        GoldText.text = String.Format($"Gold:{User.Instance.CurrentCharacter.Gold}");
    }

    void Update()
    {
        GoldText.text = String.Format($"Gold:{User.Instance.CurrentCharacter.Gold}");
    }
}

