using MertexerV2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tester.Utilities
{
    class FieldDescriptor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FieldType { get; set; }
        public string OutputConversion { get; set; }
        public string Derivation { get; set; }
        public decimal Position { get; set; }
        public int Length { get; set; }

        public FieldDescriptor(string name,
            string description,
            string fieldType,
            string outputConversion,
            string derivation,
            decimal position,
            int length) 
        {
            Name = name;
            Description = description;
            FieldType = fieldType;
            OutputConversion = outputConversion;
            Derivation = derivation;
            Position = position;
            Length = length;
        }

        public override string ToString()
        {
            return 
$@"FIELD NAME:     {Name}
POSITION:       {Position}
TYPE:           {FieldType}
LENGTH:         {Length}
OCONV:          {OutputConversion}
DESCRIPTION:    {Description}
DERIVATION:     {Derivation}
VALIDATION:     
PROCESS AFTER:  

";
        }
    }

    class FieldInterrogator
    {
        private string UniVerseFileName { get; set; }
        private string UniVerseAccount { get; set; }
        private string FieldNameFile { get; set; }
        private List<string> FieldNames { get; set; }
        private List<FieldDescriptor> FieldDescriptors { get; set; }

        private string OutputFileName { get; set; }

        private Regex FieldNameRegex = new Regex(@"^([^\s]*)", RegexOptions.Multiline);
        private Regex FieldTypeRegex = new Regex(@"^(001\s)(.)(\s*)$", RegexOptions.Multiline);
        private Regex FieldPositionRegex = new Regex(@"^(002\s)(\d*\.?\d+)(\s*)$", RegexOptions.Multiline);
        private Regex FieldDescriptionRegex = new Regex(@"^(003\s)(.*)$", RegexOptions.Multiline);
        private Regex FieldDerivationRegex = new Regex(@"^(008\s)(.*)$", RegexOptions.Multiline);
        private Regex FieldLengthRegex = new Regex(@"^(010\s)(\d*)(\s*)$", RegexOptions.Multiline);
        private Regex FieldOutputConversionRegex = new Regex(@"^(007\s)(.*)$", RegexOptions.Multiline);

        private StringBuilder OutputBuilder { get; set; }

        public FieldInterrogator(string uniVerseFileName, string fieldNameFile, string uniVerseAccount = "MERTEX")
        {
            UniVerseFileName = uniVerseFileName;
            FieldNameFile = fieldNameFile;
            UniVerseAccount = uniVerseAccount;
            FieldNames = new List<string>();
            FieldDescriptors = new List<FieldDescriptor>();
            OutputBuilder = new StringBuilder();

            OutputFileName = $"{UniVerseFileName} - {System.IO.Path.GetFileName(fieldNameFile)} - Interrogation.txt";

            FetchFieldNames();
            FetchDictionaryRecords();
            WriteOutputFile();
        }

        private void FetchFieldNames()
        {
            using (var fs = System.IO.File.OpenRead(FieldNameFile))
            {
                var line = "";
                var lineCounter = 0;

                using (var sr = new System.IO.StreamReader(fs))
                {
                    while((line = sr.ReadLine()) != null)
                    {
                        if (lineCounter > 0)
                        {
                            FieldNames.Add(FieldNameRegex.Match(line).Groups[0].Value.TrimEnd());
                        }
                        lineCounter++;
                    }
                }
            }
        }

        private void FetchDictionaryRecords()
        {
            foreach (var fieldName in FieldNames)
            {
                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var result = ce.DictionaryItem(UniVerseFileName, fieldName);

                    if (result != $"\"{fieldName}\" not found.\r\n")
                    {
                        FieldDescriptors.Add(new FieldDescriptor(
                            name: fieldName,
                            description: FieldDescriptionRegex.Match(result).Groups[2].Value.Trim(),
                            fieldType: FieldTypeRegex.Match(result).Groups[2].Value.Trim(),
                            outputConversion: FieldOutputConversionRegex.Match(result).Groups[2].Value.Trim(),
                            derivation: FieldDerivationRegex.Match(result).Groups[2].Value.Trim(),
                            position: Decimal.TryParse(FieldPositionRegex.Match(result).Groups[2].Value.Trim(), out decimal d) ? d : 0,
                            length: Int32.TryParse(FieldLengthRegex.Match(result).Groups[2].Value.Trim(), out int i) ? i : 0
                        ));
                    }
                }
            }
        }

        private void WriteOutputFile()
        {
            foreach (var fd in FieldDescriptors.OrderBy(fd => fd.Position).ThenBy(fd => fd.Name))
            {
                OutputBuilder.AppendLine($"{fd}");
            }

            System.IO.File.WriteAllText(OutputFileName, OutputBuilder.ToString());
        }
    }
}
