using MertexerV2;
using MertexerV2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using U2.Data.Client;
using Newtonsoft.Json;
using System.Xml.XPath;
using Tester.Utilities;

namespace Tester.Tests.OrderSearch
{
    public class PassSearchTerm
    {
        public string searchTerm { get; set; }
    }

    public class OrderSearchResult
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddress { get; set; }
        public string EmailAddress { get; set; }
        public string StatusDescription { get; set; }
        public List<OrderSearchLineRecord> OrderLines { get; set; }

        public OrderSearchResult()
        {
            OrderNumber = null;
            StatusDescription = null;
            OrderLines = new List<OrderSearchLineRecord>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class OrderSearchLineRecord
    {
        public string OrderNumber { get; set; }
        public string LineKey { get; set; }
        public string Status { get; set; }
        public string PickingListDocumentNumber { get; set; }
        public string StatusDescription { get; set; }
        public List<string> TrackingURLs { get; set; }
        public string HaulierCode { get; set; }
        public string ParcelDocumentKeys { get; set; }

        public OrderSearchLineRecord (string o, string k, string s, string pl)
        {
            OrderNumber = o;
            LineKey = k;
            Status = s;
            PickingListDocumentNumber = pl;
            TrackingURLs = new List<string>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public static class OrderSearchTestSuite
    {
        public static string[] newlineDelimiter = { System.Environment.NewLine };
        public static char[] mvDelimiter = { (char)253 };
        public static char[] svDelimiter = { (char)252 };

        public static void Run(PassSearchTerm objSearch)
        {
            var result = new OrderSearchResult();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
            {
                UserID = MertexConnectionDetails.UserID,
                Password = MertexConnectionDetails.Password,
                Server = MertexConnectionDetails.Server,
                Database = MertexConnectionDetails.Database,
                ServerType = MertexConnectionDetails.ServerType
            };

            var enGb = new System.Globalization.CultureInfo("en-GB");

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                using (var qe = new QueryExecutor())
                {
                    var cusOrdRef = "SHELF SAMPLE";

                    var preDoc = XDocument.Parse(qe.Run("TOEHEAD", $"CUS.ORD.REF = {cusOrdRef}", "ORDERNO SS.DATEONFILE CUSREF CUSNAME DELNAME DELADDR ECOMM.EMAIL.DES HEADER.REF26 CUS.ORD.REF"));


                    var xDoc = XDocument.Parse(qe.Run("TOEHEAD", $"@ID = {objSearch.searchTerm}", "ORDERNO SS.DATEONFILE CUSNAME DELNAME DELADDR ECOMM.EMAIL.DES HEADER.REF26 "));

                    if (!xDoc.Descendants("record").Any())
                    {
                        result.StatusDescription = $"Order {objSearch.searchTerm} was not found.";
                        return;
                    }
                    else
                    {
                        result.OrderNumber = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']")?.Attribute("value")?.Value;

                        var date = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']")?.Attribute("value")?.Value;

                        DateTime dt = DateTime.MinValue;

                        if (DateTime.TryParseExact(date, "dd/MM/yyyy", enGb, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            result.OrderDate = dt.ToString("dd MMMM yyyy").Insert(2, TesterFramework.GetDaySuffix(dt.Day));
                        }
                        else
                        {
                            result.OrderDate = date;
                        }

                        result.CustomerName = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C3']")?.Attribute("value")?.Value;

                        result.DeliveryName = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C4']")?.Attribute("value")?.Value;

                        result.DeliveryAddress = String.Join(System.Environment.NewLine, xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C5']")?.Attribute("value")?.Value.Split(TesterFramework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));

                        result.EmailAddress = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C6']")?.Attribute("value")?.Value ?? xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C7']")?.Attribute("value")?.Value;
                    }
                }

                if (!String.IsNullOrEmpty(result.OrderNumber))
                {
                    U2Command orderLineComm = conn.CreateCommand();

                    // Get STATUS from LINE
                    // LT 4 ? 
                    // No Tracking Information Available For This Line
                    // EQ 4
                    // Tracking from OEDNHEAD
                    // EQ 5
                    // Tracking from OEDNHHIST

                    orderLineComm.CommandType = System.Data.CommandType.Text;

                    orderLineComm.CommandText = @"
                        SELECT  ORDERNO,
                                LINEKEY,
                                STATUS,    
                                TL.[PL.DOC.NO]
                        FROM    TOELINE AS TL
                        WHERE   TL.ORDERNO = @OrderSearchTerm";

                    orderLineComm.Parameters.Add("@OrderSearchTerm", $"{objSearch.searchTerm}");

                    conn.Open();

                    var reader = orderLineComm.ExecuteReader();

                    while (reader.Read())
                    {
                        result.OrderLines.Add(new OrderSearchLineRecord(
                            !reader.IsDBNull(0) ? reader.GetString(0) : null,
                            !reader.IsDBNull(1) ? reader.GetString(1) : null,
                            !reader.IsDBNull(2) ? reader.GetString(2) : null,
                            !reader.IsDBNull(3) ? reader.GetString(3) : null)
                        );
                    }

                    reader.Close();

                    foreach (var ol in result.OrderLines)
                    {
                        if (!String.IsNullOrEmpty(ol.PickingListDocumentNumber))
                        {
                            var plComm = conn.CreateCommand();

                            plComm.CommandType = System.Data.CommandType.Text;

                            switch (ol.Status)
                            {
                                case "4":
                                    plComm.CommandText = @"
                                SELECT  [HAULIERCODE],
                                        [PARCEL.FULL]
                                FROM    OEDNHEAD USING DICT OEPLHEAD 
                                WHERE   [DOC.NO] = @PickingListID";
                                    break;
                                case "5":
                                    plComm.CommandText = @"
                                SELECT  [HAULIERCODE],
                                        [PARCEL.FULL]
                                FROM    OEDNHHIST AS HH
                                WHERE   HH.[DOC.NO] = @PickingListID";
                                    break;
                                default:
                                    {
                                        ol.StatusDescription = "No tracking details are available for this line at this time.";
                                        continue;
                                    }
                            }

                            plComm.Parameters.Add("@PickingListID", $"{ol.PickingListDocumentNumber}");

                            var plReader = plComm.ExecuteReader();

                            while (plReader.Read())
                            {
                                ol.HaulierCode = !plReader.IsDBNull(0) ? plReader.GetString(0) : null;
                                ol.ParcelDocumentKeys = !plReader.IsDBNull(1) ? plReader.GetString(1) : null;
                            }

                            plReader.Close();

                            if (!String.IsNullOrEmpty(ol.HaulierCode) &&
                                !String.IsNullOrEmpty(ol.ParcelDocumentKeys))
                            {
                                var parcelsplit = ol.ParcelDocumentKeys.Split(TesterFramework.ValueMarkDelimiter, StringSplitOptions.None).ToList();

                                foreach (var ps in parcelsplit)
                                {
                                    var oefindkey = $"{ol.HaulierCode}/{ps}";

                                    var oefindparcelComm = conn.CreateCommand();

                                    oefindparcelComm.CommandType = System.Data.CommandType.Text;

                                    oefindparcelComm.CommandText = @"
                                    SELECT  [SS.WEB.URL]
                                    FROM    OEFINDPARCEL
                                    WHERE   [@ID] = @FindParcelKey";

                                    oefindparcelComm.Parameters.Add("@FindParcelKey", oefindkey);

                                    var fpReader = oefindparcelComm.ExecuteReader();

                                    while (fpReader.Read())
                                    {
                                        if (!fpReader.IsDBNull(0))
                                            ol.TrackingURLs.Add(fpReader.GetString(0));
                                    }

                                    fpReader.Close();
                                }
                            }
                            else
                            {
                                ol.StatusDescription = "No tracking details are available for this line at this time.";
                            }
                        }
                        else
                        {
                            ol.StatusDescription = "No tracking details are available for this line at this time.";
                        }
                    }
                    conn.Close();
                }
                else
                {
                    result.StatusDescription = $"Order {objSearch.searchTerm} was not found.";
                }
            }

            Console.WriteLine();
            Console.WriteLine(result);
        }

        public static void OtherRun()
        {
            using (var ce = new CommandExecutor())
            {
                var commandOutput = ce.Arbitrary("LIST.ITEM OEDNHHIST 999018001-1 COL.HDR.SUPP ID.SUP COUNT.SUP").Trim();

                // Split on new lines
                // Attach those new lines which don't start with a column identifier to the end of the line above

                var result = CommandExecutor.CrunchTCLResult(commandOutput);

                Console.WriteLine(result);
            }
        }

        public static int UpdateCustomTrackingTest(string uniVerseAccount, string picklistNumber, string parcelNumber, string trackingUrl, string postcode)
        {
            int result = MertexerV2.SQLFunctions.TrackingUpdate.Update(uniVerseAccount, picklistNumber, parcelNumber, trackingUrl, postcode);

            return result;
        }
    }
}
