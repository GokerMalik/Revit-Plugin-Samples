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
    public class DeleteElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            try
            {
                //Ask user to pick an object
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                //Delete Element
                if (pickedObj != null)
                {   //Start the transacation
                    using (Transaction trans = new Transaction(doc, "Delete Element"))
                    {
                        //use the picked object and get its ID
                        trans.Start();
                        doc.Delete(pickedObj.ElementId);

                        //Create a task dialog with only the dialog title. So we can modidy other properties before
                        //the window is shown
                        TaskDialog tDialog = new TaskDialog("Delete Element?");
                        tDialog.MainContent = "Are you want to delete this element?";

                        //define two buttons by separating them with a " | "
                        tDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                        //check the result of the user choice
                        if (tDialog.Show() == TaskDialogResult.Ok)
                        {
                            //When choice is OK, commit
                            trans.Commit();
                            TaskDialog.Show("Success!", pickedObj.ElementId.ToString() + " deleted");
                        }

                        else
                        {
                            //do not commit if the choice is not OK
                            trans.RollBack();
                            TaskDialog.Show("Delete", pickedObj.ElementId.ToString() + " not deleted");
                        }

;
                    }
                }

                //return 0 either the choice is OK or Cancel
                return Result.Succeeded;
            }

            //cath if any error occurs
            catch (Exception e)
            {
                //get the message
                message = e.Message;
                return Result.Failed;
            }

        }
    }
}
