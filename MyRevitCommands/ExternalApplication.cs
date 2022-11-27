using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using System.Reflection;
using Autodesk.Revit.DB;

using System.Windows.Media.Imaging;

namespace MyRevitCommands
{
    internal class ExternalApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                //create a ribbon
                application.CreateRibbonTab("Prota Altar");

                //get the dll path for this external API
                string path = Assembly.GetExecutingAssembly().Location;

                //Create a new buttons belongs to this application. Namem, readable name, app API and
                //specify PlaceFamily as the command to be executed.
                PushButtonData butDatPush = new PushButtonData("Button1", "Place Family", path, "MyRevitCommands.PlaceFamily");

                //create a panel included in "My Commands" tab give it a name as "Commands".
                RibbonPanel panel = application.CreateRibbonPanel("Prota Altar", "Prota Destek");

                //Add button image
                Uri imagePath = new Uri(@"C:\Users\altar\Desktop\Ex_Files_Revit_Creating_C_Sharp_Plugins\Exercise Files\05_04\Start\MyRevitCommands\MyRevitCommands\Prota-Altar Logo.png");
                BitmapImage imgProta = new BitmapImage(imagePath);

                //add the button to the panel
                PushButton butPush = panel.AddItem(butDatPush) as PushButton;
                butPush.LargeImage = imgProta;

                return Result.Succeeded;

            }
            catch (Exception e)
            {
                string message = e.Message;
                TaskDialog.Show("Error", message);
                return Result.Failed;
            
            }


        }
    }
}
