using System.Linq;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.Supplier
{

    [UniVerseFileName("PLSEL1")]
    public class SupplierType : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SupplierType(string key) : base(key)
        {
            Initialise();
        }

        public SupplierType() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SupplierType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Description { get; set; }
    }
}
