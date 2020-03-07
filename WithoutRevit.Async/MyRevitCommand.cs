using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WithoutRevit.Async
{
    [Transaction(TransactionMode.Manual)]
    public class MyRevitCommand : IExternalCommand
    {
        public static ExternalEvent SomeEvent { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Register MyExternalEventHandler ahead of time
            SomeEvent = ExternalEvent.Create(new MyExternalEventHandler());
            var window = new MyWindow();
            //Show modeless window
            window.Show();
            return Result.Succeeded;
        }
    }

    public class MyExternalEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            //Running some Revit API code here to handle the button click
            //It's complicated to accept argument from the calling context and return value to the calling context
            var families = new FilteredElementCollector(app.ActiveUIDocument.Document)
                                .OfClass(typeof(Family))
                                .Cast<Family>()
                                .ToList();
            //ignore some code
            var random = new Random(Environment.TickCount);
            var randomFamily = families[random.Next(0, families.Count)];

            var document = randomFamily.Document;
            var familyDocument = document.EditFamily(randomFamily);
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var savePath = Path.Combine(desktop, $"{randomFamily.Name}.rfa");
            familyDocument.SaveAs(savePath, new SaveAsOptions { OverwriteExistingFile = true });

            var directoryName = Path.GetDirectoryName(savePath);
            var saveResult = !string.IsNullOrWhiteSpace(savePath);
            MessageBox.Show($"Family {(saveResult ? "" : "not ")}saved:\n{savePath}");
            if (saveResult && directoryName != null)
            {
                Process.Start(directoryName);
            }
        }

        public string GetName()
        {
            return "MyExternalEventHandler";
        }
    }
}