

using System;
using System.Media;
using System.Windows.Forms;

namespace Breakout
{
    partial class Form1 : Form
    {
        #region Move Window without Title bar -- Thanks to: https://www.codeproject.com/Articles/11114/Move-window-form-without-Titlebar-in-C

        // keycodes
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        // extern functions
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        public Form1()
        {
            // initalize components and window
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            Cursor.Hide();

            // game initalization

            // start the game timer
            ticker.Start();
        }

        // called per time tick
        private void ticker_Tick(object sender, EventArgs e)
        { }

        // pass the mouses postion onto the Games screen
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);


        }

        // pass the mouse down event onto the games screen
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            #region check for conditions to move the window without a title bar
            if (e.Button == MouseButtons.Left && e.Y < 50 && e.X < Width - 40)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
            #endregion
        }

        // pass the mouse up event onto the games screen
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }
    }
}
