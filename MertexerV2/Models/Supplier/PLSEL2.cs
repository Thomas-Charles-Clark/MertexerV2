using System.Linq;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.Supplier
{

    [UniVerseFileName("PLSEL2")]
    public class PLSEL2 : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PLSEL2(string key) : base(key)
        {
            Initialise();
        }

        public PLSEL2() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PLSEL2).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Description { get; set; }
    }
}
