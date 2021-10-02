
//
//  Input Manager Class
//  Created 02/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Stores the pressed keys and provides methods
//  to check what keys are pressed.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace FormsPixelGameEngine.Utility
{
    public static class InputManager
    {
        // FIELDS

        private static Dictionary<Keys, bool> keys =
        new Dictionary<Keys, bool>
        {
            { Keys.Up, false },
            { Keys.Down, false },
            { Keys.Left, false },
            { Keys.Right, false },
            { Keys.Escape, false },
            { Keys.Space, false },
            { Keys.Enter, false },
            { Keys.Back, false },
            { Keys.W, false },
            { Keys.A, false },
            { Keys.S, false },
            { Keys.D, false },
        };

        // PROPERTIES

        public static bool Up       => keys[Keys.Up] || keys[Keys.W];
        public static bool Down     => keys[Keys.Down] || keys[Keys.S];
        public static bool Left     => keys[Keys.Left] || keys[Keys.A];
        public static bool Right    => keys[Keys.Right] || keys[Keys.D];
        public static bool Select   => keys[Keys.Space] || keys[Keys.Enter];
        public static bool Escape   => keys[Keys.Escape] || keys[Keys.Back];

        // METHODS

        public static void PressKey(Keys key)
        {
            if (keys.ContainsKey(key))
                keys[key] = true;
        }

        public static void ReleaseKey(Keys key)
        {
            if (keys.ContainsKey(key))
                keys[key] = false;
        }
    }
}
