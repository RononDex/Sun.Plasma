using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Sun.Plasma
{
    /// <summary>
    /// Static class that can make a custom window resizable
    /// </summary>
    public static class ResizableWindow
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        private const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// Makes the given window resizable
        /// </summary>
        /// <param name="window"></param>
        public static void MakeWindowResizable(Window window)
        {
            window.PreviewMouseMove += new MouseEventHandler(ResetCursor);

            var bottomRightRect = window.Template.FindName("bottomRight", window) as Rectangle;
            if (bottomRightRect != null)
                SetupResizeEvents(bottomRightRect);
        }

        private static void ResetCursor(object sender, MouseEventArgs e)
        {
            var elm = sender as DependencyObject;
            Window parentWindow = GetTopLevelControl(elm) as Window;
            if (parentWindow == null && elm != null)
                parentWindow = elm as Window;
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                parentWindow.Cursor = Cursors.Arrow;
            }
        }

        private static void SetupResizeEvents(Rectangle rect)
        {
            rect.PreviewMouseDown += new MouseButtonEventHandler(Resize);
            rect.MouseMove += new MouseEventHandler(DisplayResizeCursor);
        }

        private static void Resize(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            Window parentWindow =  GetTopLevelControl(clickedRectangle) as Window;
            var hwndSource = PresentationSource.FromVisual((Visual)parentWindow) as HwndSource;

            switch (clickedRectangle.Name)
            {
                case "top":
                    parentWindow.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top, hwndSource);
                    break;
                case "bottom":
                    parentWindow.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom, hwndSource);
                    break;
                case "left":
                    parentWindow.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left, hwndSource);
                    break;
                case "right":
                    parentWindow.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right, hwndSource);
                    break;
                case "topLeft":
                    parentWindow.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft, hwndSource);
                    break;
                case "topRight":
                    parentWindow.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight, hwndSource);
                    break;
                case "bottomLeft":
                    parentWindow.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft, hwndSource);
                    break;
                case "bottomRight":
                    parentWindow.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight, hwndSource);
                    break;
                default:
                    break;
            }
        }

        private static void ResizeWindow(ResizeDirection direction, HwndSource hwndSource)
        {            
            SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private static DependencyObject GetTopLevelControl(DependencyObject control)
        {
            DependencyObject tmp = control;
            DependencyObject parent = null;
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                parent = tmp;
            }
            return parent;
        }

        private static void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            Window parentWindow = GetTopLevelControl(clickedRectangle) as Window;

            switch (clickedRectangle.Name)
            {
                case "top":
                    parentWindow.Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    parentWindow.Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    parentWindow.Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    parentWindow.Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    parentWindow.Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    parentWindow.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    parentWindow.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    parentWindow.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }
    }
}
