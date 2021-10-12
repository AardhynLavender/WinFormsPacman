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

        // CONSTRUCTOR

        public PacMan(float x, float y, World world, Game game)
            : base(START_X, START_Y, UP, TILE_WIDTH, TILE_HEIGHT, new Vector2D(), world)
        {
            speed = 1.0f;

            // create animations

            up = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(UP, 2 , 2),
                tileset.GetTileSourceRect(UP + 2, 2 , 2)
            },
            5, loop: true);

            down = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(DOWN, 2 , 2),
                tileset.GetTileSourceRect(DOWN + 2, 2 , 2)
            },
            5, loop: true);

            left = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(LEFT, 2 , 2),
                tileset.GetTileSourceRect(LEFT + 2, 2 , 2)
            },
            5, loop: true);

            right = new Animation(game, this, new List<Rectangle>
            {
                tileset.GetTileSourceRect(RIGHT, 2 , 2),
                tileset.GetTileSourceRect(RIGHT + 2, 2 , 2)
            },
            5, loop: true);

            // start animations

            up.Start();
            down.Start();
            left.Start();
            right.Start();
        }

        // PROPERTIES

        // block directions that could cause collision
        public override Direction Direction
        {
            get => base.Direction;
            set
            {
                switch (value)
                {
                    case Direction.UP:
                        direction = world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y - 1)).Wall
                            ? direction : value;

                        break;

                    case Direction.RIGHT:
                        direction = world.GetTileObject(new Vector2D(currentTile.X + 1, currentTile.Y)).Wall
                            ? direction : value;

                        break;

                    case Direction.DOWN:
                        direction = world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y + 1)).Wall
                            ? direction : value;

                        break;

                    case Direction.LEFT:
                        direction = world.GetTileObject(new Vector2D(currentTile.X - 1, currentTile.Y)).Wall
                            ? direction : value;

                        break;
                }
            }
        }

        // METHODS

        public override void Update()
        {
            // round position
            int x = (int)Math.Round(this.x);
            int y = (int)Math.Round(this.y);

            // get the current tile
            currentTile = world.GetTile(x, y);

            // calculate the target tile based on the front most pixel

            if (Trajectory.X == 1 && Trajectory.Y == 1)
                currentTile = world.GetTile(x + TILE_HEADER, y + TILE_HEADER);

            else if (Trajectory.X == 1)
                currentTile = world.GetTile(x + TILE_HEADER, y);

            else if (Trajectory.Y == 1)
                currentTile = world.GetTile(x, y + TILE_HEADER);

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

            // set animation
            switch(direction)
            {
                case Direction.UP       : CurrentAnimation = up;
                    break;

                case Direction.DOWN     : CurrentAnimation = down;
                    break;

                case Direction.LEFT     : CurrentAnimation = left;
                    break;

                case Direction.RIGHT    : CurrentAnimation = right;
                    break;
            }

            // toggle animation
            if (Trajectory.X != 0 || Trajectory.Y != 0)
                CurrentAnimation.Start();
            else
                CurrentAnimation.Animating = false;

            // update the sprite
            base.Update();
        }

        public override void Input()
        {
            if (InputManager.Up)
            {
                Direction = Direction.UP;
                Trajectory.Y = -1;
            }

            else if (InputManager.Down)
            {
                Direction = Direction.DOWN;
                Trajectory.Y = 1;
            }

            else if (InputManager.Left)
            {
                Direction = Direction.LEFT;
                Trajectory.X = -1;
            }

            else if (InputManager.Right)
            {
                Direction = Direction.RIGHT;
                Trajectory.X = 1;
            }
        }
    }
}
