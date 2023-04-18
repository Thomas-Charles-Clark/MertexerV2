using MertexerV2.Models.Stock;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    public partial class Stock
    {
        private static readonly char[] pipeDelimiter = new [] { '|' };
        private static readonly char[] secondDelimiter = new char[] { '*' };

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "BatchPiece_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable BatchPiece(string ProductCodes, string StockLocations, string EnquiryType)
        {
            var results = new List<BatchPieceResult>();

            var productCodesArray = ProductCodes.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            var uvSelectStatement = $"PRODCODE = \"{String.Join("\" OR PRODCODE = \"", productCodesArray)}\"";

            XDocument xDoc = null;

            var checkingLocations = new List<string>();

            if (StockLocations != null)
            {
                checkingLocations = StockLocations.Split(new char[] { '|' }).ToList();
            }

            using (var queryExecutor = new QueryExecutor())
            {
                xDoc = XDocument.Parse(queryExecutor.Run(
                    uvFileName: "B2C_STOCK_ITEM_BREAKDOWN",
                    uvSelectStatement: uvSelectStatement,
                    fieldsToReturn: "",
                    webData: String.IsNullOrEmpty(StockLocations) ? "MSL=0.1" : $"LOC={StockLocations};MSL=0.1",
                    returnType: "XML"));
            }

            /*
<fd UV_fieldname="PRODCODE" field_id="0.0" num_chars="15" type="text" name="Product_Code" id="C1"/>
<fd UV_fieldname="PSE.PICKABLE.WI" field_id="0.0" num_chars="20" type="text" name="Pickable" id="C2"/>
<fd UV_fieldname="PSE.RACK.WI" field_id="0.0" num_chars="20" type="text" name="Rack" id="C3"/>
<fd UV_fieldname="PSE.RACK.FREELEN.WI" field_id="0.0" num_chars="20" type="text" name="Rack_Free_Len" id="C4"/>
<fd UV_fieldname="PSE.RACKTYPE.WI" field_id="0.0" num_chars="20" type="text" name="Rack_Type" id="C5"/>
<fd UV_fieldname="PSE.RESERVE.WI" field_id="0.0" num_chars="8" type="text" name="Reserved" id="C6"/>
<fd UV_fieldname="PSE.PO.DUE.WI" field_id="0.0" num_chars="20" type="text" name="PO_Due_Date" id="C7"/>
<fd UV_fieldname="PSE.PONO.WI" field_id="0.0" num_chars="3" type="text" name="PO_Number" id="C8"/>
<fd UV_fieldname="PSE.PO.OSQTY.WI" field_id="0.0" num_chars="8" type="text" name="OS_Quantity" id="C9"/>
<fd UV_fieldname="PSE.PO.LOC.WI" field_id="0.0" num_chars="20" type="text" name="Location" id="C10"/>
<fd UV_fieldname="PSO.PO.CONF.WI" field_id="0.0" num_chars="20" type="text" name="Location" id="C11"/>

<fd id="C1" value="F2016/01"/>
<fd id="C2" value="1|1|1|1|1|1"/>
<fd id="C3" value="WH*P006159*R029269*PFA10-2B|WH*P006159*G029270*PFA2-3B|WH*P010507*Y031777*PFC32-5A|WH*P010507*Y042513*PFB9-4E|WH*P010507*G033816*PFA28-3C|WH*P010507*G042512*BFC13-13"/>
<fd id="C4" value="7.30|0.00|38.60|16.40|3.90|2.30"/>
<fd id="C5" value="P|P|P|P|P|P"/>
<fd id="C6" value="0.00|1.00|0.00|0.00|0.00|0.00"/>
<fd id="C7" value="26/11/19|26/11/19"/>\
<fd id="C8" value="P016529|P010507"/>
<fd id="C9" value="206|58"/>
<fd id="C10" value="WH|WH"/>
<fd id="C11" value="PO Conf|PO Unconf"/>
             */


            xDoc.Descendants("record").ToList().ForEach(c =>
            {
                var productCode = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                var pickableFlags = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;
                var racks = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;
                var rackFreeLengths = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;
                var rackTypes = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;
                var reserves = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;
                var purchaseOrderDueDates = c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;
                var purchaseOrderNumbers = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value;
                var purchaseOrderQuantities = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value;
                var purchaseOrderStockLocations = c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value;
                var purchaseOrderStatuses = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;


                var pickableFlagArray = pickableFlags.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var racksArray = racks.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var stockLocations = new List<string>();
                var batches = new List<string>();
                var pieces = new List<string>();
                var rackNames = new List<string>();

                // racks processing: StockLocation*Batch*Piece*Rack
                foreach (var entry in racksArray)
                {
                    var rackData = entry;

                    if (String.IsNullOrEmpty(entry))
                    {
                        rackData = "***";
                    }

                    var rackBreakdown = rackData.Split(secondDelimiter, StringSplitOptions.None);

                    stockLocations.Add(rackBreakdown[0]);
                    batches.Add(rackBreakdown[1]);
                    pieces.Add(rackBreakdown[2]);
                    rackNames.Add(rackBreakdown[3]);
                }

                var rackFreeLengthsArray = rackFreeLengths.Split(pipeDelimiter, StringSplitOptions.None).ToList().ConvertAll<decimal>(x => { 
                    return Convert.ToDecimal(String.IsNullOrEmpty(x) ? "0" : x);
                });

                var rackTypesArray = rackTypes.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var reservesArray = reserves.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var purchaseOrderDueDateArray = purchaseOrderDueDates.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var purchaseOrderNumberArray = purchaseOrderNumbers.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var purchaseOrderQuantityArray = purchaseOrderQuantities.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var purchaseOrderStockLocationArray = purchaseOrderStockLocations.Split(pipeDelimiter, StringSplitOptions.None).ToArray();
                var purchaseOrderStatusArray = purchaseOrderStatuses.Split(pipeDelimiter, StringSplitOptions.None).ToArray();

                if (String.IsNullOrEmpty(EnquiryType) || EnquiryType == "S")
                {
                    for (var i = 0; i < stockLocations.Count(); i++)
                    {
                        var add = false;

                        if (checkingLocations.Count > 0)
                        {
                            if (checkingLocations.Contains(stockLocations[i]))
                            {
                                add = true;
                            }
                        }
                        else
                        {
                            add = true;
                        }

                        if (add)
                        {
                            //if (batches[i] == "NA")
                            //{
                            //    batches[i] = "";
                            //}

                            results.Add(new BatchPieceResult(
                                type: "S",
                                productCode: productCode,
                                batch: batches[i],
                                piece: pieces[i],
                                pickable: Convert.ToBoolean(Convert.ToInt32(String.IsNullOrEmpty(pickableFlagArray[i]) ? "1" : pickableFlagArray[i])),
                                rack: rackNames[i],
                                rackType: rackTypesArray[i],
                                quantity: Convert.ToDecimal(rackFreeLengthsArray[i]),
                                onReserve: String.IsNullOrEmpty(reservesArray[i]) ? null : (decimal?)Convert.ToDecimal(reservesArray[i]),
                                poNumber: null,
                                poDueDate: null, 
                                poStatus: null,
                                locationCode: String.IsNullOrWhiteSpace(stockLocations[i]) ? null : stockLocations[i]
                            ));
                        }
                    }
                }

                if (String.IsNullOrEmpty(EnquiryType) || EnquiryType == "P")
                {
                    for (var i = 0; i < purchaseOrderNumberArray.Count(); i++)
                    {
                        var add = false;

                        if (checkingLocations.Count > 0)
                        {
                            if (checkingLocations.Contains(purchaseOrderStockLocationArray[i]))
                            {
                                add = true;
                            }
                        }
                        else
                        {
                            add = true;
                        }

                        // Because Purchase Orders are unusable without a Purchase Order Number
                        if (purchaseOrderNumberArray[i] == "")
                        {
                            add = false;
                        }

                        if (add)
                        {
                            var poStatus = purchaseOrderStatusArray[i];

                            bool? poStatusBool = null;

                            if (!String.IsNullOrEmpty(poStatus))
                            {
                                switch (poStatus)
                                {
                                    case "PO Conf":
                                        poStatusBool = true;
                                        break;
                                    case "PO Unconf":
                                        poStatusBool = false;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            results.Add(new BatchPieceResult(
                                type: "P",
                                productCode: productCode,
                                batch: null,
                                piece: null,
                                pickable: null,
                                rack: null,
                                rackType: null,
                                quantity: Convert.ToDecimal(purchaseOrderQuantityArray[i]),
                                onReserve: null,
                                poNumber: purchaseOrderNumberArray[i],
                                poDueDate: purchaseOrderDueDateArray[i],
                                poStatus: poStatusBool,
                                locationCode: purchaseOrderStockLocationArray[i]
                            ));
                        }
                    }
                }
            });

            var overLength = results.Where(p => p.Rack?.Length > 20).ToList();

            if (overLength.Any())
            {
                var currentcolour = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"Found {overLength.Count} bad records:");

                Console.WriteLine();

                overLength.ForEach(r => Console.WriteLine(r));

                SendProblemEmail(overLength);

                Console.ForegroundColor = currentcolour;
            }

            results = results.Except(overLength).ToList();

            return new ArrayList(results.ToList());
        }

        public static void BatchPiece_FillRow(
            object obj,
            out SqlString Type,
            out SqlString ProductCode,
            out SqlString LocationCode,
            out SqlString Batch,
            out SqlString Piece,
            out SqlBoolean Pickable,
            out SqlString Rack,
            out SqlString RackType,
            out SqlDecimal Quantity,
            out SqlDecimal OnReserve,
            out SqlDateTime PODate,
            out SqlString POMessage,
            out SqlBoolean POStatus
            )
        {
            var batchPiece = (BatchPieceResult)obj;

            Type = batchPiece.Type;
            ProductCode = batchPiece.ProductCode;
            LocationCode = batchPiece.LocationCode;
            Batch = batchPiece.Batch;
            Piece = batchPiece.Piece;
            Rack = batchPiece.Rack;
            RackType = batchPiece.RackType;
            Quantity = batchPiece.Quantity;
            OnReserve = batchPiece.OnReserve ?? SqlDecimal.Null;
            Pickable = batchPiece.Pickable ?? SqlBoolean.Null;
            PODate = batchPiece.PurchaseOrderDate ?? SqlDateTime.Null;
            POMessage = batchPiece.PurchaseOrderMessage;
            POStatus = batchPiece.PurchaseOrderStatus ?? SqlBoolean.Null;
        }

        public static void SendProblemEmail(List<BatchPieceResult> discards, string subject = "MertexerV2 Stock Record(s) Discarded", string body = "Muppets wielding scanners? Possibly...")
        {
            using (var client = new SmtpClient("smtp.office365.com", 587))
            {
                // set smtp-client with basicAuthentication
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                var basicAuthenticationInfo = new
                    System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                client.Credentials = basicAuthenticationInfo;

                var from = new MailAddress("portal.noreply@designersguild.com", "Designers Guild Portal");
                var to = new MailAddress("itsupport@designersguild.com", "IT Support");

                var mail = new MailMessage(from, to);

                mail.CC.Add("tclark@designersguild.com,mchristie@designersguild.com");
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;

                discards.ForEach(c =>
                {
                    body += "<br/></br>" + c.ToString().Replace(System.Environment.NewLine, "<br/>");
                });

                mail.Body = body;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                mail.IsBodyHtml = true;

                client.Send(mail);
            }
        }
    }
}
