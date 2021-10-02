
//
//  Game Object Class
//  Created 02/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  A generic representation of a object in 2D space
//  with a texture belonging to a game and being drawn
//  to a screen.
//


using System.Drawing;
using FormsPixelGameEngine.Render;

namespace FormsPixelGameEngine.GameObjects
{
    class GameObject
    {
        // CONSTANTS

        private const int STANDARD_Z = 100;

        // FIELDS

        // postion and size

        protected float x;
        protected float y;
        protected int z;
        protected int width;
        protected int height;

        // texture

        protected Image texture;
        protected Rectangle sourceRect;

        // game

        private PacManGame game;
        private GameScreen screen;

        // CONSTRUCTORS 

        // construct a textureless object
        public GameObject(float x, float y, int z = STANDARD_Z)
        {
            // initalize fields
            this.x = x;
            this.y = y;
            this.z = z;

            width = height = 0;
        }

        public GameObject(float x, float y, Image texture, Rectangle sourceRect, int z = STANDARD_Z, int tileSpanX = 1, int tileSpanY = 1)
        {
            // initalize fields

            this.x          = x;
            this.y          = y;
            this.z          = z;
            this.texture    = texture;
            this.sourceRect = sourceRect;

            // span multuple tiles if specified

            this.sourceRect.Width *= tileSpanX;
            this.sourceRect.Height *= tileSpanY;

            // set width and height

            width           = this.sourceRect.Width;
            height          = this.sourceRect.Height;
        }

        // PROPERTIES

        public Image Texture
        {
            get => texture;
            set => texture = value;
        }
        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public int Z
        {
            get => z;
            set => z = value;
        }

        public virtual int Width
        {
            get => width;
            set => width = value;
        }

        public virtual int Height
        {
            get => height;
            set => height = value;
        }

        public Rectangle SourceRect
        {
            get => sourceRect;
            set => sourceRect = value;
        }

        protected PacManGame Game 
        { 
            get => game; 
            set => game = value; 
        }

        protected GameScreen Screen 
        { 
            get => screen; 
            set => screen = value; 
        }

        // METHODS

        // draws the object to the screen
        public virtual void Draw()
            => screen.Copy(texture, sourceRect, new Rectangle((int)x, (int)y, width, height));

        // called per main loop to update any changes to the object
        public virtual void Update()
        {  }

        // called when the object is added to the game
        public virtual void OnAddGameObject()
        {  }

        // called when the object is freed from the game
        public virtual void OnFreeGameObject()
        {  }

        // raises or lowers the draw placment of the object
        public void bringForward(int increment = 1) => z += increment;
        public void pushBackward(int increment = 1) => z += -increment;
    }
}
