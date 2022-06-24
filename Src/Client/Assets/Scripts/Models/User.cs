
using Common.Data;
using Entities;
using GameObjects;
using Managers;
using Utilities;
using Protocol;
using UnityEngine;

namespace Models
{
    /// <summary>
    /// contains user's info
    /// </summary>
    public class User : Singleton<User>
    {
        public float HP
        {
            get => Character.Attributes.HP;
            set => Character.Attributes.HP = value;
        }

        public float MP
        {
            get => Character.Attributes.MP;
            set => Character.Attributes.MP = value;
        }
        public NUser NUser { get; set; }
        public Character Character { get; set; }
        public PlayerController PlayerController { get; set; }
        public NCharacter NCharacter { get; set; }
        public GameObject CurrentCharacterObject { get; set; }

        public MapDefine CurrentMapDefine
        {
            get
            {
                MapDefine mapDefine = DataManager.Instance.MapDefines[NCharacter.mapId];
                return mapDefine;
            }
        }
    }
}
