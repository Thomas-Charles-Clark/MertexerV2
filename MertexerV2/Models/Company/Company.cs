using MertexerV2.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Company
{
    [UniVerseFileName("COMPANY")]
    public class Company : UniVerseSerializationBase<string>
    {
        public Company(string key) : base(key)
        {

        }

        public Company() :base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Company).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(83)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }
    }
}
