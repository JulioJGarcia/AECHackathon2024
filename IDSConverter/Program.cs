using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IDSConverter.ExcelConnections;
using Microsoft.CSharp.RuntimeBinder;
using IDSConverter.IfcConnection;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

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

            IDS ids = null;

            switch (extension)
            {
                case ".ifc":
                    ids = RunIfc(file);
                    break;
                case ".xlsx":
                    ids = RunExcel(file);
                    break;
            }

            Console.WriteLine("Enter the path of the output ids file:");
            string idsFileName = Console.ReadLine();

            if (!Regex.Match(idsFileName, @"\.ids$").Success)
            {
                idsFileName = $"{idsFileName}.ids";
            }

            XmlSerializer serializer = new XmlSerializer(typeof(IDS),
                new Type[]
                {
                    typeof(Entity),
                    typeof(Attribute)
                });

            using (TextWriter writer = new StreamWriter(idsFileName))
            {
                serializer.Serialize(writer, ids);
            }

            Console.ReadLine();
        }

        private static IDS RunIfc(string ifcFile)
        {
            IfcReader ifcReader = new IfcReader(ifcFile);

            IDS ids = ifcReader.Run();

            return ids;
        }

        private static IDS RunExcel(string excelFile)
        {
            ExcelReader excelReader = new ExcelReader(excelFile);

            IDS ids = excelReader.Run();

            return ids;
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

                if (!Regex.Match(file, pattern).Success)
                {
                    Console.WriteLine($"The entered file is not compatible (compatible formats: {String.Join(", ", AllowedFileTypes)})");
                    file = null;
                }
            }

            return file;
        }
    }
}