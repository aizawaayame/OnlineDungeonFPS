using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UICharacterInfo : MonoBehaviour
    {
        public TMP_Text characterLevel;
        public TMP_Text characterName;
        public Image highlight;
        
        public Protocol.NCharacter Info { get; set; }
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
            if (Info != null)
            {
                this.characterLevel.text = this.Info.Level + "级";
                this.characterName.text = this.Info.Name;
            }
        }
    }
}
