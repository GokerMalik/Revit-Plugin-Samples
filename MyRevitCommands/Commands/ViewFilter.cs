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
    public class ViewFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            //Create Filter
            //First we need a list of Revit categories
            List<ElementId> cats = new List<ElementId>();
            cats.Add(new ElementId(BuiltInCategory.OST_Sections));

            //Previously
            //ElementParameterFilter filter = new ElementParameterFilter(ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.VIEW_NAME), "WIP", false));

            //Now, without the boolean parameter
            //We can create filter role by accessing to ParameterFilterRuleFactory
            ElementParameterFilter filter = new ElementParameterFilter(ParameterFilterRuleFactory.
                CreateContainsRule(new ElementId(BuiltInParameter.VIEW_NAME), "WIP"));


            try
            {
                using (Transaction trans = new Transaction(doc, "Apply Filter"))
                {
                    trans.Start();

                    //Apply Filter
                    ParameterFilterElement filterElement = ParameterFilterElement.Create(doc,
                        "My First Filter", cats, filter);

                    //Get the active view in the document
                    doc.ActiveView.AddFilter(filterElement.Id);

                    doc.ActiveView.SetFilterVisibility(filterElement.Id, false);

                    trans.Commit();
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
