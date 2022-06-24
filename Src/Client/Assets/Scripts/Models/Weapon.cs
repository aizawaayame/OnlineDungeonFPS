using Common.Data;
using Managers;

namespace Models
{
    public class Weapon
    {

        #region Public Properties
        public int Id { get; private set; }
        public WeaponDefine WeaponDefine { get; private set; }

        public string Name
        {
            get => WeaponDefine.Name;
        }
        
        public WeaponType Type { get => WeaponDefine.Type; }
        public float MaxHP { get => WeaponDefine.MaxHP; }
        public float MaxMP { get => WeaponDefine.MaxMP; }
        public float ATK { get => WeaponDefine.ATK;  }
        public float DFS { get => WeaponDefine.DFS;  }
        public float CRT { get => WeaponDefine.CRT;  }
        public float DMG { get => WeaponDefine.DMG;  }
        public float CRTR { get => WeaponDefine.CRTR;  }
        public bool Areable { get => WeaponDefine.Areable;  }
        public string Resource { get => WeaponDefine.Resource;  }
        
        #endregion

        #region Constructor

        public Weapon(int id)
        {
            Id = id;
            WeaponDefine = DataManager.Instance.WeaponDefines[id];
        }

        #endregion

    }
}
