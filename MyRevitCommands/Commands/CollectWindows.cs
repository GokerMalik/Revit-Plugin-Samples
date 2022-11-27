using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Runtime.ExceptionServices;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class CollectWindows : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            //Create Filtered Element Collector
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            //Create Filter
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);

            //Apply Filter
            //first, get the elements in the collector which passes the filter
            FilteredElementCollector collectorFiltered = collector.WherePasses(filter);

            //Also filter out element types
            FilteredElementCollector colFiltered_elementsOnly = collectorFiltered.WhereElementIsNotElementType();

            //Now store the elements in the colFiltered_elementsOnly collector into a variable called RevitWindows
            IList<Element> RevitWindows = colFiltered_elementsOnly.ToElements();

            //Show the windows collected
            TaskDialog.Show("Windows", string.Format("{0} windows counted!", RevitWindows.Count));

            //aa
            return Result.Succeeded;
        }
    }
}
