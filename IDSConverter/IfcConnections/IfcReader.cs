using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc4.Interfaces;

namespace IDSConverter
{
    public class ifcReader
    {
        public class SpatialStructureExample
        {
            public static void Show()
            {
                const string file = "C:\\Users\\modelical\\Documents\\AECHackathon2024\\IDSConverter\\Samples\\Clinic_Architectural.ifc";

                using (var model = IfcStore.Open(file))
                {
                    var project = model.Instances.FirstOrDefault<IIfcProject>();
                    PrintIfcSpaces(project);
                }
            }

            private static void PrintIfcSpaces(IIfcProject project)
            {
                var spaces = project.Model.Instances.OfType<IIfcSpace>().ToList();
                foreach (var space in spaces)
                {
                    Console.WriteLine(string.Format("{0} -> {1} [{2}]", space.LongName, space.Name, space.GetType().Name));
                }
            }
        }

    }
}
