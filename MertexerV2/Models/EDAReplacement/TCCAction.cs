using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.EDAReplacement
{
    public class TCCAction
    {
        public string ClaimID { get; set; }
        public string ClaimActionID { get; set; }
        public string ActionedBy { get; set; }
        public string ActionedByName { get; set; }
        public DateTime? ActionDT { get; set; }
        public string AddedText { get; set; }
        public string CurrentPerson { get; set; }
        public string CurrentDepartmentCode { get; set; }
        public string ContactName { get; set; }

        public TCCAction(
            string claimId,
            string claimActionId,
            string actionedBy,
            string actionedByName,
            string actionedOn,
            string actionedAt,
            string addedText,
            string currentPerson,
            string currentDepartmentCode,
            string contactName
            )
        {
            ClaimID = String.IsNullOrEmpty(claimId) ? null : claimId;

            ClaimActionID = String.IsNullOrEmpty(claimActionId) ? null : claimActionId;

            ActionedBy = String.IsNullOrEmpty(actionedBy) ? null : actionedBy;
            
            ActionedByName = String.IsNullOrEmpty(actionedByName) ? null : actionedByName;

            if (Int32.TryParse(actionedOn, out int adt))
            {
                ActionDT = Framework.DateTimeFromUniVerseOffset(adt);

                if (TimeSpan.TryParse(actionedAt, out TimeSpan ts))
                {
                    ActionDT += ts;
                }
            }

            AddedText = String.IsNullOrEmpty(addedText) ? null : addedText.Trim();

            CurrentPerson = String.IsNullOrEmpty(currentPerson) ? null : currentPerson;

            CurrentDepartmentCode = String.IsNullOrEmpty(currentDepartmentCode) ? null : currentDepartmentCode;

            ContactName = String.IsNullOrEmpty(contactName) ? null : contactName;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
