using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    [UniVerseFileName("STRTUSERS")]
    public class RadioTerminalUser : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public RadioTerminalUser(string key) : base(key)
        {
            Initialise();
        }

        public RadioTerminalUser() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(RadioTerminalUser).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            RadioTerminalUserFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("RT.USER.NAME")]
        public string Name { get; set; }
    }
}
