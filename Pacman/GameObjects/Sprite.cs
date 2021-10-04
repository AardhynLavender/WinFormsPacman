
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
        { 
            get => currentTile; 
            set => currentTile = value; 
        }

        public Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public Vector2D Trajectory;

        public override void Update()  
        {
            if ((x - world.X) % tileset.Size == 0)   
            {
                if ((direction != Direction.LEFT && direction != Direction.RIGHT)
                && !world.GetTile(new Vector2D(CurrentTile.X, CurrentTile.Y + Trajectory.Y)).Wall)
                    Trajectory.X = 0;

                if (world.GetTile(new Vector2D(CurrentTile.X + Trajectory.X, CurrentTile.Y)).Wall)
                    Trajectory.X = 0;
            }

            if ((y - world.Y) % tileset.Size == 0)
            {
                if ((direction != Direction.UP && direction != Direction.DOWN)
                && !world.GetTile(new Vector2D(CurrentTile.X + Trajectory.X, CurrentTile.Y)).Wall)
                    Trajectory.Y = 0;

                if (world.GetTile(new Vector2D(CurrentTile.X, CurrentTile.Y + Trajectory.Y)).Wall)
                    Trajectory.Y = 0;
            }
            
            x += Trajectory.X;
            y += Trajectory.Y;
        }

        public override void Draw()
        {
            screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x - 4, (int)y - 4, width, height));
        }
    }
}
