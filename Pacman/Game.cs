
//
//  Game Class
//
//  Defines functionality and members for an abstract game object.
//  that manages GameObjects and renders infomation to a Screen
//

using Breakout.GameObjects;
using Breakout.Render;
using Breakout.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;

namespace Breakout
{
    abstract class Game
    {
        // constants
        private const int TICKRATE = 17;

        // fields
        protected Screen screen;
        protected Random random;
        protected SoundPlayer Media;

        protected System.Windows.Forms.Timer ticker;
        protected long tick;

        protected List<GameObject> gameObjects;
        protected List<GameObject> deleteQueue;
        protected List<Animation> animations;

        private List<Task> taskQueue;

        protected bool processPhysics;
        protected bool processAnimations;

        private int sleepTicks;

        // properties

        protected int SleepTicks
        {
            get => sleepTicks;
            set 
            {
                sleepTicks = value;
                if (sleepTicks <= 0) processPhysics = true;
            }
        }

        public Screen Screen 
        { 
            get => screen; 
            set => screen = value; 
        }

        public Action Quit 
        { 
            get; 
            set; 
        }

        public static int TickRate
        {
            get => TICKRATE;
        }

        public long Tick
        {
            get => tick;
        }

        // constructor
        protected Game(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
        {
            this.ticker             = ticker;
            this.ticker.Interval    = TICKRATE;
            this.screen             = screen;

            random = new Random();

            gameObjects             = new List<GameObject>();
            deleteQueue             = new List<GameObject>();
            taskQueue               = new List<Task>();
            animations              = new List<Animation>();

            Media                   = media;
            processPhysics          = true;
            processAnimations       = true;
        }

        // Main loops

        protected virtual void Process()
        {
            // order by z index
            gameObjects = gameObjects.OrderBy(gameObject => gameObject.Z).ToList();

            // update objects
            if (processPhysics)
                gameObjects.ToList().ForEach(gameObject => 
                {
                    gameObject.Update();
                    gameObject.Physics();
                });

            // update animations
            if (processAnimations)
                foreach (Animation animation in animations)
                    animation.Update();

            // process queued tasks
            taskQueue.Where(task => !task.Called).ToList().ForEach(
                task => task.TryRun()
            );

            // free objects queued for removal
            freeQueue();
        }

        // draw all game objects to the screen
        protected virtual void Render()
        {
            screen.RenderClear();

            foreach (GameObject gameObject in gameObjects) 
                gameObject.Draw();

            screen.RenderPresent();
        }

        // process and render
        public void GameLoop()
        {
            Process();
            Render();

            tick++;
        }

        // Object adding and freeing

        // adds a game object to the game
        public GameObject AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);    // add to game
            gameObject.OnAddGameObject();   // tell game object it has been added

            return gameObject;
        }

        // queues an object for removal
        public void QueueFree(GameObject gameObject)
            => deleteQueue.Add(gameObject);

        // removes a game objects if not null
        private void free(GameObject gameObject)
        {
            if (!(gameObject is null))
            {
                gameObject.OnFreeGameObject();
                gameObjects.Remove(gameObject);
            }
        }

        // frees the objects in the queue
        protected void freeQueue()
        {
            // using for loop because it is lock free
            for (int i = 0; i < deleteQueue.Count; i++)
                free(deleteQueue[i]);
            
            deleteQueue.Clear();
        }

        // checks if an object is currently being processed by the game
        public bool IsInGame(GameObject gameObject) 
            => gameObjects.Contains(gameObject);

        // Animations

        // adds an animaion to the game for processing
        public Animation AddAnimation(Animation animation)
        {
            animations.Add(animation);
            return animations.Last();
        }

        // Physics

        // plays the provided sound stream
        public virtual void PlaySound(Stream sound)
        {
            sound.Position = 0;
            Media.Stream = sound;
            Media.Play();
        }

        // Queues a task to be called
        public void QueueTask(int milliseconds, Action callback)
            => taskQueue.Add(new Task(callback, milliseconds));

        // Abstract Memebers
        
        // handles set up code for a game to begin
        public abstract void StartGame();

        // handles destruction code for the game
        public abstract void EndGame();

        // handles data saving (unimplimented)
        protected abstract void SaveGame();
    }
}
