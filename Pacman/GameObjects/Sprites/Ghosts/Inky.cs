﻿
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

        private const int START_X       = 107;
        private const int START_Y       = 112;

        private const int TEXTURE_RIGHT = 336;
        private const int TEXTURE_LEFT  = 340;
        private const int TEXTURE_UP    = 344;
        private const int TEXTURE_DOWN  = 348;

        // FIELDS

        private Blinky blinky;

        // CONSTRUCTOR

        public Inky(Game game, World world, PacMan pacman, Blinky blinky)
            : base(0,0, TEXTURE_UP, world, game, pacman)
        {
            // Initalize fields

            this.blinky = blinky;

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

        // METHODS

        public override void Scatter()
            => throw new System.NotImplementedException();

        protected override Vector2D GetTargetTile()
            => new Vector2D
            {
                X = pacman.CurrentTile.X + Directions[(int)pacman.Direction].X * 2 - Vector2D.GetDifferenceVector(blinky.CurrentTile, blinky.TargetTile).X,
                Y = pacman.CurrentTile.Y + Directions[(int)pacman.Direction].Y * 2 - Vector2D.GetDifferenceVector(blinky.CurrentTile, blinky.TargetTile).Y
            };
    }
}
