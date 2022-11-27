using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class PlaceLoopElement : IExternalCommand
    {
        public Result Execute(Autodesk.Revit.UI.ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument
            Autodesk.Revit.UI.UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Autodesk.Revit.DB.Document doc = uidoc.Document;

            //Create Points 
            XYZ p1 = new XYZ(-10, -10, 0);
            XYZ p2 = new XYZ(10, -10, 0);
            XYZ p3 = new XYZ(15, 0, 0);
            XYZ p4 = new XYZ(10, 10, 0);
            XYZ p5 = new XYZ(-10, 10, 0);

            //Create Curves
            List<Curve> curves = new List<Curve> { };
            Line l1 = Line.CreateBound(p1, p2);
            Arc l2 = Arc.Create(p2, p4, p3);
            Line l3 = Line.CreateBound(p4, p5);
            Line l4 = Line.CreateBound(p5, p1);

            curves.Add(l1);
            curves.Add(l2);
            curves.Add(l3);
            curves.Add(l4);

            //creare curve loop
            CurveLoop crvLoop = CurveLoop.Create(curves);

            //previously
            //double offset = UnitUtils.ConvertToInternalUnits(135, DisplayUnitType.DUT_MILLIMETERS);

            //now
            double offset = UnitUtils.ConvertToInternalUnits(135, UnitTypeId.Millimeters);

            //offset the existing loop and provide Z axis as the normal
            CurveLoop offsetcrv = CurveLoop.CreateViaOffset(crvLoop, offset, new XYZ(0, 0, 1));

            //Previously
            //CurveArrArray cArray = new CurveArrArray();
            //foreach (Curve c in offsetcrv) 
            //{
            //    cArray.Append(c);
            //}

            //now: Applied inline solution

            ElementId levelId;

            try
            {
                //Get Level ID of the level
                levelId = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Levels)
                    .WhereElementIsNotElementType()
                    .Cast<Level>()
                    .First(x => x.Name == "Ground Floor").Id;

            }
            catch (Exception e1)
            {
                message = e1.Message;
                TaskDialog.Show("Level Not Found", message);
                return Result.Failed;
            }

            //get the default floortype
            ElementId floorTypeId = Floor.GetDefaultFloorType(doc, false);

            try
            {
                using (Transaction trans = new Transaction(doc, "Place Floor"))
                {
                    trans.Start();

                    //before
                    //doc.NewFloor(cArray, false);

                    //now
                    Floor.Create(doc, new List<CurveLoop> { offsetcrv }, floorTypeId, levelId);

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
