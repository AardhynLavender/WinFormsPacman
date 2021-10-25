
//
//  Blinky : Ghost Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A derived ghost that targets pacmans current
//  tile and <..talk about speed changes..>
//

using System;
using System.Drawing;
using System.Collections.Generic;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    class Blinky : Ghost
    {
        // CONSTANTS

        private const int ANIMATION = Time.TWENTYTH_SECOND;

        private const int START_X = 107;
        private const int START_Y = 112;

        private const int TEXTURE_RIGHT = 168;
        private const int TEXTURE_LEFT  = 172;
        private const int TEXTURE_UP    = 176;
        private const int TEXTURE_DOWN  = 180;

        // FIELDS



        // CONSTRUCTOR

        public Blinky(World world, PacMan pacman)
            : base(START_X, START_Y, TEXTURE_RIGHT, world, pacman)
        {
            // initalize fields

            scatterTile = new Vector2D(25, 0);
            Trajectory  = Directions[(int)Direction.LEFT];
            speed       = 1.0f;

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
            => pacman.CurrentTile;
    }
}