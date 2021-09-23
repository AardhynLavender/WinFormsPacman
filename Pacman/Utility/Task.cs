
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
        int sleepFor;
        bool called;
        Action callback;

        // constructor
        public Task(Action callback, int milliseconds)
        {
            sleepFor = milliseconds / Game.TickRate;
            called = false;
            this.callback = callback;
        }

        // has the task been run yet
        public bool Called => called;

        // attempt to run the task
        public bool TryRun()
        {
            if (--sleepFor < 1)
            {
                callback();
                called = true;
            }

            return called;
        }
    }
}
