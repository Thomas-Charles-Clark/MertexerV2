using IBMU2.UODOTNET;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.GeneralFunctions
{
    public static partial class Selector<T> where T : UniVerseSerializationBase<string>
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

        public static T Select(string key, bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {
            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniFile.Read($"{key}");

                    var result = UniVerseSerializer.Deserialize(typeof(T).AssemblyQualifiedName, key, extantRecord, applyConversions);

                    if (selectChildEntities)
                    {
                        foreach (var fkProp in typeof(T).GetProperties().Where(p => Attribute.IsDefined(p, typeof(UniVerseForeignKeyAttribute))))
                        {
                            if (fkProp.PropertyType.BaseType == typeof(UniVerseSerializationBase<string>))
                            {
                                var fkAttr = fkProp.GetCustomAttributes(typeof(UniVerseForeignKeyAttribute), false).Cast<UniVerseForeignKeyAttribute>().Single();

                                var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities, batchSize, uniVerseAccount);

                                fkProp.SetValue(result, entity);
                            }
                        }
                    }

//#if DEBUG
//                    Console.ForegroundColor = ConsoleColor.Yellow;

//                    Console.WriteLine(result);

//                    Console.ForegroundColor = ConsoleColor.Green;
//#endif

                    return (T)result;
                }
                catch (UniFileException ufe)
                {
                    return (T)null;
                }
            }
        }

        public static IEnumerable<T> Select(IEnumerable<string> keys, bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {

#if DEBUG
            var previousColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
#endif

            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                var results = new List<T>();

                try
                {
                    if (keys.Any())
                    {
                        var extantRecords = uniFile.ReadRecords(keys.ToArray());

                        foreach (var key in keys)
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

                                        var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                        fkProp.SetValue(result, entity);
                                    }
                                }
                            }

                            results.Add((T)result);
                        }
                    }
                    else
                    {
                        keys = ce.Arbitrary($"LIST {fileName} @ID FMT\"60L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                       .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                       .Distinct()
                       .ToList();
                        
 #if DEBUG
                        sw.Stop();

                        Console.WriteLine($"SelectAll<{typeof(T).Name}>.Execute Stage One returned in {sw.Elapsed}");

                        sw.Reset();

                        sw.Start();
#endif

                        var batches = ListExtension.Split(keys, batchSize);

                        var batchCounter = 0;

                        var locker = new object();

#if DEBUG
                        Console.WriteLine($"SelectAll<{typeof(T).Name}>.Execute Stage One saw {keys.Count()} keys, splitting into {batches.Count} batches of size {batchSize}");
                        Console.WriteLine();
                        Console.WriteLine($"SelectAll<{typeof(T).Name}>.Execute Stage One batch processing:");
                        Console.WriteLine();
#endif

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

                                            var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                            fkProp.SetValue(result, entity);
                                        }
                                    }
                                }

                                thisBatchResults.Add((T)result);
                            }

                            lock (locker)
                            {
                                results.AddRange(thisBatchResults);

#if DEBUG
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write($"Batch: {++batchCounter} - {sw.Elapsed}");
#endif

                            }
                        });
                    }

//#if DEBUG
//                    Console.ForegroundColor = ConsoleColor.Yellow;

//                    Console.WriteLine($"SelectAll<{typeof(T).Name}>.Execute read {results.Count} x {typeof(T).AssemblyQualifiedName} from {fileName}");

//                    Console.ForegroundColor = ConsoleColor.Green;
//#endif

                    return results;
                }
                catch (UniFileException)
                {
                    throw new Exception($"One or more of \"{keys.Count()}\" keys were not found on file \"{fileName}\"");
                }
            }
        }

        public static IEnumerable<T> Search(string searchTerm, IEnumerable<string> fieldsToSearch, string searchType = "startsWith", bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {
            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                var results = new List<T>();

                var query = $"LIST @ID FMT\"20L\" {fileName} ";

                var searchOperator = "";

                switch (searchType)
                {
                    case "startsWith":
                        searchOperator = $"LIKE \"{searchTerm}...\"";
                        break;
                    case "contains":
                        searchOperator = $"LIKE \"...{searchTerm}...\"";
                        break;
                    case "equals":
                        searchOperator = $"EQUALS \"{searchTerm}\"";
                        break;
                    default:
                        break;
                }

                query += $"WITH {String.Join(" OR ", fieldsToSearch.Select(f => $"{f} {searchOperator}"))} ";

                query += "ID.SUPP COL.HDR.SUPP COUNT.SUP";

                var keys = ce.Arbitrary(query)
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

                var batches = ListExtension.Split(keys, batchSize);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    var extantRecords = uniFile.ReadRecords(batch.ToArray());

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

                                    var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                    fkProp.SetValue(result, entity);
                                }
                            }
                        }

                        lock (locker)
                        {
                            results.Add((T)result);
                        }
                    }
                });
                    

#if DEBUG
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine($"Read {results.Count} x {typeof(T).AssemblyQualifiedName} from {fileName}");

                Console.ForegroundColor = ConsoleColor.Green;
