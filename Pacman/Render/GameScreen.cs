
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

        private int width;
        private int height;
        private Image bufferImage;
        private Graphics buffer;
        private Graphics display;
        private float scale;

        // PROPERTIES

        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public bool MouseDown { get; set; }

        public int WidthScaled 
        {
            get => (int)Math.Round(width * Scale); 
            set => width = value; 
        }

        public int HeightScaled 
        { 
            get => (int)Math.Round(height * Scale); 
            set => height = value; 
        }

        public float Scale { get; set; }

        public int Width
        {
            get => width;
        }

        public int Height 
        {
            get => height;
        }

        public Graphics Buffer 
        { 
            get => buffer; 
            set => buffer = value;
        }


        // CONSTRUCTOR

        public GameScreen(Graphics display, int width, int height, float scale)
        {
            // initalize fields

            WidthScaled                 = width;
            HeightScaled                = height;
            Scale                       = scale;

            // create buffer

            bufferImage                 = new Bitmap(WidthScaled, HeightScaled);
            Buffer                      = Graphics.FromImage(bufferImage);
            this.display                = display;

            // configure buffer

            Buffer.PixelOffsetMode      = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            Buffer.InterpolationMode    = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        }

        // clears the screen
        public void Clear()
            => Buffer.FillRectangle(Brushes.Black, new Rectangle(0, 0, WidthScaled, HeightScaled));

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
            => display.DrawImage(bufferImage, 0, 0, WidthScaled, HeightScaled);
    }
}
