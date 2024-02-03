using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Run()
        {
            LoadedData = ReadData();
        }

        private void ConvertToXmlFormat(List<Dictionary<string, string>> excelData)
        {
            foreach (Dictionary<string, string> row in excelData)
            {
                if (excelData.IndexOf(row) == 0)
                {
                    continue;
                }
            }
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
