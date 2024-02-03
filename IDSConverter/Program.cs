using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IDSConverter.ExcelConnections;
using Microsoft.CSharp.RuntimeBinder;
using IDSConverter.IfcConnection;

namespace IDSConverter
{
    internal class Program
    {
        public static string[] AllowedFileTypes =
        {
            ".ifc",
            ".xlsx"
        };

        static void Main(string[] args)
        {
           
            string file = ReadFile();

            string extension = Regex.Match(file, @"\.[^.]*$").Value;

            switch(extension)
            {
                case ".ifc":
                    RunIfc(file);
                    break;
                case ".xlsx":
                    RunExcel(file);
                    break;
            }

            Console.ReadLine();
        }

        private static void RunIfc(string ifcFile)
        {
            IfcReader ifcReader = new IfcConnection.IfcReader(ifcFile);
        }

        private static void RunExcel(string excelFile)
        {
            ExcelReader excelReader = new ExcelReader(excelFile);

            excelReader.Run();
        }

        private static string ReadFile()
        {
            string message = "Please provide a file or enter 'exit' to exit:";

            string file = null;

            while (file == null)
            {
                Console.WriteLine(message);

                file = Console.ReadLine();

                if (file.Trim().ToLower() == "exit")
                {
                    return null;
                }

                if (!File.Exists(file))
                {
                    Console.WriteLine("The file provided does not exist");
                }

                string pattern = "(" + String.Join("|", AllowedFileTypes) + ")$";

                if(!Regex.Match(file, pattern).Success)
                {
                    Console.WriteLine($"The entered file is not compatible (compatible formats: {String.Join(", ", AllowedFileTypes)})");
                    file = null;
                }
            }

            return file;
        }
    }
}