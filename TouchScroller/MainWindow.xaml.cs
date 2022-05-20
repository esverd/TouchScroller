﻿using System;
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
using WindowsInput.Native;
using WindowsInput;

namespace TouchScroller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InputSimulator simulator = new InputSimulator();
        public MainWindow()
        {
            InitializeComponent();
        }


        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            
            simulator.Mouse.MoveMouseTo(45000, 30000);
            //simulator.Mouse.RightButtonClick();
            simulator.Mouse.VerticalScroll(-10);
        }
    }
}
