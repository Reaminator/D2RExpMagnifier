﻿using D2RExpMagnifier.UI.ViewModel;
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

namespace D2RExpMagnifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class D2RExpMagnifierView : Window
    {
        D2RExpMagnifierViewModel viewModel;
        public D2RExpMagnifierView()
        {
            viewModel = new D2RExpMagnifierViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.ViewLoaded();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                viewModel.GridClicked();
            }
            else
            {
                if(e.ButtonState == MouseButtonState.Pressed) this.DragMove();
            }
        }
    }
}
