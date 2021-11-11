
//
//  Program name:           WinForms Pacman
//  Project file name       PacMan
//
//  Author:                 Aardhyn Lavender
//  Date:                   12 / 11 / 2021
//
//  Language:               Visual C# 7.9
//  Platform:               Microsoft Visual Studio 2019
//
//  Purpose:                Demonstrate understanding of event driven programming and the object orientated paradigm.
//  Description:            Simple Clone of Namco Pacman created in Windows Forms for the Windows 10 (and 11) Operating System
//  Known Bugs:             None
//
//  Additional Features     *   Menu System to toggle between Standard and Development modes -- basicly a start and end game splash screen.
//  and Functional          *   The Game uses a 28 by 36 tile grid (rather than a 50*50) for the maze and UI. This is so it matches the original
//  Deviations                   Pacman's size.
//                          *   The game score increases by 10 per consumed pellet (and other amounts for power pellets and ghost eating)
//                               matching the orignal games scoring system rather than 1 point.
//                          *   The Game is not won if pacman eats all pellets in the maze, but rather progresses to the next level. Levels increase
//                          *   up to 255 like the original PacMan. Winning the game is beating your own high score -- playing for as long as possible.
//                          *   The Game is lost after Pacman dies three times (running out of lives) rather than on the first time.
//                          *   The Game gives a "Game Over" message when all lives are lost, but does not explicitly tell the user they have won the
//                          *   level--just like the orignal game. There is no "you've Won!" at the end of all 255 levels, should anyone attempt to make
//                               it that far.
//                          *   The Game provides a "Start" button ( and "Develop" ) buttons to start the game.
//                          *   As the window border was removed for a cleaner app asthetic, I provided a close button in the style of the game to allow
//                          *   the user to exit the game.
//
//  Extra Credit            *   Ghosts chase Pacman and toggle between CHASE and SCATTER as per the orignal game.
//                          *   Pacman has 3 Lives for all 255 levels
//                          *   The game has levels.
//                          *   The game has Power Pellets that allow Pacman to eat the ghosts.
//

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

        private const int WIDTH     = 224;
        private const int MARGIN    = 5;
        private const int HEIGHT    = 288;
        private const float SCALE   = 3f;

        // FIELDS

        private PacManGame pacMan;

        // CONSTRUCTOR

        public Form1()
        {
            // initalize components and window
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;

            // game initalization

            // set the forms size to the GameScreen
            Width   = (int)(WIDTH * SCALE);
            Height  = (int)(HEIGHT * SCALE);

            Left = Top = MARGIN;

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
            if (e.Button == MouseButtons.Left && e.Y < 30 && e.X < Width)
            {
                if (e.X < Width - 30)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
                else Application.Exit();
            }
            #endregion
        }
    }
}
