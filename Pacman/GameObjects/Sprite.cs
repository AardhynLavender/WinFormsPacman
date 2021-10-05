
//
//  Sprite : Game Object Class
//  Created 04/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Game Object that has velocity and responds to physics
//  
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects
{
    class Sprite : GameObject
    {
        // FIELDS

        private World world;

        private Vector2D currentTile;
        public Vector2D targetTile;

        private Direction direction;

        // CONSTRUCTOR

        public Sprite(float x, float y, int index, int tileSpanX, int tileSpanY, Vector2D trajectory, World world)
            : base(x, y, tileset.GetTileSourceRect(index), 100, tileSpanX, tileSpanY )
        {
            this.world      = world;
            this.Trajectory = trajectory;
        }

        // PROPERTIES

        public Vector2D CurrentTile 
            => currentTile; 

        public Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public Vector2D Trajectory;

        public override void Update()  
        {
            Console.WriteLine(direction);

            currentTile = world.GetTile(x, y);

            if (Trajectory.X == 1 && Trajectory.Y == 1)
                currentTile = world.GetTile(x + 7, y + 7);            
            
            else if (Trajectory.X == 1)
                currentTile = world.GetTile(x + 7, y);

            else if (Trajectory.Y == 1)
                currentTile = world.GetTile(x, y + 7);

            targetTile = (direction == Direction.UP || direction == Direction.DOWN)
                ? new Vector2D(currentTile.X, currentTile.Y + Trajectory.Y)
                : new Vector2D(currentTile.X + Trajectory.X, currentTile.Y);


            if ((y - world.Y) % tileset.Size == 0)
            {
                if (world.GetTile(new Vector2D(CurrentTile.X, CurrentTile.Y + Trajectory.Y)).Wall)
                    Trajectory.Y = 0;

                if (direction != Direction.UP && direction != Direction.DOWN)
                    Trajectory.Y = 0;
            }

            if ((x - world.X) % tileset.Size == 0)
            {
                if (world.GetTile(new Vector2D(CurrentTile.X + Trajectory.X, CurrentTile.Y)).Wall)
                    Trajectory.X = 0;

                if (direction != Direction.LEFT && direction != Direction.RIGHT)
                    Trajectory.X = 0;
            }

            x += Trajectory.X;
            y += Trajectory.Y;
        }

        public override void Draw()
        {
            screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x - 3, (int)y - 3, width, height));
        }
    }
}
