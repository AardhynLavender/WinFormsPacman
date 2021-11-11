
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

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;
using System;
using System.Collections.Generic;

namespace FormsPixelGameEngine.GameObjects.Sprites
{
    class PacMan : Sprite
    {
        // CONSTANTS

        // position

        private const int START_X       = 108;
        private const int START_Y       = 208;
        private const int Z             = 200;

        // texture indices

        private const int STATIONARY    = 0;
        private const int UP            = 88;
        private const int DOWN          = 96;
        private const int LEFT          = 92;
        private const int RIGHT         = 84;

        // animation

        private const int ANIMATION     = 50;
        private const int DIRECTIONS    = 4;
        private const int DEATH_LENGTH  = 11;

        // tile infomation

        private const int SIZE_TILES    = 2;
        private const int TILE_HEADER   = 7;
        private const int OFFSET        = 3;

        // trajectory

        private const float SPEED       = 1.33f;

        // FIELDS

        // animations

        private Animation death;

        private Animation up;
        private Animation down;
        private Animation left;
        private Animation right;

        private Animation[] directionalAnimations;

        // history

        private List<Direction> directionHistory;

        // lives

        private LivesManager livesManager;
        private bool alive;

        // CONSTRUCTOR

        public PacMan(World world, LivesManager livesManager)
            : base(START_X, START_Y, STATIONARY, SIZE_TILES, SIZE_TILES, new Vector2D(), world)
        {
            // initatlize fields

            speed               = SPEED;
            directionHistory    = new List<Direction>();
            locked              = true;
            Frozen              = true;
            alive               = true;
            offsetX = offsetY   = OFFSET;
            z                   = Z;

            this.livesManager   = livesManager;

            // create animations 

            death = new Animation(game, tileset, this, SIZE_TILES, 0, DEATH_LENGTH, Time.TENTH_SECOND, () =>
            {
                death.Reset();
                sourceRect = tileset.GetTileSourceRect(-1, SIZE_TILES, SIZE_TILES);


                if (livesManager.DeductLife())
                    game.RestartLevel();

                else
                    game.EndGame();
            }, 
            false);

            up      = new Animation(game, tileset, this, SIZE_TILES, UP, SIZE_TILES, ANIMATION);
            right   = new Animation(game, tileset, this, SIZE_TILES, RIGHT, SIZE_TILES, ANIMATION);
            down    = new Animation(game, tileset, this, SIZE_TILES, DOWN, SIZE_TILES, ANIMATION);
            left    = new Animation(game, tileset, this, SIZE_TILES, LEFT, SIZE_TILES, ANIMATION);

            // store directional animations in an array    
            directionalAnimations = new Animation[DIRECTIONS]
            { up, right, down, left };

            Array.ForEach(directionalAnimations, a => a.Start());

            direction           = Direction.LEFT;
            currentAnimation    = directionalAnimations[(int)direction];
        }

        // PROPERTIES

        public bool Alive
            => alive;

        // METHODS

        // plays death animation and sound, deducts a life and resets the level.
        public void Kill()
        {
            // only kill if alive
            if (alive)
            {
                game.LooseLevel();
                alive = false;

                CurrentAnimation = death;
                CurrentAnimation.Stop();

                sourceRect = tileset.GetTileSourceRect(0, SIZE_TILES, SIZE_TILES);

                game.QueueTask(Time.TWO_SECOND, () =>
                {
                    CurrentAnimation.Start();
                    game.PlaySound(Properties.Resources.death_3);
                });
            }
        }
        
        // resets pacman
        public void Revive()
        {
            if (!alive)
            {
                CurrentAnimation.Reset();

                alive       = true;

                Frozen      = false;
                locked      = false;

                x           = START_X;
                y           = START_Y;

                direction   = Direction.LEFT;
            }
        }

        // updates positional, directional, and animation infomation
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
            currentTile = world.GetTile(x, y);

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

            if (alive)
            {
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

                if (Frozen)
                    CurrentAnimation.Animating = false;

                else
                    // update the sprite
                    base.Update();
            }

        }

        // handles input
        public override void Input()
        {
            if (locked || InputManager.MultipleKeysPressed) return;

            // grab the direction before it updates
            previousDirection = Direction == previousDirection 
                ? previousDirection 
                : Direction;

            // move UP if not already going UP
            // and not in tunnel
            // and the next tile is not a wall            
            if (InputManager.Up 
                && direction != Direction.UP
                && !inTunnel
                && !world.GetTileObject(new Vector2D(currentTile.X, currentTile.Y - 1)).Wall)
            {
                Direction = Direction.UP;
                Trajectory.Y = -1;
            }

            // move DOWN if not already going DOWN
            // and not in tunnel
            // and the next tile is not a wall            
            else if (InputManager.Down 
                && direction != Direction.DOWN 
                && !inTunnel
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