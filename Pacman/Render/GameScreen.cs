
//
//  Screen Class
//
//  an intermediary class that provides functionality to a
//  Game to render textures to a Form screen with scaling
//  and "pixel-perfect" "double buffered" rendering
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsPixelGameEngine.Render
{
    public sealed class GameScreen
    {
        // FIELDS

        private Image bufferImage;
        private Graphics buffer;
        private Graphics display;
        private float scale;

        // PROPERTIES

        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public bool MouseDown { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public float Scale { get; set; }

        public int WidthPixels
        { 
            get => (int)Math.Round(Width / Scale);
        }

        public Graphics Buffer 
        { 
            get => buffer; 
            set => buffer = value;
        }

        public int HeightPixels 
        {
            get => (int)Math.Round(Height / Scale); 
        }

        // CONSTRUCTOR

        public GameScreen(Graphics display, int width, int height, float scale)
        {
            // initalize fields

            Width                       = width;
            Height                      = height;
            Scale                       = scale;

            // create buffer

            bufferImage                 = new Bitmap(Width, Height);
            Buffer                      = Graphics.FromImage(bufferImage);
            this.display                = display;

            // configure buffer

            Buffer.PixelOffsetMode      = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            Buffer.InterpolationMode    = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        }

        // clears the screen
        public void Clear()
            => Buffer.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));

        // copies the buffer to the screen
        public void Copy(Image texture, Rectangle src, Rectangle dest)
        {
            // apply pixel scaling
            dest.X      = (int)Math.Round(dest.X * Scale);
            dest.Y      = (int)Math.Round(dest.Y * Scale);
            dest.Width  = (int)Math.Round(dest.Width * Scale);
            dest.Height = (int)Math.Round(dest.Height * Scale);

            // draw the scalled image to the buffer
            Buffer.DrawImage(texture, dest, src, GraphicsUnit.Pixel);
        }

        // presents the buffer to the display
        public void Present()
            => display.DrawImage(bufferImage, 0, 0, Width, Height);
    }
}
