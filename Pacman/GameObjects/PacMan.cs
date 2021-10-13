using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects
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

            speed = 1.33f;
            directionHistory = new List<Direction>();

            // create animations

            up = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(UP, 2 , 2),
                tileset.GetTileSourceRect(UP + 2, 2 , 2)
            },
            ANIMATION, loop: true);

            down = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(DOWN, 2 , 2),
                tileset.GetTileSourceRect(DOWN + 2, 2 , 2)
            },
            ANIMATION, loop: true);

            left = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(LEFT, 2 , 2),
                tileset.GetTileSourceRect(LEFT + 2, 2 , 2)
            },
            ANIMATION, loop: true);

            right = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(RIGHT, 2 , 2),
                tileset.GetTileSourceRect(RIGHT + 2, 2 , 2)
            },
            ANIMATION, loop: true);

            // store directional animations in an array
            directionalAnimations = new Animation[4]
            { up, down, left, right };

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

            // check for wall collisons and direction changes when pacMan is centered on a tile

            if ((y - world.Y) % tileset.Size == 0
                && (world.GetTileObject(new Vector2D(CurrentTile.X, CurrentTile.Y + Trajectory.Y)).Wall
                || direction != Direction.UP && direction != Direction.DOWN))
            {
                Trajectory.Y = 0;
            }

            if ((x - world.X) % tileset.Size == 0
                && (world.GetTileObject(new Vector2D(CurrentTile.X + Trajectory.X, CurrentTile.Y)).Wall
                || direction != Direction.LEFT && direction != Direction.RIGHT))
            {
                Trajectory.X = 0;
            }

            // determine if pacman is moving
            if (Trajectory.X != 0 || Trajectory.Y != 0)
                CurrentAnimation.Start();
            else
            {
                CurrentAnimation.Animating = false;

                // reapply previous direction (prevents 'orphan corner collision')
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
            previousDirection = Direction == previousDirection 
                ? previousDirection 
                : Direction;

            if (InputManager.Up 
                && direction != Direction.UP
                && !world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y - 1)).Wall)
            {
                Direction = Direction.UP;
                Trajectory.Y = -1;
            }
            else if (InputManager.Down 
                && direction != Direction.DOWN 
                && !world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y + 1)).Wall)
            {
                Direction = Direction.DOWN;
                Trajectory.Y = 1;
            }
            else if (InputManager.Left
                && direction != Direction.LEFT
                && !world.GetTileObject(new Vector2D(currentTile.X - 1, currentTile.Y)).Wall)
            {
                Direction = Direction.LEFT;
                Trajectory.X = -1;
            }
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
