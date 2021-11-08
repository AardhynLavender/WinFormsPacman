
//
//  Game Class
//
//  Defines functionality and members for an abstract game object.
//  that manages GameObjects and renders infomation to a Screen
//

using FormsPixelGameEngine.GameObjects;
using FormsPixelGameEngine.Render;
using FormsPixelGameEngine.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;

namespace FormsPixelGameEngine
{
    abstract class Game
    {
        // CONSTANTS

        private const int TICKRATE = 10;

        // FIELDS

        protected GameScreen screen;
        protected Random random;
        protected SoundPlayer Media;

        // ticking

        protected Stopwatch gameTime;
        protected System.Windows.Forms.Timer ticker;
        protected long tick;

        // object and task managment

        protected static List<GameObject> gameObjects;
        protected static List<GameObject> deleteQueue;
        protected static List<Animation> animations;
        private static List<Task> taskQueue;
        private static List<Task> drawQueue;

        // physics

        protected bool processPhysics;
        protected bool processAnimations;

        // CONSTRUCTOR

        protected Game(GameScreen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
        {
            // initalize fields

            this.ticker             = ticker;
            this.ticker.Interval    = TICKRATE;
            this.screen             = screen;
            Media                   = media;
            random                  = new Random();

            // runnning time

            gameTime = new Stopwatch();

            // create object managment lists

            gameObjects             = new List<GameObject>();
            deleteQueue             = new List<GameObject>();
            animations              = new List<Animation>();
            taskQueue               = new List<Task>();
            drawQueue               = new List<Task>();

            // raise processing flags

            processPhysics          = true;
            processAnimations       = true;

            gameTime.Start();
        }

        // PROPERTIES

        public GameScreen Screen
        {
            get => screen;
            set => screen = value;
        }

        // assigned exernally to call the forms quit method
        public Action Quit
            { get; set; }

        public int TickRate
            => TICKRATE;

        public long Tick 
            => tick;

        public long RunningTime 
            => gameTime.ElapsedMilliseconds;

        public Random Random
            => random;

        // LOOPS

        protected virtual void Process()
        {
            // order by z index
            gameObjects = gameObjects.OrderBy(gameObject => gameObject.Z).ToList();

            // update objects
            if (processPhysics)
                gameObjects.ToList().ForEach(gameObject => 
                {
                    gameObject.Input();
                    gameObject.Update();
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
            screen.Clear();

            foreach (GameObject gameObject in gameObjects) 
                gameObject.Draw();

            // run the tasks in the drawing queue
            drawQueue.Where(task => !task.Called).ToList().ForEach(
                task => task.TryRun()
            );

            screen.Present();
        }

        // process and render
        public void GameLoop()
        {
            Process();
            Render();

            tick++;
        }

        // OBJECT MANAGMENT

        // adds a game object to the game
        public virtual GameObject AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);    // add to game
            gameObject.OnAddGameObject();   // tell game object it has been added

            return gameObject;
        }

        // queues an object for removal
        public virtual void QueueFree(GameObject gameObject)
            => deleteQueue.Add(gameObject);

        // removes a game objects if not null
        private static void free(GameObject gameObject)
        {
            if (!(gameObject is null))
            {
                gameObject.OnFreeGameObject();
                gameObjects.Remove(gameObject);
            }
        }

        // frees the objects in the queue
        protected static void freeQueue()
        {
            // using for loop because it is lock free
            for (int i = 0; i < deleteQueue.Count; i++)
                free(deleteQueue[i]);
            
            deleteQueue.Clear();
        }

        // checks if an object is currently being processed by the game
        public static bool IsInGame(GameObject gameObject) 
            => gameObjects.Contains(gameObject);

        // ANIMATION MANAGMENT

        // adds an animaion to the game for processing
        public static Animation AddAnimation(Animation animation)
        {
            animations.Add(animation);
            return animations.Last();
        }

        // removes an animation from the game
        public static void RemoveAnimation(Animation animation)
            => animations.Remove(animation);

        // plays the provided sound stream
        public virtual void PlaySound(Stream sound)
        {
            sound.Position = 0;
            Media.Stream = sound;
            Media.Play();
        }        
        
        // plays the provided sound filepath 
        public virtual void PlaySound(string filepath)
        {
            Media.SoundLocation = filepath;
            Media.Play();
        }

        // Queues a task to be called
        public void QueueTask(int milliseconds, Action callback)
            => taskQueue.Add(new Task(callback, milliseconds, this));

        // queues a task to be called just before render presentation
        // use this method for last minute drawing to the screen
        public void QueueDraw(Action callback)
            => drawQueue.Add(new Task(callback, 0, this));

        // ABSTRACT AND VIRTUAL

        // handles set up code for a game to begin
        public virtual void StartGame()
        {
            gameTime.Restart();
            gameTime.Start();
        }

        // handles destruction code for the game
        public abstract void EndGame();

        // handles data saving (unimplimented)
        protected virtual void SaveGame()
            => Console.WriteLine("[Game] > unhandled save request");
    }
}
