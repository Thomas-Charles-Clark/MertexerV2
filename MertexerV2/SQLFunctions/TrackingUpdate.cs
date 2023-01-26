using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.SQLFunctions
{
    public class TrackingUpdate
    {
        private static Dictionary<string, string> MertexLanguageMap = new Dictionary<string, string>()
        {
            { "01", "en_GB" },       // English
            { "02", "nl_BE" },       // Flemish (Belgian Dutch)
            { "03", "da_DK" },       // Danish
            { "04", "fl_FL" },       // Finnish
            { "05", "fr_FR" },       // French
            { "06", "de_DE" },       // German
            { "07", "nl_NL" },       // Dutch
            { "08", "nn_NO" },       // Norwegian
            { "09", "es_ES" },       // Spanish
            { "10", "sv_SE" },       // Swedish
            { "11", "it_IT" },       // Italian
            { "OLD05*2", "fr_FR" },  // French
            { "12", "en_IE" },       // Irish
            { "13", "en_GB" },       // EXP Sterling (English)
            { "14", "en_GB" },       // EXP Dollars (English
            { "15", "el_GR" },       // Greek
            { "16", "pt_PT" },       // Portuguese
            { "17", "fr_BE" },       // Belgian French
            { "18", "fr_CH" },       // Swiss French
            { "19", "de_AT" },       // Austrian German
            { "20", "de_CH" },       // Swiss German
            { "21", "en_US" },       // USA English
            { "22", "en_GB" },       // English
            { "23", "en_GB" },       // English
            { "24", "en_GB" },       // English
            { "25", "en_GB" },       // English
            { "26", "en_GB" }        // English
        };


        /// <summary>
        /// A note on translation of the TrackingURL : 
        /// if I know that a given URL contains a tag / placeholder for culture
        /// then I should be able to navigte from parcel or pickinglist to an SLCUST record
        /// I should then be able to navigate to a single CLMARKTYPE record from MARKTYPE.DES
        /// The CLMARKTYPE table currently does not have a field for Culture. It would be good if we could add 
        /// one and give it an appropriate value for each market.
        /// </summary>
        /// <param name="PicklistNumber"></param>
        /// <param name="ParcelNumber"></param>
        /// <param name="TrackingURL"></param>
        /// <returns></returns>
        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int Update(string UniVerseAccount, string PicklistNumber, string ParcelNumber, string TrackingURL, string DeliveryPostcode)
        {
            int result = 0;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                // Need to lift <1>CUSREF from TOEHEAD

                // Need to lift <694>TITLE.TRANS.DES from SLCUST

                // Determine a Language.

                var orderHead = "";
                var cusRef = "";

                try
                {
                    var picklistHeadFile = ce.Session.CreateUniFile("OEDNHEAD");

                    orderHead = picklistHeadFile.ReadField(PicklistNumber, 9).StringValue;

                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    var picklistHeadFile = ce.Session.CreateUniFile("OEDNHHIST");

                    orderHead = picklistHeadFile.ReadField(PicklistNumber, 9).StringValue;
                }

                try
                {
                    var orderFile = ce.Session.CreateUniFile("TOEHEAD");

                    cusRef = orderFile.ReadField(orderHead, 1).StringValue;

                    var customerFile = ce.Session.CreateUniFile("SLCUST");

                    var mertexLanguage = customerFile.ReadField(cusRef, 694).StringValue;

                    var culture = MertexLanguageMap.Where(p => p.Key == mertexLanguage).Select(p => p.Value).Single();

                    TrackingURL = TrackingURL.Replace("{CULTURE}", culture);

                    if (TrackingURL.Contains("{POSTCODE}"))
                    {
                        if (!String.IsNullOrEmpty(DeliveryPostcode))
                            TrackingURL = TrackingURL.Replace("{POSTCODE}", DeliveryPostcode);
                        else
                            TrackingURL = TrackingURL.Replace("{POSTCODE}", "");
                    }
                }
                catch (Exception)
                {

                }







                if (String.IsNullOrEmpty(ParcelNumber))
                {
                    try
                    {
                        var file = ce.Session.CreateUniFile("OEDNHEAD");

                        var readResult = file.ReadField(PicklistNumber, 535);

                        var toWrite = ce.Session.CreateUniDynArray();

                        toWrite.Insert(0, TrackingURL);

                        file.LockRecord(PicklistNumber, 1);
                        file.WriteField(PicklistNumber, 535, toWrite);
                        file.UnlockRecord(PicklistNumber);

                        result = 1;
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {
                        var file = ce.Session.CreateUniFile("OEDNHHIST");

                        var readResult = file.ReadField(PicklistNumber, 535);

                        var toWrite = ce.Session.CreateUniDynArray();

                        toWrite.Insert(0, TrackingURL);

                        file.LockRecord(PicklistNumber, 1);
                        file.WriteField(PicklistNumber, 535, toWrite);
                        file.UnlockRecord(PicklistNumber);

                        result = 1;
                    }
                }
                else
                {
                    var file = ce.Session.CreateUniFile("OEFINDPARCEL");

                    var readResult = file.ReadField(ParcelNumber, 41);

                    var toWrite = ce.Session.CreateUniDynArray();

                    toWrite.Insert(0, TrackingURL);

                    file.LockRecord(ParcelNumber, 1);
                    file.WriteField(ParcelNumber, 41, toWrite);
                    file.UnlockRecord(ParcelNumber);

                    result = 1;
                }
            }

            return result;
        }
    }
}
