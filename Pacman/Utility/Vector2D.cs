
//
//  Vector2D Structure
//
//  A data structure to define a vector in 2D space. providing
//  methods to zero, invert, and calculate distances.
//

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

        // gets the absolute distance between two vectors
        public static void GetDistance(Vector2D a, Vector2D b)
            => new Vector2D(a.X - b.X, a.Y - b.Y);
    }
}
