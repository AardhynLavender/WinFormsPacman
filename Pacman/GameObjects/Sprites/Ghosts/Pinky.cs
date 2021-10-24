
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

        private const int START_X       = 107;
        private const int START_Y       = 112;

        private const int TEXTURE_RIGHT = 252;
        private const int TEXTURE_LEFT  = 256;
        private const int TEXTURE_UP    = 260;
        private const int TEXTURE_DOWN  = 264;

        // FIELDS



        // CONSTRUCTOR

        public Pinky(World world, PacMan pacman)
            : base(START_X,START_Y, TEXTURE_UP, world, pacman)
        {
            // initalize fields

            scatterTile = new Vector2D(0,2);
            Trajectory  = Directions[(int)Direction.LEFT];
            speed       = 1.0f;

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
            => new Vector2D()
            {
                X = pacman.CurrentTile.X + Directions[(int)pacman.Direction].X * 4,
                Y = pacman.CurrentTile.Y + Directions[(int)pacman.Direction].Y * 4,
            };
    }
}
