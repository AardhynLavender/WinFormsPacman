
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

using System;
using System.Drawing;
using FormsPixelGameEngine.Render;

namespace FormsPixelGameEngine.GameObjects
{
    class GameObject
    {
        // CONSTANT AND STATIC MEMBERS

        protected const int STANDARD_Z = 100;

        //  static members explaination
        //
        //  static marks variables, properties, or functions in a class as members
        //  of the type itself rather than any instance of it.
        //  
        //  a GameObject type by definition is part of a game, every GameObject instance
        //  will reference the same Game type, and it will never change.
        //  The same goes for GameScreen, and tileset.
        //  
        //  To me, this makes Game, screen, and tileset, static.
        //
        //  The needless copying of memory addresses though endless paramatalization of these
        //  types in object initalization had static beenexluded offers very little gain in
        //  terms of performace, but this model will scale to larger programs.
        //
        protected static PacManGame game;
        protected static GameScreen screen;
        protected static TileSet tileset;

        // initalize static members
        public static void Init(PacManGame game, GameScreen screen, TileSet tileset)
        {
            GameObject.game     = game;
            GameObject.screen   = screen;
            GameObject.tileset  = tileset;
        }

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

        // CONSTRUCTORS 

        // construct a textureless object
        public GameObject(float x, float y)
        {
            // initalize fields

            this.x = x;
            this.y = y;

            z = STANDARD_Z;
            width = height = 0;
        }

        // construct a textured object
        public GameObject(float x, float y, int index, int z = STANDARD_Z, int tileSpanX = 1, int tileSpanY = 1)
        {
            // validate static assignment of GameObject type

            if (game is null)
                throw new System.Exception("[Game Object] GameObject type requires a static reference to a 'Game' type");

            if (screen is null)
                throw new System.Exception("[Game Object] GameObject type requires a static reference to a 'GameScreen' type");

            if (tileset is null)
                throw new System.Exception("[Game Object] GameObject type requires a static reference to a 'TileSet' type");

            // initalize fields

            this.x          = x;
            this.y          = y;
            this.z          = z;

            sourceRect = (index == -1 ) 
                ? new Rectangle()
                : tileset.GetTileSourceRect(index);

            offsetX = offsetY = 0;

            // span multuple tiles if specified

            sourceRect.Width *= tileSpanX;
            sourceRect.Height *= tileSpanY;

            // set width and height

            width           = sourceRect.Width;
            height          = sourceRect.Height;
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
    }
}
