using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class OriginCountryFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string OriginCountryJSON)
        {
            var afterCare = JsonConvert.DeserializeObject<OriginCountry>(OriginCountryJSON);

            var fileName = typeof(OriginCountry).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(afterCare.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(afterCare.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(afterCare.Key, 1);

                typeof(OriginCountry).GetProperties().ToList().ForEach(pi =>
                {
                    var indices = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
                                                    .SingleOrDefault()?
                                                    .ConstructorArguments;

                    // if no UniVerseIndexAttribute was given, the field is not intended to be submitted.
                    if (indices != null)
                    {
                        var multiValue = pi.GetCustomAttributes(typeof(UniVerseMultiValueAttribute), false)
                            .Cast<UniVerseMultiValueAttribute>()
                            .SingleOrDefault() != null;

                        var conversion = pi.GetCustomAttributes(typeof(UniVerseConversionAttribute), false)
                            .Cast<UniVerseConversionAttribute>()
                            .SingleOrDefault();

                        string value = "";

                        if (multiValue)
                        {
                            List<string> values = (List<string>)pi.GetValue(afterCare) ?? new List<string>();

                            if (conversion != null && values.Any())
                            {
                                for (var i = 0; i < values.Count; i++)
                                {
                                    var val = values[i];

                                    if (Decimal.TryParse(val, out decimal v))
                                    {
                                        var match = Framework.UniVerseScalingRegex.Match(conversion.Code);

                                        if (match.Success)
                                        {
                                            if (match.Groups.Count == 3)
                                            {
                                                double scalingFactor = Convert.ToDouble(match.Groups[2].Value);

                                                var scaled = $"{v * (decimal)Math.Pow(10, scalingFactor):0.}";

                                                values[i] = scaled;
                                            }
                                        }
                                    }
                                }
                            }

                            value = String.Join($"{Framework.ValueMark}", values);
                        }
                        else
                        {
                            value = (string)pi.GetValue(afterCare);

                            if (conversion != null )
                            {
                                if (Decimal.TryParse(value, out decimal v))
                                {
                                    var match = Framework.UniVerseScalingRegex.Match(conversion.Code);

                                    if (match.Success)
                                    {
                                        if (match.Groups.Count == 3)
                                        {
                                            double scalingFactor = Convert.ToDouble(match.Groups[2].Value);

                                            var scaled = $"{v * (decimal)Math.Pow(10, scalingFactor):0.}";

                                            value = scaled;
                                        }
                                    }
                                }
                            }
                        }

                        // the UniDynArray throws when given null values: we should present with empty strings
                        // so as to be sure that the field receives some value
                        // or not, because... that's not working right.
                        if (value == null)
                        {
                            value = "";
                        }

                        var fieldIndex = (int)indices[0].Value;

                        var valueIndex = (int)indices[1].Value;

                        var subValueIndex = (int)indices[2].Value;

                        var fieldArray = uniVerseFile.ReadField(afterCare.Key, fieldIndex);// new IBMU2.UODOTNET.UniDynArray(ce.Session);

                        if (valueIndex != 0 &&
                            subValueIndex != 0)
                        {
                            fieldArray.Replace(1, valueIndex, subValueIndex, value);
                        }
                        else if (valueIndex != 0)
                        {
                            fieldArray.Replace(1, valueIndex, value);
                        }
                        else
                        {
                            fieldArray.Replace(1, value);
                        }

                        uniVerseFile.WriteField(afterCare.Key, fieldIndex, fieldArray);
                    }
                });

                uniVerseFile.UnlockRecord(afterCare.Key);

            }

            return afterCare.Key;
        }
    }
}
