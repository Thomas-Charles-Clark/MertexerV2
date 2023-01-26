using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MertexerV2.Models.Accounts;
using MertexerV2SQL.Models.Accounts;
using U2.Data.Client;
using Newtonsoft.Json;
using MertexerV2.Models.DataSource;
using System.Threading.Tasks;

namespace MertexerV2SQL.Accounts
{
    public static partial class AccountFunctionsUniSQL
    {
        public static tradeAccountEmailsModel TradeEmails(string tradeDataSourceJson)
        {
            var result = new tradeAccountEmailsModel();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Contact)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command sslPostComm = conn.CreateCommand();

                    sslPostComm.CommandText = @"
                    SELECT
                        [DOCTYPE],
                        [CONTACT.EMAIL]
                    FROM 
                        [SSLPOST.CONTACTS]
                    WHERE
                        CUSREF = @TradeAccountNumber AND
                        (
                            DOCTYPE = 'CI' OR
                            DOCTYPE = 'PB'
                        )
                    NOWAIT";

                    sslPostComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);

                    conn.Open();

                    var sslPostReader = sslPostComm.ExecuteReader();

                    while (sslPostReader.Read())
                    {
                        switch (sslPostReader.GetString(0))
                        {
                            case "CI":
                                lock (locker)
                                    result.InvoicesStatements = sslPostReader.GetString(1);
                                break;
                            case "PB":
                                lock (locker)
                                    result.Pricelists = sslPostReader.GetString(1);
                                break;
                            default: break;
                        }
                    }

                    sslPostReader.Close();

                    var slCustComm = conn.CreateCommand();

                    slCustComm.CommandText = @"
                        SELECT
                            [EMAIL],
                            [SS.MARKET.EMAIL]
                        FROM
                            [SLCUST]
                        WHERE
                            [SLCUST].[@ID] = @TradeAccountNumber
                        NOWAIT
                    ";

