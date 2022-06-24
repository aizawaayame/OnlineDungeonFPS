using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;
using Network;
using Protocol;
using Common.Battle;

namespace GameServer.Entities
{
    /// <summary>
    /// Player data class.
    /// </summary>
    class Character : Entity, IPostResponser
    {

        #region Public Properties
        public int CharacterId { get => TCharacter.ID; }
        public int ConfigId { get => TCharacter.TID; }

        public int Exp { get => NCharacter.Exp; set => NCharacter.Exp = value; }
        public int Level { get => NCharacter.Level; set => NCharacter.Level = value; }
        public long Gold { get => NCharacter.Gold; set => NCharacter.Gold = value; }
        public string CharacterName { get => TCharacter.Name; }
        public CharacterClass CharacterClass { get => (CharacterClass)TCharacter.Class; }

        public float HP
        {
            get => this.NCharacter.attrDynamic.Hp;
            set => this.NCharacter.attrDynamic.Hp = value;
        }

        public float MP
        {
            get => this.NCharacter.attrDynamic.Mp;
            set => this.NCharacter.attrDynamic.Mp = value;
        }
        public Attributes Attributes { get; set; }
        public List<NWeapon> AllWeapons { get => NCharacter.AllWeapons; }
        public NWeapon CurrentWeapon { get => NCharacter.Weapon; set => NCharacter.Weapon = value; }
        public TCharacter TCharacter { get; set; }
        public NCharacter NCharacter { get; set; }
        public CharacterDefine Define { get; private set; }
        

        #endregion

        #region Constructor

        public Character(TCharacter tCharacter) : base(Map.GetMapInitPos(1),Vector3Int.zero)
        {
            this.TCharacter = tCharacter;
            this.NCharacter = new NCharacter(){
                Class = (CharacterClass)TCharacter.Class,
                ConfigId = TCharacter.TID,
                EntityId = 0,
                Entity = this.NEntity,
                Exp = TCharacter.Exp,
                Gold = TCharacter.Gold,
                Id = TCharacter.ID,
                Level = TCharacter.Level,
                mapId = 1,
                Name = TCharacter.Name,
            };
            foreach (TWeapon tWeapon in tCharacter.Weapons)
            {
                NWeapon nWeapon = new NWeapon();
                nWeapon.Id = tWeapon.ID;
                this.NCharacter.AllWeapons.Add(nWeapon);
            }
            this.NCharacter.Weapon = NCharacter.AllWeapons.FirstOrDefault();
            this.Define = DataManager.Instance.CharacterDefines[this.ConfigId];
            WeaponDefine weaponDefine = DataManager.Instance.WeaponDefines[CurrentWeapon.Id];

            this.Attributes = new Attributes(Define, weaponDefine, Level, NCharacter.attrDynamic);
        }

        #endregion

        #region Public Methods

        internal void AddExp(int exp)
        {
            this.Exp += exp;
            this.CheckLevelUp();
        }
        public void PostProcess(NetMessageResponse message)
        {
            
        }

        public void Clear()
        {
            
        }
        #endregion
        
        #region Private Methods

        void CheckLevelUp()
        {
            // exp = power(lv,3)*10 + lv*40 +50
            long expNeeded = (long)Math.Pow(this.Level, 3) * 10 + this.Level * 40 + 50;
            if (this.Exp > expNeeded)
            {
                this.LevelUp();
            }
        }
        void LevelUp()
        {
            this.Level += 1;
            Log.Info($"Character{this.CharacterId}:{this.CharacterName} LevelUp{this.Level}");
            CheckLevelUp();
        }

        #endregion

    }
}
