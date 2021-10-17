
//
//  Ghost : Sprite Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Sprite that moves about the maze and chooses a new
//  direction at intersections based on AI implimented in
//  child classes. Provides functionality shared across
//  all ghosts such as target tile Pathfinding, and mode
//  switching and subsequent functionality augmentation
//
//  Ghosts can be in multiple MODEs
//
//  *   CHASE mode, where they follow their 
//      implimented target tile finding algorithms.

//  *   SCATTER mode, where their targe tile
//      is set to an constant implimented in derived
//      classes.
//  
//  *   EATEN mode, where their target tile is set to
//      their home tile which implimented in derived
//      classes. Thier texture is set to the EATEN 
//      varients defined.
//
//  *   FRIGHTEND mode, where their movements are based
//      on random turns at intersections rather than
//      a target tile. Ghosts use the FRIGHTENED texture
//      in this mode
//
//  Ghosts change from CHASE || SCATTER to FRIGHTENED when 
//  PacMan:sprite types enter the same tile as Energizer:TileObject
//  types, and remain in said mode until 'Eaten()', or the FrightenedTime
//  elapsed since 'Frighten()'ed;
//
//  Ghosts can be eaten by Pacman:sprite types which triggers
//  the mode from FRIGHTEND to EATEN, reverting back to CHASE 
//  when the Ghost has returned to its Home Tile.
//

using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    abstract class Ghost : Sprite
    {
        // CONSTANT AND STATIC MEMBERS

        private const int INFINITY          = 1000;
        private const int DIRECTIONS        = 4;
        private const int SIZE              = 2;
        private const int TUNNEL_DIVISOR    = 2;
        protected const int ANIMATIONS      = 2;
        protected const int ANIMATION_SPEED = Time.HUNDREDTH_SECOND;

        protected const int FRIGHTENED      = 504;
        protected const int EATEN_RIGHT     = 512;
        protected const int EATEN_LEFT      = 514;
        protected const int EATEN_UP        = 516;
        protected const int EATEN_DOWN      = 518;

        private const int OFFSET_X = 4;
        private const int OFFSET_Y = 3;

        protected static Vector2D[] Directions =
        new Vector2D[DIRECTIONS]
        {
            new Vector2D(0,-1),
            new Vector2D(1,0),
            new Vector2D(0,1),
            new Vector2D(-1,0)
        };

        // FIELDS

        protected PacMan pacman;
        protected Mode mode;

        protected Vector2D targetTile;
        protected Vector2D scatterTile;

        protected Vector2D homeTile;
        protected int houseWaitTime;

        protected Animation frightened;

        // CONSTRUCTOR

        public Ghost(float x, float y, int index, World world, PacMan pacman)
            : base(x, y, index, SIZE, SIZE, new Vector2D(), world)
        {
            // initalize fields

            this.pacman = pacman;
            mode = Mode.CHASE;
            offsetX = OFFSET_X;
            offsetY = OFFSET_Y;

            // create frightened animation
            frightened = new Animation(game, this, new List<Rectangle>(ANIMATIONS)
            {
                tileset.GetTileSourceRect(FRIGHTENED, SIZE, SIZE),
                tileset.GetTileSourceRect(FRIGHTENED + SIZE, SIZE, SIZE)
            }, ANIMATION_SPEED, loop: true);
        }

        // PROPERTIES



        // METHODS

        // reverse direction and set target tile to
        // applicable map corner
        public abstract void Scatter();

        // calculate the target tile
        protected abstract Vector2D GetTargetTile();

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
            // ...

            currentTile = world.GetTile(x, y);

            // update target tile if centered on a tile
            if (x % tileset.Size == 0 && y % tileset.Size == 0)
            {
                // update target tile
                targetTile = GetTargetTile();

                // store distances to 'target tile' from tiles ajacant to the 'current tile'
                List<int> distances = new List<int>(DIRECTIONS);

                // loop ajacant tiles in order
                for (int i = 0; i < DIRECTIONS; i++)
                {
                    Vector2D direction = Directions[i];
                    Vector2D ajacantTile = new Vector2D(CurrentTile.X + direction.X, CurrentTile.Y + direction.Y);

                    // get ajacant tile distances setting wall and previous tiles to high numbers
                    distances.Add(world.GetTileObject(ajacantTile).Wall || Trajectory.Invert().Equals(direction)
                        ? INFINITY
                        : (int)Vector2D.GetAbsDistance(world.GetCoordinate(ajacantTile), world.GetCoordinate(targetTile)));
                }

                // set the ghosts trajectory to the index of the first instance
                // of the shortest distance in the directions list<vector>
                Trajectory = Directions[distances.IndexOf(distances.Min())];
            }

            // update sprite
            base.Update();
        }
    }
}