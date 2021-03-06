
//
//  Time Class
//  Created 02/07/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A group of constants to reference time
//  steps in terms of milliseconds.
//
//  NOTE :  This is not an emum as that would require
//          excessive casting from Time to Int.
//

namespace FormsPixelGameEngine.Utility
{
    public static class Time
    {
        public const int HALF_SECOND            = 500;
        public const int QUARTER_SECOND         = 250;
        public const int TENTH_SECOND           = 100;
        public const int TWENTYTH_SECOND        = 50;
        public const int HUNDREDTH_SECOND       = 10;
        public const int TWO_HUNDREDTH_SECOND   = 5;

        public const int SECOND                 = 1000;
        public const int TWO_SECOND             = SECOND * 2;
        public const int THREE_SECOND           = SECOND * 3;
        public const int FOUR_SECOND            = SECOND * 4;
        public const int FIVE_SECOND            = SECOND * 5;

        public const int SEVEN_SECOND           = SECOND * 7;
        public const int TWENTY_SECOND          = SECOND * 20;
    }
}
