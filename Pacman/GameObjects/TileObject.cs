using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.GameObjects;

namespace Breakout.GameObjects
{
    class TileObject : GameObject
    {
        // FIELDS

        private bool wall;

        // CONSTRUCTORS

        public TileObject(float x, float y)
            : base(x, y)
        {  }

        public TileObject(float x, float y, Rectangle sourceRect)
            : base(x, y, sourceRect)
        {  }

        // PROPERTIES

        public bool Wall
        {
            get => wall;
            set => wall = value;
        }
    }
}
