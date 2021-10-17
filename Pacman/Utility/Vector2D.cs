
//
//  Vector2D Structure
//
//  A data structure to define a vector in 2D space. providing
//  methods to zero, invert, and calculate distances.
//

using System;

namespace FormsPixelGameEngine.Utility
{
    struct Vector2D
    {
        // fields
        public float X;
        public float Y;

        // constructor
        public Vector2D(float x, float y)
        {
            // initalize fields
            X = x;
            Y = y;
        }

        // resets the vector to (0,0)
        public void Zero() 
            => X = Y = 0;

        // inverts the vector
        public Vector2D Invert()
            => new Vector2D(X * -1, Y * -1);

        // gets the absolute distance between two vectors
        public static Vector2D GetDifferenceVector(Vector2D a, Vector2D b)
            => new Vector2D(a.X - b.X, a.Y - b.Y);

        public static float GetAbsDistance(Vector2D a, Vector2D b)
            => (float)Math.Abs(Math.Sqrt(Math.Pow(b.X - a.X, 2.0f) + Math.Pow(b.Y - a.Y, 2.0f)));

        public bool Equals(Vector2D vector2D)
            => vector2D.X == X && vector2D.Y == Y;

        public override string ToString()
            => $"({X}, {Y})";
    }
}
