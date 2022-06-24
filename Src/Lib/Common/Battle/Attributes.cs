using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Common.Data;
namespace Common.Battle
{
    public class Attributes
    {
        AttributeData Initial = new AttributeData();
        AttributeData Weapon = new AttributeData();
        AttributeData Basic = new AttributeData();
        AttributeData Buff = new AttributeData();
        public AttributeData Final = new AttributeData();

        int Level;

        NAttributeDynamic dynamic;

        public float HP
        {
            get => dynamic.Hp;
            set => dynamic.Hp = (int)Math.Min(MaxHP, value);
        }

        public float MP
        {
            get => dynamic.Mp;
            set => dynamic.Mp = (int)Math.Min(MaxMP, value);
        }

        public float MaxHP { get => Final.MaxHP; }
        public float MaxMP { get => Final.MaxMP; }
        public float ATK { get => Final.ATK; }
        public float DFS { get => Final.DFS; }
        public float CRT { get => Final.CRT; }

        public Attributes(CharacterDefine characterDefine, WeaponDefine weaponDefine, int level, NAttributeDynamic nAttributeDynamic)
        {
            this.dynamic = nAttributeDynamic;
            this.Level = level;
            this.LoadInitAttribute(characterDefine);
            this.LoadWeaponAttribute(weaponDefine);
            this.LoadBasicAttribute(characterDefine, level);
            this.LoadFinalAttribute();
        }
        public void Init(CharacterDefine characterDefine,WeaponDefine weaponDefine,int level,NAttributeDynamic nAttributeDynamic)
        {

            /*
            this.LoadGrowthAttribute(this.Growth, define);
            
            this.InitSecondaryAttribute();


           */
        }


        private void LoadInitAttribute(CharacterDefine define)
        {
            Initial.MaxHP = define.MaxHP;
            Initial.MaxMP = define.MaxMP;
            Initial.ATK = define.ATK;
            Initial.DFS = define.DFS;
            Initial.CRT = define.CRT;

        }
        private void LoadWeaponAttribute(WeaponDefine define)
        {
            Weapon.MaxHP = define.MaxHP;
            Weapon.MaxMP = define.MaxMP;
            Weapon.ATK = define.ATK;
            Weapon.DFS = define.DFS;
            Weapon.CRT = define.CRT;
        }

        private void LoadBasicAttribute(CharacterDefine define,int level)
        {
            Basic.MaxHP = Initial.MaxHP + Weapon.MaxHP + level * define.MaxHPUp;
            Basic.MaxMP = Initial.MaxMP + Weapon.MaxMP + level * define.MaxMP;
            Basic.ATK = Initial.ATK + Weapon.ATK + level * define.ATK;
            Basic.DFS = Initial.DFS + Weapon.DFS + level * define.DFS;
            Basic.CRT = Initial.CRT + Weapon.CRT + level * define.CRT;
        }


        private void LoadFinalAttribute()
        {
            for (int i = 0; i < (int)AttributeType.MAX; i++)
            {
                Final[i] = Basic[i];
            }
        }

    }
}
