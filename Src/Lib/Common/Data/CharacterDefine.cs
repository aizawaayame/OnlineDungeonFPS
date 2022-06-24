using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol;

namespace Common.Data
{
    /*
    class AttributeDefine
    { enum EAttributeDefine
        {
            MaxHP = 0,
            MaxMP,
            ATK,
            DFS,
            CRT,
            MAX
        }
        private float[] AttributeDefineData { get; set; } = new float[(int)EAttributeDefine.MAX];
        public AttributeDefine(float MaxHP,float MaxMP,float ATK,float DFS,float CRT)
        {
            AttributeDefineData[(int)EAttributeDefine.MaxHP] = MaxHP;
            AttributeDefineData[(int)EAttributeDefine.MaxMP] = MaxMP;
            AttributeDefineData[(int)EAttributeDefine.ATK] = ATK;
            AttributeDefineData[(int)EAttributeDefine.DFS] = DFS;
            AttributeDefineData[(int)EAttributeDefine.CRT] = CRT;
        }

        public float this[int idx]
        {
            get => AttributeDefineData[idx];
        }
    }

    class LevelUpDefine
    {
        enum ELevelUpDefine
        {
            MaxHPUp = 0,
            MaxMPUp,
            ATKUp,
            DFSUp,
            CRTUp,
            MAX
        }
        private float[] LevelUpDefineData { get; set; } = new float[(int)ELevelUpDefine.MAX];
        public LevelUpDefine(float MaxHPUp, float MaxMPUp, float ATKUp, float DFSUp, float CRTUp)
        {
            LevelUpDefineData[(int)ELevelUpDefine.MaxHPUp] = MaxHPUp;
            LevelUpDefineData[(int)ELevelUpDefine.MaxMPUp] = MaxMPUp;
            LevelUpDefineData[(int)ELevelUpDefine.ATKUp] = ATKUp;
            LevelUpDefineData[(int)ELevelUpDefine.DFSUp] = DFSUp;
            LevelUpDefineData[(int)ELevelUpDefine.CRTUp] = CRTUp;
        }

        public float this[int idx]
        {
            get => LevelUpDefineData[idx];
        }
    }
    */
    public class CharacterDefine
    {
        public int TID { get; set; }
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }

        //Basic
        public int initLevel { get; set; }
        public int Speed { get; set; }
        public int SpeedInAir { get; set; }

        //Attibutes
        public float MaxHP { get; set; }
        public float MaxMP { get; set; }
        public float ATK { get; set; }
        public float DFS { get; set; }
        public float CRT { get; set; }

        //Level Up
        public float MaxHPUp { get; set; }
        public float MaxMPUp { get; set; }
        public float ATKUp { get; set; }
        public float DFSUp { get; set; }
        public float CRTUp { get; set; }



    }
}
