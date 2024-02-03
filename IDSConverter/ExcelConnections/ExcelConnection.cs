using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IDSConverter.ExcelConnections
{
    public class ExcelConnection
    {
        public string ExcelPath { get; } = String.Empty;

        public string log { get; private set; } = String.Empty;

        public bool IsValid { get; private set; } = true;

        private DataSet DataSet { get; set; }

        public ExcelConnection(string filePath)
        {
            ExcelPath = filePath;

            if (!File.Exists(ExcelPath))
            {
                log += "File " + ExcelPath + " doesn't exist";
                IsValid = false;
                return;
            }

            try
            {
                using (FileStream stream = File.Open(ExcelPath, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader excelDataReader = ExcelReaderFactory.CreateReader(stream);

                    DataSet = excelDataReader.AsDataSet();
                }
            }
            catch (Exception e)
            {
                log += e.ToString();
                IsValid = false;
                return;
            }

            IsValid = true;
        }

        public List<string> GetSheetNames()
        {
            List<string> result = new List<string>();

            if (!IsValid || DataSet == null || DataSet.Tables == null)
            {
                return result;
            }

            foreach (DataTable table in DataSet.Tables)
            {
                result.Add(table.TableName);
            }

            return result;
        }

        /// <summary>
        /// Returns all the data in the excel sheet.
        /// List index: row index
        /// Dictionary key: column (eg. A,B,C...AA)
        /// Dictionary value: cell value
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="columnsLimit"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetTableByAsDictionary(string sheetName)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            if (!IsValid || DataSet == null || DataSet.Tables == null || !DataSet.Tables.Contains(sheetName))
            {
                throw new Exception("No se pudo leer la hoja seleccionada.");
            }

            DataTable selectedTable = null;

            foreach (DataTable table in DataSet.Tables)
            {
                if (table.TableName != sheetName)
                {
                    continue;
                }

                selectedTable = table;
                break;
            }

            foreach (DataRow row in selectedTable.Rows)
            {
                Dictionary<string, string> rowDictionary = new Dictionary<string, string>();

                List<string> cells = row.ItemArray.Select(x => x.ToString()).ToList();

                for (int j = 0; j < cells.Count(); j++)
                {
                    string columnIndex = ConverF1FormatToAFormat(String.Format("F{0}", j + 1));

                    rowDictionary.Add(columnIndex, cells[j]);
                }

                result.Add(rowDictionary);
            }

            return result;
        }

        public static string ConverF1FormatToAFormat(string f1)
        {
            Match formatMatch = Regex.Match(f1, "^F\\d*$");

            if (!formatMatch.Success)
            {
                return null;
            }

            Match numberMatch = Regex.Match(f1, "\\d*$");

            int.TryParse(numberMatch.Value, out int columnIndex);

            if (columnIndex < 1)
            {
                return null;
            }

            string columnName = "";

            while (columnIndex > 0)
            {
                int modulo = (columnIndex - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnIndex = (columnIndex - modulo) / 26;
            }

            return columnName;
        }

        public static int ConvertAFormatToInt(string a)
        {
            //starting from 1, to be used as F1..
            Match formatMatch = Regex.Match(a, "^[A-Z]+$");

            if (!formatMatch.Success)
            {
                return -1;
            }

            int index = 0;

            List<char> chars = a.ToList();

            chars.Reverse();

            for (int i = 0; i < chars.Count(); i++)
            {
                int number = chars[i] - 'A' + 1;

                index += number * (int)Math.Pow(26, i);
            }

            return index;
        }
    }
}
