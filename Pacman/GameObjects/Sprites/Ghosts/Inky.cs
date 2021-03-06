
//
//  Clyde : Ghost Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A derived ghost that 
//

using System;
using System.Drawing;
using System.Collections.Generic;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    class Inky : Ghost
    {

        // CONSTANTS

        private const int ANIMATION     = Time.TWENTYTH_SECOND;
        private const Colour COLOUR     = Colour.BLUE;

        private const int START_X       = 107;
        private const int START_Y       = 112;

        private const int PREFERENCE    = 3;
        private const int TARGET_TILE   = 450;
        private const int TEXTURE_RIGHT = 336;
        private const int TEXTURE_LEFT  = 340;
        private const int TEXTURE_UP    = 344;
        private const int TEXTURE_DOWN  = 348;

        private const int PIVOT_DISTANCE = 4;

        // FIELDS

        private Blinky blinky;

        // CONSTRUCTOR

        public Inky(World world, PacMan pacman, Blinky blinky, int pelletLimit)
            : base(START_X, START_Y, TEXTURE_UP, pelletLimit, TARGET_TILE, world, pacman, COLOUR)
        {
            // Initalize fields

            preferenceRank  = PREFERENCE;

            this.blinky     = blinky;
            scatterTile     = new Vector2D(27, 35);
            homeTile        = new Vector2D(11, 17);
            Trajectory      = Directions[(int)Direction.LEFT];
            speed           = 1.0f;

            Reset();

            // configure animations

            up      = new Animation(game, tileset, this, SIZE, TEXTURE_UP, 2, ANIMATION);
            right   = new Animation(game, tileset, this, SIZE, TEXTURE_RIGHT, 2, ANIMATION);
            down    = new Animation(game, tileset, this, SIZE, TEXTURE_DOWN, 2, ANIMATION);
            left    = new Animation(game, tileset, this, SIZE, TEXTURE_LEFT, 2, ANIMATION);

            directionalAnimations = new Animation[DIRECTIONS]
            { up, right, down, left };

            Array.ForEach(directionalAnimations, dir => dir.Start());

            currentAnimation = directionalAnimations[0];
        }

        // METHODS

        // draw inkys debugging infomation
        protected override void debugDraw()
        {
            base.debugDraw();

            if (Mode == Mode.CHASE)
            {
                Vector2D a = world.GetCoordinate(blinky.CurrentTile);
                Vector2D b = world.GetCoordinate(targetTile);

                int d = tileset.Size / 2;

                a.X += d;
                a.Y += d;
                b.X += d;
                b.Y += d;

                game.DrawLine(a, b, Colour);
            }
        }

        // get Inkys target tile
        protected override Vector2D GetTargetTile()
            => new Vector2D
            {
                X = pacman.CurrentTile.X + Directions[(int)pacman.Direction].X * PIVOT_DISTANCE - Vector2D.GetDifferenceVector(blinky.CurrentTile, pacman.CurrentTile).X,
                Y = pacman.CurrentTile.Y + Directions[(int)pacman.Direction].Y * PIVOT_DISTANCE - Vector2D.GetDifferenceVector(blinky.CurrentTile, pacman.CurrentTile).Y
            };
    }
}
