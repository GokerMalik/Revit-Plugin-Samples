using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    internal class ExternalDBapp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            //we need to close the application when Revit is shutting down to be sure that
            //is not going to be executed unintentionally

            //Give the application to be shut down.
            application.DocumentChanged -= ElementChangedEvent;

            //reurn a successful result
            return ExternalDBApplicationResult.Succeeded;

        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {

            try
            {

                //register the event
                //use delegate symbol (+=) which is a type that will hold a reference to the event
                //handler method. This allows to past the method as a parameter.
                application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(ElementChangedEvent);

            }
            catch (Exception e)
            {
                string message = e.Message;
                TaskDialog.Show("Error", message);
                return ExternalDBApplicationResult.Failed;
            }

            //External DB application has its own result enumeration different
            //than the external application result.
            return ExternalDBApplicationResult.Succeeded;

        }

        //Create an event handler method that takes the central object and document changed events arguments
        //as the parameters.
        /* parameters are
         * Object (Sender): The document in which event occurs
         * DocumentEventChangedArgs (args): Sent to the method when event occurs. It provides
         * methods to access to the elements that are associated with the event.
         * One of theses is GetModifiedElementsID method which takes an element filter.
         * We can use this filter to look for specific elements that have changed in the document
         */

        public void ElementChangedEvent(object sender, DocumentChangedEventArgs args)
        {
            //get the modified element
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementId element = args.GetModifiedElementIds(filter).First();

            //get the transaction name
            string tName = args.GetTransactionNames().First();

            TaskDialog.Show("modified element", element.ToString() +
                " changed by " + tName);
        }

    }
}
