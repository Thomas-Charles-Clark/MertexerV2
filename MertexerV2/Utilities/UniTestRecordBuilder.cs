using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities
{
    public static class UniTestRecordBuilder
    {
        public static void InitialiseTestRecord(Type type, object o)
        {
            type.GetProperties().ToList().ForEach(p =>
            {
                if (p.CanWrite)
                {
                    var indices = p.GetCustomAttributesData().Where(cad => cad.AttributeType == typeof(UniVerseIndexAttribute))
                        .SingleOrDefault()?
                        .ConstructorArguments;

                    if (indices != null)
                    {
                        var fieldIndex = (int)indices[0].Value;

                        var valueIndex = (int)indices[1].Value;

                        var subValueIndex = (int)indices[2].Value;

                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(o, $"Field: {fieldIndex}");
                        }
                        else if (p.PropertyType == typeof(IEnumerable<string>))
                        {
                            p.SetValue(o, new List<string>()
                            {
                                $"Field: {fieldIndex}, ValueMark: 0",
                                $"Field: {fieldIndex}, ValueMark: 1",
                                $"Field: {fieldIndex}, ValueMark: 2",
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<decimal?>))
                        {
                            p.SetValue(o, new List<decimal?>()
                            {
                                fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<decimal>))
                        {
                            p.SetValue(o, new List<decimal>()
                            {
                                fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<long?>))
                        {
                            p.SetValue(o, new List<long?>()
                            {
                                fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<long>))
                        {
                            p.SetValue(o, new List<long>()
                            {
                                fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<int?>))
                        {
                            p.SetValue(o, new List<int?>()
                            {
                                fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<int>))
                        {
                            p.SetValue(o, new List<int>()
                            {
                                fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<short?>))
                        {
                            p.SetValue(o, new List<short?>()
                            {
                                (short)fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<short>))
                        {
                            p.SetValue(o, new List<short>()
                            {
                                (short)fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<byte?>))
                        {
                            p.SetValue(o, new List<byte?>()
                            {
                                (byte)fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<byte>))
                        {
                            p.SetValue(o, new List<byte>()
                            {
                                (byte)fieldIndex
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<bool?>))
                        {
                            p.SetValue(o, new List<bool?>()
                            {
                                true
                            });
                        }
                        else if (p.PropertyType == typeof(IEnumerable<bool>))
                        {
                            p.SetValue(o, new List<bool>()
                            {
                                true
                            });
                        }
                        else if (p.PropertyType == typeof(decimal?))
                        {
                            p.SetValue(o, (decimal?)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(decimal))
                        {
                            p.SetValue(o, (decimal)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(long?))
                        {
                            p.SetValue(o, (long?)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(long))
                        {
                            p.SetValue(o, (long)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(int?))
                        {
                            p.SetValue(o, (int?)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(o, (int)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(short?))
                        {
                            p.SetValue(o, (short?)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(short))
                        {
                            p.SetValue(o, (short)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(byte?))
                        {
                            p.SetValue(o, (byte?)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(byte))
                        {
                            p.SetValue(o, (byte)fieldIndex);
                        }
                        else if (p.PropertyType == typeof(bool?))
                        {
                            p.SetValue(o, (bool?)true);
                        }
                        else if (p.PropertyType == typeof(bool))
                        {
                            p.SetValue(o, true);
                        }
                    }
                }
            });
        }
    }
}