                    slCustComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);

                    var slCustReader = slCustComm.ExecuteReader();

                    while (slCustReader.Read())
                    {
                        lock (locker)
                        {
                            result.OrderConfirmations = slCustReader.GetString(0);
                            result.ProductNewsEvents = slCustReader.GetString(1);
                        }
                    }

                    conn.Close();
                }
            });

            return result;
        }

        public static bool UpdateOrderConfirmations(string tradeDataSourceJson, string email)
        {
            var result = false;

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Contact)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command sslPostComm = conn.CreateCommand();

                    sslPostComm.CommandText = @"
                    UPDATE 
                        [SSLPOST.CONTACTS]
                    SET 
                        [CONTACT.EMAIL] = @Email
                    WHERE
                        CUSREF = @TradeAccountNumber AND
                        (
                            DOCTYPE = 'SOC' OR
                            DOCTYPE = 'PRO'
                        )
                    NOWAIT";

                    sslPostComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);
                    sslPostComm.Parameters.Add("@Email", email);

                    conn.Open();

                    int queryresult = sslPostComm.ExecuteNonQuery();

                    var slCustComm = conn.CreateCommand();

                    slCustComm.CommandText = @"
                        UPDATE 
                            [SLCUST]
                        SET
                            [EMAIL] = @Email
                        WHERE
                            [SLCUST].[@ID] = @TradeAccountNumber
                    ";

                    slCustComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);

                    slCustComm.Parameters.Add("@Email", email);

                    int queryresult2 = slCustComm.ExecuteNonQuery();

                    if (queryresult == 2 && queryresult2 == 1)
                    {
                        lock (locker)
                            result = true;
                    }

                    conn.Close();
                }
            });

            return result;
        }

        public static bool UpdateInvoicesStatements(string tradeDataSourceJson, string email)
        {
            var result = false;

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Contact)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command sslPostComm = conn.CreateCommand();

                    sslPostComm.CommandText = @"
                    UPDATE 
                        [SSLPOST.CONTACTS]
                    SET 
                        [CONTACT.EMAIL] = @Email
                    WHERE
                        CUSREF = @TradeAccountNumber AND
                        (
                            DOCTYPE = 'CI' OR
                            DOCTYPE = 'CN' OR
                            DOCTYPE = 'ODL' OR
                            DOCTYPE = 'STAT'
                        )
                    NOWAIT";

                    sslPostComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);
                    sslPostComm.Parameters.Add("@Email", email);

                    conn.Open();

                    int queryresult = sslPostComm.ExecuteNonQuery();

                    if (queryresult == 4)
                    {
                        lock (locker)
                            result = true;
                    }

                    conn.Close();
                }
            });

            return result;
        }

        public static bool UpdateProductNewsEvents(string tradeDataSourceJson, string email)
        {
            var result = false;

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Contact)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command slCustComm = conn.CreateCommand();

                    slCustComm.CommandText = @"
                    UPDATE 
                        [SLCUST]
                    SET 
                        [SS.MARKET.EMAIL] = @Email
                    WHERE
                        [SLCUST].[@ID] = @TradeAccountNumber
                    NOWAIT";

                    slCustComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);
                    slCustComm.Parameters.Add("@Email", email);

                    conn.Open();

                    int queryresult = slCustComm.ExecuteNonQuery();

                    if (queryresult == 1)
                    {
                        lock (locker)
                            result = true;
                    }

                    conn.Close();
                }
            });

            return result;
        }

        public static bool UpdatePricelists(string tradeDataSourceJson, string email)
        {
            var result = false;

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Contact)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command sslPostComm = conn.CreateCommand();

                    sslPostComm.CommandText = @"
                    UPDATE 
                        [SSLPOST.CONTACTS]
                    SET 
                        [CONTACT.EMAIL] = @Email
                    WHERE
                        CUSREF = @TradeAccountNumber AND
                        DOCTYPE = 'PB'
                    NOWAIT";

                    sslPostComm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);
                    sslPostComm.Parameters.Add("@Email", email);

                    conn.Open();

                    int queryresult = sslPostComm.ExecuteNonQuery();

                    if (queryresult == 1)
                    {
                        lock (locker)
                            result = true;
                    }

                    conn.Close();
                }
            });

            return result;
        }

        public static MertexInvoiceAddress InvoiceAddress(string tradeDataSourceJson)
        {
            var result = new MertexInvoiceAddress();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

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
                        COUNTRY,
                        [DEF.DELREF],
                        INPHONE,
                        EMAIL
                    FROM 
                        SLCUST
                    WHERE
                        CUSREF = @TradeAccountNumber
                    NOWAIT
                    ";

                    comm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);

                    conn.Open();

                    var reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        lock (locker)
                        {
                            result.Name = reader.IsDBNull(0) ? null : reader.GetString(0);
                            result.Line1 = reader.IsDBNull(1) ? null : reader.GetString(1);
                            result.Line2 = reader.IsDBNull(2) ? null : reader.GetString(2);
                            result.City = reader.IsDBNull(3) ? null : reader.GetString(3);
                            result.Province = reader.IsDBNull(4) ? null : reader.GetString(4);
                            result.PostalCode = reader.IsDBNull(5) ? null : reader.GetString(5);
                            result.Country = reader.IsDBNull(6) ? null : reader.GetString(6);
                            result.DefaultDeliveryReference = reader.IsDBNull(7) ? null : reader.GetString(7);
                            result.Phone = reader.IsDBNull(8) ? null : reader.GetString(8);
                            result.Email = reader.IsDBNull(9) ? null : reader.GetString(9);
                        }
                    }

                    reader.Close();

                    conn.Close();
                }
            });

            return result;
        }

        public static List<MertexAddress> AddressBook(string tradeDataSourceJson)
        {
            var result = new List<MertexAddress>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

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
                        PHONE,
                        [E.MAIL],
                        COUNTRY
                    FROM 
                        DELADD
                    WHERE
                        CUSREF = @TradeAccountNumber";

                    comm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);

                    conn.Open();

                    var reader = comm.ExecuteReader();

                    var addresses = new List<MertexAddress>();

                    while (reader.Read())
                    {
                        addresses.Add(new MertexAddress()
                        {
                            Name = reader.IsDBNull(0) ? null : reader.GetString(0).Trim(),
                            Company = reader.IsDBNull(1) ? null : reader.GetString(1).Trim(),
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

                    reader.Close();

                    conn.Close();

                    addresses.ForEach(c =>
                    {
                        c.TradeAccountCode = dataSource.CustomerReference;

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

                    lock (locker)
                    {
                        result.AddRange(addresses);
                    }
                }
            });

            result = result.OrderBy(c =>
                c.DeliveryReference
            ).ToList();

            return result;
        }

        public static List<MertexPCALookupResult> AddressBookSearch(string tradeDataSourceJson, PassPCAPredictFind objDetail)
        {
            var result = new List<MertexPCALookupResult>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .SelectMany(p => p.DataSources),
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

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
                        PHONE,
                        [E.MAIL],
                        COUNTRY
                    FROM 
                        DELADD
                    WHERE
                        CUSREF = @TradeAccountCode AND
((@Country IS NULL) OR (COUNTRY = @Country)) AND
(
    LOWER(DELREF) LIKE @SearchTerm OR
    LOWER(CONTACT) LIKE @SearchTerm OR
    LOWER(DELADDR1) LIKE @SearchTerm OR
    LOWER(DELADDR2) LIKE @SearchTerm OR
    LOWER(DELADDR3) LIKE @SearchTerm OR
    LOWER(DELADDR4) LIKE @SearchTerm OR
    LOWER(DELNAME) LIKE @SearchTerm OR
    LOWER(DELPCODE) LIKE @PostCode OR
    LOWER(COUNTRY) LIKE @SearchTerm
)
ORDER BY DELREF
NOWAIT
                    ";

                    comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);
                    comm.Parameters.Add("@Country", objDetail.countries ?? (object)DBNull.Value);
                    comm.Parameters.Add("@SearchTerm", $"%{objDetail.text?.ToLower()}%");
                    comm.Parameters.Add("@PostCode", $"%{objDetail.text?.ToLower()}%");

                    conn.Open();

                    var reader = comm.ExecuteReader();

                    var results = new List<MertexPCALookupResult>();

                    while (reader.Read())
                    {
                        results.Add(new MertexPCALookupResult()
                        {
                            Name = reader.IsDBNull(0) ? null : reader.GetString(0).Trim(),
                            Company = reader.IsDBNull(1) ? null : reader.GetString(1).Trim(),
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

                    results.ForEach(c =>
                    {
                        c.TradeAccountCode = dataSource.CustomerReference;

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

                    lock (locker)
                    {
                        result.AddRange(results);
                    }
                }
            });

            result = result.OrderBy(c =>
                c.DeliveryReferenceID
            ).ToList();

            var defaultDeliveryReference = GetDefaultDeliveryReference(tradeDataSourceJson);

            if (!String.IsNullOrEmpty(defaultDeliveryReference))
            {
                var defaultAddress = result.Where(p => p.DeliveryReference == defaultDeliveryReference).SingleOrDefault();

                if (defaultAddress != null)
                {
                    result.Remove(defaultAddress);
                    result.Insert(0, defaultAddress);
                }
            }

            return result;
        }

        public static string AddressExists(string tradeDataSourceJson, MertexerV2.Models.DeliveryAddress objAddress)
        {
            string result = null;

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

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

                    comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);
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
                        lock (locker)
                            result = reader.GetString(0);
                    }

                    reader.Close();

                    conn.Close();
                }
            });

            return result;
        }

        public static string GetDefaultDeliveryReference(string tradeDataSourceJson)
        {
            string currentDefault = null;

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command comm = conn.CreateCommand();

                    comm.CommandText = @"
                        SELECT
                            DEF.DELREF
                        FROM 
                            SLCUST
                        WHERE
                            CUSREF = @TradeAccountNumber";

                    comm.Parameters.Add("@TradeAccountNumber", dataSource.CustomerReference);

                    conn.Open();

                    var reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            lock (locker) 
                                currentDefault = reader.GetString(0);
                        }
                    }

                    reader.Close();
                }
            });

            return currentDefault;
        }

        public static void SetDefaultDeliveryReference(string tradeDataSourceJson, string deliveryReference)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    Database = dataSource.UniVerseAccount,
                    ServerType = MertexConnectionDetails.ServerType
                };

                using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
                {
                    U2Command comm = conn.CreateCommand();

                    comm.CommandText = @"
                        UPDATE 
                            SLCUST
                        SET
                            DEF.DELREF = @DeliveryReference
                        WHERE
                            CUSREF = @TradeAccountCode";

                    comm.Parameters.Add("@TradeAccountCode", dataSource.CustomerReference);
                    comm.Parameters.Add("@DeliveryReference", deliveryReference);

                    conn.Open();

                    comm.ExecuteNonQuery();

                    conn.Close();
                }
            });
        }
    }
}