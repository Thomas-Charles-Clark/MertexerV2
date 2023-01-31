using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("STRACKS")]
    public class Rack : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Rack(string key) : base(key)
        {
            Initialise();
        }

        public Rack() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Rack).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            RackFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("DESCR")]
        public string Name { get; set; }

        /// <summary>
        /// Market
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("WHOUSE.ROUTE")]
        public string WarehouseRoute { get; set; }

        /// <summary>
        /// Picking Indicator
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("PICKING.IND")]
        public bool Pickable { get; set; }

        /// <summary>
        /// Rack Type
        /// If entered must be one of: (C)utting, (B)ulk, (P)icking Type Rack or (T)ransit
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("RACK.TYPE")]
        public string Type { get; set; }
    }
}
