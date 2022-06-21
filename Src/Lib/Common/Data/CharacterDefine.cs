using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protocol;

namespace Common.Data
{
    public class CharacterDefine
    {
        public int TID { get; set; }
        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }

        //Attibute
        public int initLevel { get; set; }
        public int Speed { get; set; }
        public int SpeedInAir { get; set; }
    }
}
