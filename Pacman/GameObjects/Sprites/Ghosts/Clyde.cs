
//
//  Clyde : Ghost Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A derived ghost that 
//

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    class Clyde : Ghost
    {
        // CONSTANTS

        private const int ANIMATION     = Time.TWENTYTH_SECOND;

        private const int START_X       = 107;
        private const int START_Y       = 112;

        private const int TEXTURE_RIGHT = 420;
        private const int TEXTURE_LEFT  = 424;
        private const int TEXTURE_UP    = 428;
        private const int TEXTURE_DOWN  = 432;

        // FIELDS



        // CONSTRUCTOR

        public Clyde(World world, PacMan pacman)
            : base(0,0, TEXTURE_RIGHT, world, pacman)
        {
            // set position, speed, and direction

            X = START_X;
            Y = START_Y;

            scatterTile = new Vector2D(0,35);

            Trajectory = Directions[(int)Direction.LEFT];

            speed = 1.0f;


            // configure animations

            up = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_UP, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_UP + SIZE, SIZE, SIZE)
            }, ANIMATION, loop: true);

            right = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_RIGHT, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_RIGHT + SIZE, SIZE, SIZE)
            }, ANIMATION, loop: true);

            down = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_DOWN, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_DOWN + SIZE, SIZE, SIZE)
            }, ANIMATION, loop: true);

            left = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(TEXTURE_LEFT, SIZE, SIZE),
                tileset.GetTileSourceRect(TEXTURE_LEFT + SIZE, SIZE, SIZE)
            }, ANIMATION, loop: true);

            directionalAnimations = new Animation[DIRECTIONS]
            { up, right, down, left };

            Array.ForEach(directionalAnimations, dir => dir.Start());

            currentAnimation = directionalAnimations[0];
        }

        // PROPERTIES



        // METHODS

        protected override Vector2D GetTargetTile()
            => Vector2D.GetAbsDistance(currentTile, pacman.CurrentTile) > 8 
                ? pacman.CurrentTile 
                : scatterTile;
    }
}
