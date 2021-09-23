
// 
//  Animation Class
//
//  Animates the texture of a given object by updating
//  its texture from a set of images or tiles based
//  on the gameloop of the provided Game.
//

using System;
using System.Collections.Generic;
using System.Drawing;

using FormsPixelGameEngine.Utility;
using FormsPixelGameEngine.GameObjects;

namespace FormsPixelGameEngine.Render
{
    class Animation
    {
        private int frameCount;
        private int speed;
        private bool animating;
        private bool loop;
        private int loopCap;
        private long loops;
        private Action onAnimationEnd;

        private Game game;
        private GameObject gameObject;

        private List<Rectangle> tileFrames;
        private Rectangle orignalRect;
        private int currentFrame;

        public bool Animating 
        { 
            get => animating;
            set => animating = value;
        }

        // Construct with generic list of tile coordinates
        public Animation(Game game, GameObject gameObject, List<Rectangle> textures, int speed, Action onAnimationEnd = null, bool loop = true, int loopCap = -1)
        { 
            this.game           = game;
            this.gameObject     = gameObject;
            this.speed          = speed / Game.TickRate;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;
            this.loop           = loop;
            this.loopCap        = loopCap;

            orignalRect         = gameObject.SourceRect;
            tileFrames          = textures;
            frameCount          = textures.Count;
            animating           = false;
        }

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
                        animating = false;
                        onAnimationEnd();
                        return;
                    }
                }

                // update the source rect for the tileset
                gameObject.SourceRect = tileFrames[currentFrame];

                currentFrame++;
            }
        }

        public void Start()
            => animating = true;

        public void Stop()
        {
            animating = false;
            gameObject.SourceRect = orignalRect;
        }

        // resets the animation to its original state
        public void Reset()
        {
            Stop();
            gameObject.SourceRect = orignalRect;
            loops = currentFrame = 0;
        }
    }
}
