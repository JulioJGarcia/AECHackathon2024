using IDSConverter.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Isam.Esent.Interop.EnumeratedColumn;
using System.Xml.Linq;

namespace IDSConverter.ExcelConnections
{
    public class ExcelReader
    {
        public string File { get; }

        public List<Dictionary<string, string>> LoadedData { get; private set; }

        public string AbbreviationColumn = "A";

        public string Level1DesignationCodeColumn = "B";

        public string Level1DesignationColumn = "C";

        public string Level2DesignationCodeColumn = "D";

        public string Level2DesignationColumn = "E";

        public string Level3DesignationCodeColumn = "F";

        public string Level3DesignationColumn = "G";

        public ExcelReader(string file)
        {
            File = file;
        }

        public IDS Run()
        {
            LoadedData = ReadData();

            return ConvertToXmlFormat(LoadedData);
        }

        private IDS ConvertToXmlFormat(List<Dictionary<string, string>> excelData)
        {
            IDS specs = new IDS()
            {
                Xmlns = "http://standards.buildingsmart.org/IDS",
                //XmlnsXs = "http://www.w3.org/2001/XMLSchema",
                //XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance",
                //XsiSchemaLocation = "http://standards.buildingsmart.org/IDS ids.xsd",
                Specifications = new List<Specification>()
            };

            foreach (Dictionary<string, string> row in excelData)
            {
                if (excelData.IndexOf(row) == 0)
                {
                    continue;
                }

                string areaCode = row[Level3DesignationCodeColumn],
                    areaName = row[Level3DesignationColumn];

                Specification spec = new Specification()
                {
                    Name = $"{areaCode} {areaName}",
                    IfcVersion = "IFC4",
                    Description = "Areas ...",
                    Instructions = "...",
                    MinOccurs = "0",
                    MaxOccurs = "unbounded",
                    Applicability = new List<Entity>(),
                    Requirements = new List<Attribute>()

                };

                Entity entityType = new Entity()
                {
                    Name = new Name()
                    {
                        SimpleValue = new SimpleValue()
                        {
                            Value = "IFCSPACE"
                        }
                    }
                };

                spec.Applicability.Add(entityType);

                string keyAttr = excelData.First()[Level3DesignationCodeColumn];
                string keyValueAttr = row[Level3DesignationCodeColumn];

                Attribute attrApplicability = new Attribute()
                {
                    Name = new Name()
                    {
                        SimpleValue = new SimpleValue() { Value = keyAttr }
                    },

                    Value = new Value()
                    {
                        SimpleValue = new SimpleValue() { Value = keyValueAttr }
                    }
                };

                //spec.Applicability.Add(attrApplicability);

                for (int i = 7; i < row.Keys.Count(); i++)
                {
                    List<string> keys = row.Keys.ToList();
                    string key = keys[i];

                    string name = excelData.First()[key], value = row[key];

                    Attribute attr = new Attribute()
                    {
                        Instructions = $"For '{areaName}' areas the value of field '{name}' must match '{value}'",
                        Name = new Name()
                        {
                            SimpleValue = new SimpleValue() { Value = name }
                        },

                        Value = new Value()
                        {
                            SimpleValue = new SimpleValue() { Value = value }
                        }
                    };

                    spec.Requirements.Add(attr);
                };

                specs.Specifications.Add(spec);
            }

            return specs;
        }


        private List<Dictionary<string, string>> ReadData()
        {
            ExcelConnection excelConnection;

            List<string> sheets;

            // Read the excel file and the source sheet
            while (true)
            {
                try
                {
                    excelConnection = new ExcelConnection(File);

                    sheets = excelConnection.GetSheetNames();

                    if (sheets == null || sheets.Count() == 0)
                    {
                        throw new Exception("\nUnable to read excel sheets, Check that the excel is closed");
                    }

                    break;
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred reading excel, please check that the excel file is accessible (e: {ex.Message})");
                }
            }

            string selectedSheet;

            while (true)
            {
                Console.WriteLine("\nRead the following excel sheets:");

                Console.Write($"- {String.Join("\n- ", sheets)} \n");

                Console.WriteLine("\nSelect the sheet you want to read from or press 'exit' to exit.");

                selectedSheet = Console.ReadLine();

                if (selectedSheet.Trim().ToLower() == "exit")
                {
                    return null;
                }

                if (!sheets.Contains(selectedSheet ?? String.Empty))
                {
                    Console.WriteLine($"The sheet '{selectedSheet}' does not exist in excel, please type the name of another sheet:");
                }

                break;
            }

            Console.WriteLine($"\nSelected sheet '{selectedSheet}'");

            // Read the sheet
            List<Dictionary<string, string>> excelData = excelConnection.GetTableByAsDictionary(selectedSheet);

            return excelData;
        }
    }
}
