using System;
using KolonLibrary;

namespace KolonApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            lex lexer = new lex("int foo=(4 compare (2-1))");
            lexer.Tokenize();
            Console.ReadKey();
        }
    }
}
