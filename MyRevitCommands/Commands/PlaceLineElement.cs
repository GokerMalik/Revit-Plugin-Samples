using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceLineElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;


            Level level;
            try
            {
                //Get Level - This throws an exception when the level is not found.
                level = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Levels)
                    .WhereElementIsNotElementType()
                    .Cast<Level>()
                    .First(x => x.Name == "Ground Floor");
            }
            catch (Exception e1)
            {
                message = e1.Message;
                TaskDialog.Show("Level Not Found", message);
                return Result.Failed;
            }


            //Create points
            XYZ p1 = new XYZ(-10, -10, 0);
            XYZ p2 = new XYZ(10, -10, 0);
            XYZ p3 = new XYZ(15, 0, 0);
            XYZ p4 = new XYZ(10, 10, 0);
            XYZ p5 = new XYZ(-10, 10, 0);

            //create curves
            List<Curve> curves = new List<Curve> ();

            Line l1 = Line.CreateBound(p1, p2);
            Arc l2 = Arc.Create(p2, p4, p3);
            Line l3 = Line.CreateBound(p4, p5);
            Line l4 = Line.CreateBound(p5, p1);

            curves.Add(l1);
            curves.Add(l2);
            curves.Add(l3);
            curves.Add(l4);

            try
            {
                using (Transaction trans = new Transaction(doc, "Place Family"))
                {
                    trans.Start();

                    foreach (Curve c in curves)

                    {
                        Wall.Create(doc, c, level.Id, false);
                    }


                    trans.Commit();
                }


                return Result.Succeeded;
            }
            catch (Exception e2)
            {
                message = e2.Message;
                return Result.Failed;
            }

        }
    }
}
