﻿
//
//  Point : Tile Object Class
//  Created 13/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A TileObject that increments the games score counter
//  by VALUE when collided and makes a 'Wah' or 'Kka'
//  every collision
//

using FormsPixelGameEngine.Properties;
using System;

namespace FormsPixelGameEngine.GameObjects.Tiles
{
    class Point : TileObject
    {
        // CONSTANT AND STATIC MEMEBERS

        private const int TEXTURE   = 22;
        private const int VALUE     = 10;
        private static int count    = 0;

        // FIELDS

        private int value;

        // CONSTRUCTOR

        public Point(Game game, int index, World world)
            : base(game, index, world, TEXTURE)
        { value = VALUE; }

        public override void Update()
        {
            // does pacman collide with this tile?
            if (((PacManGame)game).PacManPosition.Equals(world.GetTile(this)))
            {
                // increment score and remove point
                ((PacManGame)game).Score += value;
                world.ClearTile(this);
                
                // play 'wah' and 'kah' alternating
                game.PlaySound(++count % 2 == 0 ? Resources.waka_a : Resources.waka_b);
            }
        }
    }
}
