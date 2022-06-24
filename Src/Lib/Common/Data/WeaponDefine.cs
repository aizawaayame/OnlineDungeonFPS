using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{   
    public enum WeaponType
    {
        Manual = 0,
        Automatic = 1,
        Charge = 2,
    }
    public class WeaponDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public WeaponType Type { get; set; }

        public float MaxHP { get; set; }
        public float MaxMP { get; set; }
        public float ATK { get; set; }
        public float DFS { get; set; }
        public float CRT { get; set; }
        public float DMG { get; set; }
        public float CRTR { get; set; }
        public bool Areable { get; set; }
        public string Resource { get; set; }
    }
}
