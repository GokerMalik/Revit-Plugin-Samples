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
    public class GetParameter : IExternalCommand
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
                    //Retrieve Element inside the IF statement (like get element ID but safer)
                    ElementId eleId = pickedObj.ElementId;
                    Element ele = doc.GetElement(eleId);

                    //Get parameter
                    Parameter param = ele.LookupParameter("Head Height");

                    //Now we have the parameter and we can start accessing its properties.
                    //Definition can be retrieved from the definition property of the parameter object.
                    //It can be either internal or external definition
                    //Every parameter has an internal definition whereas they may not have an external one,
                    //which are saved in an external file.

                    //Create an internal definition variable named paramDef
                    //we have to cast it into an internal definition type
                    InternalDefinition paramDef = param.Definition as InternalDefinition;

                    TaskDialog.Show("Parametets", string.Format("{0} parameter of type {1} with built-in parameter {2}",
                        paramDef.Name,
                        paramDef.GetDataType(),
                        paramDef.BuiltInParameter
                        ));

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
