
//
//  Tile Object : Game Object Class
//  Created 04/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Game Object that belongs to a world, and aligns to its
//  tile system. Tile Objects can be walls or not, and can be
//  given a position based on cartesian point, tile coordinate
//  or index in world. 
//  

using System;
using System.Drawing;

using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Tiles
{
    class TileObject : GameObject
    {
        // FIELDS

        protected World world;
        private int textureIndex;
        private bool wall;

        // CONSTRUCTORS

        public TileObject(float x, float y)
            : base(x, y)
        {  }

        public TileObject(int index, World world, int textureIndex)
            : base(0, 0, textureIndex)
        {
            this.textureIndex = textureIndex;
            this.world = world;

            // determine index from tile coordinate
            X = world.X + (index * tileset.Size % world.Width);
            Y = world.Y + ((int)Math.Floor((float)index++ / (world.Width / tileset.Size)) * tileset.Size);
        }

        public TileObject(Vector2D tile, World world, int textureIndex)
            : base(0, 0, textureIndex)
        {
            this.textureIndex = textureIndex;
            this.world = world;

            // convert cartesian point to tile coordinate
            X = world.X + (tile.X * tileset.Size);
            Y = world.Y + (tile.Y * tileset.Size);
        }

        // PROPERTIES

        public int TextureIndex 
            => textureIndex;

        public World World 
        { set => world = value; }

        public bool Wall
        {
            get => wall;
            set => wall = value;
        }
    }
}
