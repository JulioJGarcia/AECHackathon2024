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

        private IDS ConvertToXmlFormat(List<Dictionary<string, string>> ifcData)
        {
            IDS specs = new IDS()
            {
                Xmlns = "http://standards.buildingsmart.org/IDS",
                //XmlnsXs = "http://www.w3.org/2001/XMLSchema",
                //XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance",
                //XsiSchemaLocation = "http://standards.buildingsmart.org/IDS ids.xsd",
                Specifications = new List<Specification>()
            };

            foreach (Dictionary<string, string> row in ifcData)
            {
                if (ifcData.IndexOf(row) == 0)
                {
                    continue;
                }

                for (int i = 7; i < row.Keys.Count(); i++)
                {
                    List<string> keys = row.Keys.ToList();
                    string key = keys[i];

                    string name = ifcData.First()[key], value = row[key];

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

        private Dictionary<string, string> GetIfcSpaces(IIfcProject project)
        {
            var spaces = project.Model.Instances.OfType<IIfcSpace>().ToList();
            Dictionary<string, string> spaceDictionary = new Dictionary<string, string>();
            Dictionary<string, string> propertySet = new Dictionary<string, string>();

            foreach (var space in spaces)
            {
                spaceDictionary.Add(space.LongName, space.Name);
                //get the properties of a default property set of the space and assign it to propertySet
                var defaultPropertySet = space.IsDefinedBy.OfType<IIfcRelDefinesByProperties>().FirstOrDefault();
                if (defaultPropertySet != null)
                {
                    var properties = defaultPropertySet.RelatingPropertyDefinition as IIfcPropertySet;
                    if (properties != null)
                    {
                        foreach (var property in properties.HasProperties)
                        {
                            propertySet.Add(property.Name, property.NominalValue);
                        }
                    }
                }
                
            }
            return spaceDictionary;
        }

        public void Run()
        {
            Model = IfcStore.Open(File);
            var project = Model.Instances.FirstOrDefault<IIfcProject>();
            GetIfcSpaces(project);
        }
    }
}
