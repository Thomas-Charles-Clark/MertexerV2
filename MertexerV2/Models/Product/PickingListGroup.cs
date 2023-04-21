using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PickingList
{
    [UniVerseFileName("PICKLISTGRP.DES")]
    public class PickingListGroup : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PickingListGroup(string key) : base(key)
        {
            Initialise();
        }

        public PickingListGroup() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PickingListGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            PickingListGroupFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
