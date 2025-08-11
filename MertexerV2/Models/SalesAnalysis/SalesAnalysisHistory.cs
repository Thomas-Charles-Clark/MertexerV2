using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.SalesAnalysis
{
    [UniVerseFileName("OESANAL.HIST")]
    public class SalesAnalysisHistory : SalesAnalysis
    {
        [JsonConstructor]
        public SalesAnalysisHistory(string key) : base(key)
        {
            Initialise();
        }

        public SalesAnalysisHistory() : base()
        {

        }
    }
}
