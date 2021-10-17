
//
//  Blinky : Ghost Class
//  Created 16/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A derived ghost that targets pacmans current
//  tile and <..talk about speed changes..>
//

using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;

namespace FormsPixelGameEngine.GameObjects.Sprites.Ghosts
{
    class Blinky : Ghost
    {
        // CONSTANTS

        private const int START_X = 107;
        private const int START_Y = 112;

        private const int TEXTURE_RIGHT = 168;
        private const int TEXTURE_LEFT  = 170;
        private const int TEXTURE_UP    = 172;
        private const int TEXTURE_DOWN  = 174;

        // FIELDS

        private Animation right;
        private Animation left;
        private Animation up;
        private Animation down;

        // CONSTRUCTOR

        public Blinky(World world, PacMan pacman)
            : base(0, 0, TEXTURE_RIGHT, world, pacman)
        {
            // set position, speed, and direction

            X = START_X;
            Y = START_Y;

            Trajectory = Directions[(int)Direction.LEFT];

            speed = 1.0f;

            // configure animations


        }

        // PROPERTIES



        // METHODS

        public override void Scatter()
            => throw new System.NotImplementedException();

        protected override Vector2D GetTargetTile()
            => pacman.CurrentTile;
    }
}