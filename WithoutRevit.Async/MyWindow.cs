using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WithoutRevit.Async
{
    public class MyWindow : Window
    {
        public MyWindow()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Width = 200;
            Height = 100;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var button = new Button
            {
                Content = "Button",
                Command = new ButtonCommand(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Content = button;
        }
    }

    public class ButtonCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            //Running Revit API code directly here will result in a "Running Revit API outside of Revit API context" exception
            //Raise a predefined ExternalEvent instead
            MyRevitCommand.SomeEvent.Raise();
        }
    }
}