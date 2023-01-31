using MertexerV2.Models.General;
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
    [UniVerseFileName("STRACKGROUPS")]
    public class RackGroup : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public RackGroup(string key) : base(key)
        {
            Initialise();
        }

        public RackGroup() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(RackGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            RackGroupFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Can be Picked By
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("PICKED.BY.TDW")]
        public string PickedBy { get; set; }

        public string PickedByName 
        { 
            get
            {
                return PickedByUser?.Name;
            } 
        }

        #region Linked Entities

        [UniVerseForeignKey("PickedBy", "MertexerV2.Models.General.RadioTerminalUser")]
        public virtual RadioTerminalUser PickedByUser { get; set; }

        #endregion
    }
}
