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

namespace TouchScroller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputSimulator mouseSim;
        private IKeyboardMouseEvents m_GlobalHook;
        private Point mousePos;
        private Point mousePosPrev;

        public MainWindow()
        {
            InitializeComponent();
            mouseSim = new InputSimulator();
            Subscribe();
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

            //TODO make this adjustable or adaptable
            Point screenResolution = new Point(1920, 1200);     //resolution in px. width, height

            double absX = (mousePosPrev.X / screenResolution.X) * 65535;      //65535 because MoveMouseTo requires absolute coordinates
            double absY = (mousePosPrev.Y / screenResolution.Y) * 65535;
            mouseSim.Mouse.MoveMouseTo(absX, absY);

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

        
    }
}
