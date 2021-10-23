
//
//  Energizer : Tile Object Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  ...
//  

using System.Collections.Generic;
using System.Drawing;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Tiles
{
    sealed class Energizer : TileObject
    {
        // CONSTANTS

        private const int TEXTURE = 23;
        private const int VALUE = 50;

        // FIELDS

        private Animation strobe;

        // CONSTRUCTOR

        public Energizer(int index, World world)
            : base(index, world, TEXTURE)
        {
            strobe = Game.AddAnimation(new Animation(game, this, new List<Rectangle>
            {
                sourceRect,
                new Rectangle()
            }, Time.TENTH_SECOND));

            strobe.Start();

        }

        // METHODS

        public override void Update()
        {
            // does pacman collide with this tile?
            if (game.PacManPosition.Equals(world.GetTile(this)))
            {
                world.ClearTile(this);
                game.Score += 50;
            }
        }
    }
}