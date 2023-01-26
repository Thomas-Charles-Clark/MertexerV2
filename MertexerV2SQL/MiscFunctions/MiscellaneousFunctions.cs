using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using MertexerV2SQL.Models.Misc;
using MertexerV2SQL.Models.Stock;
using MertexerV2SQL.Models.Tracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using U2.Data.Client;

namespace MertexerV2SQL.MiscFunctions
{
    public static class MiscellaneousFunctions
    {
        public static List<(string ccr, string label)> CCRAutocomplete(
            string tradeDataSourceJson,
            PassAutocompleteTerm objTerm)
        {
            var results = new List<(string ccr, string label)>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var ce = new CommandExecutor(dataSource.UniVerseAccount))
                {
                    var selectionCriteria = "";

                    if (!String.IsNullOrEmpty(objTerm.term))
                    {
                        selectionCriteria = $"AND ID LIKE \"{objTerm.term}...\" OR SORDERNO LIKE \"{objTerm.term}...\"";
                    }

                    var preResults = ce.Arbitrary($"LIST TCCHEAD ID SORDERNO WITH CUSREF = \"{dataSource.CustomerReference}\" {selectionCriteria} BY-DSND ID SAMPLE 100 ID.SUP COL.HDR.SUPP COUNT.SUP")
                        .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var r = new List<(string ccr, string label)>();

                    preResults.ForEach(c =>
                    {
                        var parts = c.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                        {
                            r.Add((ccr: parts[0], label: $"CCR: {parts[0]} - Sales Order Number: {parts[1]} - Customer: {dataSource.CustomerReference} - UniVerse Account: {dataSource.UniVerseAccount}"));
                        }
                        else
                        {
                            r.Add((ccr: parts[0], label: $"CCR: {parts[0]} - Customer: {dataSource.CustomerReference} - UniVerse Account: {dataSource.UniVerseAccount}"));
                        }
                    });

                    lock (locker)
                    {
                        results.AddRange(r);
                    }
                }
            });

            return results;
        }

        public static List<string> OrderReferenceUniqueCheck(
            string tradeDataSourceJson,
            PassOrderReference objOrderReference)
        {
            var results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                var r = new List<string>();

                using (var ce = new CommandExecutor(dataSource.UniVerseAccount))
                {
                    var command = $"LIST TOEHEAD @ID DATEONFILE WITH CUSREF = \"{dataSource.CustomerReference}\" AND DATEONFILE.INT >= {Framework.UniVerseDateString(DateTime.Now.AddMonths(-3))} AND SS.CUSORDNO.MCL = \"[{objOrderReference.OrderReference?.Trim()?.ToLowerInvariant()}]\" BY.DSND ID ID.SUP COL.HDR.SUPP COUNT.SUP";

                    var preResult = ce.Arbitrary(command);

                    r = preResult
                        .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                r.RemoveAll(p => p.StartsWith("OCONV"));

                var enUS = new CultureInfo("en-US");

                for (var i = 0; i < r.Count; i++)
                {
                    var t = r[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (t.Length == 2)
                    {
                        if (DateTime.TryParseExact(t[1], "MM/dd/yy", enUS, DateTimeStyles.None, out DateTime orderDate))
                        {
                            t[1] = orderDate.ToString("dd/MM/yy");
                        }
                    }
                    r[i] = String.Join(" - placed on ", t);
                };

                lock (locker)
                {
                    results.AddRange(r);
                }
            });

            return results;
        }

        public static bool WaterfallCheck(string code)
        {
            var result = MertexerV2.SQLFunctions.Stock.B2CEnquiry_Singular(code, "HD").OfType<MertexerV2.Models.Stock.B2CEnquiryResult>().ToList();

            if (result.Any())
            {
                if (result[0].TotalPOQuantity > 0 || result[0].TotalFreeStock > 0)
                    return true;
                else
                    return DiscoCheck(code);
            }
            else
            {
                return DiscoCheck(code);
            }
        }

        public static bool DiscoCheck(string code)
        {
            var result = false;

            using (var ce = new CommandExecutor())
            {
                var arrayResult = ce.Arbitrary($"LIST CUR.STAT STPRODMASTER \"{code}\" ID.SUP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                if (arrayResult.Count == 1)
                {
                    if (arrayResult[0].Contains("not found"))
                    {
                        return false;
                    }

                    return arrayResult[0].Trim() != "D";
                }
            }

            return result;
        }

        public static DuplicateLineCheckResult LineDuplicateCheck(
            string tradeDataSourceJson,
            PassDuplicateLineCheck objCheck)
        {
            var result = new DuplicateLineCheckResult();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var ce = new CommandExecutor(dataSource.UniVerseAccount))
                {
                    var queryString = $"LIST TOELINE WITH CUSREF = \"{dataSource.CustomerReference}\" AND PRODCODE = \"{objCheck.productCode}\" AND SS.CUS.ORD.REF.MCU = \"{objCheck.lineReference?.Trim()?.ToUpperInvariant()}\" AND QTYORD.STK = {objCheck.quantity:N2} AND (QTYCAN.ORD = \"\" OR QTYCAN.ORD = 0) AND DATEONFILE.INT >= {Framework.UniVerseDateString(DateTime.Now.AddMonths(-3))} COL.HDR.SUPP COUNT.SUP";

                    var arrayResult = ce.Arbitrary(queryString)
                        .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    lock (locker)
                    {
                        result.orderNumbers.AddRange(arrayResult);
                    }
                }
            });

            if (result.orderNumbers.Any())
                result.isDuplicate = true;

            return result;
        }

        public static List<string> EndCustomerList(
            string tradeDataSourceJson,
            PassAutocompleteTerm objTerm)
        {
            var preResult = new List<Tuple<string, string>>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.PlaceOrder) // I have no idea what EnquiryType we'll want for this...
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xmlResult = qe.Run("LIST_END_CUSTOMERS", $"CUSREF=\"{dataSource.CustomerReference}\"", "CUSREF CUSNAME", "");

                    var xDoc = XDocument.Parse(xmlResult);

                    var pr = new List<Tuple<string, string>>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        var cusRef = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                        var cusName = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;

                        pr.Add(new Tuple<string, string>(cusRef, cusName));
                    });

                    lock (locker)
                    {
                        preResult.AddRange(pr);
                    }
                }
            });

            var result = new List<string>();

            if (!String.IsNullOrWhiteSpace(objTerm.term))
            {
                result = preResult.Where(p => (p.Item1.StartsWith(objTerm.term)) || p.Item2.Contains(objTerm.term)).Select(p => p.Item1).ToList();
            }
            else
            {
                result = preResult.Select(p => p.Item1).ToList();
            }

            return result;
        }

        public static List<(string code, string label)> HeldForSalesReasonCode(PassAutocompleteTerm objTerm)
        {
            var results = new List<(string code, string label)>();

            using (var ce = new CommandExecutor())
            {
                ce.Arbitrary("SET.TERM.TYPE wyse60 WIDTH 421 LENGTH 42");
                ce.Arbitrary("CASE OFF");
                var preResults = ce.Arbitrary($"LIST STREASON CODE FMT \"20T\" DESCRIPTION FMT \"400T\" WITH CODE LIKE \"{objTerm.term}...\" AND (CODE LIKE \"CUS...\" OR CODE LIKE \"PO...\") BY CODE ID.SUP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { "\r\n", "\u001b;" }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                preResults.ForEach(c =>
                {
                    var parts = c.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    results.Add((code: parts[0], label: $"{parts[0]}  -  {String.Join(" ", parts.Skip(1))}"));
                });
            }
            return results;
        }

        public static IEnumerable<(string supplierCode, string supplierName, MiscellaneousInvoiceAddress address)> SupplierList(PassProductCode objProductCode)
        {
            var result = new List<(string supplierCode, string supplierName, MiscellaneousInvoiceAddress address)>();

            var suppliers = new List<string>();

            using (var ce = new CommandExecutor())
            {
                suppliers = ce.Arbitrary($"LIST SUPPREF STPRODMASTER \"{objProductCode.code}\" ID.SUP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .ToList();

                var r = new List<(string supplierCode, string supplierName, MiscellaneousInvoiceAddress address)>();

                suppliers.ForEach(supplier => {

                    U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
                    conn_str.UserID = MertexConnectionDetails.UserID;
                    conn_str.Password = MertexConnectionDetails.Password;
                    conn_str.Server = MertexConnectionDetails.Server;
                    conn_str.Database = MertexConnectionDetails.Database;
                    conn_str.ServerType = MertexConnectionDetails.ServerType;

                    using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                    {
                        U2Command comm = conn.CreateCommand();

                        comm.CommandText = @"
                        SELECT
                            SUPNAME,
                            SUPNAME,
                            SUPADD1,
                            SUPADD2,
                            SUPADD3,
                            SUPADD4,
                            SUPPCODE,
                            STD.TELEPHONE,
                            EMAIL,
                            COUNTRY.CODE
                        FROM 
                            PLSUP
                        WHERE
                            PLSUP.@ID = @SupplierCode
                        NOWAIT";

                        comm.Parameters.Add("@SupplierCode", supplier);

                        conn.Open();

                        var reader = comm.ExecuteReader();

                        while (reader.Read())
                        {
                            result.Add((supplier, reader.GetString(0), new MiscellaneousInvoiceAddress()
                            {
                                CompanyName = reader.IsDBNull(0) ? null : reader.GetString(0).Trim(),
                                FullName = reader.IsDBNull(1) ? null : reader.GetString(1).Trim(),
                                AddressLine1 = reader.IsDBNull(2) ? null : reader.GetString(2).Trim(),
                                AddressLine2 = reader.IsDBNull(3) ? null : reader.GetString(3).Trim(),
                                Town = reader.IsDBNull(4) ? null : reader.GetString(4).Trim(),
                                County = reader.IsDBNull(5) ? null : reader.GetString(5).Trim(),
                                Postcode = reader.IsDBNull(6) ? null : reader.GetString(6).Trim(),
                                PhoneNumber = reader.IsDBNull(7) ? null : reader.GetString(7).Trim(),
                                Email = reader.IsDBNull(8) ? null : reader.GetString(8).Trim(),
                                PotentialCountryCode = reader.IsDBNull(9) ? null : reader.GetString(9).Trim()
                            }));
                        }

                        reader.Close();

                        conn.Close();
                    }
                });
            }

            return result;
        }

        public static List<string> Exists(
            string tradeDataSourceJson,
            MertexerV2.Models.DeliveryAddress objAddress)
        {
            List<string> results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                SELECT
                    DELREF
                FROM 
                    DELADD
                WHERE
                    CUSREF = @TradeAccountCode AND
                    CONTACT = @ContactName AND
                    DELADDR1 = @AddressLine1 AND
                    DELADDR2 = @AddressLine2 AND
                    DELADDR3 = @Town AND
                    DELADDR4 = @County AND
                    DELNAME = @CompanyName AND
                    DELPCODE = @PostCode AND
                    COUNTRY = @CountryCode
                ";

                comm.Parameters.Add("@TradeAccountCode", objAddress.TradeAccountCode);
                comm.Parameters.Add("@CompanyName", objAddress.CompanyName);
                comm.Parameters.Add("@ContactName", objAddress.ContactName);
                comm.Parameters.Add("@AddressLine1", objAddress.AddressLine1);
                comm.Parameters.Add("@AddressLine2", objAddress.AddressLine2);
                comm.Parameters.Add("@Town", objAddress.Town);
                comm.Parameters.Add("@County", objAddress.County);
                comm.Parameters.Add("@PostCode", objAddress.Postcode);
                comm.Parameters.Add("@CountryCode", objAddress.CountryCode);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(reader.GetString(0));
                }

                reader.Close();

                conn.Close();
            }

            return results;
        }

        public static List<MertexPCALookupResult> AddressBookSearch(
            string tradeDataSourceJson,
            PassPCAPredictFind objDetail)
        {
            var result = new List<MertexPCALookupResult>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                if (String.IsNullOrEmpty(objDetail.text))
                {
                    comm.CommandText = @"
                SELECT
                    DELNAME,
                    CONTACT,
                    DELREF,
                    DELADDR1,
                    DELADDR2,
                    DELADDR3,
                    DELADDR4,
                    DELPCODE,
                    PHONE,
                    [E.MAIL],
                    COUNTRY
                FROM 
                    DELADD
                WHERE" 
                + 
                (String.IsNullOrEmpty(objDetail.endCustomerTradeAccountCode) 
                    ? @"(CUSREF = @TradeAccountCode)"
                    : @"(CUSREF = @TradeAccountCode OR CUSREF = @EndCustomerTradeAccountCode)")
                       
                + @"
                ORDER BY DELREF
                SAMPLE 100";

                    comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                    if (!String.IsNullOrEmpty(objDetail.endCustomerTradeAccountCode))
                    {
                        comm.Parameters.Add("EndCustomerTradeAccountCode", objDetail.endCustomerTradeAccountCode);
                    }
                }
                else
                {
                    comm.CommandText = @"
                SELECT
                    DELNAME,
                    CONTACT,
                    DELREF,
                    DELADDR1,
                    DELADDR2,
                    DELADDR3,
                    DELADDR4,
                    DELPCODE,
                    PHONE,
                    [E.MAIL],
                    COUNTRY
                FROM 
                    DELADD
                WHERE"
                + 
                (String.IsNullOrEmpty(objDetail.endCustomerTradeAccountCode) 
                    ? @"(CUSREF = @TradeAccountCode)"
                    : @"(CUSREF = @TradeAccountCode OR CUSREF = @EndCustomerTradeAccountCode)")
                       
                +@"
                    AND
                    (
                        LOWER(DELREF) LIKE @SearchTerm OR
                        LOWER(CONTACT) LIKE @SearchTerm OR
                        LOWER(DELADDR1) LIKE @SearchTerm OR
                        LOWER(DELADDR2) LIKE @SearchTerm OR
                        LOWER(DELADDR3) LIKE @SearchTerm OR
                        LOWER(DELADDR4) LIKE @SearchTerm OR
                        LOWER(DELNAME) LIKE @SearchTerm OR
                        LOWER(DELPCODE) LIKE @PostCode
                    )
                ORDER BY DELREF
                SAMPLE 100";

                    comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                    if (!String.IsNullOrEmpty(objDetail.endCustomerTradeAccountCode))
                    {
                        comm.Parameters.Add("EndCustomerTradeAccountCode", objDetail.endCustomerTradeAccountCode);
                    }

                    comm.Parameters.Add("@SearchTerm", String.IsNullOrEmpty(objDetail.text) ? "" : $"%{objDetail.text?.ToLower()}%");
                    comm.Parameters.Add("@PostCode", String.IsNullOrEmpty(objDetail.text) ? "" : $"%{objDetail.text?.ToLower()}%");
                }

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new MertexPCALookupResult()
                    {
                        Company = reader.IsDBNull(0) ? null : reader.GetString(0).Trim(),
                        Name = reader.IsDBNull(1) ? null : reader.GetString(1).Trim(),
                        DeliveryReference = reader.IsDBNull(2) ? null : reader.GetString(2).Trim(),
                        Line1 = reader.IsDBNull(3) ? null : reader.GetString(3).Trim(),
                        Line2 = reader.IsDBNull(4) ? null : reader.GetString(4).Trim(),
                        City = reader.IsDBNull(5) ? null : reader.GetString(5).Trim(),
                        Province = reader.IsDBNull(6) ? null : reader.GetString(6).Trim(),
                        PostalCode = reader.IsDBNull(7) ? null : reader.GetString(7).Trim(),
                        Phone = reader.IsDBNull(8) ? null : reader.GetString(8).Trim(),
                        Email = reader.IsDBNull(9) ? null : reader.GetString(9).Trim(),
                        Country = reader.IsDBNull(10) ? null : reader.GetString(10).Trim()
                    });
                }

                // This does not work for End Customer address fetching.
                // This is because... End Customer accounts actually aren't End Customers, they're "possessors" of End Customers.
                // Therefore when you look up M2830, the AddressBook is full of references to OTHER accounts.
                // Their DELREF values will be e.g. M8430, M8424-1, M4304-2, etc.
                // This ruins our attempt to strip out the first half of the delref and sort by the #.
                // So... what could we reasonably do in this instance?

                // If, after the first attempt at string manipulation, we've still got something over 3 characters in length
                // it's *very* unlikely that a *normal* account will have >= 100  addresses...

                // So we might be fine to test for > 3 length, and then split(new char[] { '-' })[0].Replace

                reader.Close();

                conn.Close();

                result.ForEach(c =>
                {
                    c.TradeAccountCode = dataSource.CustomerReference;

                    //c.Country = objDetail.countries;

                    var delRef = Regex.Replace(c.DeliveryReference?.Replace($"{dataSource.CustomerReference}-", "") ?? "", "[^0-9\\-]*", "");

                    if (delRef.Length >= 3)
                    {
                        delRef = Regex.Replace(delRef.Split(new char[] { '-' })[0], "[^0-9]*", "");
                    }

                    if (String.IsNullOrWhiteSpace(delRef))
                    {
                        delRef = "0";
                    }

                    c.DeliveryReferenceID = Convert.ToInt32(delRef);
                });
            }

            result = result.OrderBy(c =>
                c.DeliveryReferenceID
            ).ToList();

            return result;
        }

        public static TradeAccountAddressReturn FetchTradeAccountAddresses(
            string tradeDataSourceJson,
            string tradeAccountCode)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            string defaultDeliveryAddressReference = null;

            var result = new TradeAccountAddressReturn();

            var invoiceAddressResult = new TradeAccountInvoiceAddress();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        SS.CONTACT.NAME,
                        CUSADD1X,
                        CUSADD2X,
                        CUSADD3X,
                        CUSADD4X,
                        CUSPCODE,
                        COUNTRY,
                        [DEF.DELREF],
                        EMAIL,
                        STD.TELEPHONE,
                        CUSNAMEX
                    FROM 
                        SLCUST
                    WHERE
                        CUSREF = @TradeAccountCode
                    NOWAIT
                    ";

                comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    invoiceAddressResult.Name = reader.IsDBNull(0) ? null : reader.GetString(0);
                    invoiceAddressResult.AddressLine1 = reader.IsDBNull(1) ? null : reader.GetString(1);
                    invoiceAddressResult.AddressLine2 = reader.IsDBNull(2) ? null : reader.GetString(2);
                    invoiceAddressResult.AddressLine3 = reader.IsDBNull(3) ? null : reader.GetString(3);
                    invoiceAddressResult.AddressLine4 = reader.IsDBNull(4) ? null : reader.GetString(4);
                    invoiceAddressResult.Postcode = reader.IsDBNull(5) ? null : reader.GetString(5);
                    invoiceAddressResult.CountryCode = reader.IsDBNull(6) ? null : reader.GetString(6);
                    defaultDeliveryAddressReference = reader.IsDBNull(7) ? null : reader.GetString(7);
                    invoiceAddressResult.Email = reader.IsDBNull(8) ? null : reader.GetString(8);
                    invoiceAddressResult.Phone = reader.IsDBNull(9) ? null : reader.GetString(9);
                    invoiceAddressResult.CompanyName = reader.IsDBNull(10) ? null : reader.GetString(10);
                }

                reader.Close();

                conn.Close();
            }

            invoiceAddressResult.DeliveryReference = "C";

            result.InvoiceAddress = invoiceAddressResult;

            var completeAddressBook = new List<MertexPCALookupResult>();

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        DELNAME,
                        CONTACT,
                        DELREF,
                        DELADDR1,
                        DELADDR2,
                        DELADDR3,
                        DELADDR4,
                        DELPCODE,
                        COUNTRY,
                        PHONE,
                        [E.MAIL]
                    FROM 
                        DELADD
                    WHERE
                        CUSREF = @TradeAccountCode
                    ";

                comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    completeAddressBook.Add(new MertexPCALookupResult()
                    {
                        Company = reader.IsDBNull(0) ? null : reader.GetString(0),
                        Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                        DeliveryReference = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Line1 = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Line2 = reader.IsDBNull(4) ? null : reader.GetString(4),
                        City = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Province = reader.IsDBNull(6) ? null : reader.GetString(6),
                        PostalCode = reader.IsDBNull(7) ? null : reader.GetString(7),
                        Country = reader.IsDBNull(8) ? null : reader.GetString(8),
                        Phone = reader.IsDBNull(9) ? null : reader.GetString(9),
                        Email = reader.IsDBNull(10) ? null : reader.GetString(10)
                    });
                }

                // This does not work for End Customer address fetching.
                // This is because... End Customer accounts actually aren't End Customers, they're "possessors" of End Customers.
                // Therefore when you look up M2830, the AddressBook is full of references to OTHER accounts.
                // Their DELREF values will be e.g. M8430, M8424-1, M4304-2, etc.
                // This ruins our attempt to strip out the first half of the delref and sort by the #.
                // So... what could we reasonably do in this instance?

                // If, after the first attempt at string manipulation, we've still got something over 3 characters in length
                // it's *very* unlikely that a *normal* account will have >= 100  addresses...

                // So we might be fine to test for > 3 length, and then split(new char[] { '-' })[0].Replace

                reader.Close();

                conn.Close();

                completeAddressBook.ForEach(c =>
                {
                    try
                    {
                        var delRef = Regex.Replace(c.DeliveryReference?.Replace($"{dataSource.CustomerReference}-", "") ?? "", "[^0-9\\-]*", "");

                        if (delRef.Length >= 3)
                        {
                            delRef = Regex.Replace(delRef.Split(new char[] { '-' })[0], "[^0-9]*", "");
                        }

                        if (String.IsNullOrWhiteSpace(delRef))
                        {
                            delRef = "0";
                        }

                        c.DeliveryReferenceID = Convert.ToInt32(delRef);
                    }
                    catch (FormatException)
                    {
                        c.DeliveryReferenceID = 0;
                    }
                });
            }

            completeAddressBook = completeAddressBook.OrderBy(c =>
                c.DeliveryReferenceID
            ).ToList();

            // this is wrong
            // we want DEF.DELREF from SLCUST...

            var firstByDefault = completeAddressBook.Where(p => p.DeliveryReference == defaultDeliveryAddressReference).SingleOrDefault();

            if (firstByDefault != null)
            {
                result.PrimaryAddress = new TradeAccountPrimaryAddress()
                {
                    DeliveryReference = firstByDefault.DeliveryReference,
                    CompanyName = firstByDefault.Company,
                    Name = firstByDefault.Name,
                    AddressLine1 = firstByDefault.Line1,
                    AddressLine2 = firstByDefault.Line2,
                    AddressLine3 = firstByDefault.City,
                    AddressLine4 = firstByDefault.Province,
                    Postcode = firstByDefault.PostalCode,
                    CountryCode = firstByDefault.Country
                };
            }
            else
            {
                result.PrimaryAddress = null;
            }

            return result;
        }

        public static TradeAccountInvoiceAddress RefreshTradeAccountInvoiceAddress(
            string tradeDataSourceJson,
            PassTradeAccountNumber objTradeAccountNumber)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            var result = new TradeAccountInvoiceAddress();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        CUSNAME,
                        CUSADD1X,
                        CUSADD2X,
                        CUSADD3X,
                        CUSADD4X,
                        CUSPCODE,
                        COUNTRY
                    FROM 
                        SLCUST
                    WHERE
                        CUSREF = @TradeAccountCode
                    NOWAIT
                    ";

                comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    result.Name = reader.IsDBNull(0) ? null : reader.GetString(0);
                    result.AddressLine1 = reader.IsDBNull(1) ? null : reader.GetString(1);
                    result.AddressLine2 = reader.IsDBNull(2) ? null : reader.GetString(2);
                    result.AddressLine3 = reader.IsDBNull(3) ? null : reader.GetString(3);
                    result.AddressLine4 = reader.IsDBNull(4) ? null : reader.GetString(4);
                    result.Postcode = reader.IsDBNull(5) ? null : reader.GetString(5);
                    result.CountryCode = reader.IsDBNull(6) ? null : reader.GetString(6);
                }

                reader.Close();

                conn.Close();
            }

            return result;
        }

        public static TradeAccountInvoiceTerms InvoiceTermsLookup(
            string tradeDataSourceJson,
            PassTradeAccountNumber objTradeAccountNumber)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Account)
                                        .Single()
                                        .DataSources
                                        .Single();
           
            var result = new TradeAccountInvoiceTerms();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        STTERMS
                    FROM 
                        SLCUST
                    WHERE
                        CUSREF = @TradeAccountCode";

                comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    result.Code = reader.IsDBNull(0) ? null : reader.GetString(0);
                }

                reader.Close();

                conn.Close();
            }

            return result;
        }



        public static string RefreshTradeAccountInvoiceEmailAddress(
            string tradeDataSourceJson,
            PassTradeAccountNumber objTradeAccountNumber)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Account)
                                        .Single()
                                        .DataSources
                                        .Single();

            var result = "";

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        EMAIL
                    FROM 
                        SLCUST
                    WHERE
                        CUSREF = @TradeAccountCode
                    NOWAIT
                    ";

                comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    result = reader.IsDBNull(0) ? null : reader.GetString(0);
                }

                reader.Close();

                conn.Close();
            }

            return result;
        }

        public static TradeAccountVATNumber VATNumberLookup(
            string tradeDataSourceJson,
            PassTradeAccountNumber objTradeAccountNumber)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Account)
                                        .Single()
                                        .DataSources
                                        .Single();

            var result = new TradeAccountVATNumber();

            try
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
                conn_str.UserID = MertexConnectionDetails.UserID;
                conn_str.Password = MertexConnectionDetails.Password;
                conn_str.Server = MertexConnectionDetails.Server;
                conn_str.Database = dataSource.UniVerseAccount;
                conn_str.ServerType = MertexConnectionDetails.ServerType;

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command comm = conn.CreateCommand();

                    comm.CommandTimeout = 2;

                    comm.CommandText = @"
                    SELECT
                        VATCOUNTRY,
                        INVVATDISPLAY
                    FROM 
                        SLCUST
                    WHERE
                        CUSREF = @TradeAccountCode";

                    comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                    conn.Open();

                    var reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        result.CountryCode = reader.IsDBNull(0) ? null : reader.GetString(0);
                        result.VATNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
                    }

                    reader.Close();

                    conn.Close();
                }
            }
            catch (Exception)
            {
                result.Errored = true;
            }

            return result;
        }

        public static void FetchDefaultDeliveryReference(
            string tradeDataSourceJson,
            string tradeAccountCode, 
            string deliveryReference)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Account)
                                        .Single()
                                        .DataSources
                                        .Single();

            string currentDefault = null;

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = dataSource.UniVerseAccount;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                SELECT
                    DEF.DELREF
                FROM 
                    SLCUST
                WHERE
                    CUSREF = @TradeAccountCode";

                comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);

                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        currentDefault = reader.GetString(0);
                    }
                }

                reader.Close();
            }
        }

        public static FetchLinkedDocumentsResult FetchLinkedDocuments(
            PassProductCode objProductCode)
        {
            var result = new FetchLinkedDocumentsResult();

            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = MertexConnectionDetails.Database;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        LINK.DOCS,
                        LINK.DOCS.DESC
                    FROM 
                        STPRODMASTER
                    WHERE
                        PRODCODE = @ProductCode";

                comm.Parameters.Add("@ProductCode", objProductCode.code);

                conn.Open();

                var reader = comm.ExecuteReader();

                var pathResults = "";
                var descriptionResults = "";

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        pathResults = reader.GetString(0);
                        descriptionResults = reader.GetString(1);
                    }
                }

                var pathSplits = pathResults.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var descriptionSplits = descriptionResults.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < pathSplits.Length; i++)
                {
                    result.LinkedDocuments.Add(new LinkedDocument()
                    {
                        Path = pathSplits[i],
                        Description = descriptionSplits[i]
                    });
                }

                reader.Close();

                conn.Close();
            }

            return result;
        }

        public static List<MertexRetailTill> FetchRetailTills()
        {
            var results = new List<MertexRetailTill>();

            using (var sql = new MertexerV2SQL.DAL.MertexSQLConnection())
            {
                var conn = sql.Connection;

                var selectComm = conn.CreateCommand();

                selectComm.CommandText = $@"SELECT  A.[TILL.NO],
                                                    A.[DESCRIPTION]
                                            FROM OETILLNO AS A
                                            INNER JOIN OESESSION AS B ON B.TILLNO = A.[TILL.NO]
                                            WHERE B.[OPEN.FLAG] = 'Y' AND B.[OPENED.ON] > @StartDT
                                            ORDER BY A.[TILL.NO]";

                var selectParams = new List<U2Parameter>()
                {
                    new U2Parameter("@StartDT", DateTime.Now.AddMonths(-1))
                };

                selectComm.Parameters.AddRange(selectParams.ToArray());

                conn.Open();

                var reader = selectComm.ExecuteReader();

                while (reader.Read())
                {
                    var sr = new MertexRetailTill()
                    {
                        TillNumber = reader.GetString(0),
                        Name = reader.GetString(1)
                    };

                    results.Add(sr);
                }

                reader.Close();

                conn.Close();
            }

            return results;
        }

        public static List<MertexRetailTillSession> FetchRetailTillSessions(PassRetailTillNumber objTill)
        {
            var results = new List<MertexRetailTillSession>();

            using (var sql = new MertexerV2SQL.DAL.MertexSQLConnection())
            {
                var conn = sql.Connection;

                var selectComm = conn.CreateCommand();

                selectComm.CommandText = $@"SELECT  SESSIONNO,
                                                    [OPENED.ON],
                                                    [OPENED.AT],
                                                    [OPENED.BY]
                                            FROM OESESSION 
                                            WHERE [OPEN.FLAG] = 'Y' AND [TILLNO] = @TillNumber
                                            ORDER BY [OPENED.ON] DESC, [OPENED.AT] DESC";

                var selectParams = new List<U2Parameter>()
                {
                    new U2Parameter("@TillNumber", objTill.retailTillNumber)
                };

                selectComm.Parameters.AddRange(selectParams.ToArray());

                conn.Open();

                var reader = selectComm.ExecuteReader();

                while (reader.Read())
                {
                    var sr = new MertexRetailTillSession()
                    {
                        SessionNumber = reader.GetString(0),
                        OpenedOn = reader.GetDateTime(1),
                        OpenedAt = reader.GetTimeSpan(2),
                        OpenedBy = reader.GetString(3)
                    };

                    sr.OpenedOn += sr.OpenedAt;

                    results.Add(sr);
                }

                reader.Close();

                conn.Close();
            }

            results = results.Where(p => p.OpenedOn > DateTime.Now.AddMonths(-1)).ToList();

            return results;
        }

        public static MertexerV2.Models.Accounts.OutstandingOrdersReturn OutstandingOrders(
            string tradeDataSourceJson,
            MertexerV2.Models.Accounts.PassOutstandingOrdersSearch objSearch
        )
        {
            var locker = new object();

            var result = new MertexerV2.Models.Accounts.OutstandingOrdersReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);


            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var uvFileName = "LIST_ORDER_LINES";

                    var cusrefType = "";

                    var sampleOrderInclusion = "";

                    var dateInclusion = "";

                    var selectionCriteria = "";

                    var ordering = "";

                    var searchTermInclusion = "";

                    if (!String.IsNullOrEmpty(objSearch.searchTerm))
                    {
                        searchTermInclusion = $" AND ((ORDNO = '{objSearch.searchTerm.ToUpperInvariant()}') OR (WI.STDPROD = '[{objSearch.searchTerm.ToUpperInvariant()}]') OR (SS.CUS.ORD.REF.LWR = '[{objSearch.searchTerm.ToLowerInvariant()}]')) ";
                    }

                    switch (objSearch.orderStatus)
                    {
                        case "outstanding":
                            cusrefType = "CUSREF.LIVE";
                            ordering = " BY ORDNO ";
                            break;
                        case "backOrders":
                            cusrefType = "CUSREF.LIVE";
                            selectionCriteria = " AND BO.IND = 'Y' AND RESERVE.FLAG = '' AND MBATCH.NO1 = '' AND WI.HOLDORDER # 'Y' ";
                            ordering = " BY DATEONFILE BY ORDNO ";
                            break;
                        case "reserveOrders":
                            cusrefType = "CUSREF.LIVE";
                            selectionCriteria = " AND BO.IND = 'Y' AND RESERVE.FLAG # '' ";
                            ordering = " BY DATEONFILE BY ORDNO BY WI.RESERVED.UNTIL BY ORDNO ";
                            break;
                        case "despatchedOrders":
                            cusrefType = "CUSREF.RECDESP";
                            selectionCriteria = "  ";
                            ordering = " BY WI.DESP.STATUS.DATE BY ORDNO ";
                            break;
                        case "ordersInProgress":
                            cusrefType = "CUSREF.LIVE";
                            selectionCriteria = " AND STATUS = '3' AND WI.HOLDORDER # 'Y' ";
                            ordering = " BY DATEONFILE BY ORDNO ";
                            break;
                        case "held":
                            cusrefType = "CUSREF.LIVE";
                            selectionCriteria = " AND WI.HOLDORDER = 'Y' ";
                            ordering = " BY ORDNO ";
                            break;
                        default:
                            break;
                    }

                    DateTime fromDt = DateTime.Now;
                    DateTime toDt = DateTime.Now;

                    switch (objSearch.dateRange)
                    {
                        case 12:
                            fromDt = DateTime.Now.AddYears(-1);
                            break;
                        case 6:
                            fromDt = DateTime.Now.AddMonths(-6);
                            break;
                        case 1:
                            fromDt = DateTime.Now.AddDays(-DateTime.Now.Day).AddDays(1);// DateTime.Now.AddMonths(-1);
                            break;
                        case -1:
                            fromDt = objSearch.fromDate ?? DateTime.Now.AddMonths(-1);
                            toDt = objSearch.toDate ?? DateTime.Now;
                            break;
                    }

                    dateInclusion = $" AND DATEONFILE >= '{fromDt.ToString("dd/MM/yyyy")}' AND DATEONFILE <= '{toDt.ToString("dd/MM/yyyy")}' ";

                    switch (objSearch.sampleOrders)
                    {
                        case true:
                            sampleOrderInclusion = " AND SAMP.PROD = 'Y' ";
                            break;
                        case false:
                            sampleOrderInclusion = " AND SAMP.PROD # 'Y' ";
                            break;
                        default:
                            break;
                    }

                    var uvSelectStatement = $"{cusrefType} = '{dataSource.CustomerReference}' {sampleOrderInclusion} {selectionCriteria} {searchTermInclusion} {dateInclusion} {ordering}";

                    var queryResult = qe.Run(uvFileName,
                        uvSelectStatement,
                        "ORDNO LINE DATEONFILE QTYORD.ORD WI.STDPROD TEXT.DESC CUS.ORD.REF WI.STATUS WI.PARCEL.LINK WT.BASEPROD.STD WI.DESP.STATUS.DATE WI.RESERVED.UNTIL WI.RESERVED MBATCH.NO.WI",
                        "",
                        "XML",
                        null,
                        false,
                        60);

                    //queryResult = queryResult.Replace("&", " and ");//Regex.Replace(queryResult, "&*", "");

                    //queryResult = queryResult.Replace(@"#38;", "");

                    //queryResult = queryResult.Replace(@"prefix_txt", "p");

                    var xDoc = XDocument.Parse(queryResult);

                    var orderCount = xDoc.Descendants("record").Count();

                    var results = new List<MertexerV2.Models.Accounts.OutstandingOrderResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        try
                        {
                            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                            DateTime holdingTime = DateTime.Now;

                            DateTime? orderDate = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                            {
                                orderDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                            }

                            DateTime? despatchDueDate = null;
                            string despatchDueDateMessage = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value))
                            {
                                if (DateTime.TryParseExact(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out holdingTime))
                                {
                                    despatchDueDate = holdingTime;
                                }
                                else
                                {
                                    despatchDueDateMessage = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;
                                }
                            }
                            else
                            {
                                despatchDueDateMessage = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;
                            }

                            DateTime? reservedUntil = null;
                            string reservedUntilMessage = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value))
                            {
                                if (DateTime.TryParseExact(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out holdingTime))
                                {
                                    reservedUntil = holdingTime;
                                }
                                else
                                {
                                    reservedUntilMessage = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value;
                                }
                            }
                            else
                            {
                                reservedUntilMessage = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value;
                            }

                            results.Add(new MertexerV2.Models.Accounts.OutstandingOrderResult(dataSource.UniVerseAccount)
                            {
                                OrderNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value),
                                LineNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value),
                                OrderDate = orderDate,
                                OrderQuantity = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value),
                                Product = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                                Description = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                                CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                Status = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                                Tracking = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                                BaseProduct = c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                                DespatchStatusDate = despatchDueDate,
                                DespatchStatusDateMessage = despatchDueDateMessage,
                                ReservedUntil = reservedUntil,
                                ReservedUntilMessage = reservedUntilMessage,
                                ReserveAction = c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value,
                                Batch = c.XPathSelectElement("descendant::fd[@id='C14']").Attribute("value").Value
                            });
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                            System.Diagnostics.Debug.WriteLine(c);
                        }
                    });

                    lock (locker)
                    {
                        result.Results.AddRange(results);
                    }
                }
            });

            return result;
        }

        public static MertexerV2.Models.Accounts.ParcelCountReturn CheckParcelCount(
            string tradeDataSourceJson,
            MertexerV2.Models.Accounts.PassParcelCount objParcel)
        {
            var result = new MertexerV2.Models.Accounts.ParcelCountReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var split = objParcel.documentNumber.Split(new string[] { "," }, StringSplitOptions.None);

                    var xDoc = XDocument.Parse(qe.Run(split[0],
                        $"DOC.NO = '{split[1]}'",
                        ""));

                    var r = new List<MertexerV2.Models.Accounts.ParcelTrackingDetail>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        r.Add(new MertexerV2.Models.Accounts.ParcelTrackingDetail()
                        {
                            Parcel = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            ProductCodes = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                            Descriptions = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                            TrackingURL = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value
                        });
                    });

                    lock (locker)
                    {
                        result.Results.AddRange(r);
                    }
                }
            });

            return result;
        }
        
        public static OrderTrackingSearchResultWrapper OrderTrackingSearch(
            string tradeDataSourceJson,
            PassAutocompleteTerm objSearch)
        {
            var overallResult = new OrderTrackingSearchResultWrapper();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                var result = new OrderTrackingSearchResultWrapper();

                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
                conn_str.UserID = MertexConnectionDetails.UserID;
                conn_str.Password = MertexConnectionDetails.Password;
                conn_str.Server = MertexConnectionDetails.Server;
                conn_str.Database = dataSource.UniVerseAccount;
                conn_str.ServerType = MertexConnectionDetails.ServerType;

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                    {
                        #region Visit TOEHEAD with an order number, if not found search MertexReportData for CustomerOrderReference

                        var xOrderHeadDoc = XDocument.Parse(qe.Run(
                                                            "TOEHEAD",
                                                            $"@ID = {objSearch.searchTerm}",
                                                            "ORDERNO " +
                                                            "SS.DATEONFILE " +
                                                            "CUSREF " +
                                                            "CUSNAME " +
                                                            "DELNAME " +
                                                            "DELADDR " +
                                                            "ECOMM.EMAIL.DES " +
                                                            "HEADER.REF26 " +
                                                            "CUS.ORD.REF " +
                                                            "HAULIERCODE " +
                                                            "HAULIERDESC "));

                        if (!xOrderHeadDoc.Descendants("record").Any())
                        {
                            result.ResultStatus = $"Order {objSearch.searchTerm} was not found.";
                            // This order was not found at this data source.
                            return;
                        }

                        #endregion

                        #region We found an Order record on TOEHEAD: Go ahead and set some properties on our result

                        var orderHeadNode = xOrderHeadDoc.Descendants("record").First();

                        result.OrderNumber = orderHeadNode.XPathSelectElement("descendant::fd[@id='C1']")?.Attribute("value")?.Value;

                        result.DateOnFile = orderHeadNode.XPathSelectElement("descendant::fd[@id='C2']")?.Attribute("value")?.Value;

                        result.CustomerReference = orderHeadNode.XPathSelectElement("descendant::fd[@id='C3']")?.Attribute("value")?.Value;

                        result.CustomerName = orderHeadNode.XPathSelectElement("descendant::fd[@id='C4']")?.Attribute("value")?.Value;

                        result.DeliveryName = orderHeadNode.XPathSelectElement("descendant::fd[@id='C5']")?.Attribute("value")?.Value;

                        result.DeliveryAddress = String.Join(System.Environment.NewLine,
                                                             orderHeadNode.XPathSelectElement("descendant::fd[@id='C6']")?.Attribute("value")?.Value
                                                                          .Split(MertexerV2SQL.Utilities.Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries)
                                                                          .Select(p => p.Trim()));

                        result.EmailAddress = orderHeadNode.XPathSelectElement("descendant::fd[@id='C7']")?.Attribute("value")?.Value ??
                                                        orderHeadNode.XPathSelectElement("descendant::fd[@id='C8']")?.Attribute("value")?.Value;

                        result.CustomerOrderReference = orderHeadNode.XPathSelectElement("descendant::fd[@id='C9']")?.Attribute("value")?.Value
                                                                     .Split(Framework.ValueMarkDelimiter)[0]; // Why is this split necessary? Goodness knows, it IS an MV field though.

                        result.HaulierCode = orderHeadNode.XPathSelectElement("descendant::fd[@id='C10']")?.Attribute("value")?.Value;

                        result.HaulierName = orderHeadNode.XPathSelectElement("descendant::fd[@id='C11']")?.Attribute("value")?.Value;

                        var ti = new System.Globalization.CultureInfo("en-GB").TextInfo;

                        result.HaulierName = ti.ToTitleCase(result.HaulierName.ToLowerInvariant());

                        #endregion

                        #region Now visit TOELINE to fetch *all* lines about this order, cancelled and not. Fetch all the properties we might want from TOELINE at this time.

                        var xOrderLineDoc = XDocument.Parse(qe.Run(
                                                            "TOELINE",
                                                            $"ORDERNO = {result.OrderNumber}",
                                                            "LINE " +
                                                            "LINEKEY " +
                                                            "PRODCODE " +
                                                            "PRODDESC " +
                                                            "QTYORD.ORD " +
                                                            "QTYCAN.ORD " +
                                                            "STOCK.UNIT " +
                                                            "STATUS " +
                                                            "STATUSDESC " +
                                                            "DNNUMBER " +
                                                            "DN.LINE.KEY " +
                                                            "SS.CURSUG.DELDATE.D4 "));

                        xOrderLineDoc.Descendants("record").ToList().ForEach(ol =>
                        {
                            result.OrderLines.Add(new OrderLineRecord(
                                lineNumber: Convert.ToInt32(ol.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value),
                                orderNumber: result.OrderNumber,
                                lineKey: ol.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                                productCode: ol.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                                productDescription: ol.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                                quantityOrdered: Convert.ToDecimal(ol.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value),
                                quantityCancelled: Convert.ToDecimal(ol.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value),
                                stockUnit: ol.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                status: ol.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                                statusDescription: ol.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                                deliveryNoteNumber: ol.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                                deliveryNoteLineNumber: ol.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                                estimatedDespatchDateOnFile: ol.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value
                            ));
                        });

                        // We can now build our collection of DeliveryNotes, as each line carries a DeliveryNoteNumber and DeliveryNoteLineKey.
                        result.DeliveryNotes = result.OrderLines
                                                     .GroupBy(ol => ol.DeliveryNoteNumber)
                                                     .Where(g => !String.IsNullOrEmpty(g.Key)) // If one or more lines haven't get been despatched, they cannot and do not have Delivery Note information
                                                     .Select(g => new DeliveryNoteHeadRecord(g.Key, g.Select(ol => ol.DeliveryNoteLineNumber))).ToList();

                        #endregion

                        #region Now visit DNL[HEAD/HIST] to pick up... oh, lots of things.

                        result = FetchParcelsFromDeliveryNoteHeaders(dataSource.UniVerseAccount, conn, result);

                        #endregion

                        #region We need to set the ParcelContents' RemainingToDespatchQuantity

                        var allParcelContents = result.DeliveryNotes.SelectMany(p => p.ParcelHead.Parcels.SelectMany(q => q.ParcelContents).ToList()).ToList();

                        var distinctOrderLineIdentifiers = allParcelContents.Select(p => p.OrderLineKey).Distinct().ToList();

                        foreach (var ol in distinctOrderLineIdentifiers)
                        {
                            var orderedQuantity = allParcelContents.Where(p => p.OrderLineKey == ol).First().OrderLineQuantity;

                            var totalParcelledQuantity = allParcelContents.Where(p => p.OrderLineKey == ol).Select(p => p.Quantity).Sum();

                            var remainingToDespatch = orderedQuantity - totalParcelledQuantity;

                            allParcelContents.Where(p => p.OrderLineKey == ol).ToList().ForEach(pc =>
                            {
                                pc.RemainingToDespatchQuantity = remainingToDespatch;
                            });
                        }

                        #endregion
                    }

                    // If found, great. Pick up Customer details, Delivery Details, etc. from Header.
                    //  
                    //  TOEHEAD
                    //      ORDERNO SS.DATEONFILE CUSREF CUSNAME DELNAME DELADDR ECOMM.EMAIL.DES HEADER.REF26 CUS.ORDER.REF HAULIERCODE
                    //      [ORD.PLHEAD]? This can take me to OEPLHEAD / OEDNHEAD / OEDNHHIST but is it the *right* way to get there?
                    // 
                    //  TOELINE
                    //      PRODCODE PRODDESC STOCK.UNIT QTYORD.ORD QTYCAN.ORD DNNUMBER DN.LINE.KEY
                    //
                    //  DNL[HEAD/HIST]
                    //      PARCEL  ...  STUFF
                    //
                    //  OEFINDPARCEL
                    //      PIECE.NO WEB.URL || CUSTOM.TRACK.URL.DES 
                    //

                    // Now, if CUSTOM.TRACK.URL.DES is NULL for any given parcel, you can fall back to forming WEBPARCEL{PARCEL}WEBPARCEL2 from OEHAULIER
                    // Which is actually precisely what WEB.URL does!

                    // to be a faithful accounting per order line of how much was ordered
                    // reduced by how much has been parcelled.
                }

                // if we get here, then the order was found on one data source. Set it to the overall result.
                lock (locker)
                {
                    overallResult = result;
                }
            });

            return overallResult;
        }

        public static OrderTrackingSearchResultWrapper FetchParcelsFromDeliveryNoteHeaders(
            string uniVerseAccount,
            U2Connection conn,
            OrderTrackingSearchResultWrapper result)
        {
            result.DeliveryNotes.ForEach(deliveryNoteHead =>
            {
                var parcelHead = new ParcelHeadResult();

                using (var ce = new CommandExecutor(uniVerseAccount))
                {
                    var cmd = $"LIST.ITEM OEDNHEAD {deliveryNoteHead.DeliveryNoteIdentifier} COL.HDR.SUPP ID.SUP COUNT.SUP";

                    var resultString = ce.Arbitrary(cmd);

                    // Return early if the record was not found
                    if (resultString.ToLowerInvariant().Contains("not found"))
                    {
                        cmd = cmd.Replace("OEDNHEAD", "OEDNHHIST USING DICT OEPLHEAD");
                        resultString = ce.Arbitrary(cmd);
                    }

                    // Delivery Note Head
                    var dnh = CommandExecutor.CrunchTCLResult(resultString);

                    // <10>OEPLLINEP
                    parcelHead.PickingLines = dnh.Columns.Where(p => p.Index == 10)
                                                                        .Single()
                                                                        .MultiValues
                                                                        .Select(p => new PickingListLineIdentifier(
                                                                            pickingListLineId: p.Value.Replace(" ", "")
                                                                        )).ToList();

                    // <13>HAULIERCODE
                    parcelHead.HaulierCode = dnh.Columns.Where(p => p.Index == 13).Single().Value;

                    //  <25>DESPDATE
                    parcelHead.DespatchedDateOnFile = dnh.Columns.Where(p => p.Index == 25).Single().Value;

                    //  <535>CUSTOM.TRACK.URL.DES
                    parcelHead.TrackingURL = dnh.Columns.Where(p => p.Index == 535).SingleOrDefault()?.Value;

                    CheckHaulierTrackable(conn, ref parcelHead); // This will tell us if the haulier at play is going to be able to provide us with a tracking url. Another "return early".

                    if (!String.IsNullOrEmpty(parcelHead.Untrackable))
                    {
                        // No point processing this delivery note header further; the haulier is not going to provide us with a tracking url.
                        return;
                    }

                    // Now, parcel numbers are a bit tricky. The prime source for them is <191>PARCEL.FULL.
                    // Helpfully, however, this can be empty. If that's the case, the next place to look is <40>PCL.NO

                    var parcels = new List<OrderParcelDetail>();

                    if (dnh.Columns.Where(p => p.Index == 191).Any())
                    {
                        // <191>PARCEL.FULL
                        parcels = dnh.Columns.Where(p => p.Index == 191)
                                             .Single()
                                             .MultiValues
                                             .Select((p, i) => new OrderParcelDetail(parcelIndex: i,
                                                                                     parcelNumber: p.Value,
                                                                                     haulierCode: parcelHead.HaulierCode,
                                                                                     despatchedDateOnFile: parcelHead.DespatchedDateOnFile,
                                                                                     trackingUrl: null)).ToList();
                    }
                    else
                    {
                        // 40: PCL.NO
                        parcels = dnh.Columns.Where(p => p.Index == 40)
                                             .Single()
                                             .MultiValues
                                             .Select((p, i) => new OrderParcelDetail(parcelIndex: i,
                                                                                    parcelNumber: p.Value,
                                                                                    haulierCode: parcelHead.HaulierCode,
                                                                                    despatchedDateOnFile: parcelHead.DespatchedDateOnFile,
                                                                                    trackingUrl: null)).ToList();
                    }


                    // <10>OEPLLINE now needs to take me to OEDNLINE or OEDNLHIST
                    // I have to get a list of ParcelNumbers from the Picking List Line level, as this is the only way I have
                    // direct access to the TOELINE identifiers belonging to a particular parcel.
                    var lines = dnh.Columns.Where(p => p.Index == 10)
                                    .Single()
                                    .MultiValues
                                    .Select(p => p.Value.Replace(" ", ""))
                                    .ToList();

                    foreach (var dnLineId in lines)
                    {
                        var plCmd = $"LIST.ITEM OEDNLINE {dnLineId} COL.HDR.SUPP ID.SUP COUNT.SUP";

                        var dnlString = ce.Arbitrary(plCmd);

                        if (dnlString.Contains("not found"))
                        {
                            plCmd = plCmd.Replace("OEDNLINE", "OEDNLHIST USING DICT OEPLLINE");
                            dnlString = ce.Arbitrary(plCmd);
                        }

                        var dnl = CommandExecutor.CrunchTCLResult(dnlString);

                        // <97>PARCEL.FULL contains all those parcels which this Picking Line gave product to
                        var deliveryNoteLineParcels = dnl.Columns.Where(p => p.Index == 97)
                                                         .Select(p => p.MultiValues)
                                                         .Single();

                        foreach (var dnlp in deliveryNoteLineParcels)
                        {
                            // <0>LINEKEY
                            var pickingListLineIdentifier = dnLineId;

                            // <3>TOELINP
                            var toelineId = dnl.Columns.Where(p => p.Index == 3)
                                                        .Single()
                                                        .Value;

                            // <4>PRODCODE
                            var prodCode = dnl.Columns.Where(p => p.Index == 4)
                                                      .Single()
                                                      .Value;

                            var thisParcel = parcels.Where(p => p.ParcelNumber == dnlp.Value).Single();

                            thisParcel.OrderLines.Add(new OrderLineStub(toelineId, thisParcel.ParcelNumber, prodCode, pickingListLineIdentifier));

                            // at this point in time, I *know* that Parcel 023120 had at least part of Toeline 999018001*1 put in it.
                            // use that information!
                        }
                    }

                    // We should now have a distinct list of parcels for this Picking List Head.
                    // Fetch the contents of these parcels.
                    parcels.ForEach(parcel =>
                    {
                        // We now want to know the parcel contents.

                        // <48>PCL.PRODCODES
                        // Only interested in those product codes which correspond to this parcel's sv index.
                        var productCodes = dnh.Columns.Where(p => p.Index == 48)
                                                        .SelectMany(p => p.MultiValues.Where(q => q.Index == parcel.ParcelIndex))
                                                        .SelectMany(p => p.SubValues)
                                                        .ToList();

                        // Got a list of product codes? Wonderful.

                        foreach (var codeEntry in productCodes)
                        {
                            // Check for pieces.
                            // <51>PCL.PIECES
                            var pieces = dnh.Columns.Where(p => p.Index == 51)
                                                    .SelectMany(p => p.MultiValues)
                                                    .Where(mv => mv.Index == parcel.ParcelIndex)
                                                    .ToList();

                            // This is where it gets a bit murky.
                            // If there exists more than one product code in this parcel, then we want pieces only for the current product code.
                            // If there was only one product code in this parcel, then we have one less level of SV to traverse.
                            if (productCodes.Count > 1)
                            {
                                var piecesForThisPartNumber = pieces.SelectMany(p => p.SubValues
                                                                                    .Where(sv => sv.Index == codeEntry.Index))
                                                                                    .ToList();

                                // Are there any pieces at this depth for this part number?
                                if (piecesForThisPartNumber.Any())
                                {
                                    parcel.ParcelContents.AddRange(ProcessParcelPieces(parcel, piecesForThisPartNumber, codeEntry, dnh, result.OrderLines));
                                }
                                else
                                {
                                    var c = ProcessParcelQuantity(parcel, codeEntry, dnh, result.OrderLines);
                                    if (c != null)
                                        parcel.ParcelContents.Add(c);
                                }
                            }
                            else
                            {
                                if (pieces.Any())
                                {
                                    parcel.ParcelContents.AddRange(ProcessParcelPieces(parcel, pieces, codeEntry, dnh, result.OrderLines));
                                }
                            }
                        }

                        #region Visit OEFINDPARCEL and null-coalesce between CUSTOM.TRACK.URL.DES and WEB.URL

                        FetchTrackingURL(conn, ref parcel);

                        #endregion

                        // Just for testing purposes
                        // This will cause the parcel head's child parcels to adopt their head's tracking URL
                        // if (parcelHead.TrackingURL != null)
                        // {
                        //     parcel.TrackingURL = parcelHead.TrackingURL;
                        // }
                    });

                    parcelHead.Parcels = parcels;
                }

                deliveryNoteHead.ParcelHead = parcelHead;
            });

            return result;
        }

        private static IEnumerable<ParcelContent> ProcessParcelPieces(
            OrderParcelDetail parcel,
            List<MertexerV2.Models.TCL.SubSubValue> piecesForThisPartNumber,
            MertexerV2.Models.TCL.SubSubValue codeEntry,
            MertexerV2.Models.TCL.TCLItemResult dnh,
            List<OrderLineRecord> orderLines)
        {
            var result = new List<ParcelContent>();

            foreach (var piece in piecesForThisPartNumber)
            {
                var productCode = codeEntry.Value.Replace(" ", ""); // Product codes should not contain spaces.

                // <53>PCL.PCELENS
                // Find the corresponding piece length for this parcel for this product code for this piece.
                var quantity = dnh.Columns.Where(p => p.Index == 53)
                                            .SelectMany(p => p.MultiValues)
                                            .Where(p => p.Index == parcel.ParcelIndex)
                                            .Select(p => p.SubValues.Count() > 1
                                                        ? p.SubValues
                                                            .Where(sv => sv.Index == piece.Index)
                                                            .Single()
                                                            .Value
                                                        : p.Value).Single();

                var orderLineStub = parcel.OrderLines.Where(p => p.ParcelNumber == parcel.ParcelNumber && p.ProductCode == productCode).FirstOrDefault();

                // The SingleOrDefault is because Shipments can contain multiple Orders
                // Our user is searching for *just one* order, and I don't agree with polluting their results
                // with orders other than the one for which they searched.

                // This null testing is necessary because Mertex sometimes creates 'ghost' parcels with no contents:
                // the PickingList header will describe say, two parcels -- and the picking lines will describe themselves
                // as contributing to both, but this fact will conflict with the MV fields on the header which can indicate that one parcel 
                // was in fact empty.
                if (orderLineStub != null)
                {
                    var orderLine = orderLines.Where(p => p.LineKey == orderLineStub.OrderLineKey).SingleOrDefault();

                    if (orderLine != null)
                    {
                        result.Add(new ParcelContent(
                            index: parcel.ParcelIndex,
                            productCode: productCode,
                            productDescription: orderLine.ProductDescription,
                            quantity: Convert.ToDecimal(quantity.Replace(" ", "")) / 1000,
                            parcelNumber: parcel.ParcelNumber,
                            orderLineKey: orderLineStub.OrderLineKey,
                            orderLineQuantity: orderLine.Quantity,
                            stockUnit: orderLine.StockUnit,
                            pieceNumber: piece.Value.Replace(" ", ""),
                            pickingListIdentifier: orderLineStub.PickingListLineIdentifier,
                            remainingToDespatchQuantity: null
                        ));
                    }
                }
            }

            return result;
        }

        public static List<ParcelContent> ProcessParcelPieces(
            OrderParcelDetail parcel,
            List<MertexerV2.Models.TCL.SubValue> pieces,
            MertexerV2.Models.TCL.SubSubValue codeEntry,
            MertexerV2.Models.TCL.TCLItemResult dnh,
            List<OrderLineRecord> orderLines)
        {
            var result = new List<ParcelContent>();

            foreach (var piece in pieces.SelectMany(p => p.SubValues))
            {
                var productCode = codeEntry.Value.Replace(" ", ""); // Product codes should not contain spaces.

                // <53>PCL.PCELENS
                // Find the corresponding piece length for this parcel for this product code for this piece.
                var quantity = dnh.Columns.Where(p => p.Index == 53)
                                            .SelectMany(p => p.MultiValues)
                                            .Where(p => p.Index == parcel.ParcelIndex)
                                            .Select(p => p.SubValues.Count() > 1
                                                        ? p.SubValues
                                                            .Where(sv => sv.Index == piece.Index)
                                                            .Single()
                                                            .Value
                                                        : p.Value).Single();

                var orderLineStub = parcel.OrderLines.Where(p => p.ParcelNumber == parcel.ParcelNumber && p.ProductCode == productCode).FirstOrDefault();

                if (orderLineStub != null)
                {
                    var orderLine = orderLines.Where(p => p.LineKey == orderLineStub?.OrderLineKey).SingleOrDefault();

                    if (orderLine != null)
                    {
                        result.Add(new ParcelContent(
                            index: parcel.ParcelIndex,
                            productCode: productCode,
                            productDescription: orderLine.ProductDescription,
                            quantity: Convert.ToDecimal(quantity.Replace(" ", "")) / 1000,
                            parcelNumber: parcel.ParcelNumber,
                            orderLineKey: orderLineStub.OrderLineKey,
                            orderLineQuantity: orderLine.Quantity,
                            stockUnit: orderLine.StockUnit,
                            pieceNumber: piece.Value.Replace(" ", ""),
                            pickingListIdentifier: orderLineStub.PickingListLineIdentifier,
                            remainingToDespatchQuantity: null
                        ));
                    }
                }
            }

            return result;
        }

        public static ParcelContent ProcessParcelQuantity(
            OrderParcelDetail parcel,
            MertexerV2.Models.TCL.SubSubValue codeEntry,
            MertexerV2.Models.TCL.TCLItemResult dnh,
            List<OrderLineRecord> orderLines)
        {
            // We should be dealing with a non-piece order line

            var productCode = codeEntry.Value.Replace(" ", ""); // Product codes should not contain spaces.

            // <54>PCL.QUANTITY
            // Find the corresponding quantity for this product code.
            var quantity = dnh.Columns.Where(p => p.Index == 54)
                                        .SelectMany(p => p.MultiValues)
                                        .Where(p => p.Index == parcel.ParcelIndex)
                                        .Select(p => p.SubValues.Count() > 1
                                                    ? p.SubValues
                                                        .Where(sv => sv.Index == codeEntry.Index)
                                                        .Single()
                                                        .Value
                                                    : p.Value).Single();

            var orderLineStub = parcel.OrderLines.Where(p => p.ParcelNumber == parcel.ParcelNumber && p.ProductCode == productCode).FirstOrDefault();

            if (orderLineStub != null)
            {
                var orderLine = orderLines.Where(p => p.LineKey == orderLineStub?.OrderLineKey).SingleOrDefault();

                if (orderLine != null)
                {
                    return new ParcelContent(
                        index: parcel.ParcelIndex,
                        productCode: productCode,
                        productDescription: orderLine.ProductDescription,
                        quantity: Convert.ToDecimal(quantity.Replace(" ", "")) / 1000,
                        parcelNumber: parcel.ParcelNumber,
                        orderLineKey: orderLineStub.OrderLineKey,
                        orderLineQuantity: orderLine.Quantity,
                        stockUnit: orderLine.StockUnit,
                        pieceNumber: null,
                        pickingListIdentifier: orderLineStub.PickingListLineIdentifier,
                        remainingToDespatchQuantity: null
                    );
                }
            }

            return null;
        }

        public static void FetchTrackingURL(
            U2Connection conn,
            ref OrderParcelDetail detail)
        {
            var parcelComm = conn.CreateCommand();

            parcelComm.CommandType = System.Data.CommandType.Text;

            parcelComm.CommandText = $@"
                SELECT  [WEB.URL],
                        [CUSTOM.TRACK.URL.DES]
                FROM    OEFINDPARCEL
                WHERE   [@ID] = @ParcelIdentifier";

            parcelComm.Parameters.Add("@ParcelIdentifier", detail.ParcelIdentifier);

            if (!conn.IsOpen)
                conn.Open();

            var reader = parcelComm.ExecuteReader();

            while (reader.Read())
            {
                string ReflexTrackingURL = !reader.IsDBNull(0) ? reader.GetString(0) : null;
                string DGTrackingURL = !reader.IsDBNull(1) ? reader.GetString(1) : null;

                detail.TrackingURL = detail.ParcelIdentifier.StartsWith("FL/") ? DGTrackingURL : ReflexTrackingURL;
                //String.IsNullOrEmpty(DGTrackingURL) 
                //? ReflexTrackingURL
                //: DGTrackingURL == "0" // Smells like more Mertex BS...
                //    ? ReflexTrackingURL 
                //    : DGTrackingURL;
            }

            reader.Close();

            conn.Close();
        }

        public static void CheckHaulierTrackable(U2Connection conn, ref ParcelHeadResult head)
        {
            var parcelComm = conn.CreateCommand();

            parcelComm.CommandType = System.Data.CommandType.Text;

            parcelComm.CommandText = $@"
                SELECT  [NAME],
                        [WEBPARCEL]
                FROM    OEHAULIER
                WHERE   [@ID] = @HaulierCode";

            parcelComm.Parameters.Add("@HaulierCode", head.HaulierCode);

            if (!conn.IsOpen)
                conn.Open();

            var reader = parcelComm.ExecuteReader();

            while (reader.Read())
            {
                head.HaulierName = !reader.IsDBNull(0) ? reader.GetString(0) : null;
                var parcelUrlPart1 = !reader.IsDBNull(1) ? reader.GetString(1) : null;

                if (!String.IsNullOrEmpty(head.HaulierName))
                {
                    var ti = new System.Globalization.CultureInfo("en-GB").TextInfo;

                    head.HaulierName = ti.ToTitleCase(head.HaulierName.ToLowerInvariant());
                }

                if (String.IsNullOrEmpty(parcelUrlPart1) && head.HaulierCode != "FL")
                {
                    head.Untrackable = $"Sorry, this was despatched via {head.HaulierCode} {(String.IsNullOrEmpty(head.HaulierName) ? "" : $" - {head.HaulierName}")} and is not trackable.";
                }
            }

            reader.Close();

            conn.Close();
        }
    }
}
