using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour {




    #region Fields
    [SerializeField]
    TMP_Text charLevel;
    [SerializeField]
    TMP_Text charName;
    [SerializeField]
    Image highlight;
    #endregion

    public Protocol.Message.NCharacterInfo Info { get; set; }
    public bool Selected
    {
        get { return highlight.IsActive(); }
        set
        {
            highlight.gameObject.SetActive(value);
        }
    }

    // Use this for initialization
    void Start () {
		if(Info!=null)
        {
            this.charLevel.text = this.Info.Level+ "级";
            this.charName.text = this.Info.Name;
        }
	}
}
