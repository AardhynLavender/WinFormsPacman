
//
//  Ghost : Sprite Class
//  Created 04/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Game Object that has velocity and responds to tile based
//  physics using a current tile based on the provided world.
//  

using System.Drawing;
using System.Collections.Generic;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    abstract class Ghost : Sprite
    {
        // CONSTANTS

        private const int SIZE              = 2;
        private const int TUNNEL_DIVISOR    = 2;
        protected const int ANIMATIONS      = 2;
        protected const int ANIMATION_SPEED = Time.HUNDREDTH_SECOND;

        protected const int FRIGHTENED      = 504;
        protected const int EATEN_RIGHT     = 512;
        protected const int EATEN_LEFT      = 514;
        protected const int EATEN_UP        = 516;
        protected const int EATEN_DOWN      = 518;

        // FIELDS

        protected Vector2D targetTile;
        protected Mode mode;

        protected Animation frightened;

        // CONSTRUCTOR

        public Ghost(float x, float y, int index, World world)
            : base(x, y, index, SIZE, SIZE, new Vector2D(), world)
        {
            mode = Mode.SCATTER;

            frightened = new Animation(game, this, new List<Rectangle>(ANIMATIONS)
            {
                tileset.GetTileSourceRect(FRIGHTENED, SIZE, SIZE),
                tileset.GetTileSourceRect(FRIGHTENED + SIZE, SIZE, SIZE)
            }, ANIMATION_SPEED, loop: true);
        }

        // PROPERTIES

        // speed
        protected abstract int speed
        { get; set; }

        // how long the ghost waits before chasing pacman
        protected abstract int houseWaitTime    
        { get; set; }

        // the target tile while in SCATTER mode
        protected abstract int scatterTile      
        { get; set; }

        // the spawn tile and EATEN target tile
        protected abstract int homeTile         
        { get; set; }

        // METHODS

        // reverse direction and set target tile to
        // applicable map corner
        public abstract void Scatter();

        // reverse direction and set mode to FRIGHTENED
        public void Frighten()
        {
            mode = Mode.FRIGHTENED;
        }

        // set mode to EATEN
        public void Eat()
        {
            mode = Mode.EATEN;
        }

        public override void Update()
        {
            // slow down for tunnel

            // if in tunnel
                speed  /= TUNNEL_DIVISOR;
            // else

            // update sprite
            base.Update();
        }
    }
}