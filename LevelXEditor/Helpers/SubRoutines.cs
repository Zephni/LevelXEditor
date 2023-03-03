using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LevelXEditor
{
    // SubRoutines once initiated starts a ticker that constantly runs in the background, and then conditional actions can be called. Call Initiate() to start the ticker.
    public static class SubRoutines
    {
        // Time
        private static DateTime startTime;
        private static DateTime lastTime;
        private static DateTime currentTime;
        private static float deltaTime;
        private static double measuringTime;

        // SubRoutines
        private static List<SubRoutine> subRoutines = new();

        // DispatchTimer
        private static System.Windows.Threading.DispatcherTimer dispatcherTimer = new();

        // Initiate
        public static void Initiate()
        {
            // Set time vars
            startTime = DateTime.Now;
            lastTime = DateTime.Now;
            currentTime = DateTime.Now;

            // Set delta time
            deltaTime = 0;

            // Set dispatcher timer
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimer.Start();
        }

        // Dispatcher timer tick
        private static void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Set time vars
            lastTime = currentTime;
            currentTime = DateTime.Now;

            // Set delta time
            deltaTime = (float)(currentTime - lastTime).TotalSeconds;

            // Run sub routines
            RunSubRoutines();
        }

        // Run sub routines
        private static void RunSubRoutines()
        {
            // Loop through sub routines
            for (int i = 0; i < subRoutines.Count; i++)
            {
                // Get sub routine
                SubRoutine subRoutine = subRoutines[i];

                // Check if condition is met
                if (subRoutine.condition())
                {
                    // Run action
                    if(subRoutine.action != null) subRoutine.action();
                }
                else
                {
                    // Run callback
                    if (subRoutine.callback != null) subRoutine.callback();

                    // Remove sub routine
                    subRoutines.RemoveAt(i);
                }
            }
        }

        // While | Runs action while condition is true
        public static void While(Func<bool> condition, Action? action, Action callback = null)
        {
            subRoutines.Add(new SubRoutine(condition, action, callback));
        }

        // WaitUntil | Runs action once condition is true
        public static void WaitUntil(Func<bool> condition, Action action)
        {
            While(() => {return !condition();}, null, action);
        }

        // Wait | Runs action after a certain amount of time
        public static void Wait(float seconds, Action action)
        {
            double endTime = Time() + seconds;
            While(() => { return Time() < endTime; }, null, action);
        }

        // Time
        public static float Time()
        {
            return (float)(DateTime.Now - startTime).TotalSeconds;
        }

        // Measuring Time Handling
        public static double SetMeasuredTime()
        {
            measuringTime = Time();
            return measuringTime;
        }

        public static double GetMeasuredTime()
        {
            return Time() - measuringTime;
        }
    }

    public class SubRoutine
    {
        public Func<bool> condition;
        public Action? action;
        public Action? callback;

        public SubRoutine(Func<bool> condition, Action? action, Action? callback = null)
        {
            this.condition = condition;
            this.action = action;
            this.callback = callback;
        }
    }
}