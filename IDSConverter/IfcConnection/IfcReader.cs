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

        private IDS ConvertToXmlFormat(List<Dictionary<string, string>> spaceData)
        {
            IDS specs = new IDS()
            {
                Xmlns = "http://standards.buildingsmart.org/IDS",
                //XmlnsXs = "http://www.w3.org/2001/XMLSchema",
                //XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance",
                //XsiSchemaLocation = "http://standards.buildingsmart.org/IDS ids.xsd",
                Specifications = new List<Specification>()
            };

            foreach (Dictionary<string, string> space in spaceData)
            {
                if (spaceData.IndexOf(space) == 0)
                {
                    continue;
                }

                string areaCode = space["Number"],
                    areaName = space["Name"];

                Specification spec = new Specification()
                {
                    Name = $"{areaCode} {areaName}",
                    IfcVersion = "IFC4",
                    Description = "Areas ...",
                    Instructions = "...",
                    MinOccurs = "0",
                    MaxOccurs = "unbounded",
                    Applicability = new List<Facet>(),
                    Requirements = new List<Facet>()

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

                string keyAttr = spaceData.First()["Number"];
                string keyValueAttr = space["Number"];

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

                spec.Applicability.Add(attrApplicability);

                for (int i = 7; i < space.Keys.Count(); i++)
                {
                    List<string> keys = space.Keys.ToList();
                    string key = keys[i];

                    string name = spaceData.First()[key], value = space[key];

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

        private List<Dictionary<string, string>> GetIfcSpaces(IIfcProject project)
        {
            var spaces = project.Model.Instances.OfType<IIfcSpace>().ToList();
            Dictionary<string, string> spaceDictionary = new Dictionary<string, string>();
            List<Dictionary<string, string>> ifcSpaceData = new List<Dictionary<string, string>>();

            foreach (var space in spaces)
            {
                spaceDictionary.Add("Name", space.LongName);
                spaceDictionary.Add("Number", space.Name);

                //get the properties of a default property set of the space and assign it to propertySet
                var defaultPropertySet = space.IsDefinedBy.OfType<IIfcRelDefinesByProperties>().FirstOrDefault();
                if (defaultPropertySet != null)
                {
                    var properties = space.IsDefinedBy
                        .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                        .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                        .OfType<IIfcPropertySingleValue>();
                    foreach (var property in properties)
                        spaceDictionary.Add(property.Name,property.NominalValue.ToString());
                }
                ifcSpaceData.Add(spaceDictionary);
            }
            return ifcSpaceData;
        }

        public IDS Run()
        {
            Model = IfcStore.Open(File);
            var project = Model.Instances.FirstOrDefault<IIfcProject>();
            IDS ids = ConvertToXmlFormat(GetIfcSpaces(project));

            return ids;
        }
    }
}
