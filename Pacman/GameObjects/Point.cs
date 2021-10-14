using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormsPixelGameEngine.Properties;

namespace FormsPixelGameEngine.GameObjects
{
    class Point : TileObject
    {
        // CONSTANTS

        private const int TEXTURE = 22;
        private const int VALUE = 10;

        // FIELDS
        
        private static int count;

        private int value;

        // CONSTRUCTOR

        public Point(int index, World world)
            : base(index, world, tileset.GetTileSourceRect(TEXTURE))
        {
            value = VALUE;
        }

        public override void Update()
        {
            if (game.PacManPosition.Equals(world.GetTile(this)))
            {
                count++;
                game.Score += value;
                world.QueueTileFree(this);
                
                game.PlaySound(count % 2 == 0 ? Resources.waka_a : Resources.waka_b);
            }
        }
    }
}
