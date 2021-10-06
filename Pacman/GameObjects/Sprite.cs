
//
//  Sprite : Game Object Class
//  Created 04/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A Game Object that has velocity and responds to tile based
//  physics using a current tile based on the provided world.
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

        protected World world;
        protected Vector2D currentTile;
        protected Direction direction;

        // CONSTRUCTOR

        public Sprite(float x, float y, int index, int tileSpanX, int tileSpanY, Vector2D trajectory, World world)
            : base(x, y, tileset.GetTileSourceRect(index), 100, tileSpanX, tileSpanY)
        {
            this.world = world;
            Trajectory = trajectory;
        }

        // PROPERTIES

        public Vector2D CurrentTile
            => currentTile;

        public bool MovingDiagonaly
            => Trajectory.Y == 1 && Trajectory.X == 1
            || Trajectory.X == -1 && Trajectory.Y == -1;

        public virtual Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public Vector2D Trajectory;

        public override void Update()  
        {
            x += Trajectory.X;
            y += Trajectory.Y;
        }

        public override void Draw()
        {
            screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x - 3, (int)y - 3, width, height));
        }
    }
}
