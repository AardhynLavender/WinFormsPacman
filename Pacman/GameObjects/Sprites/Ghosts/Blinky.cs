
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

        public Blinky(Game game, World world, PacMan pacman)
            : base(0, 0, TEXTURE_RIGHT, world, game, pacman)
        {
            // set position, speed, and direction

            X = START_X;
            Y = START_Y;

            Trajectory = Directions[(int)Direction.LEFT];

            speed = 1.0f;

            // configure animations

            up = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_UP, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_UP + SIZE, SIZE, SIZE)
            }, ANIMATION, loop:true);

            right = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_RIGHT, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_RIGHT + SIZE, SIZE, SIZE)
            }, ANIMATION, loop:true);

            down = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_DOWN, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_DOWN + SIZE, SIZE, SIZE)
            }, ANIMATION, loop:true);

            left = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_LEFT, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_LEFT + SIZE, SIZE, SIZE)
            }, ANIMATION, loop:true);

            directionalAnimations = new Animation[DIRECTIONS]
            { up, right, down, left };

            Array.ForEach(directionalAnimations, dir => dir.Start());

            currentAnimation = directionalAnimations[0];
        }

        // PROPERTIES



        // METHODS

        public override void Scatter()
            => throw new System.NotImplementedException();

        protected override Vector2D GetTargetTile()
            => pacman.CurrentTile;
    }
}