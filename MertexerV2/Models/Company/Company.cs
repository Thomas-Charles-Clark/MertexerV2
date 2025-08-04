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

        [UniVerseIndex(10)]
        [UniVerseFieldName("GLPER")]
        public byte ThisGeneralLedgerPeriod { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("GLPRDS")]
        public byte NumberOfFinancialPeriods { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(83)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }

        [UniVerseIndex(134)]
        [UniVerseFieldName("PLPER")]
        public byte? ThisPurchaseLedgerPeriod { get; set; }

        [UniVerseIndex(205)]
        [UniVerseFieldName("PREPAY.GLCODES.DES")]
        [UniVerseMultiValue]
        public IEnumerable<string> PrepaymentGeneralLedgerCodes { get; set; }
    }
}
