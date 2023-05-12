using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlTypes;
using System.Xml.Linq;
using MertexerV2SQL.DAL;
using U2.Data.Client;
using MertexerV2SQL.Exceptions;
using MertexerV2.Models;

namespace MertexerV2SQL
{
    public static class MertexerV2SQLFunctions
    {
//        public static string InsertNewDeliveryAddress(DeliveryAddress incomingAddress)
//        {
//            try
//            {
//                string newDeliveryReference = null;

//                var deliveryReferences = new List<string>();

//                var insertResult = 0;

//                using (var mertexSQL = new MertexSQLConnection())
//                {
//                    var conn = mertexSQL.Connection;

//                    var selectComm = conn.CreateCommand();

//                    selectComm.CommandText = $@"SELECT   DELREF 
//                                            FROM     DELADD 
//                                            WHERE    CUSREF = @TradeAccountCode";

//                    var selectParams = new List<U2Parameter>()
//                    {
//                        new U2Parameter("@TradeAccountCode", incomingAddress.TradeAccountCode)
//                    };

//                    selectComm.Parameters.AddRange(selectParams.ToArray());

//                    conn.Open();

//                    var reader = selectComm.ExecuteReader();

//                    while (reader.Read())
//                    {
//#if DEBUG
//                        Console.WriteLine(reader.GetString(0));
//#endif
//                        deliveryReferences.Add(reader.GetString(0));
//                    }

//                    reader.Close();

//                    if (deliveryReferences.Count == 0)
//                    {
//                        deliveryReferences.Add($"{incomingAddress.TradeAccountCode}-0");
//                    }

//                    var references = deliveryReferences.Select(p => p.Split(new char[] { '-' })[1]).ToList();

//                    var NumericReferences = new List<int>();

//                    references.ForEach(c =>
//                    {
//                        if (Int32.TryParse(c, out int referenceNumber))
//                        {
//                            NumericReferences.Add(referenceNumber);
//                        }
//                    });

//                    var nextReference = NumericReferences.Max() + 1;

//                    newDeliveryReference = $"{incomingAddress.TradeAccountCode}-{nextReference}";

//                    incomingAddress.DeliveryReference = newDeliveryReference;

//                    var insertComm = conn.CreateCommand();

//                    /*
//                     * DeliveryReference
//                     * Company Name
//                     * Address Line 1
//                     * Address Line 2
//                     * Town
//                     * County
//                     * Postcode
//                     * CountryCode
//                     * Contact Name
//                     * Email
//                     * Phone
//                     */
//                    insertComm.CommandText = @"INSERT INTO DELADD
//                                           (
//                                                DELREF,
//                                                CUSREF,
//                                                DELADDR,
//                                                COUNTRY,
//                                                CONTACT,
//                                                [E.MAIL],
//                                                PHONE
//                                            )
//                                           VALUES
//                                           (
//                                                @DeliveryReference,
//                                                @TradeAccountCode,
//                                                @DeliveryAddress,
//                                                @CountryCode,
//                                                @ContactName,
//                                                @Email,
//                                                @Phone
//                                           )
//                                           NOWAIT";

//                    var insertParams = new List<U2Parameter>()
//                    {
//                        new U2Parameter("@DeliveryReference", incomingAddress.DeliveryReference),
//                        new U2Parameter("@TradeAccountCode", incomingAddress.TradeAccountCode),
//                        new U2Parameter("@DeliveryAddress", incomingAddress.MertexAddressLine),
//                        new U2Parameter("@CountryCode", incomingAddress.CountryCode),
//                        new U2Parameter("@ContactName", incomingAddress.ContactName),
//                        new U2Parameter("@Email", incomingAddress.Email),
//                        new U2Parameter("@Phone", incomingAddress.PhoneNumber)
//                    };

//                    insertComm.Parameters.AddRange(insertParams.ToArray());

//                    insertResult = insertComm.ExecuteNonQuery();

//                    conn.Close();
//                }


//#if DEBUG
//                Console.WriteLine($"Next DeliveryReference: {incomingAddress.DeliveryReference}");
//#endif

//                if (insertResult != 1)
//                {
//                    throw new AddressInsertException(incomingAddress);
//                }

//                return incomingAddress.DeliveryReference;
//            }
//            catch(Exception ex)
//            {
//                throw new AddressInsertException(ex, incomingAddress);
//            }
//        }
    }
}
