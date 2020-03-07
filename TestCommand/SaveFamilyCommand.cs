#region Reference

using Autodesk.Revit.DB;
using Revit.Async;
using Revit.Async.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

#endregion Reference

namespace TestCommand
{
    internal class SaveFamilyCommand : ICommand
    {
        #region Constructors

        public SaveFamilyCommand()
        {
            //Create a RevitTask
            ScopedRevitTask = new RevitTask();

            //Register an external event handler to the scope
            ScopedRevitTask.Register(new SaveFamilyToDesktopExternalEventHandler());
        }

        #endregion Constructors

        #region Properties

        private IRevitTask ScopedRevitTask { get; }

        #endregion Properties

        #region Interface Implementations

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            //Run Revit API directly here
            var savePath = await RevitTask.RunAsync(async app =>
            {
                try
                {
                    //Support async task
                    //Raise global external event handler
                    Family randomFamily = await RevitTask
                        .RaiseGlobal<GetRandomFamilyExternalEventHandler, bool, Family>(parameter as bool? ?? false);

                    //Raise scoped external event handler
                    return await ScopedRevitTask.Raise<SaveFamilyToDesktopExternalEventHandler, Family, string>(randomFamily);
                }
                catch (Exception)
                {
                    return null;
                }
            });
            var saveResult = !string.IsNullOrWhiteSpace(savePath);
            MessageBox.Show($"Family {(saveResult ? "" : "not ")}saved:\n{savePath}");
            if (saveResult && Path.GetDirectoryName(savePath) is string dir)
            {
                Process.Start(dir);
            }
        }

        #endregion Interface Implementations

        #region Others

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion Others
    }
}