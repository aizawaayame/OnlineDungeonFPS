using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Battle
{
    public class AttributeData
    {
        public float[] Data = new float[(int)AttributeType.MAX];

        public float MaxHP { get => Data[(int)AttributeType.MaxHP]; set => Data[(int)AttributeType.MaxHP] = value; }
        public float MaxMP { get => Data[(int)AttributeType.MaxMP]; set => Data[(int)AttributeType.MaxMP] = value; }
        public float ATK { get => Data[(int)AttributeType.ATK]; set => Data[(int)AttributeType.ATK] = value; }
        public float DFS { get => Data[(int)AttributeType.DFS]; set => Data[(int)AttributeType.DFS] = value; }
        public float CRT { get => Data[(int)AttributeType.CRT]; set => Data[(int)AttributeType.CRT] = value; }

        public void Reset()
        {
            for (int i = 0; i < (int)AttributeType.MAX; i++)
            {
                Data[i] = 0;
            }
        }

        public float this[int idx]
        {
            get => Data[idx];
            set => Data[idx] = value;
        }

    }
}
