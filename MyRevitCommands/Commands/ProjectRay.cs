using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;


//This is to check how elements realte that don't touch
//To do this, we need tp prpject RAYS.
//It is act of sending a vetor from a location to see what it hits
//It is a way for us to find our way in 3D

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class ProjectRay : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                //Pick Object
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);                              
                
                if (pickedObj != null)
                {
                    //Retrieve Element
                    ElementId eleId = pickedObj.ElementId;
                    Element ele = doc.GetElement(eleId);

                    //Project Ray
                    //We are going to use location as the origin of our rays
                    LocationPoint locP = ele.Location as LocationPoint;
                    XYZ p1 = locP.Point;

                    //Now create the direction
                    XYZ rayd = new XYZ(0, 0, 1);

                    ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Roofs);

                    //Here, we need a 3D view object. It won't affect the results as long as
                    //the target is not hidden on the view.
                    ReferenceIntersector refI = new ReferenceIntersector(filter, FindReferenceTarget.Face,
                        //this will retrieve the active view from the document, and cast it
                        //as a 3D view object.
                        (View3D)doc.ActiveView);

                    //now we have the reference intersecting object, we can use it to find
                    //elements

                    //find nearest to get the first element it hits
                    ReferenceWithContext refC = refI.FindNearest(p1, rayd);

                    //now that we have a reference object with context object.
                    //we can use it to extract data about the reference that it's hit.

                    Reference reference = refC.GetReference();

                    //using this reference, now we can get the location of the point of hit
                    XYZ intPoint = reference.GlobalPoint;

                    double dist = p1.DistanceTo(intPoint);

                    TaskDialog.Show("Ray", string.Format("Distance to roof {0}",
                        UnitUtils.ConvertToInternalUnits(dist, UnitTypeId.Millimeters)));

                }

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
            
        }
    }
}
