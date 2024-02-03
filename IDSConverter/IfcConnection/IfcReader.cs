using IDSConverter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace IDSConverter.IfcConnection
{
    public class IfcReader
    {
        public string File { get; }

        public IfcStore Model { get; private set; }

        public IfcReader(string file)
        {
            File = file;
        }

        private IDS ConvertToXmlFormat(List<Dictionary<string, string>> excelData)
        {
            IDS specs = new IDS()
            {
                Xmlns = "http://standards.buildingsmart.org/IDS",
                XmlnsXs = "http://www.w3.org/2001/XMLSchema",
                XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance",
                XsiSchemaLocation = "http://standards.buildingsmart.org/IDS ids.xsd",
                Specifications = new List<Specification>()
            };

            foreach (Dictionary<string, string> row in excelData)
            {
                if (excelData.IndexOf(row) == 0)
                {
                    continue;
                }

                for (int i = 7; i < row.Keys.Count(); i++)
                {
                    List<string> keys = row.Keys.ToList();
                    string key = keys[i];

                    string name = excelData.First()[key], value = row[key];

                    string areaName = row[Level3DesignationColumn];

                    Specification spec = new Specification()
                    {
                        Name = areaName,
                        IfcVersion = "IFC4",
                        Description = "Areas ...",
                        Instructions = "...",
                        MinOccurs = "0",
                        MaxOccurs = "unbounded",
                        Applicability = new Applicability()
                        {
                            Entity = new Entity()
                            {
                                Name = new Name()
                                {
                                    SimpleValue = new SimpleValue()
                                    {
                                        Value = "IFCSPACE"
                                    }
                                }
                            }
                        },
                        Requirements = new Requirement()
                        {
                            Attribute = new Attribute()
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
                            }
                        }
                    };

                    specs.Specifications.Add(spec);
                };
            }

            return specs;
        }

        public class SpacesSample
        {
            public void Show()
            {
                const string file = "C:\\Users\\modelical\\Documents\\AECHackathon2024\\IDSConverter\\Samples\\Clinic_Architectural.ifc";

                using (var model = IfcStore.Open(file))
                {
                    var project = model.Instances.FirstOrDefault<IIfcProject>();
                    GetIfcSpaces(project);
                }
            }

            private Dictionary<string,string> GetIfcSpaces(IIfcProject project)
            {
                var spaces = project.Model.Instances.OfType<IIfcSpace>().ToList();
                Dictionary<string, string> spaceDictionary = new Dictionary<string, string>();
                foreach (var space in spaces)
                {
                    spaceDictionary.Add(space.LongName, space.Name);
                }
                return spaceDictionary;
            }
        }

        public void Run()
        {
            Model = IfcStore.Open(File);
        }
    }
}
