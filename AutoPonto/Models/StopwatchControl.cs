using System;
using System.Timers;

namespace AutoPonto.Models
{
    /// <summary>
    /// Classe que pausa o cronômetro ao ser instanciada, liberando o cronômetro no disposing
    /// </summary>
    public class StopwatchControl : IDisposable
    {
        private readonly Timer stopwatch;
        
        public StopwatchControl(ref Timer stopwatch)
        {
            this.stopwatch = stopwatch;

            this.stopwatch.Stop();
        }

        public void Dispose()
        {
            this.stopwatch.Start();
        }
    }
}
