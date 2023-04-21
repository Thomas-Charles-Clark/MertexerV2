using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.General;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace MertexerV2.EDAReplacement
{
    /// <summary>
    /// Searches TOEHEAD for IDs, then fetches them as quickly as possible using Session directly.
    /// </summary>

    public static class EDAReplacement
    {
        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "ListTradeAccounts_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable ListTradeAccounts(string UniVerseAccount, bool? AccountOpen, DateTime? CreatedAfterDT, DateTime? CreatedBeforeDT)
        {
            var results = new List<SLCustStub>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var selectStatement = $"";

                if (CreatedAfterDT != null) 
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT >= {Framework.UniVerseDateString(CreatedAfterDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT >= {Framework.UniVerseDateString(CreatedAfterDT.Value)}";
                }

                if (CreatedBeforeDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT <= {Framework.UniVerseDateString(CreatedBeforeDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT <= {Framework.UniVerseDateString(CreatedBeforeDT.Value)}";
                }

                if (AccountOpen != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND OPEN = {(AccountOpen == true ? "O" : "C")}";
                    else
                        selectStatement = $"OPEN = {(AccountOpen == true ? "O" : "C")}";
                }

                string[] fields = new string[] {
                    "CUSREF",
                    "OPEN"
                };

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var keys = new List<string>();

                    keys = ce.Arbitrary($"LIST SLCUST CUSREF FMT\"20L\" WITH {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP")
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    
                    var batches = ListExtension.Split(keys, 1000);

                    var toehead = ce.Session.CreateUniFile("SLCUST");

                    var locker = new object();

                    Parallel.ForEach(batches, (batch) =>
                    {
#if DEBUG
                        try
                        {
#endif
                        var theseResults = new List<SLCustStub>();

                        var records = toehead.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new SLCustStub(
                                tradeAccountCode: record.Record.Extract(1).StringValue,
                                accountOpenStatus: record.Record.Extract(2).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
#if DEBUG
                        }
                        catch (IBMU2.UODOTNET.UniFileException)
                        {
                            System.IO.File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\toehead-ghosts.csv", batch);
                        }
#endif
                    });
                }
            }

            return new ArrayList(results);
        }


        public static void ListTradeAccounts_FillRow(
            object obj,
            out SqlString TradeAccountCode,
            out SqlBoolean AccountOpen)
        {
            var result = (SLCustStub)obj;

            TradeAccountCode = String.IsNullOrEmpty(result.TradeAccountCode) ? SqlString.Null : result.TradeAccountCode.TrimReduceTruncate(50);
            AccountOpen = result.AccountOpen;
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchOrderHeads_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchOrderHeads(string UniVerseAccount, string OrderHeadIDs, DateTime? FromDT, DateTime? ToDT, string TradeAccountCode)
        {
            var results = new List<TOEHead>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var selectStatement = $"";

                if (TradeAccountCode != null)
                {
                    selectStatement = $"CUSREF = {TradeAccountCode}";
                }

                if (FromDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                }

                if (ToDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                }

                string[] fields = new string[] {
                    "CUSREF",
                    "CUSNAME",
                    "CUSORDNO.DES",
                    "DATEONFILE.INT",
                    "ORDTIME",
                    "ORDERTAKER",
                    "ORDER.PRIORITY",
                    "SO.ALLOC.PRIORITY",
                    "HOLDORDER",
                    "HOLD.DEL.DES",
                    "DELNAME",
                    "DELADDR1",
                    "DELADDR2",
                    "DELADDR3",
                    "DELADDR4",
                    "DELPCODE",
                    "TO.COUNTRY",
                    "HAULIERCODE",
                    "CARRCODE",
                    "TOTAL.PAINT.WEIGHT",
                    "PAINT.MESSAGE",
                    "HEADER.REF24",
                    "HEADER.REF25",
                    "HEADER.REF26",
                    "CHARGE.ON.DES",
                    "CARRIAGE",
                    "CARRIAGE.CUR",
                    "HAUL.ACC.NO.DES",
                    "BILL.DUTY.ACC.DES",
                    "CURRENCY",
                    "XRATE",
                    "HEADER.REF32",
                    "HEADER.REF22",
                    "DELPHONE",
                    "CUSNAME.FF",
                    "CUSADDR1.FF",
                    "CUSADDR2.FF",
                    "CUSADDR3.FF",
                    "CUSADDR4.FF",
                    "CUSPCODE.FF",
                    "ORDERTYPE"
                };

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var keys = new List<string>();

                    if (String.IsNullOrEmpty(OrderHeadIDs))
                    {
                        keys = ce.Arbitrary($"LIST TOEHEAD ORDERNO FMT\"20L\" WITH {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP")
                           .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                           .ToList();
                    }
                    else
                    {
                        keys = JsonConvert.DeserializeObject<List<OrderHead>>(OrderHeadIDs).Select(p => $"{p.OrderID}").ToList();
                    }

                    var batches = ListExtension.Split(keys, 1000);

                    var toehead = ce.Session.CreateUniFile("TOEHEAD");

                    var locker = new object();

                    Parallel.ForEach(batches, (batch) =>
                    //batches.ForEach((batch) => 
                    {
#if DEBUG
                        try
                        {
#endif
                        var theseResults = new List<TOEHead>();

                        var records = toehead.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new TOEHead(
                                b,
                                record.Record.Extract(1).StringValue,
                                record.Record.Extract(2).StringValue,
                                record.Record.Extract(3).StringValue,
                                record.Record.Extract(4).StringValue,
                                record.Record.Extract(5).StringValue,
                                record.Record.Extract(6).StringValue,
                                record.Record.Extract(7).StringValue,
                                record.Record.Extract(8).StringValue,
                                record.Record.Extract(9).StringValue,
                                record.Record.Extract(10).StringValue,
                                record.Record.Extract(11).StringValue,
                                record.Record.Extract(12).StringValue,
                                record.Record.Extract(13).StringValue,
                                record.Record.Extract(14).StringValue,
                                record.Record.Extract(15).StringValue,
                                record.Record.Extract(16).StringValue,
                                record.Record.Extract(17).StringValue,
                                record.Record.Extract(18).StringValue,
                                record.Record.Extract(19).StringValue,
                                record.Record.Extract(20).StringValue,
                                record.Record.Extract(21).StringValue,
                                record.Record.Extract(22).StringValue,
                                record.Record.Extract(23).StringValue,
                                record.Record.Extract(24).StringValue,
                                record.Record.Extract(25).StringValue,
                                record.Record.Extract(26).StringValue,
                                record.Record.Extract(27).StringValue,
                                record.Record.Extract(28).StringValue,
                                record.Record.Extract(29).StringValue,
                                record.Record.Extract(30).StringValue,
                                record.Record.Extract(31).StringValue,
                                record.Record.Extract(32).StringValue,
                                record.Record.Extract(33).StringValue,
                                record.Record.Extract(34).StringValue,
                                record.Record.Extract(35).StringValue,
                                record.Record.Extract(36).StringValue,
                                record.Record.Extract(37).StringValue,
                                record.Record.Extract(38).StringValue,
                                record.Record.Extract(39).StringValue,
                                record.Record.Extract(40).StringValue,
                                record.Record.Extract(41).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
#if DEBUG
                        }
                        catch (IBMU2.UODOTNET.UniFileException)
                        {
                            System.IO.File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\toehead-ghosts.csv", batch);
                        }
#endif
                    });
                }
            }

            return new ArrayList(results);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchOrderHeads_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchTuffnellsOrderHeads(string UniVerseAccount, DateTime? FromDT)
        {
            var results = new List<TOEHead>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var selectStatement = $"HAULIERCODE = TUF";

                if (FromDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                }

                string[] fields = new string[] {
                    "CUSREF",
                    "CUSNAME",
                    "CUSORDNO.DES",
                    "DATEONFILE.INT",
                    "ORDTIME",
                    "ORDERTAKER",
                    "ORDER.PRIORITY",
                    "SO.ALLOC.PRIORITY",
                    "HOLDORDER",
                    "HOLD.DEL.DES",
                    "DELNAME",
                    "DELADDR1",
                    "DELADDR2",
                    "DELADDR3",
                    "DELADDR4",
                    "DELPCODE",
                    "TO.COUNTRY",
                    "HAULIERCODE",
                    "CARRCODE",
                    "TOTAL.PAINT.WEIGHT",
                    "PAINT.MESSAGE",
                    "HEADER.REF24",
                    "HEADER.REF25",
                    "HEADER.REF26",
                    "CHARGE.ON.DES",
                    "CARRIAGE",
                    "CARRIAGE.CUR",
                    "HAUL.ACC.NO.DES",
                    "BILL.DUTY.ACC.DES",
                    "CURRENCY",
                    "XRATE",
                    "HEADER.REF32",
                    "HEADER.REF22",
                    "DELPHONE",
                    "CUSNAME.FF",
                    "CUSADDR1.FF",
                    "CUSADDR2.FF",
                    "CUSADDR3.FF",
                    "CUSADDR4.FF",
                    "CUSPCODE.FF",
                    "ORDERTYPE"
                };

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var keys = new List<string>();

                    var query = $"LIST TOEHEAD ORDERNO FMT\"20L\" WITH {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP";

                    keys = ce.Arbitrary(query)
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var batches = ListExtension.Split(keys, 1000);

                    var toehead = ce.Session.CreateUniFile("TOEHEAD");

                    var locker = new object();

                    Parallel.ForEach(batches, (batch) =>
                    //batches.ForEach((batch) =>
                    {
                        try
                        {
                            var theseResults = new List<TOEHead>();

                            var records = toehead.ReadRecords(batch.ToArray(), fields);

                            batch.ForEach(b =>
                            {
                                var record = records[b];

                                theseResults.Add(new TOEHead(
                                    b,
                                    record.Record.Extract(1).StringValue,
                                    record.Record.Extract(2).StringValue,
                                    record.Record.Extract(3).StringValue,
                                    record.Record.Extract(4).StringValue,
                                    record.Record.Extract(5).StringValue,
                                    record.Record.Extract(6).StringValue,
                                    record.Record.Extract(7).StringValue,
                                    record.Record.Extract(8).StringValue,
                                    record.Record.Extract(9).StringValue,
                                    record.Record.Extract(10).StringValue,
                                    record.Record.Extract(11).StringValue,
                                    record.Record.Extract(12).StringValue,
                                    record.Record.Extract(13).StringValue,
                                    record.Record.Extract(14).StringValue,
                                    record.Record.Extract(15).StringValue,
                                    record.Record.Extract(16).StringValue,
                                    record.Record.Extract(17).StringValue,
                                    record.Record.Extract(18).StringValue,
                                    record.Record.Extract(19).StringValue,
                                    record.Record.Extract(20).StringValue,
                                    record.Record.Extract(21).StringValue,
                                    record.Record.Extract(22).StringValue,
                                    record.Record.Extract(23).StringValue,
                                    record.Record.Extract(24).StringValue,
                                    record.Record.Extract(25).StringValue,
                                    record.Record.Extract(26).StringValue,
                                    record.Record.Extract(27).StringValue,
                                    record.Record.Extract(28).StringValue,
                                    record.Record.Extract(29).StringValue,
                                    record.Record.Extract(30).StringValue,
                                    record.Record.Extract(31).StringValue,
                                    record.Record.Extract(32).StringValue,
                                    record.Record.Extract(33).StringValue,
                                    record.Record.Extract(34).StringValue,
                                    record.Record.Extract(35).StringValue,
                                    record.Record.Extract(36).StringValue,
                                    record.Record.Extract(37).StringValue,
                                    record.Record.Extract(38).StringValue,
                                    record.Record.Extract(39).StringValue,
                                    record.Record.Extract(40).StringValue,
                                    record.Record.Extract(41).StringValue
                                ));
                            });

                            lock (locker)
                            {
                                results.AddRange(theseResults);
                            }
                        }
                        catch (IBMU2.UODOTNET.UniFileException)
                        {
                            System.IO.File.AppendAllLines($"{AppDomain.CurrentDomain.BaseDirectory}\\toehead-ghosts.csv", batch);
                        }
                    });
                }
            }

            return new ArrayList(results);
        }


        public static void FetchOrderHeads_FillRow(
            object obj,
            out SqlString TradeAccountCode,
            out SqlString TradeAccountName,
            out SqlString OrderID,
            out SqlString CustomerOrderReference,
            out SqlDateTime OrderDT,
            out SqlString OrderTaker,
            out SqlByte OrderPriorty,
            out SqlByte AllocationPriority,
            out SqlBoolean HoldOrder,
            out SqlBoolean HoldDelivery,
            out SqlString DeliveryName,
            out SqlString DeliveryAddressLine1,
            out SqlString DeliveryAddressLine2,
            out SqlString DeliveryTown,
            out SqlString DeliveryCounty,
            out SqlString DeliveryPostcode,
            out SqlString DeliveryCountryCode,
            out SqlString HaulierCode,
            out SqlString CarriageCode,
            out SqlDecimal TotalPaintWeight,
            out SqlString PaintMessage,
            out SqlString DeliveryAlertLandLinePhoneNumber,
            out SqlString DeliveryAlertMobilePhoneNumber,
            out SqlString DeliveryAlertEmail,
            out SqlString ChargeOn,
            out SqlDecimal DeliveryChargeSterling,
            out SqlDecimal DeliveryChargeCurrency,
            out SqlString HaulierAccountNumberForShippingCharge,
            out SqlString HaulierAccountNumberForDuty,
            out SqlString MertexCurrencyCode,
            out SqlDecimal SterlingExchangeRate,
            out SqlString DeliveryGroup,
            out SqlString EORINumber,
            out SqlString DeliveryPhone,
            out SqlString InvoiceAddressName,
            out SqlString InvoiceAddressLine1,
            out SqlString InvoiceAddressLine2,
            out SqlString InvoiceAddressTown,
            out SqlString InvoiceAddressCounty,
            out SqlString InvoiceAddressPostcode,
            out SqlString OrderType)
        {
            var result = (TOEHead)obj;

            TradeAccountCode = String.IsNullOrEmpty(result.TradeAccountCode) ? SqlString.Null : result.TradeAccountCode.TrimReduceTruncate(50);
            TradeAccountName = String.IsNullOrEmpty(result.TradeAccountName) ? SqlString.Null : result.TradeAccountName.TrimReduceTruncate(50);
            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(50);
            CustomerOrderReference = String.IsNullOrEmpty(result.CustomerOrderReference) ? SqlString.Null : result.CustomerOrderReference.TrimReduceTruncate(500);
            OrderDT = result.OrderDT ?? SqlDateTime.Null;
            OrderTaker = String.IsNullOrEmpty(result.OrderTaker) ? SqlString.Null : result.OrderTaker;
            OrderPriorty = result.OrderPriority ?? SqlByte.Null;
            AllocationPriority = result.AllocationPriority ?? SqlByte.Null;
            HoldOrder = result.HoldOrder;
            HoldDelivery = result.HoldDelivery;
            DeliveryName = String.IsNullOrEmpty(result.DeliveryName) ? SqlString.Null : result.DeliveryName.TrimReduceTruncate(50);
            DeliveryAddressLine1 = String.IsNullOrEmpty(result.DeliveryAddressLine1) ? SqlString.Null : result.DeliveryAddressLine1.TrimReduceTruncate(60);
            DeliveryAddressLine2 = String.IsNullOrEmpty(result.DeliveryAddressLine2) ? SqlString.Null : result.DeliveryAddressLine2.TrimReduceTruncate(60);
            DeliveryTown = String.IsNullOrEmpty(result.DeliveryTown) ? SqlString.Null : result.DeliveryTown.TrimReduceTruncate(50);
            DeliveryCounty = String.IsNullOrEmpty(result.DeliveryCounty) ? SqlString.Null : result.DeliveryCounty.TrimReduceTruncate(40);
            DeliveryPostcode = String.IsNullOrEmpty(result.DeliveryPostcode) ? SqlString.Null : result.DeliveryPostcode.TrimReduceTruncate(20);
            DeliveryCountryCode = String.IsNullOrEmpty(result.DeliveryCountry) ? SqlString.Null : result.DeliveryCountry.TrimReduceTruncate(3);
            HaulierCode = String.IsNullOrEmpty(result.HaulierCode) ? SqlString.Null : result.HaulierCode.TrimReduceTruncate(10);
            CarriageCode = String.IsNullOrEmpty(result.CarriageCode) ? SqlString.Null : result.CarriageCode.TrimReduceTruncate(10);
            TotalPaintWeight = result.TotalPaintWeight ?? SqlDecimal.Null;
            PaintMessage = String.IsNullOrEmpty(result.PaintMessage) ? SqlString.Null : result.PaintMessage;
            DeliveryAlertLandLinePhoneNumber = String.IsNullOrEmpty(result.DeliveryAlertLandlinePhoneNumber) ? SqlString.Null : result.DeliveryAlertLandlinePhoneNumber.TrimReduceTruncate(20);
            DeliveryAlertMobilePhoneNumber = String.IsNullOrEmpty(result.DeliveryAlertMobilePhoneNumber) ? SqlString.Null : result.DeliveryAlertMobilePhoneNumber.TrimReduceTruncate(20);
            DeliveryAlertEmail = String.IsNullOrEmpty(result.DeliveryAlertEmail) ? SqlString.Null : result.DeliveryAlertEmail.TrimReduceTruncate(254);
            ChargeOn = String.IsNullOrEmpty(result.ChargeOn) ? SqlString.Null : result.ChargeOn.TrimReduceTruncate(1);
            DeliveryChargeSterling = result.DeliveryChargeSterling ?? SqlDecimal.Null;
            DeliveryChargeCurrency = result.DeliveryChargeCurrency ?? SqlDecimal.Null;
            HaulierAccountNumberForShippingCharge = String.IsNullOrEmpty(result.HaulierAccountNumberForShippingCharge) ? SqlString.Null : result.HaulierAccountNumberForShippingCharge.TrimReduceTruncate(20);
            HaulierAccountNumberForDuty = String.IsNullOrEmpty(result.HaulierAccountNumberForDuty) ? SqlString.Null : result.HaulierAccountNumberForDuty.TrimReduceTruncate(20);
            MertexCurrencyCode = String.IsNullOrEmpty(result.MertexCurrencyCode) ? SqlString.Null : result.MertexCurrencyCode.TrimReduceTruncate(3);
            SterlingExchangeRate = result.SterlingExchangeRate;
            DeliveryGroup = String.IsNullOrEmpty(result.DeliveryGroup) ? SqlString.Null : result.DeliveryGroup.TrimReduceTruncate(100);
            EORINumber = String.IsNullOrEmpty(result.EORINumber) ? SqlString.Null : result.EORINumber.TrimReduceTruncate(20);
            DeliveryPhone = String.IsNullOrEmpty(result.DeliveryPhone) ? SqlString.Null : result.DeliveryPhone.TrimReduceTruncate(20); 
            InvoiceAddressName = String.IsNullOrEmpty(result.InvoiceAddressName) ? SqlString.Null : result.InvoiceAddressName.TrimReduceTruncate(50);
            InvoiceAddressLine1 = String.IsNullOrEmpty(result.InvoiceAddressLine1) ? SqlString.Null : result.InvoiceAddressLine1.TrimReduceTruncate(60);
            InvoiceAddressLine2 = String.IsNullOrEmpty(result.InvoiceAddressLine2) ? SqlString.Null : result.InvoiceAddressLine2.TrimReduceTruncate(60);
            InvoiceAddressTown = String.IsNullOrEmpty(result.InvoiceAddressTown) ? SqlString.Null : result.InvoiceAddressTown.TrimReduceTruncate(50);
            InvoiceAddressCounty = String.IsNullOrEmpty(result.InvoiceAddressCounty) ? SqlString.Null : result.InvoiceAddressCounty.TrimReduceTruncate(40);
            InvoiceAddressPostcode = String.IsNullOrEmpty(result.InvoiceAddressPostcode) ? SqlString.Null : result.InvoiceAddressPostcode.TrimReduceTruncate(20);
            OrderType = String.IsNullOrEmpty(result.OrderType) ? SqlString.Null : result.OrderType.TrimReduceTruncate(5);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchOrderLines_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchOrderLines(string UniVerseAccount, string OrderLineID, int? OrderHeadID, byte? LineStatusID, DateTime? FromDT, DateTime? ToDT)
        {
            var results = new List<TOELine>();

            if (String.IsNullOrEmpty(OrderLineID) && OrderHeadID == null && FromDT == null && ToDT == null)
            {
                throw new Exception("LineKey, OrderHeadID and SinceDT are all NULL: refusing to query TOELINE.");
            }

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var selectStatement = $"";

                if (!String.IsNullOrEmpty(OrderLineID))
                {
                    selectStatement = $"@ID = {OrderLineID}";
                }

                if (OrderHeadID != null)
                {
                    if (selectStatement != "")
                    {
                        selectStatement += $" AND ORDERNO = {OrderHeadID}";
                    }
                    else
                    {
                        selectStatement = $"ORDERNO = {OrderHeadID}";
                    }
                }

                if (LineStatusID != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND STATUS = \"{LineStatusID}\"";
                    else
                        selectStatement = $"STATUS = \"{LineStatusID}\"";
                }

                if (FromDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                }

                if (ToDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                    else
                        selectStatement = $"DATEONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                }

                var fields = new string[] {
                    "ORDERNO",               // 0
                    "LINE",                  // 0
                    "CUS.ORD.REF",           // 33
                    "PRODCODE",              // 1
                    "PRODDESC",              // 0
                    "QTYORD.ORD",            // 94
                    "QTYCAN.ORD",            // 96
                    "BO.IND",                // 4
                    "STATUS",                // 79
                    "LINE.STAT.DES",         // 665
                    "SS.LINE.STAT.DATE.INT", // 667
                    "LINE.STAT.TIME.DES",    // 668
                    "DNNUMBER",              // 63
                    "DN.LINE.KEY",           // 65
                    "PERC.DISC",             // 18
                    "ORDPRICE",              // 6
                    "ORDVAL",                // 7
                    "DISCOUNT",              // 28
                    "VATVAL",                // 9
                    "SS.ORDPRICE.CUR",       // 81
                    "ORDVAL.CUR",            // 83
                    "DISCOUNT.CUR",          // 17
                    "VATVAL.CUR",            // 84
                    "VATCODE",               // 8
                    "VATRATE.DES",           // 16
                    "DESPDATE.INT",          // 0
                    "L.LOCCODE",             // 19
                    "PORDNO",                // 219
                    "PARTPRODCODE",          // 269
                    "RESERVE.FLAG"           // 311
                };

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var keys = ce.Arbitrary($"LIST TOELINE LINEKEY FMT\"20L\" WITH {selectStatement} AND LINEKEY LIKE ...*... ID.SUPP COL.HDR.SUPP COUNT.SUP")
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var batches = ListExtension.Split(keys, 1000);

                    var toeline = ce.Session.CreateUniFile("TOELINE");

                    var locker = new object();

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var theseResults = new List<TOELine>();

                        var records = toeline.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new TOELine(
                                key: b,
                                orderId: record.Record.Extract(1).StringValue,
                                lineNumber: record.Record.Extract(2).StringValue,
                                customerLineReference: record.Record.Extract(3).StringValue,
                                productCode: record.Record.Extract(4).StringValue,
                                productDescription: record.Record.Extract(5).StringValue,
                                quantityOrdered: record.Record.Extract(6).StringValue,
                                quantityCancelled: record.Record.Extract(7).StringValue,
                                backOrderIndicator: record.Record.Extract(8).StringValue,
                                lineStatusId: record.Record.Extract(9).StringValue,
                                detailedLineStatus: record.Record.Extract(10).StringValue,
                                lineStatusDate: record.Record.Extract(11).StringValue,
                                lineStatusTime: record.Record.Extract(12).StringValue,
                                deliveryNoteNumber: record.Record.Extract(13).StringValue,
                                deliveryNoteLineKey: record.Record.Extract(14).StringValue,
                                discountPercentage: record.Record.Extract(15).StringValue,
                                unitValueSterling: record.Record.Extract(16).StringValue,
                                lineValueSterling: record.Record.Extract(17).StringValue,
                                lineDiscountValueSterling: record.Record.Extract(18).StringValue,
                                lineVATValueSterling: record.Record.Extract(19).StringValue,
                                unitValueCurrency: record.Record.Extract(20).StringValue,
                                lineValueCurrency: record.Record.Extract(21).StringValue,
                                lineDiscountValueCurrency: record.Record.Extract(22).StringValue,
                                lineVATValueCurrency: record.Record.Extract(23).StringValue,
                                vatCode: record.Record.Extract(24).StringValue,
                                vatRate: record.Record.Extract(25).StringValue,
                                estimatedDespatchDate: record.Record.Extract(26).StringValue,
                                stockLocation: record.Record.Extract(27).StringValue,
                                purchaseOrderLineId: record.Record.Extract(28).StringValue,
                                partnerProductCode: record.Record.Extract(29).StringValue,
                                reserveIndicator: record.Record.Extract(30).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    });
                }
            }

            return new ArrayList(results);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchOrderLines_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchOrderLinesForOrderHeads(string UniVerseAccount, string OrderHeadIDs)
        {
            var results = new List<TOELine>();

            var orderHeadIds = new List<OrderHead>();

            if (!String.IsNullOrEmpty(OrderHeadIDs))
            {
                orderHeadIds = JsonConvert.DeserializeObject<List<OrderHead>>(OrderHeadIDs);
            }
            else
            {
                throw new Exception("OrderHeadIDs was NULL or empty: refusing to query TOELINE.");
            }

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var fields = new string[] {
                    "ORDERNO",               // 0
                    "LINE",                  // 0
                    "CUS.ORD.REF",           // 33
                    "PRODCODE",              // 1
                    "PRODDESC",              // 0
                    "QTYORD.ORD",            // 94
                    "QTYCAN.ORD",            // 96
                    "BO.IND",                // 4
                    "STATUS",                // 79
                    "LINE.STAT.DES",         // 665
                    "SS.LINE.STAT.DATE.INT", // 667
                    "LINE.STAT.TIME.DES",    // 668
                    "DNNUMBER",              // 63
                    "DN.LINE.KEY",           // 65
                    "PERC.DISC",             // 18
                    "ORDPRICE",              // 6
                    "ORDVAL",                // 7
                    "DISCOUNT",              // 28
                    "VATVAL",                // 9
                    "SS.ORDPRICE.CUR",          // 81
                    "ORDVAL.CUR",            // 83
                    "DISCOUNT.CUR",          // 17
                    "VATVAL.CUR",            // 84
                    "VATCODE",               // 8
                    "VATRATE.DES",           // 16
                    "DESPDATE.INT",          // 0
                    "L.LOCCODE",             // 19
                    "PORDNO",                // 219
                    "PARTPRODCODE",          // 269
                    "RESERVE.FLAG"           // 311
                };

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var orderHeadChunks = ListExtension.Split(orderHeadIds, 50);

                    var locker = new object();

                    var keys = new List<string>();

#if DEBUG
                    var stopWatch = new System.Diagnostics.Stopwatch();
                    Console.WriteLine($"Fetching OrderLine Keys for {orderHeadIds.Count} OrderHeadIDs...");
                    stopWatch.Start();
#endif

                    orderHeadChunks.ForEach((orderHeadChunk) =>
                    {
                        var thisString = $"LIST TOELINE LINEKEY FMT\"20L\" WITH ORDERNO = {String.Join(" ", orderHeadChunk.Select(p => p.OrderID))} AND LINEKEY LIKE ...*... ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var thisResult = ce.Arbitrary(thisString);
                        var theseKeys = thisResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        lock (locker)
                        {
                            keys.AddRange(theseKeys);
                        }
                    });

#if DEBUG
                    stopWatch.Stop();
                    Console.WriteLine($"Fetched {keys.Count} OrderLine Keys in {stopWatch.Elapsed}");
#endif

                    var batches = ListExtension.Split(keys, 1000);

                    var toeline = ce.Session.CreateUniFile("TOELINE");

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var theseResults = new List<TOELine>();

                        var records = toeline.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new TOELine(
                                key: b,
                                orderId: record.Record.Extract(1).StringValue,
                                lineNumber: record.Record.Extract(2).StringValue,
                                customerLineReference: record.Record.Extract(3).StringValue,
                                productCode: record.Record.Extract(4).StringValue,
                                productDescription: record.Record.Extract(5).StringValue,
                                quantityOrdered: record.Record.Extract(6).StringValue,
                                quantityCancelled: record.Record.Extract(7).StringValue,
                                backOrderIndicator: record.Record.Extract(8).StringValue,
                                lineStatusId: record.Record.Extract(9).StringValue,
                                detailedLineStatus: record.Record.Extract(10).StringValue,
                                lineStatusDate: record.Record.Extract(11).StringValue,
                                lineStatusTime: record.Record.Extract(12).StringValue,
                                deliveryNoteNumber: record.Record.Extract(13).StringValue,
                                deliveryNoteLineKey: record.Record.Extract(14).StringValue,
                                discountPercentage: record.Record.Extract(15).StringValue,
                                unitValueSterling: record.Record.Extract(16).StringValue,
                                lineValueSterling: record.Record.Extract(17).StringValue,
                                lineDiscountValueSterling: record.Record.Extract(18).StringValue,
                                lineVATValueSterling: record.Record.Extract(19).StringValue,
                                unitValueCurrency: record.Record.Extract(20).StringValue,
                                lineValueCurrency: record.Record.Extract(21).StringValue,
                                lineDiscountValueCurrency: record.Record.Extract(22).StringValue,
                                lineVATValueCurrency: record.Record.Extract(23).StringValue,
                                vatCode: record.Record.Extract(24).StringValue,
                                vatRate: record.Record.Extract(25).StringValue,
                                estimatedDespatchDate: record.Record.Extract(26).StringValue,
                                stockLocation: record.Record.Extract(27).StringValue,
                                purchaseOrderLineId: record.Record.Extract(28).StringValue,
                                partnerProductCode: record.Record.Extract(29).StringValue,
                                reserveIndicator: record.Record.Extract(30).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    });
                }
            }

            return new ArrayList(results);
        }

        public static void FetchOrderLines_FillRow(
            object obj,
            out SqlString LineKey,
            out SqlString OrderID,
            out SqlInt16 LineNumber,
            out SqlString CustomerLineReference,
            out SqlString ProductCode,
            out SqlString ProductDescription,
            out SqlDecimal QuantityOrdered,
            out SqlDecimal QuantityCancelled,
            out SqlBoolean BackOrderIndicator,
            out SqlByte LineStatusID,
            out SqlString DetailedLineStatus,
            out SqlDateTime LineStatusDT,
            out SqlString DespatchID,
            out SqlString DespatchLineID,
            out SqlDecimal DiscountPercentage,
            out SqlDecimal UnitValueSterling,
            out SqlDecimal LineValueSterling,
            out SqlDecimal LineDiscountValueSterling,
            out SqlDecimal LineVATValueSterling,
            out SqlDecimal UnitValueCurrency,
            out SqlDecimal LineValueCurrency,
            out SqlDecimal LineDiscountValueCurrency,
            out SqlDecimal LineVATValueCurrency,
            out SqlString VATCode,
            out SqlDecimal VATRate,
            out SqlDateTime EstimatedDespatchDT,
            out SqlString StockLocationCode,
            out SqlString PurchaseOrderLineID,
            out SqlString PartnerProductCode,
            out SqlString ReserveIndicator
            )
        {
            var result = (TOELine)obj;

            LineKey = String.IsNullOrEmpty(result.Key) ? SqlString.Null : result.Key.TrimReduceTruncate(20);

            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(50);

            LineNumber = result.LineNumber;

            CustomerLineReference = String.IsNullOrEmpty(result.CustomerLineReference) ? SqlString.Null : result.CustomerLineReference.TrimReduceTruncate(500);

            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(50);

            ProductDescription = String.IsNullOrEmpty(result.ProductDescription) ? SqlString.Null : result.ProductDescription.TrimReduceTruncate(255);

            QuantityOrdered = result.QuantityOrdered;

            QuantityCancelled = result.QuantityCancelled ?? SqlDecimal.Null;

            BackOrderIndicator = result.BackOrderIndicator;

            LineStatusID = result.LineStatusID ?? SqlByte.Null;

            DetailedLineStatus = String.IsNullOrEmpty(result.DetailedLineStatus) ? SqlString.Null : result.DetailedLineStatus.TrimReduceTruncate(255);

            LineStatusDT = result.LineStatusDT ?? SqlDateTime.Null;

            DespatchID = String.IsNullOrEmpty(result.DespatchNoteNumber) ? SqlString.Null : result.DespatchNoteNumber.TrimReduceTruncate(25);

            DespatchLineID = String.IsNullOrEmpty(result.DespatchNoteLineKey) ? SqlString.Null : result.DespatchNoteLineKey.TrimReduceTruncate(30);

            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;

            UnitValueSterling = result.UnitValueSterling ?? SqlDecimal.Null;

            LineValueSterling = result.LineValueSterling ?? SqlDecimal.Null;

            LineDiscountValueSterling = result.LineDiscountValueSterling ?? SqlDecimal.Null;

            LineVATValueSterling = result.LineVATValueSterling ?? SqlDecimal.Null;

            UnitValueCurrency = result.UnitValueCurrency ?? SqlDecimal.Null;

            LineValueCurrency = result.LineValueCurrency ?? SqlDecimal.Null;

            LineDiscountValueCurrency = result.LineDiscountValueCurrency ?? SqlDecimal.Null;

            LineVATValueCurrency = result.LineVATValueCurrency ?? SqlDecimal.Null;

            VATCode = String.IsNullOrEmpty(result.VATCode) ? SqlString.Null : result.VATCode.TrimReduceTruncate(2);

            VATRate = result.VATRate ?? SqlDecimal.Null;

            EstimatedDespatchDT = result.EstimatedDespatchDT ?? SqlDateTime.Null;

            StockLocationCode = String.IsNullOrEmpty(result.StockLocationCode) ? SqlString.Null : result.StockLocationCode.TrimReduceTruncate(10);

            PurchaseOrderLineID = String.IsNullOrEmpty(result.PurchaseOrderLineID) ? SqlString.Null : result.PurchaseOrderLineID.TrimReduceTruncate(30);

            PartnerProductCode = String.IsNullOrEmpty(result.PartnerProductCode) ? SqlString.Null : result.PartnerProductCode.TrimReduceTruncate(50);

            ReserveIndicator = String.IsNullOrEmpty(result.ReserveIndicator) ? SqlString.Null : result.ReserveIndicator.TrimReduceTruncate(1);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchProductLocations_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchProductLocations(string UniVerseAccount, string ProductCodes, string LocationCodes)
        {
            var results = new List<STProdLoc>();

            if (String.IsNullOrEmpty(ProductCodes) && String.IsNullOrEmpty(LocationCodes))
            {
                throw new Exception("ProductCodes and LocationCodes are both NULL: refusing to query STPRODLOC.");
            }

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var prodLocKeys = new List<string>();

                string[] fields = new string[] {
                    "PRODUCT",
                    "LOCATION",
                    "FREESTOCK"
                };

                var locker = new object();

                // Unfortunately can't do this: you'll get a Record Not Found exception for any key which we've just crafted which
                // does not exist on STPRODLOC
                if (!String.IsNullOrEmpty(ProductCodes) && !String.IsNullOrEmpty(LocationCodes))
                {
                    var productCodes = JsonConvert.DeserializeObject<List<ProductCode>>(ProductCodes);
                    var locationCodes = JsonConvert.DeserializeObject<List<LocationCode>>(LocationCodes);

                    prodLocKeys = productCodes.Select(p => p.Code).SelectMany(p => locationCodes.Select(q => $"{p}*{q.Code}")).ToList();

                    // caan... weeee... not.... take this list of keys and 
                    // query for EXISTING keys based on this list

                    var prodLocKeyChunks = ListExtension.Split(prodLocKeys, 100);

                    var foundKeys = new List<string>();

                    prodLocKeyChunks.ForEach((prodLocKeyChunk) =>
                    {
                        var thisString = $"LIST STPRODLOC PRODLOCCODE FMT\"20L\" WITH PRODLOCCODE = {String.Join(" ", prodLocKeyChunk)} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var thisResult = ce.Arbitrary(thisString);
                        var theseKeys = thisResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        foundKeys.AddRange(theseKeys);
                    });

                    Console.WriteLine($"Of {prodLocKeys.Count} concocted keys, {foundKeys.Count} were found on STPRODLOC");

                    prodLocKeys = foundKeys;
                }
                else
                {
                    if (!String.IsNullOrEmpty(ProductCodes))
                    {
                        var productCodes = JsonConvert.DeserializeObject<List<ProductCode>>(ProductCodes).Select(p => p.Code).ToList();

                        var productCodeChunks = ListExtension.Split(productCodes, 100);

                        productCodeChunks.ForEach((productCodeChunk) =>
                        {
                            var thisString = $"LIST STPRODLOC PRODLOCCODE FMT\"80L\" WITH PRODUCT = {String.Join(" ", productCodeChunk)} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                            var thisResult = ce.Arbitrary(thisString);
                            var theseKeys = thisResult
                                .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList();

                            prodLocKeys.AddRange(theseKeys);
                        });
                    }
                    else if (!String.IsNullOrEmpty(LocationCodes))
                    {
                        var locationCodes = JsonConvert.DeserializeObject<List<LocationCode>>(LocationCodes).Select(p => p.Code).ToList();

                        var locationCodeChunks = ListExtension.Split(locationCodes, 100);

                        locationCodeChunks.ForEach((locationCodeChunk) =>
                        {
                            var thisString = $"LIST STPRODLOC PRODLOCCODE FMT\"80L\" WITH LOCATION = {String.Join(" ", locationCodeChunk)} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                            var thisResult = ce.Arbitrary(thisString);
                            var theseKeys = thisResult
                                .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList();

                            prodLocKeys.AddRange(theseKeys);
                        });
                    }
                }

                var batches = ListExtension.Split(prodLocKeys, 1000);

                var stprodloc = ce.Session.CreateUniFile("STPRODLOC");

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<STProdLoc>();

                    var records = stprodloc.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        theseResults.Add(new STProdLoc(
                            record.Record.Extract(1).StringValue,
                            record.Record.Extract(2).StringValue,
                            record.Record.Extract(3).StringValue
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void FetchProductLocations_FillRow(
            object obj,
            out SqlString ProductCode,
            out SqlString LocationCode,
            out SqlDecimal FreeStock
            )
        {
            var result = (STProdLoc)obj;

            ProductCode = result.ProductCode;
            LocationCode = result.LocationCode;
            FreeStock = result.FreeStock;
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchPurchaseOrders_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchPurchaseOrders(string UniVerseAccount, string PurchaseOrderHeads, DateTime? FromDT, DateTime? ToDT)
        {
            var results = new List<POHead>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var selectStatement = $"";


                if (FromDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                    else
                        selectStatement = $"WITH DATEONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                }

                if (ToDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND DATEONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                    else
                        selectStatement = $"WITH DATEONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                }

                string[] fields = new string[] {
                    "PONO",
                    "SORDNO",
                    "ORDERSTATUS",
                    "LOCATION",
                    "SS.F2",
                    "SUPPLIER",
                    "ORDERPROCESS",
                    "AUTHOR.CODE",
                    "CALLOFF.DES",
                    "DEL.NAME",
                    "DEL.ADDR1",
                    "DEL.ADDR2",
                    "DEL.ADDR3",
                    "DEL.ADDR4",
                    "DEL.PC",
                    "DEL.PHONE"
                };

                var poheadKeys = new List<string>();

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    if (!String.IsNullOrEmpty(PurchaseOrderHeads))
                    {
                        var poheads = JsonConvert.DeserializeObject<List<PurchaseOrderHead>>(PurchaseOrderHeads).Select(p => p.PurchaseOrderHeadID).ToList();

                        var poheadChunks = ListExtension.Split(poheads, 100);

                        poheadChunks.ForEach((poheadChunk) =>
                        {
                            var thisString = $"LIST POHEAD PONO FMT\"80L\" {String.Join(" ", poheadChunk)} {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                            var thisResult = ce.Arbitrary(thisString);
                            var theseKeys = thisResult
                                .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList();

                            poheadKeys.AddRange(theseKeys);
                        });
                    }
                    else
                    {
                        var thisString = $"LIST POHEAD PONO FMT\"80L\" {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var thisResult = ce.Arbitrary(thisString);
                        var theseKeys = thisResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        poheadKeys.AddRange(theseKeys);
                    }

                    var batches = ListExtension.Split(poheadKeys, 1000);

                    var pohead = ce.Session.CreateUniFile("POHEAD");

                    var locker = new object();

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var theseResults = new List<POHead>();

                        var records = pohead.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new POHead(
                                poNumber: record.Record.Extract(1).StringValue,
                                orderId: record.Record.Extract(2).StringValue,
                                orderStatus: record.Record.Extract(3).StringValue,
                                stockLocation: record.Record.Extract(4).StringValue,
                                dateOnFile: record.Record.Extract(5).StringValue,
                                supplier: record.Record.Extract(6).StringValue,
                                processCode: record.Record.Extract(7).StringValue,
                                authoriserCode: record.Record.Extract(8).StringValue,
                                callOffIndicator: record.Record.Extract(9).StringValue,
                                deliveryName: record.Record.Extract(10).StringValue,
                                deliveryAddressLine1: record.Record.Extract(11).StringValue,
                                deliveryAddressLine2: record.Record.Extract(12).StringValue,
                                deliveryTown: record.Record.Extract(13).StringValue,
                                deliveryCounty: record.Record.Extract(14).StringValue,
                                deliveryPostcode: record.Record.Extract(15).StringValue,
                                deliveryPhone: record.Record.Extract(16).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    });
                }
            }

            return new ArrayList(results);
        }


        public static void FetchPurchaseOrders_FillRow(
            object obj,
            out SqlString PurchaseOrderHeadID,
            out SqlString OrderID,
            out SqlByte OrderStatus,
            out SqlString StockLocation,
            out SqlDateTime DateOnFile,
            out SqlString Supplier,
            out SqlString ProcessCode,
            out SqlString AuthoriserCode,
            out SqlBoolean CallOffIndicator,
            out SqlString DeliveryName,
            out SqlString DeliveryAddressLine1,
            out SqlString DeliveryAddressLine2,
            out SqlString DeliveryTown,
            out SqlString DeliveryCounty,
            out SqlString DeliveryPostcode,
            out SqlString DeliveryPhone
            )
        {
            var result = (POHead)obj;

            PurchaseOrderHeadID = String.IsNullOrEmpty(result.PurchaseOrderHeadID) ? SqlString.Null : result.PurchaseOrderHeadID.TrimReduceTruncate(25);

            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);

            OrderStatus = result.PurchaseOrderStatusID ?? SqlByte.Null;

            StockLocation = String.IsNullOrEmpty(result.StockLocationCode) ? SqlString.Null : result.StockLocationCode.TrimReduceTruncate(10);

            DateOnFile = result.DateOnFile ?? SqlDateTime.Null;

            Supplier = String.IsNullOrEmpty(result.SupplierCode) ? SqlString.Null : result.SupplierCode.TrimReduceTruncate(50);

            ProcessCode = String.IsNullOrEmpty(result.ProcessCode) ? SqlString.Null : result.ProcessCode.TrimReduceTruncate(10);

            AuthoriserCode = String.IsNullOrEmpty(result.AuthoriserCode) ? SqlString.Null : result.AuthoriserCode.TrimReduceTruncate(10);

            CallOffIndicator = result.CallOffIndicator;

            DeliveryName = String.IsNullOrEmpty(result.DeliveryName) ? SqlString.Null : result.DeliveryName.TrimReduceTruncate(50);

            DeliveryAddressLine1 = String.IsNullOrEmpty(result.DeliveryAddressLine1) ? SqlString.Null : result.DeliveryAddressLine1.TrimReduceTruncate(60);

            DeliveryAddressLine2 = String.IsNullOrEmpty(result.DeliveryAddressLine2) ? SqlString.Null : result.DeliveryAddressLine2.TrimReduceTruncate(60);

            DeliveryTown = String.IsNullOrEmpty(result.DeliveryTown) ? SqlString.Null : result.DeliveryTown.TrimReduceTruncate(50);

            DeliveryCounty = String.IsNullOrEmpty(result.DeliveryCounty) ? SqlString.Null : result.DeliveryCounty.TrimReduceTruncate(40);

            DeliveryPostcode = String.IsNullOrEmpty(result.DeliveryPostcode) ? SqlString.Null : result.DeliveryPostcode.TrimReduceTruncate(20);

            DeliveryPhone = String.IsNullOrEmpty(result.DeliveryPhone) ? SqlString.Null : result.DeliveryPhone.TrimReduceTruncate(20);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchPurchaseOrderDetails_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchPurchaseOrderDetails(string UniVerseAccount, string PurchaseOrderHeads)
        {
            var results = new List<PODetail>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var selectStatement = $"";

                if (!String.IsNullOrEmpty(PurchaseOrderHeads))
                {
                    var pos = JsonConvert.DeserializeObject<List<PurchaseOrderHead>>(PurchaseOrderHeads);

                    selectStatement = $"@ID = {String.Join(" ", pos)}";
                }

                string[] fields = new string[] {
                    "DETAILKEY",
                    "SORDNO",
                    "PRODCODE",
                    "CONV.QTY",
                    "RCVDQTY",
                    "SS.DATEREQUIRED.INT",
                    "SS.DATEPROMISED.INT",
                    "SS.EXPARRIVAL.INT",
                    "DATE.CONFIRMED",
                    "DELETED",
                    "LNCOMPLETE",
                    "LINENO",
                    "ORDERPRICE",
                    "ORDPRICE.CUR",
                    "CONV.PURCONV",
                    "FINISHERS.REF",
                    "SUBMIT",
                    "SS.POHEADKEY",
                    "PROM.CH.REASON"
                };

                var podetailKeys = new List<string>();

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    if (!String.IsNullOrEmpty(PurchaseOrderHeads))
                    {
                        var poheads = JsonConvert.DeserializeObject<List<PurchaseOrderHead>>(PurchaseOrderHeads).Select(p => p.PurchaseOrderHeadID).ToList();

                        var poheadChunks = ListExtension.Split(poheads, 100);

                        poheadChunks.ForEach((poheadChunk) =>
                        {
                            var thisString = $"LIST PODETAIL DETAILKEY FMT\"80L\" WITH SS.POHEADKEY = {String.Join(" ", poheadChunk)} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                            var thisResult = ce.Arbitrary(thisString);
                            var theseKeys = thisResult
                                .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList();

                            podetailKeys.AddRange(theseKeys);
                        });
                    }
                    else
                    {
                        var thisString = $"LIST PODETAIL DETAILKEY FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var thisResult = ce.Arbitrary(thisString);
                        var theseKeys = thisResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        podetailKeys.AddRange(theseKeys);
                    }

                    var batches = ListExtension.Split(podetailKeys, 1000);

                    var podetail = ce.Session.CreateUniFile("PODETAIL");

                    var locker = new object();

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var theseResults = new List<PODetail>();

                        var records = podetail.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new PODetail(
                                purchaseOrderLineId: record.Record.Extract(1).StringValue,
                                orderId: record.Record.Extract(2).StringValue,
                                productCode: record.Record.Extract(3).StringValue,
                                orderedQuantity: record.Record.Extract(4).StringValue,
                                receivedQuantity: record.Record.Extract(5).StringValue,
                                dateRequired: record.Record.Extract(6).StringValue,
                                datePromised: record.Record.Extract(7).StringValue,
                                dateExpected: record.Record.Extract(8).StringValue,
                                isDateConfirmed: record.Record.Extract(9).StringValue,
                                deleted: record.Record.Extract(10).StringValue,
                                lineComplete: record.Record.Extract(11).StringValue,
                                purchaseOrderLineNumber: record.Record.Extract(12).StringValue,
                                orderPrice: record.Record.Extract(13).StringValue,
                                orderPriceCurr: record.Record.Extract(14).StringValue,
                                stockUnitCode: record.Record.Extract(15).StringValue,
                                supplierProductCode: record.Record.Extract(16).StringValue,
                                sampleRequired: record.Record.Extract(17).StringValue,
                                purchaseOrderHead: record.Record.Extract(18).StringValue,
                                statusDescription: record.Record.Extract(19).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    });
                }
            }

            return new ArrayList(results);
        }


        public static void FetchPurchaseOrderDetails_FillRow(
            object obj,
            out SqlString PurchaseOrderLineID,
            out SqlString PurchaseOrderNumber,
            out SqlString OrderID,
            out SqlString ProductCode,
            out SqlDecimal OrderedQuantity,
            out SqlDecimal ReceivedQuantity,
            out SqlDateTime PromisedDT,
            out SqlDateTime ExpectedDT,
            out SqlBoolean IsDateConfirmed,
            out SqlBoolean Deleted,
            out SqlBoolean IsLineComplete,
            out SqlInt16 PurchaseOrderLineNumber,
            out SqlDecimal LineUnitPriceSTG,
            out SqlDecimal LineUnitPriceCURR,
            out SqlString StockUnitCode,
            out SqlString SupplierProductCode,
            out SqlBoolean SampleRequired,
            out SqlString PurchaseOrderStatusDescription
            )
        {
            var result = (PODetail)obj;

            PurchaseOrderLineID = String.IsNullOrEmpty(result.PurchaseOrderLineID) ? SqlString.Null : result.PurchaseOrderLineID.TrimReduceTruncate(30);
            PurchaseOrderNumber = String.IsNullOrEmpty(result.PurchaseOrderHeadID) ? SqlString.Null : result.PurchaseOrderHeadID.TrimReduceTruncate(25);
            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);
            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(50);
            OrderedQuantity = result.OrderQuantity ?? SqlDecimal.Null;
            ReceivedQuantity = result.ReceivedQuantity ?? SqlDecimal.Null;
            PromisedDT = result.DatePromised ?? SqlDateTime.Null;
            ExpectedDT = result.DateExpected ?? SqlDateTime.Null;
            IsDateConfirmed = result.IsDateConfirmed;
            Deleted = result.Deleted;
            IsLineComplete = result.LineComplete;
            PurchaseOrderLineNumber = result.PurchaseOrderLineNumber ?? SqlInt16.Null;
            LineUnitPriceSTG = result.LineUnitPriceSTG ?? SqlDecimal.Null;
            LineUnitPriceCURR = result.LineUnitPriceCURR ?? SqlDecimal.Null;
            StockUnitCode = String.IsNullOrEmpty(result.StockUnitCode) ? SqlString.Null : result.StockUnitCode.TrimReduceTruncate(10);
            SupplierProductCode = String.IsNullOrEmpty(result.SupplierProductCode) ? SqlString.Null : result.SupplierProductCode.TrimReduceTruncate(50);
            SampleRequired = result.SampleRequired;
            PurchaseOrderStatusDescription = String.IsNullOrEmpty(result.PurchaseOrderStatusDescription) ? SqlString.Null : result.PurchaseOrderStatusDescription.TrimReduceTruncate(255);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchProductRecords_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchProductRecords(string UniVerseAccount, string ProductCodes)
        {
            var results = new List<STProdMaster>();

            string[] fields = new string[] {
                "PRODCODE",
                "PRODDESC",
                "CUR.STAT",
                "GROUPCODE",
                "ITEMTYPE",
                "JLSTOCKREF",
                "SS.FIELD7",
                "MID",
                "INTRASTAT",
                "SS.FIELD6",
                "USCOMM",
                "ORCOUNTRY",
                "LATEST.COST.PRICE",
                "SUPPREF",
                "QUALITY",
                "PROD.COLL.DES",
                "STOCKCODE",
                "LINKEDRUGSAMP",
                "UPDGRADE",
                "SS.FIELD10",
                "SS.MAXFOLD",
                "SS.WIDTH",
                "SS.LENGTH",
                "SS.HEIGHT",
                "SS.DIAM",
                "SS.SIZEDESC",
                "ADDITIONAL",
                "MARTINDALE.DES",
                "HPATT.REP",
                "VPATT.REP",
                "WEBDESC.DES",
                "CARE.RPH.DES",
                "SALES.DIV.DES",
                "SS.COLNAME",
                "DESIGN.DES",
                "EAN.PRODCODE",
                "SS.LAUNCH",
                "SS.CFWD.DT.DES",
                "SS.ORDERGROUP.TITLE",
                "PRLISTD.DES",
                "ADD.PRODS.DES",
                "PAINTCOLNO",
                "PAINTFINISH",
                "CLR.DISC.DES",
                "SS.DESIGN",
                "COMMODITY.DG",
                "SS.PACK.DESC"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var stprodmaster = ce.Session.CreateUniFile("STPRODMASTER");

                var locker = new object();

                var codes = new List<string>();

                if (!String.IsNullOrEmpty(ProductCodes))
                {
                    codes = JsonConvert.DeserializeObject<List<ProductCode>>(ProductCodes).Select(p => p.Code).ToList();

                    var batches = ListExtension.Split(codes, 1000);

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var theseResults = new List<STProdMaster>();

                        var records = stprodmaster.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new STProdMaster(
                                productCode: record.Record.Extract(1).StringValue,
                                productName: record.Record.Extract(2).StringValue,
                                status: record.Record.Extract(3).StringValue,
                                groupCode: record.Record.Extract(4).StringValue,
                                itemType: record.Record.Extract(5).StringValue,
                                jlStockReference: record.Record.Extract(6).StringValue,
                                newJlStockReference: record.Record.Extract(7).StringValue,
                                manufacturersId: record.Record.Extract(8).StringValue?.Split(Framework.ValueMarkDelimiter)[0],
                                euExportCommodityCode: record.Record.Extract(9).StringValue,
                                euExportCommodityCodeDescription: record.Record.Extract(10).StringValue,
                                usCommodityCode: record.Record.Extract(11).StringValue,
                                originCountry: record.Record.Extract(12).StringValue,
                                latestCostPrice: record.Record.Extract(13).StringValue,
                                supplierReference: record.Record.Extract(14).StringValue?.Split(Framework.ValueMarkDelimiter)[0],
                                qualityCode: record.Record.Extract(15).StringValue,
                                collectionCode: record.Record.Extract(16).StringValue,
                                stockCode: record.Record.Extract(17).StringValue,
                                rugSampleProductCode: record.Record.Extract(18).StringValue,
                                upholsteryGrade: record.Record.Extract(19).StringValue,
                                deliveryBand: record.Record.Extract(20).StringValue,
                                maxFoldLength: record.Record.Extract(21).StringValue,
                                width: record.Record.Extract(22).StringValue,
                                length: record.Record.Extract(23).StringValue,
                                height: record.Record.Extract(24).StringValue,
                                diameter: record.Record.Extract(25).StringValue,
                                size: record.Record.Extract(26).StringValue,
                                flamCode: record.Record.Extract(27).StringValue,
                                martindale: record.Record.Extract(28).StringValue,
                                horizontalPatternRepeat: record.Record.Extract(29).StringValue,
                                verticalPatternRepeat: record.Record.Extract(30).StringValue,
                                additionalInformation: record.Record.Extract(31).StringValue,
                                pileHeightMm: record.Record.Extract(32).StringValue,
                                brandCode: record.Record.Extract(33).StringValue,
                                colourName: record.Record.Extract(34).StringValue,
                                designName: record.Record.Extract(35).StringValue,
                                ean: record.Record.Extract(36).StringValue,
                                launchDate: record.Record.Extract(37).StringValue,
                                carryForwardDate: record.Record.Extract(38).StringValue,
                                orderGroupTitle: record.Record.Extract(39).StringValue,
                                priceListDescription: record.Record.Extract(40).StringValue,
                                additionalProducts: record.Record.Extract(41).StringValue,
                                paintColourNumber: record.Record.Extract(42).StringValue,
                                paintFinish: record.Record.Extract(43).StringValue,
                                clrDiscDesc: record.Record.Extract(44).StringValue,
                                ssDesign: record.Record.Extract(45).StringValue,
                                importCommodityCode: record.Record.Extract(46).StringValue,
                                packagingDescription: record.Record.Extract(47).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    });
                }
                else
                {
                    var thisString = $"LIST STPRODMASTER PRODCODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var thisResult = ce.Arbitrary(thisString);
                    var theseKeys = thisResult
                        .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var batches = ListExtension.Split(theseKeys, 1000);

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var theseResults = new List<STProdMaster>();

                        var records = stprodmaster.ReadRecords(batch.ToArray(), fields);

                        batch.ForEach(b =>
                        {
                            var record = records[b];

                            theseResults.Add(new STProdMaster(
                                productCode: record.Record.Extract(1).StringValue,
                                productName: record.Record.Extract(2).StringValue,
                                status: record.Record.Extract(3).StringValue,
                                groupCode: record.Record.Extract(4).StringValue,
                                itemType: record.Record.Extract(5).StringValue,
                                jlStockReference: record.Record.Extract(6).StringValue,
                                newJlStockReference: record.Record.Extract(7).StringValue,
                                manufacturersId: record.Record.Extract(8).StringValue,
                                euExportCommodityCode: record.Record.Extract(9).StringValue,
                                euExportCommodityCodeDescription: record.Record.Extract(10).StringValue,
                                usCommodityCode: record.Record.Extract(11).StringValue,
                                originCountry: record.Record.Extract(12).StringValue,
                                latestCostPrice: record.Record.Extract(13).StringValue,
                                supplierReference: record.Record.Extract(14).StringValue?.Split(Framework.ValueMarkDelimiter)[0],
                                qualityCode: record.Record.Extract(15).StringValue,
                                collectionCode: record.Record.Extract(16).StringValue,
                                stockCode: record.Record.Extract(17).StringValue,
                                rugSampleProductCode: record.Record.Extract(18).StringValue,
                                upholsteryGrade: record.Record.Extract(19).StringValue,
                                deliveryBand: record.Record.Extract(20).StringValue,
                                maxFoldLength: record.Record.Extract(21).StringValue,
                                width: record.Record.Extract(22).StringValue,
                                length: record.Record.Extract(23).StringValue,
                                height: record.Record.Extract(24).StringValue,
                                diameter: record.Record.Extract(25).StringValue,
                                size: record.Record.Extract(26).StringValue,
                                flamCode: record.Record.Extract(27).StringValue,
                                martindale: record.Record.Extract(28).StringValue,
                                horizontalPatternRepeat: record.Record.Extract(29).StringValue,
                                verticalPatternRepeat: record.Record.Extract(30).StringValue,
                                additionalInformation: record.Record.Extract(31).StringValue,
                                pileHeightMm: record.Record.Extract(32).StringValue,
                                brandCode: record.Record.Extract(33).StringValue,
                                colourName: record.Record.Extract(34).StringValue,
                                designName: record.Record.Extract(35).StringValue,
                                ean: record.Record.Extract(36).StringValue,
                                launchDate: record.Record.Extract(37).StringValue,
                                carryForwardDate: record.Record.Extract(38).StringValue,
                                orderGroupTitle: record.Record.Extract(39).StringValue,
                                priceListDescription: record.Record.Extract(40).StringValue,
                                additionalProducts: record.Record.Extract(41).StringValue,
                                paintColourNumber: record.Record.Extract(42).StringValue,
                                paintFinish: record.Record.Extract(43).StringValue,
                                clrDiscDesc: record.Record.Extract(44).StringValue,
                                ssDesign: record.Record.Extract(45).StringValue,
                                importCommodityCode: record.Record.Extract(46).StringValue,
                                packagingDescription: record.Record.Extract(47).StringValue
                            ));
                        });

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    });
                }
            }

            return new ArrayList(results);
        }


        public static void FetchProductRecords_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString Status,
            out SqlString GroupCode,
            out SqlString ItemType,
            out SqlString JLStockReference,
            out SqlString NewJLStockReference,
            out SqlString ManufacturersID,
            out SqlString CommodityCodeDescription,
            out SqlInt64 EUIntrastatCode,
            out SqlInt64 USCommodityCode,
            out SqlString OriginCountry,
            out SqlDecimal LatestCostPrice,
            out SqlString DefaultSupplierCode,
            out SqlString CollectionCode,
            out SqlString StockCode,
            out SqlString RugSampleProductCode,
            out SqlString UpholsteryGrade,
            out SqlString DeliveryBand,
            out SqlByte MaxFoldLength,
            out SqlDecimal Width,
            out SqlDecimal Length,
            out SqlDecimal Height,
            out SqlDecimal Diameter,
            out SqlString Size,
            out SqlString QualityCode,
            out SqlString FlamCode,
            out SqlInt32 Martindale,
            out SqlDecimal HorizontalPatternRepeat,
            out SqlDecimal VerticalPatternRepeat,
            out SqlString AdditionalInformation,
            out SqlInt16 PileHeightMM,
            out SqlString BrandCode,
            out SqlString ColourName,
            out SqlString DesignName,
            out SqlString EAN,
            out SqlDateTime LaunchDate,
            out SqlDateTime CarryForwardDate,
            out SqlString OrderGroupTitle,
            out SqlString PriceListDescription,
            out SqlString AdditionalProductsJSON,
            out SqlString ImportCommodityCode,
            out SqlString PackagingDescription)
        {
            var result = (STProdMaster)obj;

            Code = result.Code?.TrimReduceTruncate(50);
            Name = result.Name?.TrimReduceTruncate(100);
            Status = result.Status?.TrimReduceTruncate(1);
            GroupCode = result.GroupCode?.TrimReduceTruncate(10);
            ItemType = result.ItemType?.TrimReduceTruncate(10);
            JLStockReference = result.JLStockReference?.TrimReduceTruncate(50);
            NewJLStockReference = result.NewJLStockReference?.TrimReduceTruncate(50);
            ManufacturersID = result.ManufacturersID?.TrimReduceTruncate(50);
            CommodityCodeDescription = result.EUExportCommodityCodeDescription?.TrimReduceTruncate(100);
            EUIntrastatCode = result.EUExportCommodityCode ?? SqlInt64.Null;
            USCommodityCode = result.USCommodityCode ?? SqlInt64.Null;
            OriginCountry = result.OriginCountry?.TrimReduceTruncate(3);
            LatestCostPrice = result.LatestCostPrice ?? SqlDecimal.Null;
            DefaultSupplierCode = result.SupplierReference?.TrimReduceTruncate(50);
            CollectionCode = result.CollectionCode?.TrimReduceTruncate(50);
            StockCode = result.StockCode?.TrimReduceTruncate(50);
            RugSampleProductCode = result.RugSampleProductCode?.TrimReduceTruncate(50);
            UpholsteryGrade = result.UpholsteryGrade?.TrimReduceTruncate(1);
            DeliveryBand = result.DeliveryBand?.TrimReduceTruncate(10);
            MaxFoldLength = result.MaxFoldLength ?? SqlByte.Null;
            Width = result.Width ?? SqlDecimal.Null;
            Length = result.Length ?? SqlDecimal.Null;
            Height = result.Height ?? SqlDecimal.Null;
            Diameter = result.Diameter ?? SqlDecimal.Null;
            Size = result.Size?.TrimReduceTruncate(50);
            QualityCode = result.QualityCode?.TrimReduceTruncate(50);
            FlamCode = result.FlamCode?.TrimReduceTruncate(1);
            Martindale = result.Martindale ?? SqlInt32.Null;
            HorizontalPatternRepeat = result.HorizontalPatternRepeat ?? SqlDecimal.Null;
            VerticalPatternRepeat = result.VerticalPatternRepeat ?? SqlDecimal.Null;
            AdditionalInformation = result.AdditionalInformation?.TrimReduceTruncate(300);
            PileHeightMM = result.PileHeightMM ?? SqlInt16.Null;
            BrandCode = result.BrandCode?.TrimReduceTruncate(5);
            ColourName = result.ColourName?.TrimReduceTruncate(50);
            DesignName = result.DesignName?.TrimReduceTruncate(50);
            EAN = result.EAN?.TrimReduceTruncate(20);
            LaunchDate = result.LaunchDate ?? SqlDateTime.Null;
            CarryForwardDate = result.CarryForwardDate ?? SqlDateTime.Null;
            OrderGroupTitle = result.OrderGroupTitle?.TrimReduceTruncate(50);
            PriceListDescription = String.IsNullOrEmpty(result.PriceListDescription) ? SqlString.Null : result.PriceListDescription?.TrimReduceTruncate(100);
            AdditionalProductsJSON = result.AdditionalProducts.Any() ? JsonConvert.SerializeObject(result.AdditionalProducts) : SqlString.Null;

            ImportCommodityCode = result.ImportCommodityCode?.TrimReduceTruncate(20);
            PackagingDescription = result.PackagingDescription?.TrimReduceTruncate(200);
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchProductCodes_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchProductCodes(string UniVerseAccount)
        {
            var results = new List<ProdCode>();

            string[] fields = new string[] {
                "PRODCODE"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var locker = new object();

                var codes = new List<string>();

                var thisString = $"LIST STPRODMASTER PRODCODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var thisResult = ce.Arbitrary(thisString);
                codes = thisResult
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                results = codes.Select(c => new ProdCode(c)).ToList();
            }

            return new ArrayList(results);
        }



        public static void FetchProductCodes_FillRow(
            object obj,
            out SqlString Code)
        {
            var result = (ProdCode)obj;

            Code = String.IsNullOrEmpty(result.Code) ? SqlString.Null : result.Code.TrimReduceTruncate(50);
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchCarriageRecords_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchCarriageRecords(string UniVerseAccount)
        {
            var results = new List<OECarriage>();

            string[] fields = new string[] {
                "CODE",
                "DESC50",
                "ADDITIONAL.LABEL.DES",
                "PRTY.ORDER.DES",
                "DISP.GB.IND.DES"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var oecarriage = ce.Session.CreateUniFile("OECARRIAGE");

                var keys = ce.Arbitrary("LIST OECARRIAGE CODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                var codes = new List<string>();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<OECarriage>();

                    var records = oecarriage.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        theseResults.Add(new OECarriage(
                            code: record.Record.Extract(1).StringValue,
                            description: record.Record.Extract(2).StringValue,
                            despatchPathway: record.Record.Extract(3).StringValue,
                            priorityOrder: record.Record.Extract(4).StringValue,
                            gbIndicator: record.Record.Extract(5).StringValue
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }


        public static void FetchCarriageRecords_FillRow(
            object obj,
            out SqlString Key,
            out SqlString CarriageCode,
            out SqlString HaulierCode,
            out SqlString Description,
            out SqlString DespatchPathway,
            out SqlBoolean PriorityOrder,
            out SqlBoolean GBIndicator
            )
        {
            var result = (OECarriage)obj;

            Key = result.Code?.TrimReduceTruncate(50) ?? SqlString.Null;
            CarriageCode = result.CarriageCode?.TrimReduceTruncate(20) ?? SqlString.Null;
            HaulierCode = result.HaulierCode?.TrimReduceTruncate(20) ?? SqlString.Null;
            Description = result.Description?.TrimReduceTruncate(100) ?? SqlString.Null;
            DespatchPathway = result.DespatchPathway?.TrimReduceTruncate(100) ?? SqlString.Null;
            PriorityOrder = result.PriorityOrder;
            GBIndicator = result.GBIndicator;
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchStockLocations_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchStockLocations(string UniVerseAccount)
        {
            var results = new List<CodeDescription>();

            string[] fields = new string[] {
                "CODE",
                "TITLE40"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var stlocation = ce.Session.CreateUniFile("STLOCATION");

                var keys = ce.Arbitrary("LIST STLOCATION CODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                var codes = new List<string>();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<CodeDescription>();

                    var records = stlocation.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        theseResults.Add(new CodeDescription(
                            record.Record.Extract(1).StringValue,
                            record.Record.Extract(2).StringValue
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }


        public static void FetchStockLocations_FillRow(
            object obj,
            out SqlString LocationCode,
            out SqlString Name
            )
        {
            var result = (CodeDescription)obj;

            LocationCode = result.Code;
            Name = result.Description;
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchSalesMarkets_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchSalesMarkets(string UniVerseAccount)
        {
            var results = Framework.GetMarkets(UniVerseAccount).ToList();

            return new ArrayList(results);
        }

        public static void FetchSalesMarkets_FillRow(
            object obj,
            out SqlString MarketCode,
            out SqlString Name
            )
        {
            var result = (CodeDescription)obj;

            MarketCode = result.Code;
            Name = result.Description;
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchItemTypes_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchItemTypes(string UniVerseAccount)
        {
            var results = Framework.GetItemTypes(UniVerseAccount).ToList();

            return new ArrayList(results);
        }

        public static void FetchItemTypes_FillRow(
            object obj,
            out SqlString ItemTypeCode,
            out SqlString Name
            )
        {
            var result = (CodeDescription)obj;

            ItemTypeCode = result.Code;
            Name = result.Description;
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchQualities_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchQualities(string UniVerseAccount)
        {
            var results = new List<CLQualDes>();

            string[] fields = new string[] {
                "CODE",
                "WIDTH",
                "COMPCODE",
                "DESIGN",
                "APPGMS",
                "GMSLIN",
                "COMPCODE"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var qualities = ce.Session.CreateUniFile("CLQUALDES");

                var keys = ce.Arbitrary("LIST CLQUALDES CODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                var codes = new List<string>();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<CLQualDes>();

                    var records = qualities.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        theseResults.Add(new CLQualDes(
                            qualityCode: record.Record.Extract(1).StringValue,
                            width: record.Record.Extract(2).StringValue,
                            compositionCode: record.Record.Extract(3).StringValue,
                            name: record.Record.Extract(4).StringValue,
                            gsm: record.Record.Extract(5).StringValue,
                            glm: record.Record.Extract(6).StringValue
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void FetchQualities_FillRow(
            object obj,
            out SqlString QualityCode,
            out SqlString Name,
            out SqlDecimal GSM,
            out SqlDecimal GLM,
            out SqlString CompositionCode,
            out SqlDecimal Width
            )
        {
            var result = (CLQualDes)obj;

            QualityCode = String.IsNullOrEmpty(result.QualityCode) ? SqlString.Null : result.QualityCode.TrimReduceTruncate(20);
            Name = String.IsNullOrEmpty(result.Name) ? SqlString.Null : result.Name.TrimReduceTruncate(100);
            GSM = result.GSM ?? SqlDecimal.Null;
            GLM = result.GLM ?? SqlDecimal.Null;
            CompositionCode = String.IsNullOrEmpty(result.CompositionCode) ? SqlString.Null : result.CompositionCode.TrimReduceTruncate(20);
            Width = result.Width ?? SqlDecimal.Null;
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchCompositions_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchCompositions(string UniVerseAccount)
        {
            var results = new List<CLComp>();

            string[] fields = new string[] {
                "CODE",
                "DESC"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var compositions = ce.Session.CreateUniFile("CLCOMP");

                var keys = ce.Arbitrary("LIST CLCOMP CODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                var codes = new List<string>();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<CLComp>();

                    var records = compositions.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        var descs = record.Record.Extract(2).StringValue?.Split(Framework.ValueMarkDelimiter);

                        if (descs.Length < 5)
                        {
                            var newDesc = descs.ToList();

                            for (var i = 0; i < (5 - descs.Length); i++)
                            {
                                newDesc.Add("");
                            }

                            descs = newDesc.ToArray();
                        }

                        theseResults.Add(new CLComp(
                            code: record.Record.Extract(1).StringValue,
                            description1: descs[0],
                            description2: descs[1],
                            description3: descs[2],
                            description4: descs[3],
                            description5: descs[4]
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void FetchCompositions_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description1,
            out SqlString Description2,
            out SqlString Description3,
            out SqlString Description4,
            out SqlString Description5,
            out SqlString Description
            )
        {
            var result = (CLComp)obj;

            Code = String.IsNullOrEmpty(result.Code) ? SqlString.Null : result.Code.TrimReduceTruncate(50);
            Description1 = String.IsNullOrEmpty(result.Desc1) ? SqlString.Null : result.Desc1.TrimReduceTruncate(50);
            Description2 = String.IsNullOrEmpty(result.Desc2) ? SqlString.Null : result.Desc2.TrimReduceTruncate(50);
            Description3 = String.IsNullOrEmpty(result.Desc3) ? SqlString.Null : result.Desc3.TrimReduceTruncate(50);
            Description4 = String.IsNullOrEmpty(result.Desc4) ? SqlString.Null : result.Desc4.TrimReduceTruncate(50);
            Description5 = String.IsNullOrEmpty(result.Desc5) ? SqlString.Null : result.Desc5.TrimReduceTruncate(50);
            Description = String.IsNullOrEmpty(result.Description) ? SqlString.Null : result.Description.TrimReduceTruncate(250);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchFlamCodes_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchFlamCodes(string UniVerseAccount)
        {
            var results = new List<EDAADDITIONAL>();

            string[] fields = new string[] {
                "CODE",
                "DETAILS"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var additionals = ce.Session.CreateUniFile("STADDITIONAL");

                var keys = ce.Arbitrary("LIST STADDITIONAL CODE FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                var codes = new List<string>();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<EDAADDITIONAL>();

                    var records = additionals.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        theseResults.Add(new EDAADDITIONAL(
                            code: record.Record.Extract(1).StringValue,
                            value: record.Record.Extract(2).StringValue
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void FetchFlamCodes_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Value
            )
        {
            var result = (EDAADDITIONAL)obj;

            Code = String.IsNullOrEmpty(result.Code) ? SqlString.Null : result.Code.TrimReduceTruncate(15);
            Value = String.IsNullOrEmpty(result.Value) ? SqlString.Null : result.Value.TrimReduceTruncate(300);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchAftercares_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchAftercares(string UniVerseAccount)
        {
            var results = new List<STProdAftCare>();

            string[] fields = new string[] {
                "PRODCODE",
                "AFTCARECODES"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var additionals = ce.Session.CreateUniFile("STPRODMASTER");

                var keys = ce.Arbitrary("LIST STPRODMASTER PRODCODE FMT\"80L\" WITH AFTCARECODES # \"\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                var codes = new List<string>();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<STProdAftCare>();

                    var records = additionals.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        var prodcode = record.Record.Extract(1).StringValue;
                        var aftercares = record.Record.Extract(2).StringValue.Split(Framework.ValueMarkDelimiter);

                        for (var i = 0; i < aftercares.Length; i++)
                        {
                            theseResults.Add(new STProdAftCare(
                                productCode: prodcode,
                                aftercareCode: aftercares[i],
                                sortOrder: $"{i}"
                            ));
                        }
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void FetchAftercares_FillRow(
            object obj,
            out SqlString ProductCode,
            out SqlString AftercareCode,
            out SqlByte SortOrder
            )
        {
            var result = (STProdAftCare)obj;

            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(50);
            AftercareCode = String.IsNullOrEmpty(result.AftercareCode) ? SqlString.Null : result.AftercareCode.TrimReduceTruncate(5);
            SortOrder = result.SortOrder ?? SqlByte.Null;
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchDespatchNoteHeads_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchDespatchNoteHeads(string UniVerseAccount, string DespatchHeadIDs)
        {
            var results = new List<OEDNHead>();

            if (String.IsNullOrEmpty(DespatchHeadIDs))
            {
                throw new Exception("@DespatchIDs was NULL or empty. Refusing to query PLHEAD / OEDNHEAD / OEDNHHIST");
            }

            int[] fields = new int[] {
                9,   //"ORDERNO",
                0,   //"DOC.NO",
                25,  //"DESPDATE",
                19,  //"CONFTIME",
                5,   //"DELNAME",
                     //"DELADDR1",
                     //"DELADDR2",
                     //"DELADDR3",
                     //"DELADDR4",
                     //"DELPCODE",
                179, //"TO.COUNTRY",
                352, //"CARR.CODE"
                13,  //"HAULIERCODE",
                191, //"PARCEL.FULL",
                41,  //"PCL.NETWT",
                43,  //"PCL.WID",
                45,  //"PCL.HGT",
                44,  //"PCL.DEP",
                16,  //"CARRIAGE"
                60,  //"CARRIAGE.CUR"
                7,   //"TOELINEP"
                40   //"PARCEL"
            };

            int[] toelineFields = new int[]
            {
                1,  //"PRODCODE"
                70, //"MBATCH.NO"
                91  //"MPIECE.NO"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var oeplhead = ce.Session.CreateUniFile("OEPLHEAD");
                var oednhead = ce.Session.CreateUniFile("OEDNHEAD");
                var oednhhist = ce.Session.CreateUniFile("OEDNHHIST");
                var toeline = ce.Session.CreateUniFile("TOELINE");
                //var oefindparcel = ce.Session.CreateUniFile("OEFINDPARCEL");

                var hauliers = Framework.GetHauliers(UniVerseAccount);

                var oeplheadKeys = new List<string>();
                var oednheadKeys = new List<string>();
                var oednhhistKeys = new List<string>();

                var despatchIds = JsonConvert.DeserializeObject<List<DespatchHeadIDOrderID>>(DespatchHeadIDs);

                var despatchChunks = ListExtension.Split(despatchIds, 100);

                despatchChunks.ForEach((despatchChunk) =>
                {
                    var oeplheadStringResult = $"LIST OEPLHEAD DOC.NO FMT\"20L\" WITH DOC.NO = {String.Join(" ", despatchChunk.Select(p => p.DespatchHeadID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oeplheadResult = ce.Arbitrary(oeplheadStringResult);
                    var oeplheads = oeplheadResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    oeplheadKeys.AddRange(oeplheads);

                    var oednheadStringResult = $"LIST OEDNHEAD USING DICT OEPLHEAD DOC.NO FMT\"20L\" WITH DOC.NO = {String.Join(" ", despatchChunk.Select(p => p.DespatchHeadID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oednheadResult = ce.Arbitrary(oednheadStringResult);
                    var oednheads = oednheadResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    oednheadKeys.AddRange(oednheads);

                    var oednhhistStringResult = $"LIST OEDNHHIST USING DICT OEPLHEAD DOC.NO FMT\"20L\" WITH DOC.NO = {String.Join(" ", despatchChunk.Select(p => p.DespatchHeadID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oednhhistResult = ce.Arbitrary(oednhhistStringResult);
                    var oednhhists = oednhhistResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    oednhhistKeys.AddRange(oednhhists);
                });

                var locker = new object();

                oeplheadKeys = oeplheadKeys.Except(oednheadKeys).Except(oednhhistKeys).ToList();
                oednheadKeys = oednheadKeys.Except(oednhhistKeys).ToList();

                var oeplheadBatches = ListExtension.Split(oeplheadKeys, 1000);
                var oednheadBatches = ListExtension.Split(oednheadKeys, 1000);
                var oednhhistBatches = ListExtension.Split(oednhhistKeys, 1000);

                Parallel.ForEach(oeplheadBatches, (batch) =>
                {
                    var theseResults = new List<OEDNHead>();

                    var records = oeplhead.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        var address = record.Record.Extract(5).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.None);

                        if (address.Length < 6)
                        {
                            var newAddress = address.ToList();

                            for (var i = 0; i < (6 - address.Length); i++)
                            {
                                newAddress.Add("");
                            }

                            address = newAddress.ToArray();
                        }

                        var parcels = new List<Parcel>();

                        var parcelNumbers = record.Record.Extract(9).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        var pclDotNos = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (pclDotNos.Count() < parcelNumbers.Length)
                        {
                            for (var i = 0; i < (parcelNumbers.Length - pclDotNos.Count()); i++)
                            {
                                pclDotNos.Add("");
                            }
                        }

                        if (!parcelNumbers.Any())
                        {
                            parcelNumbers = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        }

                        var weights = record.Record.Extract(10).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var widths = record.Record.Extract(11).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var heights = record.Record.Extract(12).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var depths = record.Record.Extract(13).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        for (var i = 0; i < parcelNumbers.Length; i++)
                        {
                            parcels.Add(new Parcel(
                                parcelNumber: parcelNumbers[i],
                                parcelId: $"{record.Record.Extract(8).StringValue}/{parcelNumbers[i]}",
                                haulierCode: record.Record.Extract(8).StringValue,
                                weight: weights.Length > i ? weights[i] : null,
                                width: widths.Length > i ? widths[i] : null,
                                height: heights.Length > i ? heights[i] : null,
                                depth: depths.Length > i ? depths[i] : null,
                                pclNo: pclDotNos[i]
                            ));
                        }

                        theseResults.Add(new OEDNHead(
                            source: "OEPLHEAD",
                            orderId: record.Record.Extract(1).StringValue,
                            despatchHeadId: record.Record.Extract(2).StringValue,
                            despatchedDate: record.Record.Extract(3).StringValue,
                            despatchedTime: record.Record.Extract(4).StringValue,
                            deliveryName: address[0],
                            deliveryAddressLine1: address[1],
                            deliveryAddressLine2: address[2],
                            deliveryTown: address[3],
                            deliveryCounty: address[4],
                            deliveryPostcode: address[5],
                            deliveryCountry: record.Record.Extract(6).StringValue,
                            carriageCode: record.Record.Extract(7).StringValue,
                            haulierCode: record.Record.Extract(8).StringValue,
                            deliveryChargeSterling: record.Record.Extract(14).StringValue,
                            deliveryChargeCurrency: record.Record.Extract(15).StringValue,
                            parcels: parcels
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    };
                });

                Parallel.ForEach(oednheadBatches, (batch) =>
                {
                    var theseResults = new List<OEDNHead>();

                    var records = oednhead.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        var address = record.Record.Extract(5).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.None);

                        if (address.Length < 6)
                        {
                            var newAddress = address.ToList();

                            for (var i = 0; i < (6 - address.Length); i++)
                            {
                                newAddress.Add("");
                            }

                            address = newAddress.ToArray();
                        }

                        var parcels = new List<Parcel>();

                        var parcelNumbers = record.Record.Extract(9).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        var pclDotNos = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (!parcelNumbers.Any())
                        {
                            parcelNumbers = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        }

                        if (pclDotNos.Count() < parcelNumbers.Length)
                        {
                            for (var i = 0; i < (parcelNumbers.Length - pclDotNos.Count()); i++)
                            {
                                pclDotNos.Add("");
                            }
                        }

                        var weights = record.Record.Extract(10).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var widths = record.Record.Extract(11).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var heights = record.Record.Extract(12).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var depths = record.Record.Extract(13).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        for (var i = 0; i < parcelNumbers.Length; i++)
                        {
                            parcels.Add(new Parcel(
                                parcelNumber: parcelNumbers[i],
                                parcelId: $"{record.Record.Extract(8).StringValue}/{parcelNumbers[i]}",
                                haulierCode: record.Record.Extract(8).StringValue,
                                weight: weights.Length > i ? weights[i] : null,
                                width: widths.Length > i ? widths[i] : null,
                                height: heights.Length > i ? heights[i] : null,
                                depth: depths.Length > i ? depths[i] : null,
                                pclNo: pclDotNos[i]
                            ));
                        }

                        theseResults.Add(new OEDNHead(
                            source: "OEDNHEAD",
                            orderId: record.Record.Extract(1).StringValue,
                            despatchHeadId: record.Record.Extract(2).StringValue,
                            despatchedDate: record.Record.Extract(3).StringValue,
                            despatchedTime: record.Record.Extract(4).StringValue,
                            deliveryName: address[0],
                            deliveryAddressLine1: address[1],
                            deliveryAddressLine2: address[2],
                            deliveryTown: address[3],
                            deliveryCounty: address[4],
                            deliveryPostcode: address[5],
                            deliveryCountry: record.Record.Extract(6).StringValue,
                            carriageCode: record.Record.Extract(7).StringValue,
                            haulierCode: record.Record.Extract(8).StringValue,
                            deliveryChargeSterling: record.Record.Extract(14).StringValue,
                            deliveryChargeCurrency: record.Record.Extract(15).StringValue,
                            parcels: parcels
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });

                Parallel.ForEach(oednhhistBatches, (batch) =>
                {
                    var theseResults = new List<OEDNHead>();

                    var records = oednhhist.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        var address = record.Record.Extract(5).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.None);

                        if (address.Length < 6)
                        {
                            var newAddress = address.ToList();

                            for (var i = 0; i < (6 - address.Length); i++)
                            {
                                newAddress.Add("");
                            }

                            address = newAddress.ToArray();
                        }

                        var parcels = new List<Parcel>();


                        var parcelNumbers = record.Record.Extract(9).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        var pclDotNos = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (!parcelNumbers.Any())
                        {
                            parcelNumbers = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        }

                        if (pclDotNos.Count() < parcelNumbers.Length)
                        {
                            for (var i = 0; i < (parcelNumbers.Length - pclDotNos.Count()); i++)
                            {
                                pclDotNos.Add("");
                            }
                        }

                        var weights = record.Record.Extract(10).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var widths = record.Record.Extract(11).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var heights = record.Record.Extract(12).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var depths = record.Record.Extract(13).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        // not guaranteed to have ANY weights, widths, heights or depths

                        for (var i = 0; i < parcelNumbers.Length; i++)
                        {
                            parcels.Add(new Parcel(
                                parcelNumber: parcelNumbers[i],
                                parcelId: $"{record.Record.Extract(8).StringValue}/{parcelNumbers[i]}",
                                haulierCode: record.Record.Extract(8).StringValue,
                                weight: weights.Length > i ? weights[i] : null,
                                width: widths.Length > i ? widths[i] : null,
                                height: heights.Length > i ? heights[i] : null,
                                depth: depths.Length > i ? depths[i] : null,
                                pclNo: pclDotNos[i]
                            ));
                        }

                        theseResults.Add(new OEDNHead(
                            source: "OEDNHHIST",
                            orderId: record.Record.Extract(1).StringValue,
                            despatchHeadId: record.Record.Extract(2).StringValue,
                            despatchedDate: record.Record.Extract(3).StringValue,
                            despatchedTime: record.Record.Extract(4).StringValue,
                            deliveryName: address[0],
                            deliveryAddressLine1: address[1],
                            deliveryAddressLine2: address[2],
                            deliveryTown: address[3],
                            deliveryCounty: address[4],
                            deliveryPostcode: address[5],
                            deliveryCountry: record.Record.Extract(6).StringValue,
                            carriageCode: record.Record.Extract(7).StringValue,
                            haulierCode: record.Record.Extract(8).StringValue,
                            deliveryChargeSterling: record.Record.Extract(14).StringValue,
                            deliveryChargeCurrency: record.Record.Extract(15).StringValue,
                            parcels: parcels
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });

                var oednhhistRecords = results.Where(p => p.Source == "OEDNHEAD" || p.Source == "OEDNHHIST").Select(p => new DespatchHeadIDOrderID()
                {
                    DespatchHeadID = p.DespatchHeadID,
                    OrderID = p.OrderID
                }).ToList();

                string[] oeoanalFields = new string[] {
                    "USER.ID",
                    "AUDIT.DATE.INT",
                    "AUDIT.TIME"
                };

                var oeoanal = ce.Session.CreateUniFile("OEOANAL");

                var oeoanalKeys = new List<DespatchHeadIDOrderIDOEOANALKey>();

                if (oednhhistRecords.Any())
                {
                    var oednhhistChunks = ListExtension.Split<DespatchHeadIDOrderID>(oednhhistRecords, 10);

                    oednhhistChunks.ForEach((despatchChunk) =>
                    {
                        var orderIds = $"ORDER.NO = {String.Join(" ", despatchChunk.Select(despatch => despatch.OrderID))}";
                        var pickLists = $"PICK.LIST = {String.Join(" ", despatchChunk.Select(despatch => despatch.DespatchHeadID))}";

                        var despatchStringResult = $"LIST OEOANAL CODE FMT\"14L\" ORDER.NO FMT\"18L\" PICK.LIST FMT\"18L\" WITH {orderIds} AND {pickLists} AND MOVE.TYPE = PIK ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var despatchResult = ce.Arbitrary(despatchStringResult);
                        var theseDespatches = despatchResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList()
                            .Select(oeo =>
                            {
                                var p = oeo.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                return new DespatchHeadIDOrderIDOEOANALKey()
                                {
                                    OEOANALKey = p[0],
                                    OrderID = p[1],
                                    DespatchHeadID = p[2],
                                };
                            })
                            .ToList();
                        oeoanalKeys.AddRange(theseDespatches);
                    });
                }

                var batches = ListExtension.Split(oeoanalKeys, 1000);

                var oeoanalResults = new List<OEOANALInvoiceMovement>();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<OEOANALInvoiceMovement>();

                        var records = oeoanal.ReadRecords(batch.Select(p => p.OEOANALKey).ToArray(), oeoanalFields);

                        foreach (var b in batch)
                        {
                            var record = records[b.OEOANALKey];

                            theseResults.Add(new OEOANALInvoiceMovement(
                                despatchHeadId: b.DespatchHeadID,
                                orderId: b.OrderID,
                                oeoanalKey: b.OEOANALKey,
                                despatchStation: record.Record.Extract(1).StringValue,
                                movementDate: record.Record.Extract(2).StringValue,
                                movementTime: record.Record.Extract(3).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            oeoanalResults.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });

                oeoanalResults = oeoanalResults.GroupBy(p => p.DespatchHeadID).Select(p => p.First()).ToList();

                results.ForEach(r =>
                {
                    r.DespatchedBy = oeoanalResults.Where(p => p.DespatchHeadID == r.DespatchHeadID).Select(p => p.DespatchStation).FirstOrDefault();
                });
            }

            return new ArrayList(results);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchDespatchNoteHeads_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchTuffnellsDespatchNoteHeads(string UniVerseAccount, string DespatchHeadIDs, DateTime? FromDT)
        {
            var results = new List<OEDNHead>();

            if (String.IsNullOrEmpty(DespatchHeadIDs) && FromDT == null)
            {
                throw new Exception("@DespatchIDs was NULL or empty and FromDT was NULL. Refusing to query OEDNHEAD");
            }

            int[] fields = new int[] {
                9,   //"ORDERNO",
                0,   //"DOC.NO",
                25,  //"DESPDATE",
                19,  //"CONFTIME",
                5,   //"DELNAME",
                     //"DELADDR1",
                     //"DELADDR2",
                     //"DELADDR3",
                     //"DELADDR4",
                     //"DELPCODE",
                179, //"TO.COUNTRY",
                352, //"CARR.CODE"
                13,  //"HAULIERCODE",
                191, //"PARCEL.FULL",
                41,  //"PCL.NETWT",
                43,  //"PCL.WID",
                45,  //"PCL.HGT",
                44,  //"PCL.DEP",
                16,  //"CARRIAGE"
                60,  //"CARRIAGE.CUR"
                7,   //"TOELINEP"
                40   //"PARCEL"
            };

            int[] toelineFields = new int[]
            {
                1,  //"PRODCODE"
                70, //"MBATCH.NO"
                91  //"MPIECE.NO"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var oednhead = ce.Session.CreateUniFile("OEDNHEAD");
                var toeline = ce.Session.CreateUniFile("TOELINE");

                var oednheadkeys = new List<string>();

                if (!String.IsNullOrEmpty(DespatchHeadIDs))
                {
                    var despatchIds = JsonConvert.DeserializeObject<List<DespatchHeadIDOrderID>>(DespatchHeadIDs);

                    var despatchChunks = ListExtension.Split(despatchIds, 100);

                    despatchChunks.ForEach((despatchChunk) =>
                    {
                        var oednhhistStringResult = $"LIST OEDNHEAD USING DICT OEPLHEAD DOC.NO FMT\"20L\" WITH DOC.NO = {String.Join(" ", despatchChunk.Select(p => p.DespatchHeadID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var oednhhistResult = ce.Arbitrary(oednhhistStringResult);
                        var oednhhists = oednhhistResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        oednheadkeys.AddRange(oednhhists);
                    });
                }
                else if (FromDT.HasValue)
                {
                    var oednhhistStringResult = $"LIST OEDNHEAD USING DICT OEPLHEAD DOC.NO FMT\"20L\" WITH HAULIERCODE = \"TUF\" AND SS.DESPDATE.INT >= {Framework.UniVerseDateString(FromDT.Value)} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oednhhistResult = ce.Arbitrary(oednhhistStringResult);
                    var oednhhists = oednhhistResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    oednheadkeys.AddRange(oednhhists);
                }
                else
                {
                    var oednhhistStringResult = $"LIST OEDNHEAD USING DICT OEPLHEAD DOC.NO FMT\"20L\" WITH HAULIERCODE = TUF ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oednhhistResult = ce.Arbitrary(oednhhistStringResult);
                    var oednhhists = oednhhistResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    oednheadkeys.AddRange(oednhhists);
                }

                var locker = new object();

                var oednheadbatches = ListExtension.Split(oednheadkeys, 1000);

                Parallel.ForEach(oednheadbatches, (batch) =>
                {
                    var theseResults = new List<OEDNHead>();

                    var records = oednhead.ReadRecords(batch.ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b];

                        var address = record.Record.Extract(5).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.None);

                        if (address.Length < 6)
                        {
                            var newAddress = address.ToList();

                            for (var i = 0; i < (6 - address.Length); i++)
                            {
                                newAddress.Add("");
                            }

                            address = newAddress.ToArray();
                        }

                        var parcels = new List<Parcel>();

                        var parcelNumbers = record.Record.Extract(9).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        var pclDotNos = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (!parcelNumbers.Any())
                        {
                            parcelNumbers = record.Record.Extract(17).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        }

                        if (pclDotNos.Count() < parcelNumbers.Length)
                        {
                            for (var i = 0; i < (parcelNumbers.Length - pclDotNos.Count()); i++)
                            {
                                pclDotNos.Add("");
                            }
                        }

                        var weights = record.Record.Extract(10).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var widths = record.Record.Extract(11).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var heights = record.Record.Extract(12).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var depths = record.Record.Extract(13).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        // not guaranteed to have ANY weights, widths, heights or depths

                        for (var i = 0; i < parcelNumbers.Length; i++)
                        {
                            parcels.Add(new Parcel(
                                parcelNumber: parcelNumbers[i],
                                parcelId: $"{record.Record.Extract(8).StringValue}/{parcelNumbers[i]}",
                                haulierCode: record.Record.Extract(8).StringValue,
                                weight: weights.Length > i ? weights[i] : null,
                                width: widths.Length > i ? widths[i] : null,
                                height: heights.Length > i ? heights[i] : null,
                                depth: depths.Length > i ? depths[i] : null,
                                pclNo: pclDotNos[i]
                            ));
                        }

                        theseResults.Add(new OEDNHead(
                            source: "OEDNHHIST",
                            orderId: record.Record.Extract(1).StringValue,
                            despatchHeadId: record.Record.Extract(2).StringValue,
                            despatchedDate: record.Record.Extract(3).StringValue,
                            despatchedTime: record.Record.Extract(4).StringValue,
                            deliveryName: address[0],
                            deliveryAddressLine1: address[1],
                            deliveryAddressLine2: address[2],
                            deliveryTown: address[3],
                            deliveryCounty: address[4],
                            deliveryPostcode: address[5],
                            deliveryCountry: record.Record.Extract(6).StringValue,
                            carriageCode: record.Record.Extract(7).StringValue,
                            haulierCode: record.Record.Extract(8).StringValue,
                            deliveryChargeSterling: record.Record.Extract(14).StringValue,
                            deliveryChargeCurrency: record.Record.Extract(15).StringValue,
                            parcels: parcels
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });

                var oednhheadRecords = results.Where(p => p.Source == "OEDNHHIST").Select(p => new DespatchHeadIDOrderID()
                {
                    DespatchHeadID = p.DespatchHeadID,
                    OrderID = p.OrderID
                }).ToList();

                string[] oeoanalFields = new string[] {
                    "USER.ID",
                    "AUDIT.DATE.INT",
                    "AUDIT.TIME"
                };

                var oeoanal = ce.Session.CreateUniFile("OEOANAL");

                var oeoanalKeys = new List<DespatchHeadIDOrderIDOEOANALKey>();

                if (oednhheadRecords.Any())
                {
                    var oednhhistChunks = ListExtension.Split<DespatchHeadIDOrderID>(oednhheadRecords, 10);

                    oednhhistChunks.ForEach((despatchChunk) =>
                    {
                        var orderIds = $"ORDER.NO = {String.Join(" ", despatchChunk.Select(despatch => despatch.OrderID))}";
                        var pickLists = $"PICK.LIST = {String.Join(" ", despatchChunk.Select(despatch => despatch.DespatchHeadID))}";

                        var despatchStringResult = $"LIST OEOANAL CODE FMT\"14L\" ORDER.NO FMT\"18L\" PICK.LIST FMT\"18L\" WITH {orderIds} AND {pickLists} AND MOVE.TYPE = PIK ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var despatchResult = ce.Arbitrary(despatchStringResult);
                        var theseDespatches = despatchResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList()
                            .Select(oeo =>
                            {
                                var p = oeo.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                return new DespatchHeadIDOrderIDOEOANALKey()
                                {
                                    OEOANALKey = p[0],
                                    OrderID = p[1],
                                    DespatchHeadID = p[2],
                                };
                            })
                            .ToList();
                        oeoanalKeys.AddRange(theseDespatches);
                    });
                }

                var batches = ListExtension.Split(oeoanalKeys, 1000);

                var oeoanalResults = new List<OEOANALInvoiceMovement>();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<OEOANALInvoiceMovement>();

                        var records = oeoanal.ReadRecords(batch.Select(p => p.OEOANALKey).ToArray(), oeoanalFields);

                        foreach (var b in batch)
                        {
                            var record = records[b.OEOANALKey];

                            theseResults.Add(new OEOANALInvoiceMovement(
                                despatchHeadId: b.DespatchHeadID,
                                orderId: b.OrderID,
                                oeoanalKey: b.OEOANALKey,
                                despatchStation: record.Record.Extract(1).StringValue,
                                movementDate: record.Record.Extract(2).StringValue,
                                movementTime: record.Record.Extract(3).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            oeoanalResults.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });

                oeoanalResults = oeoanalResults.GroupBy(p => p.DespatchHeadID).Select(p => p.First()).ToList();

                results.Where(p => p.Source == "OEDNHHIST").ToList().ForEach(r =>
                {
                    r.DespatchedBy = oeoanalResults.Where(p => p.DespatchHeadID == r.DespatchHeadID).Select(p => p.DespatchStation).FirstOrDefault();
                });
            }

            return new ArrayList(results);
        }

        public static void FetchDespatchNoteHeads_FillRow(
            object obj,
            out SqlString DespatchHeadID,
            out SqlString OrderID,
            out SqlString Source,
            out SqlDateTime DespatchedDT,
            out SqlString DespatchedBy,
            out SqlString DeliveryName,
            out SqlString DeliveryAddressLine1,
            out SqlString DeliveryAddressLine2,
            out SqlString DeliveryTown,
            out SqlString DeliveryCounty,
            out SqlString DeliveryPostcode,
            out SqlString DeliveryCountryCode,
            out SqlString HaulierCode,
            out SqlString CarriageCode,
            out SqlDecimal DeliveryChargeSterling,
            out SqlDecimal DeliveryChargeCurrency,
            out SqlString ParcelJSON)
        {
            var result = (OEDNHead)obj;

            DespatchHeadID = String.IsNullOrEmpty(result.DespatchHeadID) ? SqlString.Null : result.DespatchHeadID.TrimReduceTruncate(25);

            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);

            Source = String.IsNullOrEmpty(result.Source) ? SqlString.Null : result.Source.TrimReduceTruncate(20);

            DespatchedDT = result.DespatchedDT ?? SqlDateTime.Null;

            DespatchedBy = String.IsNullOrEmpty(result.DespatchedBy) ? SqlString.Null : result.DespatchedBy.TrimReduceTruncate(30);

            DeliveryName = String.IsNullOrEmpty(result.DeliveryName) ? SqlString.Null : result.DeliveryName.TrimReduceTruncate(50);

            DeliveryAddressLine1 = String.IsNullOrEmpty(result.DeliveryAddressLine1) ? SqlString.Null : result.DeliveryAddressLine1.TrimReduceTruncate(60);

            DeliveryAddressLine2 = String.IsNullOrEmpty(result.DeliveryAddressLine2) ? SqlString.Null : result.DeliveryAddressLine2.TrimReduceTruncate(60);

            DeliveryTown = String.IsNullOrEmpty(result.DeliveryTown) ? SqlString.Null : result.DeliveryTown.TrimReduceTruncate(50);

            DeliveryCounty = String.IsNullOrEmpty(result.DeliveryCounty) ? SqlString.Null : result.DeliveryCounty.TrimReduceTruncate(40);

            DeliveryPostcode = String.IsNullOrEmpty(result.DeliveryPostcode) ? SqlString.Null : result.DeliveryPostcode.TrimReduceTruncate(20);

            DeliveryCountryCode = String.IsNullOrEmpty(result.DeliveryCountry) ? SqlString.Null : result.DeliveryCountry.TrimReduceTruncate(3);

            HaulierCode = String.IsNullOrEmpty(result.HaulierCode) ? SqlString.Null : result.HaulierCode.TrimReduceTruncate(10);

            CarriageCode = String.IsNullOrEmpty(result.CarriageCode) ? SqlString.Null : result.CarriageCode.TrimReduceTruncate(10);

            DeliveryChargeSterling = result.DeliveryChargeSterling ?? SqlDecimal.Null;

            DeliveryChargeCurrency = result.DeliveryChargeCurrency ?? SqlDecimal.Null;

            ParcelJSON = result.Parcels.Any() ? JsonConvert.SerializeObject(result.Parcels) : SqlString.Null;
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchDespatchNoteLines_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchDespatchNoteLines(string UniVerseAccount, string DespatchLineIDs)
        {
            var results = new List<OEDNLine>();

            if (String.IsNullOrEmpty(DespatchLineIDs))
            {
                throw new Exception("@DespatchLineIDs was NULL or empty. Refusing to query PLLINE / OEDNLINE / OEDNLHIST");
            }

            int[] fields = new int[] {
                2,  //"OELPHEADP"
                4,  //"PRODCODE",
                34, //"PARCEL"
                97, //"PARCEL.FULL"
                30, //"PIECE"
                31, //"BATCH"
                42, //"DESPQTY.STK"
                55  //"INVQTY.ORD"
            };

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var oeplline = ce.Session.CreateUniFile("OEPLLINE");
                var oednline = ce.Session.CreateUniFile("OEDNLINE");
                var oednlhist = ce.Session.CreateUniFile("OEDNLHIST");
                var oefindparcel = ce.Session.CreateUniFile("OEFINDPARCEL");

                var hauliers = Framework.GetHauliers(UniVerseAccount);

                var oepllineKeys = new List<DespatchLine>();
                var oednlineKeys = new List<DespatchLine>();
                var oednlhistKeys = new List<DespatchLine>();

                var despatchIds = JsonConvert.DeserializeObject<List<DespatchLine>>(DespatchLineIDs);

                var despatchChunks = ListExtension.Split(despatchIds, 50);

                despatchChunks.ForEach((despatchChunk) =>
                {
                    var oepllineStringResult = $"LIST OEPLLINE LINEKEY FMT\"20L\" WITH LINEKEY = {String.Join(" ", despatchChunk.Select(p => p.DespatchLineID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oepllineResult = ce.Arbitrary(oepllineStringResult);
                    var oepllines = oepllineResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p =>
                        {
                            return new DespatchLine()
                            {
                                DespatchLineID = p,
                                HaulierCode = despatchIds.Where(q => q.DespatchLineID == p).Select(q => q.HaulierCode).Single()
                            };
                        })
                        .ToList();
                    oepllineKeys.AddRange(oepllines);

                    var oednlineStringResult = $"LIST OEDNLINE USING DICT OEPLLINE LINEKEY FMT\"20L\" WITH LINEKEY = {String.Join(" ", despatchChunk.Select(p => p.DespatchLineID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oednlineResult = ce.Arbitrary(oednlineStringResult);
                    var oednlines = oednlineResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p =>
                        {
                            return new DespatchLine()
                            {
                                DespatchLineID = p,
                                HaulierCode = despatchIds.Where(q => q.DespatchLineID == p).Select(q => q.HaulierCode).Single()
                            };
                        })
                        .ToList();
                    oednlineKeys.AddRange(oednlines);

                    var oednlhistStringResult = $"LIST OEDNLHIST LINEKEY FMT\"20L\" WITH LINEKEY = {String.Join(" ", despatchChunk.Select(p => p.DespatchLineID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var oednlhistResult = ce.Arbitrary(oednlhistStringResult);
                    var oednlhists = oednlhistResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p =>
                        {
                            return new DespatchLine()
                            {
                                DespatchLineID = p,
                                HaulierCode = despatchIds.Where(q => q.DespatchLineID == p).Select(q => q.HaulierCode).Single()
                            };
                        })
                        .ToList();

                    oednlhistKeys.AddRange(oednlhists);
                });

                oepllineKeys = oepllineKeys.Except(oednlineKeys).Except(oednlhistKeys).ToList();
                oednlineKeys = oednlineKeys.Except(oednlhistKeys).ToList();

                var locker = new object();

                var oepllineBatches = ListExtension.Split(oepllineKeys, 1000);
                var oednlineBatches = ListExtension.Split(oednlineKeys, 1000);
                var oednlhistBatches = ListExtension.Split(oednlhistKeys, 1000);

                Parallel.ForEach(oepllineBatches, (batch) =>
                {
                    var theseResults = new List<OEDNLine>();

                    var records = oeplline.ReadRecords(batch.Select(p => p.DespatchLineID).ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b.DespatchLineID];

                        theseResults.Add(new OEDNLine(
                            source: "OEPLLINE",
                            despatchLineId: b.DespatchLineID,
                            despatchHeadId: record.Record.Extract(1).StringValue,
                            productCode: record.Record.Extract(2).StringValue,
                            ProcessParcelInformationLineLevel(
                                HaulierCode: b.HaulierCode,
                                ProductCode: record.Record.Extract(2).StringValue,
                                ParcelNumbers: record.Record.Extract(3).StringValue,
                                ParcelIDs: record.Record.Extract(4).StringValue,
                                Pieces: record.Record.Extract(5).StringValue,
                                Batches: record.Record.Extract(6).StringValue,
                                DespatchedQuantities: record.Record.Extract(7).StringValue,
                                InvoicedQuantities: record.Record.Extract(8).StringValue,
                                OEFindParcel: oefindparcel,
                                Hauliers: hauliers)
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });

                Parallel.ForEach(oednlineBatches, (batch) =>
                {
                    var theseResults = new List<OEDNLine>();

                    var records = oednline.ReadRecords(batch.Select(p => p.DespatchLineID).ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b.DespatchLineID];

                        theseResults.Add(new OEDNLine(
                            source: "OEDNLINE",
                            despatchLineId: b.DespatchLineID,
                            despatchHeadId: record.Record.Extract(1).StringValue,
                            productCode: record.Record.Extract(2).StringValue,
                            ProcessParcelInformationLineLevel(
                                HaulierCode: b.HaulierCode,
                                ProductCode: record.Record.Extract(2).StringValue,
                                ParcelNumbers: record.Record.Extract(3).StringValue,
                                ParcelIDs: record.Record.Extract(4).StringValue,
                                Pieces: record.Record.Extract(5).StringValue,
                                Batches: record.Record.Extract(6).StringValue,
                                DespatchedQuantities: record.Record.Extract(7).StringValue,
                                InvoicedQuantities: record.Record.Extract(8).StringValue,
                                OEFindParcel: oefindparcel,
                                Hauliers: hauliers)
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });

                Parallel.ForEach(oednlhistBatches, (batch) =>
                {
                    var theseResults = new List<OEDNLine>();

                    var records = oednlhist.ReadRecords(batch.Select(p => p.DespatchLineID).ToArray(), fields);

                    batch.ForEach(b =>
                    {
                        var record = records[b.DespatchLineID];

                        theseResults.Add(new OEDNLine(
                            source: "OEDNLHIST",
                            despatchLineId: b.DespatchLineID,
                            despatchHeadId: record.Record.Extract(1).StringValue,
                            productCode: record.Record.Extract(2).StringValue,
                            ProcessParcelInformationLineLevel(
                                HaulierCode: b.HaulierCode,
                                ProductCode: record.Record.Extract(2).StringValue,
                                ParcelNumbers: record.Record.Extract(3).StringValue,
                                ParcelIDs: record.Record.Extract(4).StringValue,
                                Pieces: record.Record.Extract(5).StringValue,
                                Batches: record.Record.Extract(6).StringValue,
                                DespatchedQuantities: record.Record.Extract(7).StringValue,
                                InvoicedQuantities: record.Record.Extract(8).StringValue,
                                OEFindParcel: oefindparcel,
                                Hauliers: hauliers)
                        ));
                    });

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static List<Parcel> ProcessParcelInformationLineLevel(
            string HaulierCode,
            string ProductCode,
            string ParcelNumbers,
            string ParcelIDs,
            string Pieces,

            string Batches,
            string DespatchedQuantities,
            string InvoicedQuantities,
            IBMU2.UODOTNET.UniFile OEFindParcel,
            IEnumerable<CodeDescription> Hauliers)
        {
            var results = new List<Parcel>();

            var haulierFields = new string[] { "WEBPARCEL", "WEBPARCEL2" };

            if (String.IsNullOrEmpty(ParcelIDs))
            {
                ParcelIDs = ParcelNumbers;
            }

            if (!String.IsNullOrEmpty(ParcelIDs))
            {
                var parcelNumbers = ParcelNumbers.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var parcelIds = ParcelIDs.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var pieces = Pieces.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var batches = Batches.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var despatchedQuantities = DespatchedQuantities.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                var invoicedQuantities = InvoicedQuantities.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                if (batches.Length < parcelNumbers.Length && Batches.Contains(Framework.SubValueMark))
                {
                    batches = Batches.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                }

                try
                {
                    if (parcelIds.Length == 0)
                    {
                        // this is a hackish workaround because sometimes Mertex just makes you want to cry
                        parcelIds = parcelNumbers;
                    }

                    IBMU2.UODOTNET.UniDataSet oefindparcels = null;

                    try
                    {
                        oefindparcels = OEFindParcel.ReadRecords(parcelIds.Select(p => $"{HaulierCode}/{p}").Distinct().ToArray(), new string[]
                        {
                            "CUSTOM.TRACK.URL.DES"
                        });
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {
                        HaulierCode = "INT";

                        oefindparcels = OEFindParcel.ReadRecords(parcelIds.Select(p => $"{HaulierCode}/{p}").Distinct().ToArray(), new string[]
                        {
                            "CUSTOM.TRACK.URL.DES"
                        });
                    }

                    for (var i = 0; i < parcelIds.Length; i++)
                    {
                        var parcelId = $"{HaulierCode}/{parcelIds[i]}";

                        var oefindparcel = oefindparcels[parcelId];

                        var trackingUrl = String.IsNullOrEmpty(oefindparcel.Record.Extract(1).StringValue) ? null : oefindparcel.Record.Extract(1).StringValue;

                        if (trackingUrl == null && (HaulierCode == "UPS" || HaulierCode == "UCP"))
                        {
                            var haulier = Hauliers.Where(p => p.Code == HaulierCode).Select(p => p.Errata).Single();
                            trackingUrl = $"{haulier["Webparcel"]}{parcelIds[i]}{haulier["Webparcel2"]}";
                        }

                        var parcel = new Parcel(
                            parcelIds[i],
                            parcelId,
                            HaulierCode,
                            trackingUrl);

                        var theseDespatchedQuantities = new string[] { };

                        try
                        {
                            theseDespatchedQuantities = despatchedQuantities[i].Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // this parcel might have some products declared, but they have not provided any despatch quantities
                            // give up and go retrain as a landscape gardener
                            continue;
                        }


                        var theseInvoicedQuantities = new string[] { };

                        try
                        {
                            theseInvoicedQuantities = invoicedQuantities[i].Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // this parcel might have some products declared, but they have not provided any invoice quantities
                            // give up and go retrain as a landscape gardener
                            theseInvoicedQuantities = theseDespatchedQuantities;
                        }


                        // can quite readily have no batches and no pieces...
                        var theseBatches = new string[] { };
                        var thesePieces = new string[] { };

                        try
                        {
                            if (batches.Any())
                            {
                                theseBatches = batches[i].Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }

                        try
                        {
                            if (pieces.Any())
                            {
                                thesePieces = pieces[i].Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }

                        for (var j = 0; j < theseDespatchedQuantities.Length; j++)
                        {
                            string batch = null;
                            string piece = null;

                            try
                            {
                                batch = theseBatches[j];
                            }
                            catch (IndexOutOfRangeException)
                            {

                            }

                            if (thesePieces.Length > j)
                                piece = thesePieces[j];

                            parcel.ParcelContents.Add(new ParcelContent(
                                parcelId,
                                batch,
                                piece,
                                ProductCode,
                                Convert.ToDecimal(theseDespatchedQuantities[j]) / 1000,
                                Convert.ToDecimal(theseInvoicedQuantities[j]) / 1000
                            ));
                        }

                        //for (var j = 0; j < theseBatches.Length; j++)
                        //{
                        //    string batch = theseBatches[j];
                        //    string piece = null;

                        //    if (thesePieces.Length > j)
                        //        piece = thesePieces[j];

                        //    parcel.ParcelContents.Add(new ParcelContent(
                        //        parcelId,
                        //        batch,
                        //        piece,
                        //        ProductCode,
                        //        Convert.ToDecimal(theseDespatchedQuantities[j]) / 1000,
                        //        Convert.ToDecimal(theseInvoicedQuantities[j]) / 1000
                        //    ));
                        //}

                        results.Add(parcel);
                    }
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // We've tried to read a record from OEFINDPARCEL which does not exist. Oh, well...

                    // This can be demonstrated with DespatchID 040003
                }
            }

            results = results.GroupBy(p => new
            {
                p.ParcelNumber,
                p.ParcelID,
                p.HaulierCode,
                p.TrackingURL
            }).Select(p =>
                new Parcel(
                    p.Key.ParcelNumber,
                    p.Key.ParcelID,
                    p.Key.HaulierCode,
                    p.Key.TrackingURL)
                {
                    ParcelContents = p.SelectMany(q => q.ParcelContents).ToList()
                }).ToList();

            return results;
        }


        public static void FetchDespatchNoteLines_FillRow(
            object obj,
            out SqlString DespatchHeadID,
            out SqlString DespatchLineID,
            out SqlString Source,
            out SqlString ProductCode,
            out SqlString ParcelJSON
            )
        {
            var result = (OEDNLine)obj;

            DespatchHeadID = String.IsNullOrEmpty(result.DespatchHeadID) ? SqlString.Null : result.DespatchHeadID.TrimReduceTruncate(25);

            DespatchLineID = String.IsNullOrEmpty(result.DespatchLineID) ? SqlString.Null : result.DespatchLineID.TrimReduceTruncate(30);

            Source = String.IsNullOrEmpty(result.Source) ? SqlString.Null : result.Source.TrimReduceTruncate(20);

            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(20);

            ParcelJSON = result.Parcels.Any() ? JsonConvert.SerializeObject(result.Parcels) : SqlString.Null;
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchHauliers_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchHauliers(string UniVerseAccount)
        {
            var results = new List<OEHaulier>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var prodLocKeys = new List<string>();

                string[] fields = new string[] {
                    "CODE",
                    "DESCRIPTION",
                    "CUT.OFF.TIME.DES",
                    "PARCEL.SYSTEM"
                };

                var oehaulier = ce.Session.CreateUniFile("OEHAULIER");

                var oehaulierKeys = new List<string>();

                var oehaulierList = $"LIST OEHAULIER CODE FMT\"20L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var oehaulierResult = ce.Arbitrary(oehaulierList);
                var oehauliers = oehaulierResult
                    .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                oehaulierKeys.AddRange(oehauliers);

                var records = oehaulier.ReadRecords(oehaulierKeys.ToArray(), fields);

                foreach (var hk in oehaulierKeys)
                {
                    var record = records[hk];

                    results.Add(new OEHaulier(
                        record.Record.Extract(1).StringValue,
                        record.Record.Extract(2).StringValue,
                        record.Record.Extract(3).StringValue,
                        record.Record.Extract(4).StringValue
                    ));
                }
            }

            return new ArrayList(results);
        }

        public static void FetchHauliers_FillRow(
            object obj,
            out SqlString HaulierCode,
            out SqlString Description,
            out SqlDateTime CutOffTime,
            out SqlBoolean ExpectParcelData
            )
        {
            var result = (OEHaulier)obj;

            HaulierCode = String.IsNullOrEmpty(result.HaulierCode) ? SqlString.Null : result.HaulierCode.TrimReduceTruncate(10);
            Description = String.IsNullOrEmpty(result.Description) ? SqlString.Null : result.Description.TrimReduceTruncate(100);
            CutOffTime = result.CutOffTime.HasValue ? SqlDateTime.MinValue + result.CutOffTime.Value : SqlDateTime.Null;
            ExpectParcelData = result.ExpectParcelData;
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchSuppliers_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchSuppliers(string UniVerseAccount)
        {
            var results = new List<Supplier>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var prodLocKeys = new List<string>();

                string[] fields = new string[] {
                    "ACC.CODE",
                    "SUPNAME",
                    "SUPADD1",
                    "SUPADD2",
                    "SUPADD3",
                    "SUPADD4",
                    "SUPPCODE",
                    "COUNTRY.CODE",
                    "CURRENCY"
                };

                var supplier = ce.Session.CreateUniFile("SUPPLIER");

                var supplierKeys = new List<string>();

                var supplierList = $"LIST SUPPLIER ACC.CODE FMT\"20L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var supplierResult = ce.Arbitrary(supplierList);
                var suppliers = supplierResult
                    .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                supplierKeys.AddRange(suppliers);

                var records = supplier.ReadRecords(supplierKeys.ToArray(), fields);

                foreach (var hk in supplierKeys)
                {
                    var record = records[hk];

                    results.Add(new Supplier(
                        record.Record.Extract(1).StringValue,
                        record.Record.Extract(2).StringValue,
                        record.Record.Extract(3).StringValue,
                        record.Record.Extract(4).StringValue,
                        record.Record.Extract(5).StringValue,
                        record.Record.Extract(6).StringValue,
                        record.Record.Extract(7).StringValue,
                        record.Record.Extract(8).StringValue,
                        record.Record.Extract(9).StringValue
                    ));
                }
            }

            return new ArrayList(results);
        }

        public static void FetchSuppliers_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString CountryCode,
            out SqlString MertexCurrencyCode
            )
        {
            var result = (Supplier)obj;

            Code = String.IsNullOrEmpty(result.Code) ? SqlString.Null : result.Code.TrimReduceTruncate(50);
            Name = String.IsNullOrEmpty(result.Name) ? SqlString.Null : result.Name.TrimReduceTruncate(50);
            AddressLine1 = String.IsNullOrEmpty(result.AddressLine1) ? SqlString.Null : result.AddressLine1.TrimReduceTruncate(60);
            AddressLine2 = String.IsNullOrEmpty(result.AddressLine2) ? SqlString.Null : result.AddressLine2.TrimReduceTruncate(60);
            Town = String.IsNullOrEmpty(result.Town) ? SqlString.Null : result.Town.TrimReduceTruncate(50);
            County = String.IsNullOrEmpty(result.County) ? SqlString.Null : result.County.TrimReduceTruncate(40);
            Postcode = String.IsNullOrEmpty(result.Postcode) ? SqlString.Null : result.Postcode.TrimReduceTruncate(20);
            CountryCode = String.IsNullOrEmpty(result.CountryCode) ? SqlString.Null : result.CountryCode.TrimReduceTruncate(3);
            MertexCurrencyCode = String.IsNullOrEmpty(result.MertexCurrencyCode) ? SqlString.Null : result.MertexCurrencyCode.TrimReduceTruncate(3);
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchPurchasePrices_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchPurchasePrices(string UniVerseAccount, string SupplierProductCodes)
        {
            var results = new List<POListPrice>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "PRICE.KEY",
                    "PRODCODE",
                    "PRICE",
                    "QUANTITY",
                    "CURRENCY"
                };

                var poListPrice = ce.Session.CreateUniFile("PO.PRICE.LIST");

                var poListPriceKeys = new List<string>();

                if (String.IsNullOrEmpty(SupplierProductCodes))
                {
                    var poListPriceList = $"LIST PO.PRICE.LIST PRICE.KEY FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var poListPriceResult = ce.Arbitrary(poListPriceList);
                    var poListPrices = poListPriceResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var reducedList = poListPrices
                        .Where(p => System.Text.RegularExpressions.Regex.Match(p, ".*\\*.+").Success)
                        .ToList();

                    var exclusions = poListPrices.Except(reducedList);

                    poListPriceKeys.AddRange(reducedList);
                }
                else
                {
                    poListPriceKeys = JsonConvert.DeserializeObject<List<SupplierProductCode>>(SupplierProductCodes)
                                                 .Select(p => $"{p.Key}").ToList();
                }

                var batches = ListExtension.Split(poListPriceKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<POListPrice>();

                    var records = poListPrice.ReadRecords(batch.ToArray(), fields);

                    foreach (var b in batch)
                    {
                        var record = records[b];

                        var supplierReference = record.Record.Extract(1).StringValue;
                        var productCode = record.Record.Extract(2).StringValue;

                        var thesePrices = record.Record.Extract(3).StringValue.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        var theseQuantities = record.Record.Extract(4).StringValue.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        var mertexCurrencyCode = record.Record.Extract(5).StringValue;

                        for (var i = 0; i < thesePrices.Length; i++)
                        {
                            theseResults.Add(new POListPrice(
                                supplierReference: supplierReference,
                                productCode: productCode,
                                purchasePrice: thesePrices[i],
                                quantityBreak: theseQuantities.Length == thesePrices.Length ? theseQuantities[i] : null,
                                mertexCurrencyCode: mertexCurrencyCode
                            ));
                        }
                    }

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void FetchPurchasePrices_FillRow(
            object obj,
            out SqlString SupplierReference,
            out SqlString ProductCode,
            out SqlDecimal PurchasePrice,
            out SqlDecimal QuantityBreak,
            out SqlString MertexCurrencyCode
            )
        {
            var result = (POListPrice)obj;

            SupplierReference = String.IsNullOrEmpty(result.SupplierReference) ? SqlString.Null : result.SupplierReference.TrimReduceTruncate(50);
            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(50);
            PurchasePrice = result.PurchasePrice ?? SqlDecimal.Null;
            QuantityBreak = result.QuantityBreak ?? SqlDecimal.Null;
            MertexCurrencyCode = String.IsNullOrEmpty(result.MertexCurrencyCode) ? SqlString.Null : result.MertexCurrencyCode.TrimReduceTruncate(5);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "ExchangeRates_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchExchangeRates(string UniVerseAccount)
        {
            var results = new List<XRate>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "CODE",
                    "RATE"
                };

                var xrate = ce.Session.CreateUniFile("XRATE");

                var xrateKeys = new List<string>();

                var xRateList = $"LIST XRATE CODE FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var xRateResult = ce.Arbitrary(xRateList);
                var xRates = xRateResult
                    .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                xrateKeys.AddRange(xRates);

                var batches = ListExtension.Split(xrateKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    var theseResults = new List<XRate>();

                    var records = xrate.ReadRecords(batch.ToArray(), fields);

                    foreach (var b in batch)
                    {
                        var record = records[b];

                        theseResults.Add(new XRate(
                            mertexCurrencyCode: record.Record.Extract(1).StringValue,
                            sterlingExchangeRate: record.Record.Extract(2).StringValue
                        ));
                    }

                    lock (locker)
                    {
                        results.AddRange(theseResults);
                    }
                });
            }

            return new ArrayList(results);
        }

        public static void ExchangeRates_FillRow(
            object obj,
            out SqlString MertexCurrencyCode,
            out SqlDecimal SterlingExchangeRate
            )
        {
            var result = (XRate)obj;

            MertexCurrencyCode = String.IsNullOrEmpty(result.MertexCurrencyCode) ? SqlString.Null : result.MertexCurrencyCode.TrimReduceTruncate(5);

            SterlingExchangeRate = result.SterlingExchangeRate;
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "DeliveryAddress_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchDeliveryAddresses(string UniVerseAccount, string DeliveryReference, string Customers, string Markets)
        {
            var results = new List<DelAdd>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "CUSREF",
                    "DELADDR1",
                    "DELADDR2",
                    "DELADDR3",
                    "DELADDR4",
                    "DELPCODE",
                    "COUNTRY"
                };

                var deladd = ce.Session.CreateUniFile("DELADD");

                var deladdKeys = new List<string>();

                if (!String.IsNullOrEmpty(DeliveryReference))
                {
                    var deladdList = $"LIST DELADD DELREF FMT\"40L\" {DeliveryReference} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var deladdResult = ce.Arbitrary(deladdList);
                    var deladds = deladdResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    deladdKeys.AddRange(deladds);
                }
                else if (!String.IsNullOrEmpty(Customers))
                {
                    var cusrefs = JsonConvert.DeserializeObject<List<TradeAccountCode>>(Customers);

                    var cusrefChunks = ListExtension.Split(cusrefs, 100);

                    cusrefChunks.ForEach((cusrefChunk) =>
                    {
                        var oeplheadStringResult = $"LIST DELADD DELREF FMT\"40L\" WITH CUSREF = {String.Join(" ", cusrefChunk.Select(p => p.Code))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var oeplheadResult = ce.Arbitrary(oeplheadStringResult);
                        var oeplheads = oeplheadResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        deladdKeys.AddRange(oeplheads);
                    });
                }
                else if (!String.IsNullOrEmpty(Markets))
                {
                    var markets = JsonConvert.DeserializeObject<List<Market>>(Markets);

                    var marketChunks = ListExtension.Split(markets, 100);

                    marketChunks.ForEach((marketChunk) =>
                    {
                        var oeplheadStringResult = $"LIST DELADD DELREF FMT\"40L\" WITH CUSMARKET = {String.Join(" ", marketChunk.Select(p => p.Code))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var oeplheadResult = ce.Arbitrary(oeplheadStringResult);
                        var oeplheads = oeplheadResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        deladdKeys.AddRange(oeplheads);
                    });
                }
                else
                {
                    var deladdList = $"LIST DELADD DELREF FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var deladdResult = ce.Arbitrary(deladdList);
                    var deladds = deladdResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    deladdKeys.AddRange(deladds);
                }

                var batches = ListExtension.Split(deladdKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<DelAdd>();

                        var records = deladd.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b];

                            theseResults.Add(new DelAdd(
                                deliveryReference: b,
                                tradeAccountCode: record.Record.Extract(1).StringValue,
                                addressLine1: record.Record.Extract(2).StringValue,
                                addressLine2: record.Record.Extract(3).StringValue,
                                town: record.Record.Extract(4).StringValue,
                                county: record.Record.Extract(5).StringValue,
                                postcode: record.Record.Extract(6).StringValue,
                                mertexCountryCode: record.Record.Extract(7).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });
            }

            return new ArrayList(results);
        }

        public static void DeliveryAddress_FillRow(
            object obj,
            out SqlString DeliveryReference,
            out SqlString TradeAccountCode,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString MertexCountryCode)
        {

            var result = (DelAdd)obj;

            DeliveryReference = String.IsNullOrEmpty(result.DeliveryReference) ? SqlString.Null : result.DeliveryReference.TrimReduceTruncate(20);
            TradeAccountCode = String.IsNullOrEmpty(result.TradeAccountCode) ? SqlString.Null : result.TradeAccountCode.TrimReduceTruncate(50);
            AddressLine1 = String.IsNullOrEmpty(result.AddressLine1) ? SqlString.Null : result.AddressLine1.TrimReduceTruncate(60);
            AddressLine2 = String.IsNullOrEmpty(result.AddressLine2) ? SqlString.Null : result.AddressLine2.TrimReduceTruncate(60);
            Town = String.IsNullOrEmpty(result.Town) ? SqlString.Null : result.Town.TrimReduceTruncate(50);
            County = String.IsNullOrEmpty(result.County) ? SqlString.Null : result.County.TrimReduceTruncate(40);
            Postcode = String.IsNullOrEmpty(result.Postcode) ? SqlString.Null : result.Postcode.TrimReduceTruncate(20);
            MertexCountryCode = String.IsNullOrEmpty(result.MertexCountryCode) ? SqlString.Null : result.MertexCountryCode.TrimReduceTruncate(3);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "CCRHeaders_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchCCRHeaders(string UniVerseAccount, string CCRNumbers, string MarketCodes, DateTime? FromDT, DateTime? ToDT)
        {
            var results = new List<TCCHead>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "CLAIMNO",
                    "SORDERNO",
                    "DOC.NO",
                    "INVOICE1",
                    "SS.ONFILE.INT",
                    "ONTIME",
                    "CUSREF",
                    "SS.MARKET",
                    "CONTACT1",
                    "COLNAME",
                    "COLADDR",
                    "SS.COLCOUNTRY",
                    "DEL.ADDRESS",
                    "CLAIMTAKER",
                    "CURRENTPERSON",
                    "CURRENTSECTION",
                    "COMPLETED",
                    "RESULT",
                    "CARRIER.REF"
                };

                var tcchead = ce.Session.CreateUniFile("TCCHEAD");

                var ccrKeys = new List<string>();

                var selectStatement = $"";

                if (MarketCodes != null)
                {
                    selectStatement = $"SS.MARKTYPE = {String.Join(" ", JsonConvert.DeserializeObject<List<MarketCode>>(MarketCodes).Select(p => p.SalesMarketCode))}";
                }

                if (FromDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND SS.ONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                    else
                        selectStatement = $"SS.ONFILE.INT >= {Framework.UniVerseDateString(FromDT.Value)}";
                }

                if (ToDT != null)
                {
                    if (selectStatement != "")
                        selectStatement += $" AND SS.ONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                    else
                        selectStatement = $"SS.ONFILE.INT <= {Framework.UniVerseDateString(ToDT.Value)}";
                }

                if (!String.IsNullOrEmpty(CCRNumbers))
                {
                    if (selectStatement != "")
                        selectStatement = $"AND {selectStatement}";

                    var ccrs = JsonConvert.DeserializeObject<List<CCRHead>>(CCRNumbers);

                    var ccrChunks = ListExtension.Split(ccrs, 100);

                    ccrChunks.ForEach((ccrChunk) =>
                    {
                        var ccrheadStringResult = $"LIST TCCHEAD CLAIMNO FMT\"40L\" WITH CLAIMNO = {String.Join(" ", ccrChunk.Select(p => p.ClaimID))} {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var ccrheadResult = ce.Arbitrary(ccrheadStringResult);
                        var ccrheads = ccrheadResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        ccrKeys.AddRange(ccrheads);
                    });
                }
                else
                {
                    if (selectStatement != "")
                        selectStatement = $"WITH {selectStatement}";

                    var ccrList = $"LIST TCCHEAD CLAIMNO FMT\"40L\" {selectStatement} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var ccrResult = ce.Arbitrary(ccrList);
                    var ccrs = ccrResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    ccrKeys.AddRange(ccrs);
                }

                var batches = ListExtension.Split(ccrKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<TCCHead>();

                        var records = tcchead.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b];

                            theseResults.Add(new TCCHead(
                                claimId: record.Record.Extract(1).StringValue,
                                orderId: record.Record.Extract(2).StringValue,
                                documentNumber: record.Record.Extract(3).StringValue,
                                invoiceNumber: record.Record.Extract(4).StringValue,
                                dateOnFile: record.Record.Extract(5).StringValue,
                                timeOnFile: record.Record.Extract(6).StringValue,
                                tradeAccountCode: record.Record.Extract(7).StringValue,
                                salesMarketCode: record.Record.Extract(8).StringValue,
                                contactName: record.Record.Extract(9).StringValue,
                                collectionAddress: $"{record.Record.Extract(10).StringValue}{Framework.ValueMark}{record.Record.Extract(11).StringValue}",
                                collectionCountryCode: record.Record.Extract(12).StringValue,
                                deliveryAddress: record.Record.Extract(13).StringValue,
                                claimTaker: record.Record.Extract(14).StringValue,
                                currentPerson: record.Record.Extract(15).StringValue,
                                currentDepartment: record.Record.Extract(16).StringValue,
                                completed: record.Record.Extract(17).StringValue,
                                result: record.Record.Extract(18).StringValue,
                                carrierReferences: record.Record.Extract(19).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList()
                            ));
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });
            }

            return new ArrayList(results);
        }

        public static void CCRHeaders_FillRow(
            object obj,
            out SqlString ClaimID,
            out SqlString OrderID,
            out SqlString DocumentNumber,
            out SqlString InvoiceNumber,
            out SqlString TradeAccountCode,
            out SqlDateTime ClaimCreatedDT,
            out SqlString ClaimTaker,
            out SqlString CurrentPerson,
            out SqlString CurrentDepartmentCode,
            out SqlBoolean Completed,
            out SqlString Result,
            out SqlString SalesMarketCode,
            out SqlString ContactName,
            out SqlString CollectionName,
            out SqlString CollectionAddressLine1,
            out SqlString CollectionAddressLine2,
            out SqlString CollectionTown,
            out SqlString CollectionCounty,
            out SqlString CollectionPostcode,
            out SqlString CollectionCountryCode,
            out SqlString DeliveryName,
            out SqlString DeliveryAddressLine1,
            out SqlString DeliveryAddressLine2,
            out SqlString DeliveryTown,
            out SqlString DeliveryCounty,
            out SqlString DeliveryPostcode,
            out SqlString DeliveryCountry,
            out SqlString CarrierReferencesJSON)
        {

            var result = (TCCHead)obj;

            ClaimID = String.IsNullOrEmpty(result.ClaimID) ? SqlString.Null : result.ClaimID.TrimReduceTruncate(25);

            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);

            DocumentNumber = String.IsNullOrEmpty(result.DocumentNumber) ? SqlString.Null : result.DocumentNumber.TrimReduceTruncate(50);

            InvoiceNumber = String.IsNullOrEmpty(result.InvoiceNumber) ? SqlString.Null : result.InvoiceNumber.TrimReduceTruncate(50);

            TradeAccountCode = String.IsNullOrEmpty(result.TradeAccountCode) ? SqlString.Null : result.TradeAccountCode.TrimReduceTruncate(50);

            ClaimCreatedDT = result.DateOnFile ?? SqlDateTime.Null;

            ClaimTaker = String.IsNullOrEmpty(result.ClaimTaker) ? SqlString.Null : result.ClaimTaker.TrimReduceTruncate(10);

            CurrentPerson = String.IsNullOrEmpty(result.CurrentPerson) ? SqlString.Null : result.CurrentPerson.TrimReduceTruncate(10);

            CurrentDepartmentCode = String.IsNullOrEmpty(result.CurrentDepartmentCode) ? SqlString.Null : result.CurrentDepartmentCode.TrimReduceTruncate(10);

            Completed = result.Completed;

            Result = String.IsNullOrEmpty(result.Result) ? SqlString.Null : result.Result.TrimReduceTruncate(10);

            SalesMarketCode = String.IsNullOrEmpty(result.SalesMarketCode) ? SqlString.Null : result.SalesMarketCode.TrimReduceTruncate(2);

            ContactName = String.IsNullOrEmpty(result.ContactName) ? SqlString.Null : result.ContactName.TrimReduceTruncate(50);

            CollectionName = String.IsNullOrEmpty(result.CollectionName) ? SqlString.Null : result.CollectionName.TrimReduceTruncate(50);

            CollectionAddressLine1 = String.IsNullOrEmpty(result.CollectionAddressLine1) ? SqlString.Null : result.CollectionAddressLine1.TrimReduceTruncate(60);

            CollectionAddressLine2 = String.IsNullOrEmpty(result.CollectionAddressLine2) ? SqlString.Null : result.CollectionAddressLine2.TrimReduceTruncate(60);

            CollectionTown = String.IsNullOrEmpty(result.CollectionTown) ? SqlString.Null : result.CollectionTown.TrimReduceTruncate(50);

            CollectionCounty = String.IsNullOrEmpty(result.CollectionCounty) ? SqlString.Null : result.CollectionCounty.TrimReduceTruncate(40);

            CollectionPostcode = String.IsNullOrEmpty(result.CollectionPostcode) ? SqlString.Null : result.CollectionPostcode.TrimReduceTruncate(20);

            CollectionCountryCode = String.IsNullOrEmpty(result.CollectionCountryCode) ? SqlString.Null : result.CollectionCountryCode.TrimReduceTruncate(3);

            DeliveryName = String.IsNullOrEmpty(result.DeliveryName) ? SqlString.Null : result.CollectionName.TrimReduceTruncate(50);

            DeliveryAddressLine1 = String.IsNullOrEmpty(result.DeliveryAddressLine1) ? SqlString.Null : result.DeliveryAddressLine1.TrimReduceTruncate(60);

            DeliveryAddressLine2 = String.IsNullOrEmpty(result.DeliveryAddressLine2) ? SqlString.Null : result.DeliveryAddressLine2.TrimReduceTruncate(60);

            DeliveryTown = String.IsNullOrEmpty(result.DeliveryTown) ? SqlString.Null : result.DeliveryTown.TrimReduceTruncate(50);

            DeliveryCounty = String.IsNullOrEmpty(result.DeliveryCounty) ? SqlString.Null : result.DeliveryCounty.TrimReduceTruncate(40);

            DeliveryPostcode = String.IsNullOrEmpty(result.DeliveryPostcode) ? SqlString.Null : result.DeliveryPostcode.TrimReduceTruncate(20);

            DeliveryCountry = String.IsNullOrEmpty(result.DeliveryCountry) ? SqlString.Null : result.DeliveryCountry.TrimReduceTruncate(3);

            CarrierReferencesJSON = result.CarrierReferences.Any() ? result.CarrierReferencesJSON : SqlString.Null;
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "CCRLines_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchCCRLines(string UniVerseAccount, string ClaimIDs)
        {
            var results = new List<TCCLine>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "CCCLAIMNO",
                    "CCLINEKEY",
                    "SORDLINE",
                    "PRODCODE",
                    "FAULTCODE",
                    "INVOICENO",
                    "SS.INVDATE.INT",
                    "QTY.CLAIM",
                    "QTY.ACCEPT",
                    "QTY.RETURNED",
                    "S.CREDAMT",
                    "RETURN.STOCK",
                    "ORDPRICE",
                    "PERC.DISC",
                    "TEXT"
                };

                var tccline = ce.Session.CreateUniFile("TCCLINE");

                var claimKeys = new List<string>();

                if (!String.IsNullOrEmpty(ClaimIDs))
                {
                    var claims = JsonConvert.DeserializeObject<List<CCRHead>>(ClaimIDs);

                    var claimChunks = ListExtension.Split(claims, 100);

                    claimChunks.ForEach((claimChunk) =>
                    {
                        var claimStringResult = $"LIST TCCLINE CCLINEKEY FMT\"40L\" WITH CCCLAIMNO = {String.Join(" ", claimChunk.Select(p => p.ClaimID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var claimResult = ce.Arbitrary(claimStringResult);
                        var theseClaims = claimResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        claimKeys.AddRange(theseClaims);
                    });
                }
                else
                {
                    var claimtList = $"LIST TCCLINE CCLINEKEY FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var claimResult = ce.Arbitrary(claimtList);
                    var theseClaims = claimResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    claimKeys.AddRange(theseClaims);
                }

                var batches = ListExtension.Split(claimKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<TCCLine>();

                        var records = tccline.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b];

                            if (record.Record.Extract(3).StringValue?.Contains(Framework.ValueMark) == true)
                            {
                                throw new Exception($"Value Mark found on {b} SORDLINE");
                            }

                            if (record.Record.Extract(3).StringValue?.Contains(Framework.SubValueMark) == true)
                            {
                                throw new Exception($"SubValue Mark found on {b} SORDLINE");
                            }

                            theseResults.Add(new TCCLine(
                                claimId: record.Record.Extract(1).StringValue,
                                claimLineId: record.Record.Extract(2).StringValue,
                                orderLineId: record.Record.Extract(3).StringValue,
                                productCode: record.Record.Extract(4).StringValue,
                                faultCode: record.Record.Extract(5).StringValue,
                                invoiceNumber: record.Record.Extract(6).StringValue,
                                invoiceDt: record.Record.Extract(7).StringValue,
                                claimQuantity: record.Record.Extract(8).StringValue,
                                acceptedQuantity: record.Record.Extract(9).StringValue,
                                returnedQuantity: record.Record.Extract(10).StringValue,
                                creditAmount: record.Record.Extract(11).StringValue,
                                returnToStock: record.Record.Extract(12).StringValue,
                                orderPriceSterlingEx: record.Record.Extract(13).StringValue,
                                discountPercentage: record.Record.Extract(14).StringValue,
                                notes: record.Record.Extract(15).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });
            }

            return new ArrayList(results);
        }

        public static void CCRLines_FillRow(
            object obj,
            out SqlString ClaimID,
            out SqlString ClaimLineID,
            out SqlString OrderLineID,
            out SqlString ProductCode,
            out SqlString FaultCode,
            out SqlString InvoiceNumber,
            out SqlDateTime InvoiceDT,
            out SqlDecimal ClaimQuantity,
            out SqlDecimal AcceptedQuantity,
            out SqlDecimal ReturnedQuantity,
            out SqlDecimal CreditAmount,
            out SqlBoolean ReturnToStock,
            out SqlDecimal OrderPriceSterlingEx,
            out SqlDecimal DiscountPercentage,
            out SqlString Notes
            )
        {
            var result = (TCCLine)obj;

            ClaimID = String.IsNullOrEmpty(result.ClaimID) ? SqlString.Null : result.ClaimID.TrimReduceTruncate(25);

            ClaimLineID = String.IsNullOrEmpty(result.ClaimLineID) ? SqlString.Null : result.ClaimLineID.TrimReduceTruncate(255);

            OrderLineID = String.IsNullOrEmpty(result.OrderLineID) ? SqlString.Null : result.OrderLineID.TrimReduceTruncate(30);

            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(50);

            FaultCode = String.IsNullOrEmpty(result.FaultCode) ? SqlString.Null : result.FaultCode.TrimReduceTruncate(10);

            InvoiceNumber = String.IsNullOrEmpty(result.InvoiceNumber) ? SqlString.Null : result.InvoiceNumber.TrimReduceTruncate(50);

            InvoiceDT = result.InvoiceDT ?? SqlDateTime.Null;

            ClaimQuantity = result.ClaimQuantity ?? SqlDecimal.Null;

            AcceptedQuantity = result.AcceptedQuantity ?? SqlDecimal.Null;

            ReturnedQuantity = result.ReturnedQuantity ?? SqlDecimal.Null;

            CreditAmount = result.CreditAmount ?? SqlDecimal.Null;

            ReturnToStock = result.ReturnToStock;

            OrderPriceSterlingEx = result.OrderPriceSterlingEx ?? SqlDecimal.Null;

            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;

            Notes = String.IsNullOrEmpty(result.Notes) ? SqlString.Null : result.Notes.TrimReduceTruncate(500);
        }




        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "CCRActions_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchCCRActions(string UniVerseAccount, string ClaimIDs)
        {
            var results = new List<TCCAction>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "COMPNO",
                    "CCLINEKEY",
                    "ACTIONBY",
                    "ACTIONNAME",
                    "SS.ACTIONEDON.INT",
                    "ACTIONEDAT",
                    "ADDEDTEXT",
                    "CURRENTPERSON",
                    "CURRENTSECTION",
                    "CONTACT"
                };

                var tccaction = ce.Session.CreateUniFile("TCCACTION");

                var actionKeys = new List<string>();

                if (!String.IsNullOrEmpty(ClaimIDs))
                {
                    var claims = JsonConvert.DeserializeObject<List<CCRHead>>(ClaimIDs);

                    var claimChunks = ListExtension.Split(claims, 100);

                    claimChunks.ForEach((claimChunk) =>
                    {
                        var actionStringResult = $"LIST TCCACTION CCLINEKEY FMT\"40L\" WITH COMPNO = {String.Join(" ", claimChunk.Select(p => p.ClaimID))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var actionResult = ce.Arbitrary(actionStringResult);
                        var theseActions = actionResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        actionKeys.AddRange(theseActions);
                    });
                }
                else
                {
                    var actionList = $"LIST TCCACTION CCLINEKEY FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var actionResult = ce.Arbitrary(actionList);
                    var theseActions = actionResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    actionKeys.AddRange(theseActions);
                }

                var batches = ListExtension.Split(actionKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<TCCAction>();

                        var records = tccaction.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b];

                            theseResults.Add(new TCCAction(
                                claimId: record.Record.Extract(1).StringValue,
                                claimActionId: record.Record.Extract(2).StringValue,
                                actionedBy: record.Record.Extract(3).StringValue,
                                actionedByName: record.Record.Extract(4).StringValue,
                                actionedOn: record.Record.Extract(5).StringValue,
                                actionedAt: record.Record.Extract(6).StringValue,
                                addedText: String.Join(Environment.NewLine, record.Record.Extract(7).StringValue?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries)),
                                currentPerson: record.Record.Extract(8).StringValue,
                                currentDepartmentCode: record.Record.Extract(9).StringValue,
                                contactName: record.Record.Extract(10).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });
            }

            return new ArrayList(results);
        }

        public static void CCRActions_FillRow(
            object obj,
            out SqlString ClaimID,
            out SqlString ClaimActionID,
            out SqlString ActionedBy,
            out SqlString ActionedByName,
            out SqlDateTime ActionDT,
            out SqlString AddedText,
            out SqlString CurrentPerson,
            out SqlString CurrentDepartmentCode,
            out SqlString ContactName
            )
        {
            var result = (TCCAction)obj;

            ClaimID = String.IsNullOrEmpty(result.ClaimID) ? SqlString.Null : result.ClaimID.TrimReduceTruncate(25);

            ClaimActionID = String.IsNullOrEmpty(result.ClaimActionID) ? SqlString.Null : result.ClaimID.TrimReduceTruncate(30);

            ActionedBy = String.IsNullOrEmpty(result.ActionedBy) ? SqlString.Null : result.ActionedBy.TrimReduceTruncate(10);

            ActionedByName = String.IsNullOrEmpty(result.ActionedByName) ? SqlString.Null : result.ActionedByName.TrimReduceTruncate(50);

            ActionDT = result.ActionDT ?? SqlDateTime.Null;

            AddedText = String.IsNullOrEmpty(result.AddedText) ? SqlString.Null : result.AddedText.TrimReduceTruncate(500);

            CurrentPerson = String.IsNullOrEmpty(result.CurrentPerson) ? SqlString.Null : result.CurrentPerson.TrimReduceTruncate(10);

            CurrentDepartmentCode = String.IsNullOrEmpty(result.CurrentDepartmentCode) ? SqlString.Null : result.CurrentDepartmentCode.TrimReduceTruncate(10);

            ContactName = String.IsNullOrEmpty(result.ContactName) ? SqlString.Null : result.ContactName.TrimReduceTruncate(50);
        }


        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "Sundries_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchSundries(string UniVerseAccount, string SundryCodes)
        {
            var results = new List<EDAINVSUND>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "SUNDCD",
                    "DESC",
                    "PRODGROUP",
                    "ITEMTYPE",
                    "PROD.COLL.DES",
                    "AMT",
                    "PRICE",
                    "VAT.COUNTRY.CODE"
                };

                var invsund = ce.Session.CreateUniFile("INVSUND");

                var sundryKeys = new List<string>();

                if (!String.IsNullOrEmpty(SundryCodes))
                {
                    var sundries = JsonConvert.DeserializeObject<List<InvSund>>(SundryCodes);

                    var sundryChunks = ListExtension.Split(sundries, 100);

                    sundryChunks.ForEach((sundryChunk) =>
                    {
                        var sundryStringResult = $"LIST INVSUND SUNDCD FMT\"40L\" WITH SUNDCD = {String.Join(" ", sundryChunk.Select(p => p.SundryCode))} ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var sundryResult = ce.Arbitrary(sundryStringResult);
                        var theseSundries = sundryResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                        sundryKeys.AddRange(theseSundries);
                    });
                }
                else
                {
                    var sundryList = $"LIST INVSUND SUNDCD FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                    var sundryResult = ce.Arbitrary(sundryList);
                    var theseSundries = sundryResult
                        .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    sundryKeys.AddRange(theseSundries);
                }

                var batches = ListExtension.Split(sundryKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<EDAINVSUND>();

                        var records = invsund.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b];

                            theseResults.Add(new EDAINVSUND(
                                sundryCode: record.Record.Extract(1).StringValue,
                                description: record.Record.Extract(2).StringValue,
                                productGroup: record.Record.Extract(3).StringValue,
                                itemType: record.Record.Extract(4).StringValue,
                                collectionCode: record.Record.Extract(5).StringValue,
                                amount: record.Record.Extract(6).StringValue,
                                priceEx: record.Record.Extract(7).StringValue,
                                vatCode: record.Record.Extract(8).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });
            }

            return new ArrayList(results);
        }

        public static void Sundries_FillRow(
            object obj,
            out SqlString SundryCode,
            out SqlString Description,
            out SqlString ProductGroup,
            out SqlString ItemType,
            out SqlString CollectionCode,
            out SqlDecimal Amount,
            out SqlDecimal PriceEx,
            out SqlString VATCode)
        {

            var result = (EDAINVSUND)obj;

            SundryCode = String.IsNullOrEmpty(result.SundryCode) ? SqlString.Null : result.SundryCode.TrimReduceTruncate(50);

            Description = String.IsNullOrEmpty(result.Description) ? SqlString.Null : result.Description.TrimReduceTruncate(255);

            ProductGroup = String.IsNullOrEmpty(result.ProductGroup) ? SqlString.Null : result.ProductGroup.TrimReduceTruncate(10);

            ItemType = String.IsNullOrEmpty(result.ItemType) ? SqlString.Null : result.ItemType.TrimReduceTruncate(10);

            CollectionCode = String.IsNullOrEmpty(result.CollectionCode) ? SqlString.Null : result.CollectionCode.TrimReduceTruncate(20);

            Amount = result.Amount ?? SqlDecimal.Null;

            PriceEx = result.PriceEx ?? SqlDecimal.Null;

            VATCode = String.IsNullOrEmpty(result.VATCode) ? SqlString.Null : result.VATCode.TrimReduceTruncate(2);
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchDespatchStationsForDespatches_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchDespatchStationsForDespatches(string UniVerseAccount, string DespatchHeadIDOrderIDs)
        {
            var results = new List<OEOANALInvoiceMovement>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "USER.ID",
                    "AUDIT.DATE.INT",
                    "AUDIT.TIME"
                };

                var oeoanal = ce.Session.CreateUniFile("OEOANAL");

                var oeoanalKeys = new List<DespatchHeadIDOrderIDOEOANALKey>();

                if (String.IsNullOrEmpty(DespatchHeadIDOrderIDs))
                {
                    throw new Exception("@DespatchHeadIDs was NULL: refusing to query OEOANAL");
                }
                else
                {
                    var despatches = JsonConvert.DeserializeObject<List<DespatchHeadIDOrderID>>(DespatchHeadIDOrderIDs);

                    var despatchChunks = ListExtension.Split<DespatchHeadIDOrderID>(despatches, 10);

                    despatchChunks.ForEach((despatchChunk) =>
                    {
                        var orderIds = $"ORDER.NO = {String.Join(" ", despatchChunk.Select(despatch => despatch.OrderID))}";
                        var pickLists = $"PICK.LIST = {String.Join(" ", despatchChunk.Select(despatch => despatch.DespatchHeadID))}";

                        var despatchStringResult = $"LIST OEOANAL CODE FMT\"14L\" ORDER.NO FMT\"18L\" PICK.LIST FMT\"18L\" WITH {orderIds} AND {pickLists} AND MOVE.TYPE = PIK ID.SUPP COL.HDR.SUPP COUNT.SUP";
                        var despatchResult = ce.Arbitrary(despatchStringResult);
                        var theseDespatches = despatchResult
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList()
                            .Select(oeo =>
                            {
                                var p = oeo.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                return new DespatchHeadIDOrderIDOEOANALKey()
                                {
                                    OEOANALKey = p[0],
                                    OrderID = p[1],
                                    DespatchHeadID = p[2],
                                };
                            })
                            .ToList();
                        oeoanalKeys.AddRange(theseDespatches);
                    });
                }

                var batches = ListExtension.Split(oeoanalKeys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<OEOANALInvoiceMovement>();

                        var records = oeoanal.ReadRecords(batch.Select(p => p.OEOANALKey).ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b.OEOANALKey];

                            theseResults.Add(new OEOANALInvoiceMovement(
                                despatchHeadId: b.DespatchHeadID,
                                orderId: b.OrderID,
                                oeoanalKey: b.OEOANALKey,
                                despatchStation: record.Record.Extract(1).StringValue,
                                movementDate: record.Record.Extract(2).StringValue,
                                movementTime: record.Record.Extract(3).StringValue
                            ));
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });

                results = results.GroupBy(p => p.DespatchHeadID).Select(p => p.First()).ToList();
            }


            return new ArrayList(results);
        }

        public static void FetchDespatchStationsForDespatches_FillRow(
            object obj,
            out SqlString DespatchHeadID,
            out SqlString OrderID,
            out SqlString OEOANALKey,
            out SqlString DespatchStation,
            out SqlDateTime MovementDT)
        {

            var result = (OEOANALInvoiceMovement)obj;

            DespatchHeadID = String.IsNullOrEmpty(result.DespatchHeadID) ? SqlString.Null : result.DespatchHeadID.TrimReduceTruncate(25);

            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);

            OEOANALKey = String.IsNullOrEmpty(result.OEOANALKey) ? SqlString.Null : result.OEOANALKey.TrimReduceTruncate(25);

            DespatchStation = String.IsNullOrEmpty(result.DespatchStation) ? SqlString.Null : result.DespatchStation.TrimReduceTruncate(20);

            MovementDT = result.MovementDT ?? SqlDateTime.Null;
        }

        public static string JohnLewisProductReference(string UniVerseAccount, string JohnLewisOrderReference, short LineNumber)
        {
            var result = (string)null;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var orderIds = $@"";

                var headerKeyResult = $@"LIST EDIHEADER KEY FMT""30L"" WITH CUS.ORD.NUM = ""{JohnLewisOrderReference}"" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var headerKey = ce.Arbitrary(headerKeyResult)
                            .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => $"{p}*{LineNumber}")
                            .ToArray();

                if (headerKey != null)
                {
                    string[] fields = new string[] {
                        "CUSPRODCODE"
                    };

                    var ediOrders = ce.Session.CreateUniFile("EDIORDERS");

                    try
                    {
                        var records = ediOrders.ReadRecords(headerKey, fields);

                        result = records.GetRecord(headerKey[0]).Record.Extract(1).StringValue;
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                }
            }

            return result;
        }



        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "PriceCrosses_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchPriceCrosses(string UniVerseAccount)
        {
            var results = new List<PRICECROSS>();

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                string[] fields = new string[] {
                    "SS.SELLPRICE",
                    "SS.RETPRICE"
                };

                var invsund = ce.Session.CreateUniFile("SS.PRICE.CROSS");

                var keys = new List<string>();

                var listCmd = $"LIST SS.PRICE.CROSS KEY FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var listResult = ce.Arbitrary(listCmd);
                var theseKeys = listResult
                    .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                keys.AddRange(theseKeys);

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<PRICECROSS>();

                        var records = invsund.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var record = records[b];

                            var sellingPriceString = record.Record.Extract(1).StringValue;
                            
                            var retailPriceString = record.Record.Extract(2).StringValue;

                            var tradePriceListCodes = sellingPriceString.Split(Framework.ValueMarkDelimiter, StringSplitOptions.None).ToList();
                            
                            var rrpPriceListCodes = retailPriceString.Split(Framework.ValueMarkDelimiter, StringSplitOptions.None).ToList();

                            if (rrpPriceListCodes.Count() < tradePriceListCodes.Count())
                            {
                                for (var i = rrpPriceListCodes.Count(); i < tradePriceListCodes.Count(); i++)
                                {
                                    rrpPriceListCodes.Add(null);
                                }
                            }

                            for (var i = 0; i < tradePriceListCodes.Count(); i++)
                            {
                                theseResults.Add(new PRICECROSS(tradePriceListCodes[i], rrpPriceListCodes[i]));
                            }
                        }

                        lock (locker)
                        {
                            results.AddRange(theseResults);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {

                    }
                });
            }

            return new ArrayList(results);
        }

        public static void PriceCrosses_FillRow(
            object obj,
            out SqlString TradePriceListCode,
            out SqlString RRPPriceListCode)
        {
            var result = (PRICECROSS)obj;

            TradePriceListCode = String.IsNullOrEmpty(result.TradePriceListCode)
                ? SqlString.Null
                : result.TradePriceListCode.TrimReduceTruncate(10);

            RRPPriceListCode = String.IsNullOrEmpty(result.RRPPriceListCode)
                ? SqlString.Null 
                : result.RRPPriceListCode.TrimReduceTruncate(10);
        }
    }
}