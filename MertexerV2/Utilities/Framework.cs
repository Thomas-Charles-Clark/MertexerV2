using MertexerV2.Models.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace MertexerV2.Utilities
{
    public static class Framework
    {
        public static Regex UniVerseScalingRegex = new Regex(@"MR(\d)(\d)");
        public static Newtonsoft.Json.Formatting IndentedFormatting = Newtonsoft.Json.Formatting.Indented;
        public static JsonSerializerSettings JsonSettingsIndentedIgnoreNulls = new JsonSerializerSettings()
        {
            Formatting = Framework.IndentedFormatting,
            NullValueHandling = NullValueHandling.Ignore
        };
        public static JsonSerializerSettings JsonSettingsIndentedIncludeNullsAlphabetical = new JsonSerializerSettings()
        {
            Formatting = Framework.IndentedFormatting,
            NullValueHandling = NullValueHandling.Include,
            ContractResolver = new OrderedContractResolver()
        };
        public static JsonSerializerSettings JsonSettingsIndentedIgnoreNullsAlphabetical = new JsonSerializerSettings()
        {
            Formatting = Framework.IndentedFormatting,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new OrderedContractResolver()
        };
        public static readonly char ValueMark = (char)253;
        public static readonly char SubValueMark = (char)252;

        public static readonly string[] EnvironmentNewLineDelimiter = new string[] { System.Environment.NewLine };
        public static readonly char[] UnixNewLineDelimiter = new char[] { '\n' };
        public static readonly char[] SpaceDelimiter = new char[] { ' ' };
        public static readonly char[] AsteriskDelimiter = new char[] { '*' };
        public static readonly char[] ValueMarkDelimiter = new char[] { (char)253 };
        public static readonly char[] SubValueMarkDelimiter = new char[] { (char)252 };

        public static readonly List<char> YN = new List<char>() { 'Y', 'N' };

        public static readonly string WavyLineBreak = $"\r\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n";

        public static System.Globalization.TextInfo gbTextInfo = new System.Globalization.CultureInfo("en-GB").TextInfo;

        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        //Thank you, buddy: https://stackoverflow.com/questions/8331119/escape-invalid-xml-characters-in-c-sharp#answer-36885492
        public static string RemoveInvalidXmlChars(string text)
        {
            if (text == null)
                return text;
            if (text.Length == 0)
                return text;

            // a bit complicated, but avoids memory usage if not necessary
            StringBuilder result = null;
            for (int i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (XmlConvert.IsXmlChar(ch))
                {
                    result?.Append(ch);
                }
                else if (result == null)
                {
                    result = new StringBuilder();
                    result.Append(text.Substring(0, i));
                }
            }

            if (result == null)
                return text; // no invalid xml chars detected - return original text
            else
                return result.ToString();

        }

        public static string UniVerseDateString(DateTime val)
        {
            return $"{((int)(val - DateTime.ParseExact("31/12/1967", "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB"))).TotalDays)}";
        }

        public static DateTime DateTimeFromUniVerseOffset(int val)
        {
            try
            {
                return DateTime.ParseExact("31/12/1967", "dd/MM/yyyy", new System.Globalization.CultureInfo("en-GB")).AddDays(val);
            }
            catch(System.ArgumentOutOfRangeException)
            {
                return DateTime.MaxValue;
            }
        }

        public static IEnumerable<CodeDescription> GetOrderEntryCodeGroups(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "OECODEGROUPS",
                "",
                "CODE.GROUP",
                "CODE.G.DESC");
        }

        public static IEnumerable<CodeDescription> GetMarkets(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "CLMARKTYPE",
                "",
                "CODE",
                "DESC");
        }

        public static IEnumerable<CodeDescription> GetCustomerTypes(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLTYPE",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetCustomerClasses(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLCUSCLASS",
                "",
                "CLASS.CODE",
                "DESCRIPTION");
        }

        public static IEnumerable<CodeDescription> GetSalesRepresentatives(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLSEL1",
                "",
                "CODE",
                "TITLE",
                new List<string>() { "EMAIL" });
        }

        public static IEnumerable<CodeDescription> GetSalesAgents(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLSEL2",
                "",
                "CODE",
                "TITLE",
                new List<string>() { "EMAIL" });
        }

        public static IEnumerable<CodeDescription> GetSalesAreas(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLSEL3",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetSalesDivisions(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLDIVISIONS",
                "",
                "DIVISION.CODE.TDW",
                "DIVISION.DESC.TDW");
        }

        public static IEnumerable<CodeDescription> GetVATCodes(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "CFVAT",
                "",
                "CODE",
                "TITLE",
                new List<string>() { "VATRATE" });
        }

        public static IEnumerable<CodeDescription> GetCurrencies(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "XRATE",
                "",
                "CODE",
                "DESC",
                new List<string>() { "RATE", "WEB.DESC" });
        }

        public static IEnumerable<CodeDescription> GetBanks(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "BANK",
                "",
                "CODE",
                "TITLE2");
        }

        public static IEnumerable<CodeDescription> GetCreditControllers(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SL.CONTROLLER",
                "",
                "CONT.CODE",
                "CONT.NAME");
        }

        public static IEnumerable<CodeDescription> GetPaymentTerms(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLTERMS",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetHauliers(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "OEHAULIER",
                "",
                "CODE",
                "DESCRIPTION",
                new List<string>() { "WEBPARCEL", "WEBPARCEL2" });
        }

        public static IEnumerable<CodeDescription> GetCarriageServices(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "OECARRIAGE",
                "",
                "CARRCODE",
                "DESC50",
                new List<string>() { "HAULCODE" });
        }

        public static IEnumerable<CodeDescription> GetDeliveryAddresses(string tradeAccountCode, string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "DELADD",
                $"CUSREF = {tradeAccountCode}",
                "DELREF",
                "DELNAME",
                new List<string>() { "CUSREF" });
        }

        public static IEnumerable<CodeDescription> GetStockLocations(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "STLOCATION",
                "",
                "CODE",
                "TITLE40");
        }

        public static IEnumerable<CodeDescription> GetPromotionCodes(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "OEPROMOTIONS.DES",
                "",
                "PROMOTION.CODE",
                "PROMOTION.DESC");
        }

        public static IEnumerable<CodeDescription> GetSellingCompanies(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SLSELLCO",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetDistributors(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "OEDISTRIBUTOR",
                "",
                "DISTREF",
                "DELNAME");
        }

        public static IEnumerable<CodeDescription> GetLanguages(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "REP.TITLES.DES",
                "",
                "LANGUAGE",
                "LANG.DESCRIPTION");
        }

        public static IEnumerable<CodeDescription> GetProductExclusionLists(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "STPRODEXCLUDE",
                "",
                "EXCLUDE.KEY",
                "EXCLUSION.DESC");
        }

        public static IEnumerable<CodeDescription> GetStockUnits(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "STUNITS",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetOrderCategories(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "ORD.CATAGORY.DES",
                "",
                "ORD.CAT.TYPE.DES",
                "ORD.CAT.DESC.DES");
        }

        public static IEnumerable<CodeDescription> GetStockistCategories(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "SL.STOCKIST.CAT",
                "",
                "CATEGORY",
                "CAT.DESC");
        }

        public static IEnumerable<CodeDescription> GetProductGroups(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount,
                "STPRODGROUP",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetItemTypes(string universeAccount = "MERTEX")
        {
            return GetList(
                universeAccount, 
                "STITEMTYPE",
                "",
                "CODE",
                "TITLE");
        }

        public static IEnumerable<CodeDescription> GetPriceLists(string universeAccount = "MERTEX")
        {
            var result = new List<CodeDescription>();

            using (var ce = new CommandExecutor(universeAccount))
            {
                var uniFile = ce.Session.CreateUniFile("OEDEFN");

                var fields = uniFile.ReadFields("SELLPRICES.STD", new int[] { 3, 4 });

                var codes = fields.Extract(1);
                var descriptions = fields.Extract(2);

                for (var i = 1; i < fields.Dcount(1) + 1; i++)
                {
                    result.Add(new CodeDescription(codes.Extract(1, i).StringValue, descriptions.Extract(1, i).StringValue));
                }
            }

            return result;
        }

        public static IEnumerable<CodeDescription> GetList(
            string universeAccount, 
            string fileName, 
            string selectStatement,
            string codeField, 
            string descriptionField,
            List<string> otherFields = null)
        {
            var result = new List<CodeDescription>();

            using (var qe = new QueryExecutor(universeAccount))
            {
                var xDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run(
                    fileName,
                    selectStatement,
                    $"{codeField} {descriptionField} {(otherFields == null ? "" : String.Join(" ", otherFields))}")));

                xDoc.Descendants("record").ToList().ForEach(c => 
                {
                    var cd = new CodeDescription(
                        c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                        c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value);

                    var ctr = 3;

                    if (otherFields != null)
                    {
                        var d = new Dictionary<string, string>();

                        foreach(var f in otherFields)
                        {
                            var v = c.XPathSelectElement($"descendant::fd[@id='C{ctr++}']").Attribute("value").Value;
                            if (String.IsNullOrEmpty(v))
                                v = null;
                            d.Add(gbTextInfo.ToTitleCase(f.ToLowerInvariant()), v);
                        }

                        cd.Errata = d;
                    }

                    result.Add(cd);
                });
            }

            return result;
        }

        internal static void InitialiseUniProperty(object o, PropertyInfo p)
        {
            if (p.CanWrite)
            {
                //if (p.PropertyType == typeof(string))
                //{
                //    p.SetValue(o, ""); // why do we want to be initialising strings to empty...
                //}
                if (p.PropertyType == typeof(IEnumerable<string>))
                {
                    p.SetValue(o, new List<string>());
                }
                else if (p.PropertyType == typeof(IEnumerable<decimal?>))
                {
                    p.SetValue(o, new List<decimal?>());
                }
                else if (p.PropertyType == typeof(IEnumerable<decimal>))
                {
                    p.SetValue(o, new List<decimal>());
                }
                else if (p.PropertyType == typeof(IEnumerable<long?>))
                {
                    p.SetValue(o, new List<long?>());
                }
                else if (p.PropertyType == typeof(IEnumerable<long>))
                {
                    p.SetValue(o, new List<long>());
                }
                else if (p.PropertyType == typeof(IEnumerable<int?>))
                {
                    p.SetValue(o, new List<int?>());
                }
                else if (p.PropertyType == typeof(IEnumerable<int>))
                {
                    p.SetValue(o, new List<int>());
                }
                else if (p.PropertyType == typeof(IEnumerable<short?>))
                {
                    p.SetValue(o, new List<short?>());
                }
                else if (p.PropertyType == typeof(IEnumerable<short>))
                {
                    p.SetValue(o, new List<short>());
                }
                else if (p.PropertyType == typeof(IEnumerable<byte?>))
                {
                    p.SetValue(o, new List<byte?>());
                }
                else if (p.PropertyType == typeof(IEnumerable<byte>))
                {
                    p.SetValue(o, new List<byte>());
                }
                else if (p.PropertyType == typeof(IEnumerable<bool?>))
                {
                    p.SetValue(o, new List<bool?>());
                }
                else if (p.PropertyType == typeof(IEnumerable<bool>))
                {
                    p.SetValue(o, new List<bool>());
                }
                else if (p.PropertyType == typeof(decimal?))
                {
                    p.SetValue(o, (decimal?)0);
                }
                else if (p.PropertyType == typeof(decimal))
                {
                    p.SetValue(o, (decimal)0);
                }
                else if (p.PropertyType == typeof(long?))
                {
                    p.SetValue(o, (long?)0);
                }
                else if (p.PropertyType == typeof(long))
                {
                    p.SetValue(o, (long)0);
                }
                else if (p.PropertyType == typeof(int?))
                {
                    p.SetValue(o, (int?)0);
                }
                else if (p.PropertyType == typeof(int))
                {
                    p.SetValue(o, (int)0);
                }
                else if (p.PropertyType == typeof(short?))
                {
                    p.SetValue(o, (short?)0);
                }
                else if (p.PropertyType == typeof(short))
                {
                    p.SetValue(o, (short)0u);
                }
                else if (p.PropertyType == typeof(byte?))
                {
                    p.SetValue(o, (byte?)0);
                }
                else if (p.PropertyType == typeof(byte))
                {
                    p.SetValue(o, (byte)0);
                }
                else if (p.PropertyType == typeof(bool?))
                {
                    p.SetValue(o, (bool?)null);
                }
                else if (p.PropertyType == typeof(bool))
                {
                    p.SetValue(o, false);
                }
            }
        }
    }

    // Cheers
    // https://stackoverflow.com/questions/37954376/ignore-jsonignore-attribute-on-serialization-deserialization#answer-37954657
    public class JsonIgnoreAttributeIgnorerContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.Ignored = false; // Here is the magic
            return property;
        }
    }

    public static class StringExtension
    {
        public static string TrimReduceTruncate(this string str, int length)
        {
            if (str == null)
            {
                return "";
            }

            var result = ConvertWhitespacesToSingleSpaces(str).Trim();

            if (result.Length > length)
            {
                return result.Substring(0, length);
            }
            else
            {
                return result;
            }
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        private static readonly System.Globalization.TextInfo TextInfo = new System.Globalization.CultureInfo("en-GB").TextInfo;

        public static string ToCamelCase(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                var regexed = Regex.Replace(value, "([a-z])([A-Z])", "$1 $2").Trim();

                regexed = Regex.Replace(regexed, "([A-Z])([A-Z])", "$1 $2").Trim();

                var split = regexed.Split(new char[] { ' ' }).Select(s => TextInfo.ToTitleCase(s.ToLowerInvariant())).ToList();

                var firstWord = split.First();

                var firstLetter = firstWord[0].ToString().ToLowerInvariant();

                firstWord = firstWord.Substring(1);

                split.RemoveAt(0);

                split.Insert(0, $"{firstLetter}{firstWord}");

                split.ForEach(s =>
                {
                    if (s == "ID")
                    {
                        s = "Id";
                    }
                });

                var result = String.Join("", split);

                if (result.EndsWith("ID"))
                {
                    result = result.TrimEnd(new char[] { 'D' });
                    result += "d";
                }

                return result;
            }
        }
    }

    public static class ListExtension
    {
        public static List<List<T>> Split<T>(IEnumerable<T> collection, int size)
        {
            var chunks = new List<List<T>>();
            var chunkCount = collection.Count() / size;

            if (collection.Count() % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(collection.Skip(i * size).Take(size).ToList());

            return chunks;
        }
    }

    public class StringToJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var o = JsonConvert.DeserializeObject(value.ToString());
            serializer.Serialize(writer, o);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = serializer.Deserialize(reader);
            return JsonConvert.SerializeObject(o);
        }
    }

    public class StringToArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // we're either going to be processing an IEnumerable<T> or a string representing an IEnumerable<T>.

            var type = value.GetType();

            if (type == typeof(string))
            {
                var o = JsonConvert.DeserializeObject<T>(value.ToString());

                serializer.Serialize(writer, o);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JsonConvert.DeserializeObject<T>(reader.Value?.ToString() ?? "[]");// JArray.Parse(reader.Value?.ToString() ?? "[]");
        }
    }

}
