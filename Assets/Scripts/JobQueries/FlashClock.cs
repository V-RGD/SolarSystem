using System.Diagnostics;

namespace JobQueries
{
    public class FlashClock
    {
        readonly Stopwatch _watch = new Stopwatch();

        public void Start()
        {
            _watch.Reset();
            _watch.Start();
        }
        
        public float FlashReset()
        {
            _watch.Stop();
            float value = (float)_watch.ElapsedMilliseconds / 1000;
            
            _watch.Reset();
            _watch.Start();
            
            return value;
        }
    }
}
