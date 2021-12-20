using System;
using System.IO;
using KolonLibrary;

namespace KolonApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var lines = File.ReadAllLines(args[1]);

                Lexer lexer = new(lines);
                lexer.Tokenize();
            }
            else Console.WriteLine("you must pass a text file with at least one line as an argument"); 
        }
    }
}
