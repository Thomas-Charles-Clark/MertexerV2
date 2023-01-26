using IBMU2.UODOTNET;

using MertexerV2.Models.Product;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities
{
    static class UniUpsert<T> where T : UniVerseSerializationBase<string>
    {
        public static void Upsert(object source, UniFile uniVerseFile)
        {
            var key = ((UniVerseSerializationBase<string>)source).Key;

            typeof(T).GetProperties().ToList().ForEach(pi =>
            {
                var indices = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
                                                .SingleOrDefault()?
                                                .ConstructorArguments;

                // if no UniVerseIndexAttribute was given, the field is not intended to be submitted.
                if (indices != null)
                {
                    var value = UniVerseSerializer.Serialize(pi, source);

                    var fieldIndex = (int)indices[0].Value;

                    var valueIndex = (int)indices[1].Value;

                    var subValueIndex = (int)indices[2].Value;

                    var fieldArray = uniVerseFile.ReadField(key, fieldIndex);// new IBMU2.UODOTNET.UniDynArray(ce.Session);

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

                    uniVerseFile.WriteField(key, fieldIndex, fieldArray);
                }
            });
        }
    }
}
