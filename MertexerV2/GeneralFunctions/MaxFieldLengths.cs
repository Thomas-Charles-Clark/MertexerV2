using IBMU2.UODOTNET;

using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MertexerV2.GeneralFunctions
{
    public static class MaxFieldLengths<T> where T : UniVerseSerializationBase<string>
    {
        private static string fileName = typeof(T).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                           .Cast<UniVerseFileNameAttribute>()
                           .Single()
                           .FileName;

        private static string[] fieldNames = typeof(T)
                .GetProperties()
                .SelectMany(p => p.GetCustomAttributes(typeof(UniVerseFieldNameAttribute), false)
                .Cast<UniVerseFieldNameAttribute>()
                .Select(f => f.Name))
                .ToArray();

        private static int[] fieldIndices = typeof(T)
                .GetProperties()
                .SelectMany(p => p.GetCustomAttributes(typeof(UniVerseIndexAttribute), false)
                .Cast<UniVerseIndexAttribute>()
                .Select(f => f.FieldIndex))
                .ToArray();

        public static T Execute(string[] keys = null, bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {

#if DEBUG
            var previousColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
#endif

            var results = SelectForMaxFieldLengths(keys, applyConversions, selectChildEntities, batchSize, uniVerseAccount).ToList();

#if DEBUG
            sw.Stop();

            Console.WriteLine($"MaxFieldLength<{typeof(T).Name}>.Execute Stage One returned in {sw.Elapsed}");

            sw.Reset();

            sw.Start();
#endif

            var dummy = Activator.CreateInstance<T>();

            results.ForEach(r =>
            {
                typeof(T).GetProperties().ToList().ForEach(p =>
                {
                    if (p.CanWrite)
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            var currentValue = p.GetValue(r)?.ToString() ?? "";

                            var dummyValue = p.GetValue(dummy)?.ToString() ?? "";

                            if (currentValue.Length > dummyValue.Length)
                            {
                                p.SetValue(dummy, currentValue);
                            }
                        }
                        else if (p.PropertyType == typeof(IEnumerable<string>))
                        {
                            var rv = p.GetValue(r) as List<string>;
                            var dv = p.GetValue(dummy) as List<string>;

                            var rvi = 0;
                            var dvi = 0;

                            if (rv != null && rv.Any())
                            {
                                rvi = rv.Select(i =>
                                {
                                    var k = 0;

                                    if (Int32.TryParse(i, out int j))
                                    {
                                        k = j;
                                    }

                                    return k;
                                }).Max();
                            }

                            if (dv != null && dv.Any())
                            {
                                dvi = dv.Select(i =>
                                {
                                    var k = 0;

                                    if (Int32.TryParse(i, out int j))
                                    {
                                        k = j;
                                    }

                                    return k;
                                }).Max();
                            }

                            if (rvi > dvi)
                            {
                                if (!dv.Any())
                                {
                                    dv.Add($"{rvi}");
                                }
                                else
                                {
                                    dv = new List<string>() { $"{rvi}" };
                                }

                                p.SetValue(dummy, dv);
                            }
                        }
                        else if (p.PropertyType == typeof(decimal?) || p.PropertyType == typeof(decimal))
                        {
                            var rv = p.GetValue(r) as decimal?;
                            var dv = p.GetValue(dummy) as decimal?;

                            if (rv != null && rv > dv)
                            {
                                p.SetValue(dummy, rv);
                            }
                        }
                        else if (p.PropertyType == typeof(long?) || p.PropertyType == typeof(long))
                        {
                            var rv = p.GetValue(r) as long?;
                            var dv = p.GetValue(dummy) as long?;

                            if (rv != null && rv > dv)
                            {
                                p.SetValue(dummy, rv);
                            }
                        }
                        else if (p.PropertyType == typeof(int?) || p.PropertyType == typeof(int))
                        {
                            var rv = p.GetValue(r) as int?;
                            var dv = p.GetValue(dummy) as int?;

                            if (rv != null && rv > dv)
                            {
                                p.SetValue(dummy, rv);
                            }
                        }
                        else if (p.PropertyType == typeof(short?) || p.PropertyType == typeof(short))
                        {
                            var rv = p.GetValue(r) as short?;
                            var dv = p.GetValue(dummy) as short?;

                            if (rv != null && rv > dv)
                            {
                                p.SetValue(dummy, rv);
                            }
                        }
                        else if (p.PropertyType == typeof(byte?) || p.PropertyType == typeof(byte))
                        {
                            var rv = p.GetValue(r) as byte?;
                            var dv = p.GetValue(dummy) as byte?;

                            if (rv != null && rv > dv)
                            {
                                p.SetValue(dummy, rv);
                            }
                        }
                    }
                });
            });

            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                if (p.CanWrite)
                {
                    if (p.PropertyType == typeof(string))
                    {
                        if (p.GetValue(dummy) != null)
                        {
                            p.SetValue(dummy, $"{((string)p.GetValue(dummy)).Length}");
                        }
                    }
                }
            });

#if DEBUG
                sw.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine($"MaxFieldLength<{typeof(T).Name}>.Execute Stage Two returned in {sw.Elapsed}");

            Console.ForegroundColor = previousColour;

