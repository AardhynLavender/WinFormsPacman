
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

        private const int ANIMATION         = Time.TWENTYTH_SECOND;
        private const Colour COLOUR         = Colour.ORANCE;

        private const float SCATTER_RADIUS  = 8;

        private const int START_X           = 107;
        private const int START_Y           = 112;

        private const int PREFERENCE        = 4;
        private const int TARGET_TILE       = 450;
        private const int TEXTURE_RIGHT     = 420;
        private const int TEXTURE_LEFT      = 424;
        private const int TEXTURE_UP        = 428;
        private const int TEXTURE_DOWN      = 432;

        // FIELDS



        // CONSTRUCTOR

        public Clyde(World world, PacMan pacman, int pelletLimit)
            : base(START_X, START_Y, TEXTURE_RIGHT, pelletLimit, TARGET_TILE, world, pacman, COLOUR)
        {
            // initalize fields

            preferenceRank  = PREFERENCE;
            scatterTile     = new Vector2D(0,35);
            homeTile        = new Vector2D(15, 17);
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

        // draw clydes debugging infomation
        protected override void debugDraw()
        {
            base.debugDraw();

            // draw scatter radius
            if (Mode == Mode.CHASE 
                && !locked 
                && !Frozen)
            {
                game.DrawEllipse(world.GetCoordinate(currentTile), SCATTER_RADIUS * tileset.Size, Colour);
            }
        }

        // get Clydes target tile
        protected override Vector2D GetTargetTile()
            => Vector2D.GetAbsDistance(currentTile, pacman.CurrentTile) > SCATTER_RADIUS
                ? pacman.CurrentTile 
                : scatterTile;
    }
}
