using Entities;
using Protocol.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class UINameBar : MonoBehaviour
{
    public Text uiNameBar;
    public Text uiNameLevel;
    public Character character;

    /*
    public Image avatar;
    public Text characterName;
    public Character character;
    */
    
    void Start()
    {
        /*if (character!=null)
        {
            if (character.Info.Type == CharacterType.Monster)
            {
                this.avatar.gameObject.SetActive(false);
            }
            else
                this.avatar.gameObject.SetActive(true);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInfo();
    }
    private void UpdateInfo()
    {
        if (character!=null)
        {
            string name = character.Name;
            string level="Lv."+character.Info.Level.ToString();
            if (name!= this.uiNameBar.text)
            {
                this.uiNameBar.text = name;
            }
            if (level!=this.uiNameLevel.text)
            {
                this.uiNameLevel.text =level;
            }
        }

    }
}
