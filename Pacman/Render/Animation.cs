
//
//  Animation
//  Created 25/10/2021
//
//  WinForms PacMan v0.0.1
//  Aardhyn Lavender 2021
//
//  Animates the texture of a given object by updating
//  its texture from a provided range, size, and starting
//  index
//

using System;
using System.Collections.Generic;
using System.Drawing;

using FormsPixelGameEngine.Utility;
using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.GameObjects.Sprites.Ghosts;

namespace FormsPixelGameEngine.Render
{
    class Animation
    {
        // FIELDS

        private bool animating;
        
        private int speed;
        private bool loop;
        private int loopCap;
        private long loops;

        private Action onAnimationEnd;

        // TODO :: convert to static memebers too
        private Game game;
        private TileSet tileset;
        private GameObject gameObject;

        private Rectangle orignalRect;
        private int currentFrame;
        private int size;
        private int startIndex;
        private int frameCount;

        public bool Animating
        {
            get => animating;
            set => animating = value;
        }

        // CONSTRUCTOR

        public Animation(Game game, TileSet tileset, GameObject gameObject, int size, int startIndex, int frameCount, int speed, Action onAnimationEnd = null, bool loop = true, int loopCap = -1)
        {
            this.game           = game;
            this.tileset        = tileset;
            this.gameObject     = gameObject;
            this.size           = size;
            this.startIndex     = startIndex;
            this.speed          = speed / game.TickRate;
            this.loop           = loop;
            this.loopCap        = loopCap;
            this.frameCount     = frameCount;

            this.onAnimationEnd = (onAnimationEnd is null) 
                ? () => { } // empty lambda
                : onAnimationEnd;

            orignalRect         = gameObject.SourceRect;
            animating           = false;
        }

        // PROPERTIES



        // METHODS

        public void Update()
        {
            if (game.Tick % speed == 0 && animating)
            {
                if (currentFrame == frameCount)
                {
                    loops++;
                    if (loop)
                    {
                        currentFrame = 0;
                    }
                    else if (loops > loopCap)
                    {
                        // stop animating and call the Action Delegate
                        Stop();
                        onAnimationEnd();
                        return;
                    }
                }

                // update the source rect for the tileset
                gameObject.SourceRect = tileset.GetTileSourceRect(startIndex + (currentFrame * size), size, size);

                currentFrame++;
            }
        }
    
        // starts animation
        public void Start()
            => animating = true;

        // pauses animation
        public void Stop()
            => animating = false;

        // pauses and reverts texture
        public void Reset()
        {
            Stop();
            gameObject.SourceRect = orignalRect ;
        }
    }
}
