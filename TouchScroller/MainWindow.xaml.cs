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
using System.Diagnostics;
using WindowsHook;
using System.Runtime.InteropServices;

namespace TouchScroller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputSimulator mouseSim;            //used to simulate mouse event
        private IKeyboardMouseEvents m_GlobalHook;      //used for logging last mouse click
        private Point mousePos;         //stores position of latest mouse click
        private Point mousePosPrev;     //stores position of previous mouse click, to return to

        public MainWindow()
        {
            InitializeComponent();
            mouseSim = new InputSimulator();
            Subscribe();        //subscribe to start mouse logging
            mousePos = new Point(500, 500);
            mousePosPrev = new Point();
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            moveToLastClick();
            mouseSim.Mouse.LeftButtonClick();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            moveToLastClick();
            mouseSim.Mouse.RightButtonClick();
        }

        private void btnMid_Click(object sender, RoutedEventArgs e)
        {
            moveToLastClick();
            mouseSim.Mouse.MiddleButtonClick();
        }

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
        }

        //TODO make this adjustable or adaptable
        Point screenResolution = new Point(1920, 1200);     //resolution in px. width, height
        private void moveToLastClick()
        {
            //simulator.Mouse.MoveMouseTo(45000, 30000);
            //simulator.Mouse.RightButtonClick();
            //simulator.Mouse.VerticalScroll(-10);

            //double height = SystemParameters.VirtualScreenHeight;
            //double width = SystemParameters.VirtualScreenWidth;
            //double height = SystemParameters.FullPrimaryScreenHeight;
            //double width = SystemParameters.FullPrimaryScreenWidth;
            //double height = SystemParameters.PrimaryScreenHeight;
            //double width = SystemParameters.PrimaryScreenWidth;
            //Debug.WriteLine(height + " " + width);

            double absX = (mousePosPrev.X / screenResolution.X) * 65535;      //65535 because MoveMouseTo requires absolute coordinates
            double absY = (mousePosPrev.Y / screenResolution.Y) * 65535;
            mouseSim.Mouse.MoveMouseTo(absX, absY);

            mousePos.X = mousePosPrev.X;    //keeps coordinates for previous mouse click
            mousePos.Y = mousePosPrev.Y;    //so multiple button presses wont change mouse position

            //mouseSim.Mouse.MoveMouseToPositionOnVirtualDesktop TODO test this function
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            //Debug.WriteLine(e.Button.ToString() + " " + e.X + " " + e.Y);
            mousePosPrev.X = mousePos.X;
            mousePosPrev.Y = mousePos.Y;
            mousePos.X = e.X;
            mousePos.Y = e.Y;
            Debug.WriteLine(String.Format("Btn: {0}. Current: ({1}, {2}). Prev: ({3}, {4})", 
                e.Button.ToString(), mousePos.X, mousePos.Y, mousePosPrev.X, mousePosPrev.Y));

            //mouseSim.Mouse.MoveMouseTo()
        }

        public void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        private Point scrollPoint = new Point(0, 0);
        double scrollCounter;
        private void gridSplittScroller_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            Debug.WriteLine("test1");
            scrollPoint.X = mousePos.X;
            scrollPoint.Y = mousePos.Y;
            scrollCounter = 0;
            //start mouse logging
            //m_GlobalHook.MouseMove += GlobalHookMouseMove;
        }

        //private void GlobalHookMouseMove(object sender, MouseEventExtArgs e)
        //{

        //}

        private void gridSplittScroller_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Debug.WriteLine("test3 " + e.VerticalChange);

            //get mouse position in gridsplitter
            //move to last click
            //scroll up or down at location of last click //scroll with keyboard?
            //return to previous position in gridsplitter

            //moveToLastClick();
            scrollHandler(e.VerticalChange);

            //return to gridsplitter
            //mouseSim.Mouse.MoveMouseTo(scrollPoint.X / screenResolution.X * 65535, scrollPoint.Y / screenResolution.Y * 65535);
        }
        private void gridSplittScroller_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Debug.WriteLine("test2");
            //return to last click
            //moveToLastClick();
        }

        private void scrollHandler(double verticalChange)
        {
            double scrollFactor = 5;

            int scrollDirection = (int)(verticalChange / Math.Abs(verticalChange));
            Debug.WriteLine("Direction " + scrollDirection);

            if (scrollCounter % scrollFactor == 0)
            {
                //deactivate scrolling in gridsplitter while this happens?
                //store curent mouse position: P1
                //move to mouse action position
                //scroll up or down
                //move back to P1

                Debug.WriteLine("Relative position " + Mouse.GetPosition(gridSplittScroller).X);
                
                
                //mouseSim.Mouse.VerticalScroll(scrollDirection * 1);
            }
            scrollCounter++;


            
        }

        private void btnShift_Click(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Relative position " + Mouse.GetPosition(Application.).X);
            //PointToScreen(Mouse.GetPosition(gridSplittScroller));
            //Point GetMousePos() => PointToScreen(Mouse.GetPosition(gridSplittScroller));
            Point p1 = Mouse.GetPosition(mainGrid);
            Point p2 = PointToScreen(p1);
            Debug.WriteLine("p1 = " + p1 + "    p2 = " + p2);

            SetCursorPos((int)p2.X, (int)p2.Y);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
    }
}
