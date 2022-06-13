using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterSelect
{
    public class UICharacterInfo : MonoBehaviour
    {
        public SkillBridge.Message.NCharacterInfo info;

        public TMP_Text characterLevel;
        public TMP_Text characterName;
        public TMP_Text characterClass;
        public Image highlight;

        public bool IsSelected
        {
            get { return highlight.IsActive(); }
            set
            {
                highlight.gameObject.SetActive(value);
            }
        }

        void Start()
        {
            if (info != null)
            {
                this.characterLevel.text = this.info.Level + "级";
                this.characterName.text = this.info.Name;
                
            }
        }
    }
}
