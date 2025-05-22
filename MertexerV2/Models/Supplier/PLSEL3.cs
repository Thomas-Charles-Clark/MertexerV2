using System.Linq;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.Supplier
{
    [UniVerseFileName("PLSEL3")]
    public class PLSEL3 : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PLSEL3(string key) : base(key)
        {
            Initialise();
        }

        public PLSEL3() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PLSEL3).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Description { get; set; }
    }
}
