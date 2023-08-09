using MertexerV2.AccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.CustomerCare
{
    [UniVerseFileName("TCCACTION")]
    public class CustomerCareRecordAction : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public CustomerCareRecordAction(string key) : base(key)
        {
            Initialise();
        }

        public CustomerCareRecordAction() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(CustomerCareRecordAction).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            CustomerCareRecordFunctions.UpsertAction(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("ACTIONBY")]
        public string ActionedBy { get; set; }

        public DateTime? ActionedDT { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("ACTIONEDON")]
        public string ActionedDate
        {
            get
            {
                if (ActionedDT.HasValue)
                {
                    return Framework.UniVerseDateString(ActionedDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        ActionedDT = Framework.DateTimeFromUniVerseOffset(valInt);

                        if (TimeSpan.TryParse(ActionedTime, out TimeSpan ts))
                        {
                            ActionedDT += ts;
                        }
                    }
                    else
                    {
                        ActionedDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(3)]
        [UniVerseFieldName("ACTIONEDAT")]
        public string ActionedTime
        {
            get
            {
                if (ActionedDT.HasValue)
                {
                    return ActionedDT.Value.TimeOfDay.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && ActionedDT.HasValue)
                {
                    if (TimeSpan.TryParse(ActionedTime, out TimeSpan ts))
                    {
                        ActionedDT += ts;
                    }
                }
            }
        }

        [UniVerseIndex(4)]
        [UniVerseMultiValue]
        [UniVerseFieldName("ADDEDTEXT")]
        public IEnumerable<string> AddedText { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("CURRENTPERSON")]
        public string AssignedToPerson { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("CURRENTSECTION")]
        public string AssignedToDepartment { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("CONTACT")]
        public string ContactName { get; set; }
    }
}
