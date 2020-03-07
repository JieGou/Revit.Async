#region Reference

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using System;
using System.IO;
using System.Reflection;

#endregion Reference

namespace TestCommand
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        #region Constructors

        public Command()
        {
            //Subscribe this event in case add-in manager fails to locate the Revit.Async.dll
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        #endregion Constructors

        #region Interface Implementations

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //always initialize RevitTask in Revit API context before calling any RunAsync method
            RevitTask.Initialize();
            //Register your own global generic external event handler
            RevitTask.RegisterGlobal(new GetRandomFamilyExternalEventHandler());
            var window = new TestWindow();
            window.Show();
            return Result.Succeeded;
        }

        #endregion Interface Implementations

        #region Others

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (name.Name != "Revit.Async")
            {
                return null;
            }

            var s = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (s == null)
            {
                return null;
            }

            return Assembly.LoadFrom(Path.Combine(s, "Revit.Async.dll"));
        }

        #endregion Others
    }
}