

using System;
using System.Windows.Forms;
using System.Media;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine
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

        // CONSTANTS

        private const int WIDTH = 224;
        private const int HEIGHT = 288;
        private const float SCALE = 3f;

        // FIELDS

        private PacManGame pacMan;

        // CONSTRUCTOR

        public Form1()
        {
            // initalize components and window
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            //Cursor.Hide();

            // game initalization

            // set the forms size to the GameScreen
            Width = (int)(WIDTH * SCALE);
            Height = (int)(HEIGHT * SCALE);

            pacMan = new PacManGame(new GameScreen(CreateGraphics(), WIDTH, HEIGHT, SCALE), new SoundPlayer(), ticker);
            
            // start the game timer
            ticker.Start();
        }

        // called per time tick
        private void ticker_Tick(object sender, EventArgs e)
            => pacMan.GameLoop();

        // called when key is pressed
        protected override void OnKeyDown(KeyEventArgs e)
            => InputManager.PressKey(e.KeyCode);

        // called when key is released
        protected override void OnKeyUp(KeyEventArgs e)
            => InputManager.ReleaseKey(e.KeyCode);

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
    }
}
