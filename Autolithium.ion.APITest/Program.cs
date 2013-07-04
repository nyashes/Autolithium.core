using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Autolithium.ion.APITest
{
    class Program
    {
        static void Main(string[] args)
        {
            Mouse.X = 0;
            Mouse.SpeedX = 0.005f;
            Console.ReadKey();
        }
    }
}
