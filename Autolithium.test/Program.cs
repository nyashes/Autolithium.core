using Autolithium.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autolithium.test
{
    class Program
    {
        static void Main(string[] args)
        {
            //string l = "ConsoleWriteError('test')";
            //string l2 = "$a = 1\nif $a = 1 then\n$a=2\nelseif $a=2 then\n$a=3\nelse\n$a=4\nendif";
            //var a = Autolithium.core.Lexer.CSLex(1, l);
            //var b = Autolithium.core.LiParser.Parse(l);
            //var b2 = Autolithium.core.LiParser.Parse(l2);
            //Console.ReadKey();
            dynamic a = 1;
            //dynamic b = 2;
            Console.WriteLine(a);
            a = "Hello world!!";
            Console.WriteLine(a);

        }
    }
}
