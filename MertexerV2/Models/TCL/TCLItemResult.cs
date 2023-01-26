using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MertexerV2.Models.TCL
{
    public class TCLItemResult
    {
        public List<ColumnValue> Columns { get; set; }

        public TCLItemResult()
        {
            Columns = new List<ColumnValue>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class ColumnValue
    {
        private static char delim = (char)253;
        private static char[] delimiter = { delim };

        public int Index { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public IEnumerable<SubValue> MultiValues
        {
            get
            {
                var ci = this.Index;

                if (Value?.Contains(delim) == true)
                {
                    return Value?.Split(delimiter, StringSplitOptions.None).Select((p, i) => new SubValue()
                    {
                        ColumnIndex = ci,
                        Index = i,
                        Value = p
                    });
                }
                else
                {
                    return new SubValue[] 
                    { 
                        new SubValue() 
                        { 
                            ColumnIndex = ci,
                            Index = 0, 
                            Value = this.Value 
                        } 
                    };
                }
            }
        }
    }

    public class SubValue
    {
        private static char delim = (char)252;
        private static char[] delimiter = { delim };

        public int ColumnIndex { get; set; }
        public int Index { get; set; }
        public string Value { get; set; }

        public IEnumerable<SubSubValue> SubValues
        {
            get
            {
                var ci = this.ColumnIndex;
                if (Value?.Contains(delim) == true)
                {
                    return Value?.Split(delimiter, StringSplitOptions.None).Select((p, i) => 
                    new SubSubValue() 
                    {
                        ColumnIndex = ci,
                        Index = i, 
                        Value = p 
                    });
                }
                else
                {
                    return new SubSubValue[] 
                    { 
                        new SubSubValue() 
                        { 
                            ColumnIndex = ci,
                            Index = 0, 
                            Value = this.Value 
                        } 
                    };
                }
            }
        }
    }

    // This is getting silly... so I'm stopping here.
    public struct SubSubValue
    {
        public int ColumnIndex { get; set; }
        public int Index { get; set; }
        public string Value { get; set; }
    }
}
