using BLL.Models;
using System;

namespace LieDetectorConsole
{
    internal class ProgressForConsole : IProgress<ProgressModel>
    {
        public void Report(ProgressModel value)
        {
            Console.WriteLine("value = "+value.Message);
        }
    }
}