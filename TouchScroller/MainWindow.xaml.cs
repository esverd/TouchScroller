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
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        private InputSimulator mouseSim;            //used to simulate mouse event
        private IKeyboardMouseEvents m_GlobalHook;      //used for logging last mouse click
        private Point mousePos;         //stores position of latest mouse click
        private Point mousePosPrev;     //stores position of previous mouse click, to return to
        private int scrollCounter;      //counts number of times scroll loop has iterated
        private int scrollFactor;       //used to set trigger threshold for scrolling 1 line
        private SolidColorBrush btnActiveColor;
        private SolidColorBrush btnInactiveColor;
        private bool shiftToggled, ctrlToggled;

        public MainWindow()
        {
            InitializeComponent();
            mouseSim = new InputSimulator();
            Subscribe();        //subscribe to start mouse logging
            mousePos = new Point(500, 500);
            mousePosPrev = new Point();
            scrollFactor = 5;
            btnActiveColor = new SolidColorBrush(Color.FromRgb(90, 90, 90));
            btnInactiveColor = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            shiftToggled = false;
            ctrlToggled = false;
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

            //mouseSim.Mouse.MoveMouseToPositionOnVirtualDesktop TODO test this function

            //mousePosPrev = PointToScreen(mousePosPrev);
            //SetCursorPos((int)mousePosPrev.X, (int)mousePosPrev.Y);

            mousePos.X = mousePosPrev.X;    //keeps coordinates for previous mouse click
            mousePos.Y = mousePosPrev.Y;    //so multiple button presses wont change mouse position
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

        private void gridSplittScroller_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            scrollCounter = 0;
        }

        private void gridSplittScroller_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            scrollHandler(e.VerticalChange);
        }
        private void gridSplittScroller_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //moveToLastClick();      //return to last click
        }

        private void scrollHandler(double verticalChange)
        {
            int scrollDirection = (int)(verticalChange / Math.Abs(verticalChange));
            //Debug.WriteLine("Direction " + scrollDirection);

            if (scrollCounter % scrollFactor == 0)
            {
                //deactivate scrolling in gridsplitter while this happens?
                //store curent mouse position: P1
                //move to mouse action position
                //scroll up or down
                //move back to P1

                Point p1 = PointToScreen(Mouse.GetPosition(mainGrid));
                moveToLastClick();
                mouseSim.Mouse.VerticalScroll(scrollDirection * 1);
                SetCursorPos((int)p1.X, (int)p1.Y);
            }
            scrollCounter++;
        }

        private void btnShift_Click(object sender, RoutedEventArgs e)
        {
            if(!shiftToggled)
            {
                btnShift.Background = btnActiveColor;
                mouseSim.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
            }
            else
            {
                btnShift.Background = btnInactiveColor;
                mouseSim.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            }
            shiftToggled = !shiftToggled;
        }

        private void btnCtrl_Click(object sender, RoutedEventArgs e)
        {
            if (!ctrlToggled)
            {
                btnCtrl.Background = btnActiveColor;
                mouseSim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
            }
            else
            {
                btnCtrl.Background = btnInactiveColor;
                mouseSim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            }
            ctrlToggled = !ctrlToggled;
        }

        private void btnAlt_Click(object sender, RoutedEventArgs e)
        {
            moveToLastClick();
            mouseSim.Keyboard.KeyPress(VirtualKeyCode.MENU);
        }
    }
}
