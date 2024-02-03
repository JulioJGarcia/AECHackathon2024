using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot("ids", Namespace = "http://standards.buildingsmart.org/IDS", IsNullable = false)]
public class IDS
{
    [XmlAttribute("xmlns")]
    public string Xmlns { get; set; }

    [XmlAttribute("xmlns:xs")]
    public string XmlnsXs { get; set; }

    [XmlAttribute("xmlns:xsi")]
    public string XmlnsXsi { get; set; }

    [XmlAttribute("xsi:schemaLocation")]
    public string XsiSchemaLocation { get; set; }

    public Info Info { get; set; }

    public Specifications Specifications { get; set; }
}

public class Info
{
    public string Title { get; set; }
    public string Copyright { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string Date { get; set; }
    public string Purpose { get; set; }
}

public class Specifications
{
    [XmlElement("specification")]
    public List<Specification> Specification { get; set; }
}

public class Specification
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("ifcVersion")]
    public string IfcVersion { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlAttribute("instructions")]
    public string Instructions { get; set; }

    [XmlAttribute("minOccurs")]
    public int MinOccurs { get; set; }

    [XmlAttribute("maxOccurs")]
    public string MaxOccurs { get; set; }

    public Applicability Applicability { get; set; }

    public Requirements Requirements { get; set; }
}

public class Applicability
{
    public Entity Entity { get; set; }
}

public class Entity
{
    public Name Name { get; set; }
}

public class Name
{
    public SimpleValue SimpleValue { get; set; }
}

public class SimpleValue
{
    public string Value { get; set; }
}

public class Requirements
{
    public Attribute Attribute { get; set; }
}

public class Attribute
{
    [XmlAttribute("instructions")]
    public string Instructions { get; set; }

    public Name Name { get; set; }

    public Value Value { get; set; }
}

public class Value
{
    public SimpleValue SimpleValue { get; set; }
}

class Program
{
    static void Main()
    {
        IDS idsData = new IDS
        {
            Xmlns = "http://standards.buildingsmart.org/IDS",
            XmlnsXs = "http://www.w3.org/2001/XMLSchema",
            XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance",
            XsiSchemaLocation = "http://standards.buildingsmart.org/IDS ids.xsd",
            Info = new Info
            {
                Title = "buildingSMART Sample IDS",
                Copyright = "buildingSMART",
                Version = "1.0.0",
                Description = "These are example specifications for those learning how to use IDS",
                Author = "foo@bar.com",
                Date = "2022-01-01",
                Purpose = "Contractual requirements"
            },
            Specifications = new Specifications
            {
                Specification = new List<Specification>
                {
                    // Add specifications here
                }
            }
        };

        // Serialize the IDS instance to XML
        XmlSerializer serializer = new XmlSerializer(typeof(IDS));
        using (TextWriter writer = new StreamWriter("output.xml"))
        {
            serializer.Serialize(writer, idsData);
        }

        // Deserialize the XML back to IDS instance
        using (TextReader reader = new StreamReader("output.xml"))
        {
            IDS deserializedIdsData = (IDS)serializer.Deserialize(reader);
            Console.WriteLine("Deserialized Data:");
            Console.WriteLine($"Title: {deserializedIdsData.Info.Title}");
            // Add more properties as needed
        }
    }
}
