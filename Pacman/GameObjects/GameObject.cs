
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
        // CONSTANT AND STATIC MEMBERS

        protected const int STANDARD_Z = 100;
        protected static TileSet tileset = null;

        // FIELDS

        // postion and size

        protected float x;
        protected float y;
        protected int z;

        protected int width;
        protected int height;

        // texture

        protected Rectangle sourceRect;
        protected int offsetX;
        protected int offsetY;

        // game

        protected PacManGame game;
        protected GameScreen screen;

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

        // construct a textured object
        public GameObject(float x, float y, Rectangle sourceRect, int z = STANDARD_Z, int tileSpanX = 1, int tileSpanY = 1)
        {
            if (tileset is null) 
                throw new System.Exception("[Game Object] GameObject class does not contain a definition for static member 'texture'");

            // initalize fields

            this.x          = x;
            this.y          = y;
            this.z          = z;
            this.sourceRect = sourceRect;

            offsetX = offsetY = 0;

            // span multuple tiles if specified

            this.sourceRect.Width *= tileSpanX;
            this.sourceRect.Height *= tileSpanY;

            // set width and height

            width           = this.sourceRect.Width;
            height          = this.sourceRect.Height;
        }

        // PROPERTIES

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

        public PacManGame Game 
        { 
            protected get => game; 
            set => game = value; 
        }

        public GameScreen Screen 
        { 
            protected get => screen; 
            set => screen = value; 
        }

        // STATIC PROPERTIES

        public static TileSet Texture 
        {
            private get => tileset; 
            set => tileset = value; 
        }

        // METHODS

        // draws the object to the screen
        public virtual void Draw()
            => screen.Copy(tileset.Texture, sourceRect, new Rectangle((int)x, (int)y, width, height));

        // called per main loop to update any changes to the object
        public virtual void Update()
        {  }

        // called per main loop to process anything relating to user input
        public virtual void Input()
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
