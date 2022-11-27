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
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class ElementIntersection : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument and Document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                //Let the user select an object
                //In our case, it is going to be a column family
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);                              
                
                if (pickedObj != null)
                {
                    //Retrieve the Element from the selected object
                    ElementId eleId = pickedObj.ElementId;
                    Element ele = doc.GetElement(eleId);

                    //Get Geometry from that element
                    Options gOptions = new Options();
                    gOptions.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement geom = ele.get_Geometry(gOptions);

                    //Create a solid element equals to Null. This is to be used to
                    //assign a solitary
                    Solid gSolid = null;

                    //Traverse Geometry
                    foreach (GeometryObject gObj in geom)
                    {
                        //Creare a geometry instance variable as we are going to
                        //select an instance of a column family.
                        GeometryInstance gInst = gObj as GeometryInstance;


                        //new we can retrieve the geometry element contained within the
                        //geometry instance
                        if (gInst != null) {

                            GeometryElement gEle = gInst.GetInstanceGeometry();
                            foreach (GeometryObject gO in gEle) {

                                gSolid = gO as Solid;
                            }
                        }
                    }


                    //now finally we have the solid from the column family.

                    //Filter for Intersection
                    FilteredElementCollector collector = new FilteredElementCollector(doc);

                    //Create interset solid filter (it is a slow one)
                    ElementIntersectsSolidFilter filter = new ElementIntersectsSolidFilter(gSolid);

                    //Before applying it, make a quick filter to process the data faster.
                    ICollection<ElementId> intersects = collector.OfCategory(BuiltInCategory.OST_Roofs).
                        //now apply the slow filter
                        WherePasses(filter).
                        //get the intersecting elements as element IDs
                        ToElementIds();


                    //Show them to the user by selecting.
                    uidoc.Selection.SetElementIds(intersects);

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
