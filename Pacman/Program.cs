
//
//  PacMan Class : Game
//  Created 01/07/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Entry point for the Forms application initalizing
//  and running the Form1:Form Class
//

using System;
using System.Windows.Forms;

namespace FormsPixelGameEngine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
