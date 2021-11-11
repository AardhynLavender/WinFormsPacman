
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
        // CONSTANTS

        private const int TUNNEL_Y          = 17;
        private const int TUNNEL_START_X    = 6;
        private const int TUNNEL_END_X      = 21;

        // FIELDS

        protected float localX;
        protected float localY;

        protected bool locked;
        private bool frozen;
        protected float speed;
        private bool hidden;

        protected World world;
        protected Vector2D currentTile;
        protected Direction direction;
        protected Direction previousDirection;
        protected Animation currentAnimation;

        // CONSTRUCTOR

        public Sprite(float x, float y, int index, int tileSpanX, int tileSpanY, Vector2D trajectory, World world)
            : base(x, y, index, STANDARD_Z, tileSpanX, tileSpanY)
        {
            // initalize fields

            this.world  = world;
            Trajectory  = trajectory;
            hidden      = false;
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
            => (currentTile.X < TUNNEL_START_X || currentTile.X > TUNNEL_END_X)
                && currentTile.Y == TUNNEL_Y;

        public bool Locked
        {
            get => locked;
            set => locked = value;
        }

        public bool Frozen
        {
            get => frozen;
            set => frozen = value;
        }

        // METHODS

        // object will not be drawn
        public void Hide()
            => hidden = true;

        // object will be drawn
        public void Show()
            => hidden = false;

        // shifts the game object by its defined trajectory
        public override void Update()
        {
            if (!frozen)
            {
                x += Trajectory.X * speed;
                y += Trajectory.Y * speed;

                localX = (int)Math.Floor(x % tileset.Size);
                localY = (int)Math.Floor(y % tileset.Size);
            }
        }

        // draws the gameobject translated by defined offset
        public override void Draw()
        {
            if (!hidden)
                screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x - offsetX, (int)y - offsetY, width, height));
        }
    }
}
