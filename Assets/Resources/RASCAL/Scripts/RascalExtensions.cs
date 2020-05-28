using UnityEngine;
using System.Collections;

namespace RASCAL {
    public static class RascalExtensions {


        public static void Lap(this System.Diagnostics.Stopwatch sw, string msg = "") {
            sw.Stop();
            Debug.Log(msg + "-" + sw.ElapsedMilliseconds + ":" + sw.ElapsedTicks);
            sw.Reset();
            sw.Start();
        }

        public static double LapTime(this System.Diagnostics.Stopwatch sw) {
            sw.Stop();
            var time = sw.Elapsed.TotalMilliseconds;
            sw.Reset();
            sw.Start();

            return time;
        }

        public static void Restart(this System.Diagnostics.Stopwatch sw) {
            sw.Reset();
            sw.Start();
        }
    }
}
