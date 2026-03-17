using IBMU2.UODOTNET;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MertexerV2.Utilities
{
    public static class UniVerseSerializer
    {
        public static string Serialize(PropertyInfo pi, object source)//, string uniVerseAccount = "MERTEX")
        {
            var indices = pi.GetCustomAttributesData().Where(cad => cad.AttributeType == typeof(UniVerseIndexAttribute))
                .SingleOrDefault()?
                .ConstructorArguments;
            
            var uniVerseFieldName = pi.GetCustomAttributesData().Where(cad => cad.AttributeType == typeof(UniVerseFieldNameAttribute))
                .SingleOrDefault()?
                .ConstructorArguments;

            // UniVerse always returns me an empty string if a field is null, so I'll give one back.
            var result = ""; 

            // Is this property decorated with the MultiValue attribute? If yes, it's an IEnumerable<T>
            var multiValue = pi.GetCustomAttributes(typeof(UniVerseMultiValueAttribute), false)
                .Cast<UniVerseMultiValueAttribute>()
                .SingleOrDefault() != null;

            // Does this property have a Conversion attribute? If yes, we'll need to perform multiplication by the given factor. 
            var conversion = pi.GetCustomAttributes(typeof(UniVerseConversionAttribute), false)
                .Cast<UniVerseConversionAttribute>()
                .SingleOrDefault();

            if (multiValue)
            {
                // we're ultimately dealing with strings regardless of the type in our given class
                var values = new List<string>();

                // however, the given property is not guaranteed to be IEnumerable<string>.
                if (pi.PropertyType == typeof(IEnumerable<string>))
                {
                    values = (List<string>)pi.GetValue(source);
                }
                else if (pi.PropertyType == typeof(IEnumerable<decimal>))
                {
                    values = ((List<decimal>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<decimal?>))
                {
                    values = ((List<decimal?>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<long>))
                {
                    values = ((List<long>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<long?>))
                {
                    values = ((List<long?>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<int>))
                {
                    values = ((List<int>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<int?>))
                {
                    values = ((List<int?>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<short>))
                {
                    values = ((List<short>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<short?>))
                {
                    values = ((List<short?>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<byte>))
                {
                    values = ((List<byte>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<byte?>))
                {
                    values = ((List<byte?>)pi.GetValue(source)).Select(p => $"{p}").ToList();
                }
                else if (pi.PropertyType == typeof(IEnumerable<bool>))
                {
					var truthCondition = pi.GetCustomAttributes(typeof(UniVerseBooleanTrueWhen), false)
						.Cast<UniVerseBooleanTrueWhen>()
						.SingleOrDefault();

					if (truthCondition != null)
					{
                        values = ((List<bool>)pi.GetValue(source)).Select(b => b ? truthCondition.Value : "").ToList();
					}
					else
					{
						values = ((List<bool>)pi.GetValue(source)).Select(b => b ? "Y" : "").ToList();
					}
                }

                if (conversion != null && values.Any()) // we have some conversion to deal with...
                {
                    for (var i = 0; i < values.Count; i++)
                    {
                        var val = values[i];

                        // Always treat as decimal when handling conversion
                        if (Decimal.TryParse($"{val}", out decimal v))
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

                #if DEBUG

                //if (uniVerseAccount != "MERTEX" && indices != null && uniVerseFieldName != null)
                //{
                //    var fieldName = (string)uniVerseFieldName[0].Value;

                //    var fieldIndex = (int)indices[0].Value;

                //    var valueIndex = (int)indices[1].Value;

                //    var subValueIndex = (int)indices[2].Value;

                //    values = values.Select((v, i) => $"<{fieldIndex},{i}> \"{fieldName}\"").ToList(); 
                //}
    
                #endif

                // Squish our MV field values back into UniVerse format
                if (values?.Any() == true)
                {
                    // If you nested to the SubValue level, God help you... because I won't.
                    result = String.Join($"{Framework.ValueMark}", values);
                }
            }
            else
            {
                // however, the given property is not guaranteed to be IEnumerable<string>.
                if (pi.PropertyType == typeof(string))
                {
                    result = (string)pi.GetValue(source);
                }
                else if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(decimal?))
                {
                    result = $"{(decimal?)pi.GetValue(source)}";
                }
                else if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                {
                    result = $"{(long?)pi.GetValue(source)}";
                }
                else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                {
                    result = $"{(int?)pi.GetValue(source)}";
                }
                else if (pi.PropertyType == typeof(short) || pi.PropertyType == typeof(short?))
                {
                    result = $"{(short?)pi.GetValue(source)}";
                }
                else if (pi.PropertyType == typeof(byte) || pi.PropertyType == typeof(byte?))
                {
                    result = $"{(byte?)pi.GetValue(source)}";
                }
                else if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool?))
                {
                    result = (bool?)pi.GetValue(source) == true ? "Y" : "";
                }

                if (conversion != null)
                {
                    if (Decimal.TryParse(result, out decimal v))
                    {
                        var match = Framework.UniVerseScalingRegex.Match(conversion.Code);

                        if (match.Success)
                        {
                            if (match.Groups.Count == 3)
                            {
                                double scalingFactor = Convert.ToDouble(match.Groups[2].Value);

                                var scaled = $"{v * (decimal)Math.Pow(10, scalingFactor):0.}";

                                result = scaled;
                            }
                        }
                    }
                }

                #if DEBUG

                //if (uniVerseAccount != "MERTEX" && indices != null && uniVerseFieldName != null)
                //{
                //    var fieldName = (string)uniVerseFieldName[0].Value;

                //    var fieldIndex = (int)indices[0].Value;

                //    var valueIndex = (int)indices[1].Value;

                //    var subValueIndex = (int)indices[2].Value;

                //    result = $"<{fieldIndex}> ";

                //    if (valueIndex > 0)
                //    {
                //        result = $"<{fieldIndex},{valueIndex}>";
                //    }

                //    if (subValueIndex > 0)
                //    {
                //        result = $"<{fieldIndex},{valueIndex},{subValueIndex}>";
                //    }

                //    result += $" \"{fieldName}\"";
                //}

                #endif

            }

            // sanity check
            if (result == null)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName">The type to be deserialized</param>
        /// <param name="key">The key of the record</param>
        /// <param name="input">The UniDynArray holding values</param>
        /// <param name="applyConversions">Whether or not to apply MR type conversions</param>
        /// <param name="selectedSpecificFields">
        /// This one is interesting: if reading the whole file, leave this true: you must reference extracts by UniVerseIndex.
        /// If false, you're reading just certain fields from a file, and the input will have those fields 0...n in order.
        /// </param>
        /// <returns></returns>
        ///  selecting specified fields simply doesn't work, it looks like our UniVerse installation decides to randomly close sockets when doing this in parallel. Boo.
        //public static object Deserialize(string typeName, string key, UniDynArray input, bool applyConversions = true, bool selectedSpecificFields = true)
        public static object Deserialize(string typeName, string key, UniDynArray input, bool applyConversions = true)
        {
            var T = Type.GetType(typeName);

            var result = Activator.CreateInstance(T, new object[] { key });

            //var propertyCounter = 0;

            T.GetProperties().ToList()
                .ForEach(pi =>
            {
                if (pi.CanWrite)
                {
                    var indices = pi.GetCustomAttributesData()
                        .Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
                        .SingleOrDefault()?
                        .ConstructorArguments;

                    // if no UniVerseIndexAttribute was given, the field is not intended to be submitted.
                    if (indices != null)
                    {
                        if ((int)indices[0].Value == 0)
                        {
                            var uniVerseFieldName = pi.GetCustomAttributesData().Where(cad => cad.AttributeType == typeof(UniVerseFieldNameAttribute))
                                .SingleOrDefault()?
                                .ConstructorArguments;
                        }

                        // however if we have an index, we want to read that... field from the dyn array
                        var value = input.Extract((int)indices[0].Value, (int)indices[1].Value, (int)indices[2].Value).StringValue;
                        
                        var multiValue = pi.GetCustomAttributes(typeof(UniVerseMultiValueAttribute), false)
                            .Cast<UniVerseMultiValueAttribute>()
                            .SingleOrDefault() != null;

                        var conversion = pi.GetCustomAttributes(typeof(UniVerseConversionAttribute), false)
                            .Cast<UniVerseConversionAttribute>()
                            .SingleOrDefault();

                        if (multiValue)
                        {
                            var values = new List<string>();

                            var splitMark = Framework.ValueMark;

                            if (value.Contains(Framework.SubValueMark))
                            {
                                splitMark = Framework.SubValueMark;
                            }

                            foreach (var val in value.Split(splitMark))
                            {
                                if (conversion != null && applyConversions == true)
                                {
                                    if (Decimal.TryParse(val, out decimal v))
                                    {
                                        var match = Framework.UniVerseScalingRegex.Match(conversion.Code);

                                        if (match.Success)
                                        {
                                            if (match.Groups.Count == 3)
                                            {
                                                double scalingFactor = Convert.ToDouble(match.Groups[2].Value);

                                                var scaled = $"{v / (decimal)Math.Pow(10, scalingFactor)}";

                                                if (!String.IsNullOrEmpty(scaled))
                                                    values.Add(scaled);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(val))
                                        values.Add(val);
                                }
                            }

                            if (values.Any())
                            {
                                if (pi.PropertyType == typeof(IEnumerable<string>))
                                {
                                    pi.SetValue(result, values);
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<decimal>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? 0M : Convert.ToDecimal(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<decimal?>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (decimal?)null : Convert.ToDecimal(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<long>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? 0L : Convert.ToInt64(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<long?>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (long?)null : Convert.ToInt64(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<int>) )
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? 0 : Convert.ToInt32(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<int?>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (int?)null : Convert.ToInt32(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<short>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (short)0 : Convert.ToInt16(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<short?>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (short?)null : Convert.ToInt16(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<byte>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (byte)0 : Convert.ToByte(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<byte?>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (byte?)null : Convert.ToByte(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<sbyte>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (sbyte)0 : Convert.ToSByte(v)));
                                }
                                else if (pi.PropertyType == typeof(IEnumerable<sbyte?>))
                                {
                                    pi.SetValue(result, values.Select(v => String.IsNullOrEmpty(v) ? (sbyte?)null : Convert.ToSByte(v)));
                                }
                            }
                        }
                        else
                        {
                            if (conversion != null && applyConversions == true)
                            {
                                if (Decimal.TryParse(value, out decimal v))
                                {
                                    var match = Framework.UniVerseScalingRegex.Match(conversion.Code);

                                    if (match.Success)
                                    {
                                        if (match.Groups.Count == 3)
                                        {
                                            double scalingFactor = Convert.ToDouble(match.Groups[2].Value);

                                            value = $"{v / (decimal)Math.Pow(10, scalingFactor)}";
                                        }
                                    }
                                }
                            }

                            if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(result, String.IsNullOrEmpty(value) ? null : value);
                            }
                            else if (pi.PropertyType == typeof(bool))
                            {
                                var b = false;

                                var truthCondition = pi.GetCustomAttributes(typeof(UniVerseBooleanTrueWhen), false)
                                    .Cast<UniVerseBooleanTrueWhen>()
                                    .SingleOrDefault();

                                if (truthCondition != null)
                                {
                                    b = (value == truthCondition.Value);
                                }
                                else
                                {
                                    if (Boolean.TryParse(value, out bool parsed))
                                    {
                                        b = parsed;
                                    }
                                    else
                                    {
                                        switch (value)
                                        {
                                            case "Y":
                                                b = true;
                                                break;
                                            case "N":
                                                b = false;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }

                                pi.SetValue(result, b);
                            }
                            else if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(decimal?))
                            {
                                decimal? d = conversion == null ? (decimal?)null : 0m;

                                if (Decimal.TryParse(value, out decimal parsed))
                                {
                                    d = parsed;
                                }

                                pi.SetValue(result, d);
                            }
                            else if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(long?))
                            {
                                long? i = conversion == null ? (long?)null : 0;

                                if (Int64.TryParse(value, out long parsed))
                                {
                                    i = parsed;
                                }

                                pi.SetValue(result, i);
                            }
                            else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                            {
                                int? i = conversion == null ? (int?)null : 0;

                                if (Int32.TryParse(value, out int parsed))
                                {
                                    i = parsed;
                                }

                                pi.SetValue(result, i);
                            }
                            else if (pi.PropertyType == typeof(short) || pi.PropertyType == typeof(short?))
                            {
                                short? i = conversion == null ? (short?)null : 0;

                                if (Int16.TryParse(value, out short parsed))
                                {
                                    i = parsed;
                                }

                                pi.SetValue(result, i);
                            }
                            else if (pi.PropertyType == typeof(byte) || pi.PropertyType == typeof(byte?))
                            {
                                byte? i = conversion == null ? (byte?)null : 0;

                                if (Byte.TryParse(value, out byte parsed))
                                {
                                    i = parsed;
                                }

                                pi.SetValue(result, i);
                            }
                            else if (pi.PropertyType == typeof(sbyte) || pi.PropertyType == typeof(sbyte?))
                            {
                                sbyte? i = conversion == null ? (sbyte?)null : 0;

                                if (SByte.TryParse(value, out sbyte parsed))
                                {
                                    i = parsed;
                                }

                                pi.SetValue(result, i);
                            }
                        }
                    }
                }
            });

            return result;
        }
    }
}