#endif

                return results;
            }
        }

        public static IEnumerable<T> Filter(IEnumerable<Tuple<string, string, string>> fieldsAndValues, bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {
            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                var results = new List<T>();

                var query = $"LIST @ID FMT\"60L\" {fileName} ";

                query += $"WITH {String.Join(" AND ", fieldsAndValues.Select(f => $"{f.Item1} {f.Item2} {f.Item3}"))} ";

                query += "ID.SUPP COL.HDR.SUPP COUNT.SUP";

                var keys = ce.Arbitrary(query)
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

                var batches = ListExtension.Split(keys, batchSize);

                var locker = new object();

                Parallel.ForEach(batches, (batch) =>
                {
                    var extantRecords = uniFile.ReadRecords(batch.ToArray());

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

                                    var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                    fkProp.SetValue(result, entity);
                                }
                            }
                        }

                        lock (locker)
                        {
                            results.Add((T)result);
                        }
                    }
                });


#if DEBUG
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine($"Read {results.Count} x {typeof(T).AssemblyQualifiedName} from {fileName}");

                Console.ForegroundColor = ConsoleColor.Green;
#endif

                return results;
            }
        }

        public static IEnumerable<T> SelectAll(bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {

#if DEBUG
            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
#endif
         
            var results = Select(new string[] { }, applyConversions, selectChildEntities, batchSize, uniVerseAccount);

#if DEBUG
            sw.Stop();

            var previousColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();

            Console.WriteLine($"{typeof(T).Name} SelectAll returned in {sw.Elapsed}");

            Console.ForegroundColor = previousColour;

#endif

            return results;
        }

        public static IEnumerable<T> SelectAllNonParallel(bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {
#if DEBUG
            var previousColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
#endif

            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                var results = new List<T>();

                var keys = ce.Arbitrary($"LIST {fileName} @ID FMT\"60L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToList();

                var batches = ListExtension.Split(keys, batchSize);

#if DEBUG
                sw.Stop();

                Console.WriteLine($"SelectAllNonParallel<{typeof(T).Name}>.Execute Stage One returned in {sw.Elapsed}");
                Console.WriteLine();
                Console.WriteLine($"SelectAllNonParallel<{typeof(T).Name}>.Execute Stage One saw {keys.Count()} keys, splitting into {batches.Count()} batches of size {batchSize}");
                Console.WriteLine();
                Console.WriteLine($"SelectAllNonParallel<{typeof(T).Name}>.Execute Stage One batch processing:");
                Console.WriteLine();

                sw.Reset();

                sw.Start();

#endif

                var batchCounter = 0;

                batchCounter = 0;

                for (var i = 0; i < batches.Count; i++)
                {
                    var extantRecords = uniFile.ReadRecords(batches[i].ToArray());

                    var thisBatchResults = new List<T>();

                    foreach (var key in batches[i])
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

                                    var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                    fkProp.SetValue(result, entity);
                                }
                            }
                        }

                        thisBatchResults.Add((T)result);
                    }

#if DEBUG
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"Batch: {++batchCounter} - {sw.Elapsed}");
#endif

                    for (var j = 0; j < thisBatchResults.Count; j++)
                    {
                        yield return thisBatchResults[j];
                    }
                }

#if DEBUG
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine($"SelectAllNonParallel<{typeof(T).Name}>.Execute processed {results.Count} x {typeof(T).AssemblyQualifiedName} from {fileName}");

                Console.ForegroundColor = ConsoleColor.Green;
#endif

            }
        }

        static T ProcessItem(string key, UniDynArray extantRecord, bool applyConversions, bool selectChildEntities)
        {
            var result = UniVerseSerializer.Deserialize(typeof(T).AssemblyQualifiedName, key, extantRecord, applyConversions);

            if (selectChildEntities)
            {
                foreach (var fkProp in typeof(T).GetProperties().Where(p => Attribute.IsDefined(p, typeof(UniVerseForeignKeyAttribute))))
                {
                    if (fkProp.PropertyType.BaseType == typeof(UniVerseSerializationBase<string>))
                    {
                        var fkAttr = fkProp.GetCustomAttributes(typeof(UniVerseForeignKeyAttribute), false).Cast<UniVerseForeignKeyAttribute>().Single();

                        var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                        fkProp.SetValue(result, entity);
                    }
                }
            }

            return (T)result;
        }

        internal static object SelectChildEntity(string typeName, string key = null, bool applyConversions = true, bool selectChildEntities = true, short batchSize = 1000, string uniVerseAccount = "MERTEX")
        {
            if (key == null)
                return null;

            var fileName = Type.GetType(typeName).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var uniFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniFile.Read($"{key}");

                    var result = UniVerseSerializer.Deserialize(typeName, key, extantRecord, applyConversions);

                    if (selectChildEntities)
                    {
                        foreach (var fkProp in Type.GetType(typeName).GetProperties().Where(p => Attribute.IsDefined(p, typeof(UniVerseForeignKeyAttribute))))
                        {
                            if (fkProp.PropertyType.BaseType == typeof(UniVerseSerializationBase<string>))
                            {
                                var fkAttr = fkProp.GetCustomAttributes(typeof(UniVerseForeignKeyAttribute), false).Cast<UniVerseForeignKeyAttribute>().Single();

                                var entity = SelectChildEntity(fkAttr.PropertyType, (string)typeof(T).GetProperty(fkAttr.PropertyName).GetValue(result), applyConversions, selectChildEntities);

                                fkProp.SetValue(result, entity);
                            }
                        }
                    }

#if DEBUG
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine(result);

                    Console.ForegroundColor = ConsoleColor.Green;
#endif

                    return result;
                }
                catch (UniFileException)
                {
                    return null;
                    //throw new Exception($"Record \"{key}\" not found on file \"{fileName}\"");
                }
            }
        }
    }
}