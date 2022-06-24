using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Battle
{
    public enum AttributeType
    {
        None = -1,
        
        MaxHP = 0,

        MaxMP = 1,
        /// <summary>
        /// Attack.
        /// </summary>
        ATK = 2,
        /// <summary>
        /// Defense
        /// </summary>
        DFS = 3,
        /// <summary>
        /// Attack speed.
        /// </summary>
        DPD = 4,
        /// <summary>
        /// Critical ratio.
        /// </summary>
        CRT = 5,

        MAX
    }
}
