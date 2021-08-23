using System;
using KolonLibrary;

namespace KolonApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            lex lexer = new lex("a :: int = 4 b :: int = 9");
            lexer.Tokenize();
            Console.ReadKey();
        }
    }
}
