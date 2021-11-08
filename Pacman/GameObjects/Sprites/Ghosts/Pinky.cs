
//
//  Pinky : Ghost Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A derived ghost that targets the tile 
//

using System;
using System.Collections.Generic;
using System.Drawing;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    class Pinky : Ghost
    {
        // CONSTANTS

        private const int ANIMATION     = Time.TWENTYTH_SECOND;
        private const Colour COLOUR     = Colour.PINK;

        private const int START_X       = 107;
        private const int START_Y       = 112;

        private const int TEXTURE_RIGHT = 252;
        private const int TEXTURE_LEFT  = 256;
        private const int TEXTURE_UP    = 260;
        private const int TEXTURE_DOWN  = 264;

        private const int PELLET_LIMIT  = 0;

        // FIELDS



        // CONSTRUCTOR

        public Pinky(World world, PacMan pacman)
            : base(START_X, START_Y, TEXTURE_UP, PELLET_LIMIT, 408, world, pacman, COLOUR)
        {

            // initalize fields

            preferenceRank  = 2;
            scatterTile     = new Vector2D(0,3);
            homeTile        = new Vector2D(13, 17);
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

        // PROPERTIES



        // METHODS

        protected override Vector2D GetTargetTile()
            => new Vector2D()
            {
                X = pacman.CurrentTile.X + Directions[(int)pacman.Direction].X * 4,
                Y = pacman.CurrentTile.Y + Directions[(int)pacman.Direction].Y * 4,
            };
    }
}
