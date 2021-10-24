
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
using System.Drawing;

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites
{
    class Sprite : GameObject
    {
        // FIELDS

        protected float localX;
        protected float localY;

        protected bool locked;
        protected float speed;

        protected World world;
        protected Vector2D currentTile;
        protected Direction direction;
        protected Direction previousDirection;
        protected Animation currentAnimation;

        // CONSTRUCTOR

        public Sprite(float x, float y, int index, int tileSpanX, int tileSpanY, Vector2D trajectory, World world)
            : base(x, y, index, STANDARD_Z, tileSpanX, tileSpanY)
        {
            this.world = world;
            Trajectory = trajectory;
        }

        // PROPERTIES

        public Vector2D CurrentTile
            => currentTile;

        public Animation CurrentAnimation
        {
            get => currentAnimation;
            set
            {
                Game.RemoveAnimation(currentAnimation);
                currentAnimation = value;
                Game.AddAnimation(currentAnimation);
            }
        }

        public virtual Direction Direction
        {
            get => direction;
            set => direction = value;
        }

        public Vector2D Trajectory;

        protected bool inTunnel
            => (currentTile.X < 6 || currentTile.X > 21)
                && currentTile.Y == 17;

        public bool Locked
        {
            get => locked;
            set => locked = value;
        }

        // METHODS

        public override void Update()  
        {
            x += Trajectory.X * speed;
            y += Trajectory.Y * speed;

            localX = (int)Math.Floor(x % tileset.Size);
            localY = (int)Math.Floor(y % tileset.Size);
        }

        public override void Draw()
            => screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x - offsetX, (int)y - offsetY, width, height));
    }
}
