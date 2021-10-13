﻿
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
using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects
{
    class Sprite : GameObject
    {
        // CONSTATNS

        private const int TILE_CENTER = 3;

        // FIELDS

        protected float localX;
        protected float localY;

        protected World world;
        protected Vector2D currentTile;
        protected Direction direction;
        protected Direction previousDirection;
        protected Animation currentAnimation;
        protected float speed = 0;

        // CONSTRUCTOR

        public Sprite(float x, float y, int index, int tileSpanX, int tileSpanY, Vector2D trajectory, World world)
            : base(x, y, tileset.GetTileSourceRect(index), STANDARD_Z, tileSpanX, tileSpanY)
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

        public override void Update()  
        {
            x += Trajectory.X * speed;
            y += Trajectory.Y * speed;

            localX = (int)Math.Floor(x % tileset.Size);
            localY = (int)Math.Floor(y % tileset.Size);
        }

        public override void Draw()
            => screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x - TILE_CENTER, (int)y - TILE_CENTER, width, height));
    }
}