
//
//  GameObject class
//
//  A generic representation of a object in 2D space
//  with a texture and basic physics infomation.
//


using System.Drawing;
using Breakout.Render;
using Breakout.Utility;

namespace Breakout.GameObjects
{
    class GameObject
    {
        private const int STANDARD_Z = 100;

        private static Screen screen;

        // postion and size
        protected float x;
        protected float y;
        protected int z;
        protected int width;
        protected int height;

        // texture
        protected Image texture;
        protected Rectangle sourceRect;

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

        public GameObject(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public GameObject(float x, float y, Image texture, bool ghost = false)
        {
            X = x;
            Y = y;

            this.texture = texture;

            sourceRect = new Rectangle();

            sourceRect.Width = width = texture.Width;
            sourceRect.Height = height = texture.Height;
        }

        public GameObject(float x, float y, Image texture, Rectangle sourceRect, int z = STANDARD_Z, int tileSpanX = 1, int tileSpanY = 1, bool ghost = false)
        {
            this.x          = x;
            this.y          = y;
            this.z          = z;
            this.texture    = texture;
            this.sourceRect = sourceRect;

            // span multuple tiles if specified
            this.sourceRect.Width *= tileSpanX;
            this.sourceRect.Height *= tileSpanY;

            width           = this.sourceRect.Width;
            height          = this.sourceRect.Height;
        }

        public virtual void Draw()
            => screen.RenderCopy(texture, sourceRect, new Rectangle((int)x, (int)y, width, height));

        public virtual void Physics()
        {  }

        // called per main loop to update any changes to the object
        public virtual void Update()
        {  }

        // TODO :: try your hardest to get rid of this method
        public virtual void OnCollsion(GameObject collider)
        {  }

        // called when the object is added to the game
        public virtual void OnAddGameObject()
        {  }

        // called when the object is freed from the game
        public virtual void OnFreeGameObject()
        {  }

        public void bringForward(int increment = 1) => z += increment;
        public void pushBackward(int increment = 1) => z += -increment;
    }
}
