using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    public class FieldIndices
    {
        public int[] Indices { get; set; }

        public FieldIndices(int[] fieldIndices)
        {
            Indices = fieldIndices;
        }
    }
}
