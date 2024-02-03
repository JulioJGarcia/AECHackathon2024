using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace IDSConverter
{
    class ifcReader
    {
        class SpatialStructureExample
        {
            public static void Show()
            {
                const string file = "./Samples/";

                using (var model = IfcStore.Open(file))
                {
                    var project = model.Instances.FirstOrDefault<IIfcProject>();
                    PrintHierarchy(project, 0);
                }
            }

            private static void PrintHierarchy(IIfcObjectDefinition o, int level)
            {
                Console.WriteLine(string.Format("{0}{1} [{2}]", GetIndent(level), o.Name, o.GetType().Name));

                //only spatial elements can contain building elements
                var spatialElement = o as IIfcSpatialStructureElement;
                if (spatialElement != null)
                {
                    //using IfcRelContainedInSpatialElement to get contained elements
                    var containedElements = spatialElement.ContainsElements.SelectMany(rel => rel.RelatedElements);
                    foreach (var element in containedElements)
                        Console.WriteLine(string.Format("{0}    ->{1} [{2}]", GetIndent(level), element.Name, element.GetType().Name));
                }

                //using IfcRelAggregares to get spatial decomposition of spatial structure elements
                foreach (var item in o.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
                    PrintHierarchy(item, level + 1);
            }

            private static string GetIndent(int level)
            {
                var indent = "";
                for (int i = 0; i < level; i++)
                    indent += "  ";
                return indent;
            }
        }

        static void Main()
        {
            try
            {
                using (var model = IfcStore.Open("path/to/your/ifc/file.ifc"))
                {
                    var spaceCount = model.Instances.OfType<IfcSpace>().Count();
                    Console.WriteLine($"Number of IfcSpaces in the model: {spaceCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
