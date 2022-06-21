using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class MapDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Resource { get; set; }
        public int MapPosX { get; set; }
        public int MapPosY { get; set; }
        public int MapPosZ { get; set; }
        public string Description { get; set; }
    }
}
