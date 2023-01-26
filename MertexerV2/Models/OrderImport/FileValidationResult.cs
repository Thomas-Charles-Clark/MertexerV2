using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.OrderImport
{
    public class FileValidationResult
    {
        public long TradeOrderImportFileID { get; set; }

        public string FileName { get; set; }

        public List<LineValidationResult> LineValidationFailures { get; set; }

        public string Exception { get; set; }

        public bool Valid 
        { 
            get
            {
                return Exception == null && !LineValidationFailures.Any();
            }
        }

        public List<OrderHeadToImport> OrdersToImport { get; set; }

        public string HeaderRow { get; set; }

        public List<ImportLine> FileContents { get; set; }

        public FileValidationResult(long tradeOrderImportFileId, string fileName, string headerRow, List<ImportLine> fileContents)
        {
            TradeOrderImportFileID = tradeOrderImportFileId;
            FileName = fileName;
            HeaderRow = headerRow?.ToUpperInvariant();
            FileContents = fileContents;
            Exception = null;
            LineValidationFailures = new List<LineValidationResult>();
            OrdersToImport = new List<OrderHeadToImport>();
        }

        public void ProcessOrders()
        {
            if (HeaderRow?.StartsWith("HEADER,") == true)
            {
                // Dutch file, already processed into orders due to the nature of the file format.
            }
            else
            {
                if (!FileName.Contains("_DISTRIBUTOR_") && !FileName.Contains("_INTERNAL_"))
                {
                    var orderHeads = FileContents.GroupBy(p =>
                    {
                        return new
                        {
                            tradeAccountNumber = p.TradeAccountNumber,
                            orderReference = p.OrderReference,
                            deliveryName = p.DeliveryName,
                            deliveryAddressLine1 = p.DeliveryAddressLine1,
                            deliveryAddressLine2 = p.DeliveryAddressLine2,
                            deliveryTown = p.DeliveryTown,
                            deliveryCounty = p.DeliveryCounty,
                            deliveryPostcode = p.DeliveryPostcode,
                            deliveryCountryCode = p.DeliveryCountryCode,
                            deliveryPhoneNumber = p.DeliveryPhoneNumber,
                            email = p.Email,
                            shippingAlertEmailAddress = p.ShippingAlertEmailAddress,
                            contactPhone = p.ContactPhone
                        };
                    }).ToList();

                    orderHeads.ForEach(c =>
                    {
                        var head = new OrderHeadToImport(
                            tradeOrderImportFileId: TradeOrderImportFileID,
                            orderType: "Standard",
                            tradeAccountNumber: c.Key.tradeAccountNumber,
                            orderReference: c.Key.orderReference,
                            deliveryName: c.Key.deliveryName,
                            deliveryAddressLine1: c.Key.deliveryAddressLine1,
                            deliveryAddressLine2: c.Key.deliveryAddressLine2,
                            deliveryTown: c.Key.deliveryTown,
                            deliveryCounty: c.Key.deliveryCounty,
                            deliveryPostcode: c.Key.deliveryPostcode,
                            deliveryCountryCode: c.Key.deliveryCountryCode,
                            deliveryPhoneNumber: c.Key.deliveryPhoneNumber,
                            email: c.Key.email,
                            shippingAlertEmailAddress: c.Key.shippingAlertEmailAddress,
                            contactPhone: c.Key.contactPhone,
                            heldForSales: false
                        );

                        var lines = new List<OrderLineToImport>();

                        foreach (var line in c)
                        {
                            //var line = l.Split(new string[] { "," }, StringSplitOptions.None);

                            var d = Decimal.MinValue;

                            if (Decimal.TryParse(line.Quantity, out d))
                            {
                                lines.Add(new OrderLineToImport(
                                    fileLineNumber: line.FileLineNumber,
                                    productCode: line.ProductCode,
                                    quantity: d,
                                    type: line.Type,
                                    lineReference: line.LineReference,
                                    memo: line.Memo
                                ));
                            } 
                            else
                            {
                                throw new Exception($"Error in TradeOrderImportFileID: {head.TradeOrderImportFileID} - processing {head.TradeAccountNumber}'s order, ref. {head.DeliveryReference}: attempting to convert \"{line.Quantity}\" to decimal.");
                            }
                        }

                        head.OrderLines.AddRange(lines);

                        OrdersToImport.Add(head);

                    });
                }
                else
                {
                    if (FileName.Contains("_DISTRIBUTOR_"))
                    {
                        var orderHeads = FileContents.GroupBy(p =>
                        {
                            return new
                            {
                                tradeAccountNumber = p.TradeAccountNumber,
                                orderReference = p.OrderReference,
                                deliveryName = p.DeliveryName,
                                deliveryAddressLine1 = p.DeliveryAddressLine1,
                                deliveryAddressLine2 = p.DeliveryAddressLine2,
                                deliveryTown = p.DeliveryTown,
                                deliveryCounty = p.DeliveryCounty,
                                deliveryPostcode = p.DeliveryPostcode,
                                deliveryCountryCode = p.DeliveryCountryCode,
                                deliveryPhoneNumber = p.DeliveryPhoneNumber,
                                email = p.Email,
                                shippingAlertEmailAddress = p.ShippingAlertEmailAddress,
                                contactPhone = p.ContactPhone,
                                haulierCode = p.HaulierCode,
                                carriageCode = p.CarriageCode,
                                haulierAccountNumber = p.HaulierAccountNumber,
                                taxesDutiesToInvoiceAccount = p.TaxesDutiesToInvoiceAccount,
                                deliveryTerms = p.DeliveryTerms,
                                deliveryType = p.DeliveryType,
                                endCustomer = p.EndCustomer
                            };
                        }).ToList();

                        orderHeads.ForEach(c =>
                        {
                            var head = new OrderHeadToImport(
                                tradeOrderImportFileId: TradeOrderImportFileID,
                                orderType: "Standard",
                                tradeAccountNumber: c.Key.tradeAccountNumber,
                                orderReference: c.Key.orderReference,
                                deliveryName: c.Key.deliveryName,
                                deliveryAddressLine1: c.Key.deliveryAddressLine1,
                                deliveryAddressLine2: c.Key.deliveryAddressLine2,
                                deliveryTown: c.Key.deliveryTown,
                                deliveryCounty: c.Key.deliveryCounty,
                                deliveryPostcode: c.Key.deliveryPostcode,
                                deliveryCountryCode: c.Key.deliveryCountryCode,
                                deliveryPhoneNumber: c.Key.deliveryPhoneNumber,
                                email: c.Key.email,
                                shippingAlertEmailAddress: c.Key.shippingAlertEmailAddress,
                                contactPhone: c.Key.contactPhone,
                                haulierCode: c.Key.haulierCode,
                                carriageCode: c.Key.carriageCode,
                                haulierAccountNumber: c.Key.haulierAccountNumber,
                                taxesDutiesToInvoiceAccount: c.Key.taxesDutiesToInvoiceAccount,
                                deliveryTerms: c.Key.deliveryTerms,
                                deliveryType: c.Key.deliveryType,
                                endCustomer: c.Key.endCustomer,
                                heldForSales: false
                            );

                            var lines = new List<OrderLineToImport>();

                            foreach (var line in c)
                            {
                                var d = Decimal.MinValue;

                                if (Decimal.TryParse(line.Quantity, out d))
                                {
                                    lines.Add(new OrderLineToImport(
                                        fileLineNumber: line.FileLineNumber,
                                        productCode: line.ProductCode,
                                        quantity: d,
                                        type: line.Type,
                                        lineReference: line.LineReference,
                                        memo: line.Memo
                                    ));
                                }
                                else
                                {
                                    throw new Exception($"Error in TradeOrderImportFileID: {head.TradeOrderImportFileID} - processing {head.TradeAccountNumber}'s order, ref. {head.DeliveryReference}: attempting to convert \"{line.Quantity}\" to decimal.");
                                }
                            }

                            head.OrderLines.AddRange(lines);

                            OrdersToImport.Add(head);

                        });
                    }

                    if (FileName.Contains("_INTERNAL_"))
                    {
                        var orderHeads = FileContents.GroupBy(p =>
                        {
                            return new
                            {
                                tradeAccountNumber = p.TradeAccountNumber,
                                orderReference = p.OrderReference,
                                deliveryName = p.DeliveryName,
                                deliveryAddressLine1 = p.DeliveryAddressLine1,
                                deliveryAddressLine2 = p.DeliveryAddressLine2,
                                deliveryTown = p.DeliveryTown,
                                deliveryCounty = p.DeliveryCounty,
                                deliveryPostcode = p.DeliveryPostcode,
                                deliveryCountryCode = p.DeliveryCountryCode,
                                deliveryPhoneNumber = p.DeliveryPhoneNumber,
                                email = p.Email,
                                shippingAlertEmailAddress = p.ShippingAlertEmailAddress,
                                contactPhone = p.ContactPhone,
                                haulierCode = p.HaulierCode,
                                carriageCode = p.CarriageCode,
                                haulierAccountNumber = p.HaulierAccountNumber,
                                taxesDutiesToInvoiceAccount = p.TaxesDutiesToInvoiceAccount,
                                deliveryTerms = p.DeliveryTerms,
                                deliveryType = p.DeliveryType,
                                endCustomer = p.EndCustomer,
                                heldForSales = p.HeldForSales
                            };
                        }).ToList();

                        orderHeads.ForEach(c =>
                        {
                            var head = new OrderHeadToImport(
                                tradeOrderImportFileId: TradeOrderImportFileID,
                                orderType: "Standard",
                                tradeAccountNumber: c.Key.tradeAccountNumber,
                                orderReference: c.Key.orderReference,
                                deliveryName: c.Key.deliveryName,
                                deliveryAddressLine1: c.Key.deliveryAddressLine1,
                                deliveryAddressLine2: c.Key.deliveryAddressLine2,
                                deliveryTown: c.Key.deliveryTown,
                                deliveryCounty: c.Key.deliveryCounty,
                                deliveryPostcode: c.Key.deliveryPostcode,
                                deliveryCountryCode: c.Key.deliveryCountryCode,
                                deliveryPhoneNumber: c.Key.deliveryPhoneNumber,
                                email: c.Key.email,
                                shippingAlertEmailAddress: c.Key.shippingAlertEmailAddress,
                                contactPhone: c.Key.contactPhone,
                                haulierCode: c.Key.haulierCode,
                                carriageCode: c.Key.carriageCode,
                                haulierAccountNumber: c.Key.haulierAccountNumber,
                                taxesDutiesToInvoiceAccount: c.Key.taxesDutiesToInvoiceAccount,
                                deliveryTerms: c.Key.deliveryTerms,
                                deliveryType: c.Key.deliveryType,
                                endCustomer: c.Key.endCustomer,
                                heldForSales: c.Key.heldForSales == "Y" ? true : false
                            );

                            var lines = new List<OrderLineToImport>();

                            foreach (var line in c)
                            {
                                var d = Decimal.MinValue;

                                if (Decimal.TryParse(line.Quantity, out d))
                                {
                                    lines.Add(new OrderLineToImport(
                                        fileLineNumber: line.FileLineNumber,
                                        productCode: line.ProductCode,
                                        quantity: d,
                                        type: line.Type,
                                        lineReference: line.LineReference,
                                        memo: line.Memo,
                                        promoCode: line.PromoCode,
                                        discountPercentage: String.IsNullOrEmpty(line.DiscountPercentage) ? (decimal?)null : Decimal.Parse(line.DiscountPercentage.Replace("%", ""))
                                    ));
                                }
                                else
                                {
                                    throw new Exception($"Error in TradeOrderImportFileID: {head.TradeOrderImportFileID} - processing {head.TradeAccountNumber}'s order, ref. {head.DeliveryReference}: attempting to convert \"{line.Quantity}\" to decimal.");
                                }
                            }

                            head.OrderLines.AddRange(lines);

                            OrdersToImport.Add(head);

                        });
                    }
                }
            }
        }

        public string Errors()
        {
            var lineValidations = "";

            LineValidationFailures.ForEach(c => 
            {
                lineValidations += $"{c}{Environment.NewLine}";
            });

            return $@"
<table style=""table-layout: fixed; width: auto"">
    <thead>
    <tr>
    <th style=""width: 80px"" width=""80"">
        Line
    </th>
    <th style=""width: 240px; border-right: 0;"" width=""240"">
        Field
    </th>
    <th style=""width: 580px; border-left: 0;"" width=""580"">
        Error
    </th>
    </tr>
    </thead>
    <tbody>
        {String.Join(Environment.NewLine, LineValidationFailures.GroupBy(p => p.LineNumber).Select(p => $@"
        <tr>
            <td style=""text-align: right; vertical-align: top; width: 80px"" width=""80"">
                {p.Key}.
            </td>
            <td style=""vertical-align: top;"" colspan=""2"">
                <table style=""border: 0; width: 100%; table-layout: fixed"">
                    <tbody>
                        {String.Join(@"<tr><td colspan=""2"" style=""border: 0""></td></tr>", p.Select(q => $@"<tr><td style=""border: 0; padding: 0 5px 0 0; vertical-align: top; width: 240px"" width=""240"">{q.Field}</td><td style=""border: 0; padding: 0 0 0 5px; vertical-align: top; width: 580px; color: #e30b0b"" width=""580"">{q.ErrorMessage}</td></tr>").ToArray())}
                    </tbody>
                </table>
            </td>
        </tr>").ToArray())}
    </tbody>
</table>";
        }

        public override string ToString()
        {
            var lineValidations = "";

            LineValidationFailures.ForEach(c => 
            {
                lineValidations += $"{c}{Environment.NewLine}";
            });

            return  $"{Environment.NewLine}\t\tFile:\t\t{System.IO.Path.GetFileName(FileName)}" +
                    $"{Environment.NewLine}\t\tValid:\t\t{Valid}" +
                    $"{(Valid ? "" : $"{Environment.NewLine}\t\tError(s):\t\t{lineValidations}")}" +
                    $"{(String.IsNullOrEmpty(Exception) ? "" : $"{Environment.NewLine}\t\tException:\t\t{Exception}")}";
        }
    }

    public class LineValidationResult
    {
        public int? LineNumber { get; set; }
        public string Field { get; set; }
        public string ErrorMessage { get; set; }

        public LineValidationResult(int? ln,
            string f,
            string em)
        {
            LineNumber = ln;
            Field = f;
            ErrorMessage = em;
        }

        public override string ToString()
        {
            return  $"{Environment.NewLine}\t\t\t\tLine:\t{LineNumber?.ToString() ?? "[Unknown, please check all lines]"}" +
                    $"{Environment.NewLine}\t\t\t\tField:\t{Field}" +
                    $"{Environment.NewLine}\t\t\t\tError:\t{ErrorMessage}";
        }
    }
}