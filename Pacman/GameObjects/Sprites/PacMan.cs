
//
//  PacMan : Sprite Class
//  Created 5/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Sprite that moves according to provided input, animating
//  in the appropriate direction, and cornering at intersections.
//
//  PacMan sprites die when colliding with ghosts and eat them
//  when their mode is FRIGHTENED
//

using System;
using System.Collections.Generic;
using System.Drawing;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites
{
    class PacMan : Sprite
    {
        // CONSTANTS

        // start position

        private const int START_X       = 108;
        private const int START_Y       = 208;

        // texture indices
        private const int UP            = 88;
        private const int DOWN          = 96;
        private const int LEFT          = 92;
        private const int RIGHT         = 84;

        // animation

        private const int ANIMATION     = 20;
        private const int DIRECTIONS    = 4;

        // tile infomation

        private const int TILE_WIDTH    = 2;
        private const int TILE_HEIGHT   = 2;
        private const int TILE_HEADER   = 7;

        // FIELDS

        // animations

        private Animation up;
        private Animation down;
        private Animation left;
        private Animation right;

        private Animation[] directionalAnimations;

        // history

        private List<Direction> directionHistory;

        // CONSTRUCTOR

        public PacMan(float x, float y, World world, Game game)
            : base(START_X, START_Y, UP, TILE_WIDTH, TILE_HEIGHT, new Vector2D(), world)
        {
            // initatlize fields

            speed               = 1f;
            directionHistory    = new List<Direction>();
            locked              = true;
            offsetX = offsetY   = 3;

            // create animations

            up = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(UP, TILE_WIDTH , TILE_WIDTH),
                tileset.GetTileSourceRect(UP + TILE_WIDTH, TILE_WIDTH , TILE_WIDTH)
            },
            ANIMATION, loop: true);

            down = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(DOWN, TILE_WIDTH , TILE_WIDTH),
                tileset.GetTileSourceRect(DOWN + TILE_WIDTH, TILE_WIDTH , TILE_WIDTH)
            },
            ANIMATION, loop: true);

            left = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(LEFT, TILE_WIDTH , TILE_WIDTH),
                tileset.GetTileSourceRect(LEFT + TILE_WIDTH, TILE_WIDTH , TILE_WIDTH)
            },
            ANIMATION, loop: true);

            right = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(RIGHT, TILE_WIDTH , TILE_WIDTH),
                tileset.GetTileSourceRect(RIGHT + TILE_WIDTH, TILE_WIDTH , TILE_WIDTH)
            },
            ANIMATION, loop: true);

            // store directional animations in an array    
            directionalAnimations = new Animation[DIRECTIONS]
            { up, right, down, left };

            // start animations

            up.Start();
            down.Start();
            left.Start();
            right.Start();

            direction = Direction.UP;
            currentAnimation = directionalAnimations[(int)direction];
        }

        // PROPERTIES



        // METHODS

        public override void Update()
        {
            // round position
            int x = (int)Math.Round(this.x);
            int y = (int)Math.Round(this.y);

            // set animation and trajectory
            switch (direction)
            {
                case Direction.UP       : Trajectory.Y = -1;    break;
                case Direction.DOWN     : Trajectory.Y = 1;     break;
                case Direction.LEFT     : Trajectory.X = -1;    break;
                case Direction.RIGHT    : Trajectory.X = 1;     break;
            }

            // get the current tile

            Vector2D previousTile = currentTile;
            currentTile = world.GetTile(x, y); ;

            // calculate the target tile based on the front most pixel

            if (Trajectory.X == 1 && Trajectory.Y == 1)
                currentTile = world.GetTile(x + TILE_HEADER, y + TILE_HEADER);

            else if (Trajectory.X == 1)
                currentTile = world.GetTile(x + TILE_HEADER, y);

            else if (Trajectory.Y == 1)
                currentTile = world.GetTile(x, y + TILE_HEADER);

            if (!previousTile.Equals(currentTile))
                directionHistory.Add(direction);

            // check for wall collisons and direction changes when pacMan is centered on a tile origin

            // y axis tile origin
            if ((y - world.Y) % tileset.Size == 0
                && (world.GetTileObject(new Vector2D(CurrentTile.X, CurrentTile.Y + Trajectory.Y)).Wall
                || direction != Direction.UP && direction != Direction.DOWN))
            {
                Trajectory.Y = 0;
            }

            // x axis tile origin
            if ((x - world.X) % tileset.Size == 0
                && (world.GetTileObject(new Vector2D(CurrentTile.X + Trajectory.X, CurrentTile.Y)).Wall
                || direction != Direction.LEFT && direction != Direction.RIGHT))
            {
                // block wall x-axis collisions only while on the board
                if (currentTile.X > 0 && currentTile.X < world.WidthTiles)
                    Trajectory.X = 0;
            }

            // check for tunnel travel

            if (currentTile.X < -1 && direction == Direction.LEFT)
            {
                // teleport pacman to opposite side
                X = world.Width + tileset.Size + 1;

                locked = true;
                game.QueueTask(Time.QUARTER_SECOND, () => locked = false);                

                // animate tunnel leftward
                world.Slide(Direction.LEFT);
            }

            else if (X > world.Width && direction == Direction.RIGHT)
            {
                // teleport pacman to opposite side
                X = -tileset.Size * 2 - 1;

                locked = true;
                game.QueueTask(Time.QUARTER_SECOND, () => locked = false);

                // animate tunnel rightward
                world.Slide(Direction.RIGHT);
            }


            // determine if pacman is moving
            if (Trajectory.X != 0 || Trajectory.Y != 0)
                CurrentAnimation.Start();

            else
            {
                CurrentAnimation.Animating = false;

                // re-apply previous direction (prevents 'orphaned corner collision')
                if (directionHistory.Count > 2)
                {
                    Direction direction = directionHistory[directionHistory.Count - 2];
                    Direction = direction;
                }
            }

            // set the correct animation
            CurrentAnimation = directionalAnimations[(int)direction];

            // update the sprite
            base.Update();
        }

        public override void Input()
        {
            if (locked) return;

            // grab the direction before it updates
            previousDirection = Direction == previousDirection 
                ? previousDirection 
                : Direction;

            // move UP if not already going UP
            // and the next tile is not a wall            
            if (InputManager.Up 
                && direction != Direction.UP
                && !world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y - 1)).Wall)
            {
                Direction = Direction.UP;
                Trajectory.Y = -1;
            }

            // move DOWN if not already going DOWN
            // and the next tile is not a wall            
            else if (InputManager.Down 
                && direction != Direction.DOWN 
                && !world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y + 1)).Wall)
            {
                Direction = Direction.DOWN;
                Trajectory.Y = 1;
            }

            // move LEFT if not already going LEFT
            // and the next tile is not a wall
            else if (InputManager.Left
                && direction != Direction.LEFT
                && !world.GetTileObject(new Vector2D(currentTile.X - 1, currentTile.Y)).Wall)
            {
                Direction = Direction.LEFT;
                Trajectory.X = -1;
            }

            // move RIGHT if not already going RIGHT
            // and the next tile is not a wall
            else if (InputManager.Right
                && direction != Direction.RIGHT
                && !world.GetTileObject(new Vector2D(currentTile.X + 1, currentTile.Y)).Wall)
            {
                Direction = Direction.RIGHT;
                Trajectory.X = 1;
            } 
        }
    }
}
