using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainTests.Shared
{
    public class Helper
    {
        public static async Task SpinDelay(Func<Task<bool>> funcEvaluation, int delayMilliseconds, int maxCount)
        {
            int cnt = 0;
            while(!await funcEvaluation() && cnt < maxCount) 
            {
                await Task.Delay(delayMilliseconds);
                cnt++;
            }
        }

        public static async Task SpinDelay(Func<bool> funcEvaluation, int delayMilliseconds, int maxCount)
        {
            int cnt = 0;
            while (!funcEvaluation() && cnt < maxCount)
            {
                await Task.Delay(delayMilliseconds);
                cnt++;
            }
        }
    }
}