#endif

            return dummy;
        }

        public static IEnumerable<T> SelectForMaxFieldLengths(IEnumerable<string> keys = null, bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {
            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                var results = new List<T>();

                try
                {
                    if (keys == null || !keys.Any())
                    {
                        keys = ce.Arbitrary($"LIST {fileName} @ID FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                           .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                           .Distinct()
                            .ToList();

                        Console.WriteLine($"Key Field Max Length: {keys.Select(k => k.Length).Max()}");
                    }

                    var batches = ListExtension.Split(keys, batchSize);

#if DEBUG
                    Console.WriteLine($"MaxFieldLength<{typeof(T).Name}>.Execute Stage One saw {keys.Count()} keys, splitting into {batches.Count} batches of size {batchSize}");
                    Console.WriteLine(); 
                    Console.WriteLine($"MaxFieldLength<{typeof(T).Name}>.Execute Stage One batch processing:");
                    Console.WriteLine();
#endif

                    var locker = new object();

                    var sw = new System.Diagnostics.Stopwatch();

                    sw.Start();

                    var batchCounter = 0;

                    Parallel.ForEach(batches, (batch) =>
                    {
                        var extantRecords = uniFile.ReadRecords(batch.ToArray());

                        var thisBatchResults = new List<T>();

                        foreach (var key in batch)
                        {
                            var extantRecord = extantRecords.GetRecord(key).Record;

                            var result = UniVerseSerializer.Deserialize(typeof(T).AssemblyQualifiedName, key, extantRecord, applyConversions);

                            if (selectChildEntities)
                            {
                                foreach (var fkProp in typeof(T).GetProperties().Where(p => Attribute.IsDefined(p, typeof(UniVerseForeignKeyAttribute))))
                                {
                                    if (fkProp.PropertyType.BaseType == typeof(UniVerseSerializationBase<string>))
                                    {
                                        var fkAttr = fkProp.GetCustomAttributes(typeof(UniVerseForeignKeyAttribute), false).Cast<UniVerseForeignKeyAttribute>().Single();

                                        var entity = Selector<T>.SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                        fkProp.SetValue(result, entity);
                                    }
                                }
                            }

                            thisBatchResults.Add((T)result);
                        }

                        var dummy = Activator.CreateInstance<T>();

                        thisBatchResults.ForEach(r =>
                        {
                            typeof(T).GetProperties().ToList().ForEach(p =>
                            { 
                                if (p.CanWrite)
                                {
                                    if (p.PropertyType == typeof(string))
                                    {
                                        var currentValue = p.GetValue(r)?.ToString() ?? "";

                                        var dummyValue = p.GetValue(dummy)?.ToString() ?? "";

                                        if (currentValue.Length > dummyValue.Length)
                                        {
                                            p.SetValue(dummy, currentValue);
                                        }

                                        //var rv = p.GetValue(r) as string;
                                        //var dv = p.GetValue(dummy) as string;

                                        //if (rv != null && (rv.Length > (dv?.Length ?? 0)))
                                        //{
                                        //    p.SetValue(dummy, $"{rv.Length}");
                                        //}
                                    }
                                    else if (p.PropertyType == typeof(IEnumerable<string>))
                                    {
                                        var rv = p.GetValue(r) as List<string>;
                                        var dv = p.GetValue(dummy) as List<string>;

                                        if (rv != null && rv.Any())
                                        {
                                            dv.Add($"{rv.Select(q => q.Length).Max()}");
                                        }
                                    }
                                    else if (p.PropertyType == typeof(decimal?) || p.PropertyType == typeof(decimal))
                                    {
                                        var rv = p.GetValue(r) as decimal?;
                                        var dv = p.GetValue(dummy) as decimal?;

                                        if (rv != null && rv > dv)
                                        {
                                            p.SetValue(dummy, rv);
                                        }
                                    }
                                    else if (p.PropertyType == typeof(long?) || p.PropertyType == typeof(long))
                                    {
                                        var rv = p.GetValue(r) as long?;
                                        var dv = p.GetValue(dummy) as long?;

                                        if (rv != null && rv > dv)
                                        {
                                            p.SetValue(dummy, rv);
                                        }
                                    }
                                    else if (p.PropertyType == typeof(int?) || p.PropertyType == typeof(int))
                                    {
                                        var rv = p.GetValue(r) as int?;
                                        var dv = p.GetValue(dummy) as int?;

                                        if (rv != null && rv > dv)
                                        {
                                            p.SetValue(dummy, rv);
                                        }
                                    }
                                    else if (p.PropertyType == typeof(short?) || p.PropertyType == typeof(short))
                                    {
                                        var rv = p.GetValue(r) as short?;
                                        var dv = p.GetValue(dummy) as short?;

                                        if (rv != null && rv > dv)
                                        {
                                            p.SetValue(dummy, rv);
                                        }
                                    }
                                    else if (p.PropertyType == typeof(byte?) || p.PropertyType == typeof(byte))
                                    {
                                        var rv = p.GetValue(r) as byte?;
                                        var dv = p.GetValue(dummy) as byte?;

                                        if (rv != null && rv > dv)
                                        {
                                            p.SetValue(dummy, rv);
                                        }
                                    }
                                }
                            });
                        });

                        lock (locker)
                        {
                            results.Add(dummy);

#if DEBUG
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($"Batch: {++batchCounter} - {sw.Elapsed}");
#endif

                        }
                    });

#if DEBUG
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine();

                    Console.WriteLine($"Read {results.Count} x {typeof(T).AssemblyQualifiedName} from {fileName}");

                    Console.ForegroundColor = ConsoleColor.Green;
#endif

                    return results;
                }
                catch (UniFileException)
                {
                    throw new Exception($"One or more of \"{keys.Count()}\" keys were not found on file \"{fileName}\"");
                }
            }
        }
    }
}
