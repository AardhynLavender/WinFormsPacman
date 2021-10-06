using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects
{
    class PacMan : Sprite
    {
        // CONSTANTS

        private const int START_X       = 108;
        private const int START_Y       = 208;
        private const int TEXTURE       = 84;
        private const int TILE_WIDTH    = 2;
        private const int TILE_HEIGHT   = 2;
        private const int TILE_HEADER   = 7;

        // FIELDS



        // CONSTRUCTOR

        public PacMan(float x, float y, World world)
            : base(START_X, START_Y, TEXTURE, TILE_WIDTH, TILE_HEIGHT, new Vector2D(), world)
        {  }

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

            // update the sprite

            base.Update();
        }
    }
}
