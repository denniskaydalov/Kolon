using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KolonLibrary
{
    public class Rules
    {
        private List<TokenMatch> TokenMatches;

        public Rules(List<TokenMatch> TokenMatches)
        {
            this.TokenMatches = TokenMatches;
        }

        public void VerifyMatches()
        {
            switch(TokenMatches[0].TokenType)
            {
                case TokenType.Int or TokenType.Bool :
                    if(TokenMatches[1].TokenType == TokenType.Ident)
                        if(TokenMatches[2].TokenType == TokenType.Equals)
                            Console.WriteLine(Expression(TokenMatches.GetRange(3, TokenMatches.Count - 3))); //pass in the expression part of the variable, ignoring the type, ident and equals token
                    break;
            }
        }

        /// <summary>
        /// recursive function, checks the validity of an arithmetic expression
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        bool Expression(List<TokenMatch> matches)
        {
            /*
            foreach (var match in matches)
            {
                Console.Write($"{match.TokenType}, ");
            }
            Console.WriteLine("\nExpresion method called\n");
            */
            if (matches.Count > 0)
            {
                //switch case for the first token, to check whether the function should validate a binary expression, or a grouping expression, etc..
                switch (matches[0].TokenType)
                {
                    case TokenType.IntValue:
                        try
                        {
                            //check if the expression matches: number - boolean - expression
                            if (matches[1].TokenType == TokenType.Operator && Expression(matches.GetRange(2, matches.Count - 2)))
                                return true; 
                            else return false;
                        }
                        catch (ArgumentOutOfRangeException) { return true; }
                        catch (Exception e) { Console.WriteLine(e); return false; }
                    case TokenType.OpeningParen:
                        //find the matching closing parenthese to the current opening parenthese
                        int ParenDepth = 0;
                        for (int i = 0; i < matches.Count; i++)
                        {
                            TokenMatch? match = matches[i];
                            if (match.TokenType == TokenType.ClosingParen)
                            {
                                ParenDepth--;
                                if (ParenDepth == 0)
                                {
                                    //instead of creating a new variable to hold the index of the closing parenthese, use the, now useless, ParenDepth variable
                                    ParenDepth = i;
                                    break;
                                }
                            }
                            else if (match.TokenType == TokenType.OpeningParen) ParenDepth++;
                        }
                        //check if the expresion between the parentheses is valid
                        if (Expression(matches.GetRange(1, ParenDepth - 1)))
                        {
                            //check for any arithmetic expressions after the closing parenthese that was just checked, and check if the next expression is valid
                            if (ParenDepth != matches.Count - 1)
                            {
                                if (matches[ParenDepth + 1].TokenType == TokenType.Operator)
                                {
                                    if (Expression(matches.GetRange(ParenDepth + 2, matches.Count - ParenDepth - 2)))
                                        return true;
                                }
                            }
                            else return true;
                        }
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }
    }

    public class Node
    {
        public Node? Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public TokenMatch? TokenMatch { get; set; }
    }

    public class Parser
    {
        //TODO : run a check somewhere to make sure that a line is not just an empty line, and actually contains tokens
        private List<List<TokenMatch>> TokenMatches = new List<List<TokenMatch>>();

        public Parser(List<List<TokenMatch>> TokenMatches)
        {
            this.TokenMatches = TokenMatches;
        }

        public void Parse()
        {
            foreach (var list in TokenMatches)
            {
                List<TokenMatch> TokenMatchesOptimized = new List<TokenMatch>();
                TokenMatchesOptimized.AddRange(list);
                foreach (var match in list)
                {
                    if (match.TokenType == TokenType.DoubleEqual || match.TokenType == TokenType.NotEqual || match.TokenType == TokenType.LesserThan || match.TokenType == TokenType.GreaterThan || match.TokenType == TokenType.Add || match.TokenType == TokenType.Sub || match.TokenType == TokenType.Mul || match.TokenType == TokenType.Div)
                        TokenMatchesOptimized[TokenMatchesOptimized.IndexOf(match)] = new TokenMatch() { TokenType = TokenType.Operator, Value = match.Value };
                }
                Rules rules = new Rules(TokenMatchesOptimized);
                rules.VerifyMatches();
            }
        }
    }
}
