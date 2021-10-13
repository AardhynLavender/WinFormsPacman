using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Utility;

namespace Breakout.GameObjects
{
    class TileObject : GameObject
    {
        // FIELDS

        protected World world;
        private bool wall;

        // CONSTRUCTORS

        public TileObject(float x, float y)
            : base(x, y)
        {  }

        public TileObject(int index, World world, Rectangle sourceRect = new Rectangle())
            : base(0, 0, sourceRect)
        {
            this.world = world;
            X = world.X + (index * tileset.Size % world.Width);
            Y = world.Y + ((int)Math.Floor((float)index++ / (world.Width / tileset.Size)) * tileset.Size);
        }

        public TileObject(Vector2D tile, World world, Rectangle sourceRect = new Rectangle())
            : base(0, 0, sourceRect)
        {
            this.world = world;
            X = world.X + (tile.X * tileset.Size);
            Y = world.Y + (tile.Y * tileset.Size);
        }

        public TileObject(float x, float y, Rectangle sourceRect)
            : base(x, y, sourceRect)
        {  }

        // PROPERTIES

        public World World { set => world = value; }

        public bool Wall
        {
            get => wall;
            set => wall = value;
        }
    }
}
