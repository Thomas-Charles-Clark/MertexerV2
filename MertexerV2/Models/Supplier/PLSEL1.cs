using System.Linq;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.Supplier
{

    [UniVerseFileName("PLSEL1")]
    public class PLSEL1 : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PLSEL1(string key) : base(key)
        {
            Initialise();
        }

        public PLSEL1() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PLSEL1).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Description { get; set; }
    }
}
