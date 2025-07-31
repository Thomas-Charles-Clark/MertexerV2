using IBMU2.UODOTNET;

using MertexerV2.Models;
using MertexerV2.Models.Product;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities
{
    static class UniUpsert<T> where T : UniVerseSerializationBase<string>
    {
        //public static void Upsert(object source, UniFile uniVerseFile, string uniVerseAccount = "MERTEX")
        //{
        //    var key = ((UniVerseSerializationBase<string>)source).Key;

        //    typeof(T).GetProperties().ToList().ForEach(pi =>
        //    {
        //        var indices = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
        //                                        .SingleOrDefault()?
        //                                        .ConstructorArguments;

        //        // if no UniVerseIndexAttribute was given, the field is not intended to be submitted.
        //        if (indices != null)
        //        {
        //            var value = UniVerseSerializer.Serialize(pi, source, uniVerseAccount);

        //            var fieldIndex = (int)indices[0].Value;

        //            var valueIndex = (int)indices[1].Value;

        //            var subValueIndex = (int)indices[2].Value;

        //            var fieldArray = uniVerseFile.ReadField(key, fieldIndex);// new IBMU2.UODOTNET.UniDynArray(ce.Session);

        //            if (valueIndex != 0 &&
        //                subValueIndex != 0)
        //            {
        //                fieldArray.Replace(1, valueIndex, subValueIndex, value);
        //            }
        //            else if (valueIndex != 0)
        //            {
        //                fieldArray.Replace(1, valueIndex, value);
        //            }
        //            else
        //            {
        //                fieldArray.Replace(1, value);
        //            }

        //            uniVerseFile.WriteField(key, fieldIndex, fieldArray);
        //        }
        //    });
        //}

        public static void Upsert(string specificJson, UniFile uniVerseFile, string uniVerseAccount = "MERTEX")
        {
            var parsed = JObject.Parse(specificJson);

            var allProperties = typeof(T).GetProperties().ToList();
            
            var source = JsonConvert.DeserializeObject<T>(specificJson);

            specificJson = JsonConvert.SerializeObject(source, Framework.JsonSettingsIndentedIgnoreNulls);

            var t = typeof(T);

            var keyProperty = t.GetProperty("Key");

            var hasRequiredAttribute = Attribute.IsDefined(keyProperty, typeof(JsonRequiredAttribute));

            if (hasRequiredAttribute && String.IsNullOrEmpty(source.Key))
            {
                throw new Exception("Key not found in source. Please provide either Key or Code. One of either must be present, but not both.");
            }

            foreach (var jprop in parsed.Properties())
            {
                var propertyName = jprop.Name;

                //Console.WriteLine(propertyName);

                var pi = UpsertProperty(allProperties, jprop.Name, source, uniVerseFile);//, uniVerseAccount);

                if (pi != null)
                {
                    var alsoUpdates = pi.GetCustomAttributes().SingleOrDefault(cad => cad.GetType() == typeof(JsonAlsoUpdatesAttribute)) as JsonAlsoUpdatesAttribute;

                    if (alsoUpdates != null)
                    {
                        var alsoUpdatesList = alsoUpdates.Fields;

                        foreach (var propName in alsoUpdatesList)
                        {
                            UpsertProperty(allProperties, propName, source, uniVerseFile);//, uniVerseAccount);
                        }
                    }
                }
            }
        }

        private static PropertyInfo UpsertProperty(IEnumerable<PropertyInfo> allProperties, string propertyName, T source, UniFile uniVerseFile)//, string uniVerseAccount)
        {
            // Sometimes have to use JSON indirection to fill properties from other properties

            // If you're debugging this and your propertyName is "Code" and you're wondering
            // "why the hell isn't it updating Code!" that'd be because YOU SHOULD NOT BE ATTEMPTING TO UPDATE A FIELD NAMED CODE.
            // "Code" is the JSON alias for Key: the outside-world-facing key property for almost all the things.
            // A few classes override Key themselves for this purpose, such as TradeAccount with TradeAccountCode.

            var pi = allProperties.FirstOrDefault(p => 
                p.GetCustomAttributesData()
                 .SingleOrDefault(cad => cad.AttributeType == typeof(JsonPropertyAttribute))
                 ?.ConstructorArguments
                 ?.FirstOrDefault().Value.ToString() == propertyName);

            if (pi == null)
            {
                pi = allProperties.FirstOrDefault(p => p.Name == propertyName);
            }

            // Ah... yes.
            // Because the JSON doesn't contain "CreatedDate", we aren't attempting to update it.

            if (pi != null)
            {
                var indices = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
                                            .SingleOrDefault()?
                                            .ConstructorArguments;

                // if no UniVerseIndexAttribute was given, the field is not intended to be submitted.
                if (indices != null)
                {
                    var value = UniVerseSerializer.Serialize(pi, source);//, uniVerseAccount);

                    var fieldIndex = (int)indices[0].Value;

                    var valueIndex = (int)indices[1].Value;

                    var subValueIndex = (int)indices[2].Value;

                    var fieldArray = uniVerseFile.ReadField(source.Key ?? "", fieldIndex);// new IBMU2.UODOTNET.UniDynArray(ce.Session);

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

                    uniVerseFile.WriteField(source.Key, fieldIndex, fieldArray);
                }
            }

            return pi;
        }
    }
}
