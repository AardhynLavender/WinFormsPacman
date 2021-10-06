
//
//  Task Class
//  
//  Defines a simple object to delay a function being
//  called for a specifed amount of game loops 
//
//  This avoids the need to use threading for asynchronous 
//  function callbacks and ensures the method is called at
//  the end of the game loop rather than loosing control
//  of execution
//

using System;

namespace FormsPixelGameEngine.Utility
{
    class Task
    {
        // fields
        Game game;
        bool called;
        long callTime;
        Action callback;

        // constructor
        public Task(Action callback, int milliseconds, Game game)
        {
            callTime        = game.RunningTime + milliseconds;
            called          = false;

            this.game       = game;
            this.callback   = callback;
        }

        // has the task been run yet
        public bool Called => called;

        // attempt to run the task
        public bool TryRun()
        {
            if (game.RunningTime > callTime)
            {
                callback();
                called = true;
            }

            return called;
        }
    }
}
