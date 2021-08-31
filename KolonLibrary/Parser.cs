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
                case TokenType.Int:
                    if (TokenMatches[1].TokenType == TokenType.Ident)
                    {
                        if (TokenMatches[2].TokenType == TokenType.Equals)
                        {
                            //start the AST with 3 nodes: Int > Ident > Equals
                            Node node = new Node() { TokenMatch = TokenMatches[2] };
                            node.Parent = new Node() { TokenMatch = TokenMatches[1] };
                            node.Parent.Children.Add(node);
                            node.Parent.Parent = new Node() { TokenMatch = TokenMatches[0] };
                            node.Parent.Parent.Children.Add(node.Parent);
                        Console.WriteLine(Expression(TokenMatches.GetRange(3, TokenMatches.Count - 3), node, out node)); //pass in the expression part of the variable, ignoring the type, ident and equals token
                            node.ListInfo();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// recursive function, checks the validity of an arithmetic expression
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        bool Expression(List<TokenMatch> matches, Node node, out Node outNode)
        {
            /*
            foreach (var match in matches)
            {
                Console.Write($"{match.TokenType}, ");
            }
            Console.WriteLine("\nExpresion method called\n");
            */
            Console.WriteLine("Expression method called");
            //node.ListInfo();
            outNode = node;
            if (matches.Count > 0)
            {
                //switch case for the first token, to check whether the function should validate a binary expression, or a grouping expression, etc..
                switch (matches[0].TokenType)
                {
                    case TokenType.IntValue:
                        try
                        {
                            if (matches[1].GroupingType == TokenType.Operator)
                            {
                                Node BinaryExpressionNode = new Node() { TokenMatch = matches[1], Parent = node };
                                BinaryExpressionNode.Children.Add(new Node() { TokenMatch = matches[0], Parent = BinaryExpressionNode });
                                node.Children.Add(BinaryExpressionNode);
                                //check if the expression matches: number - operator - expression
                                if (Expression(matches.GetRange(2, matches.Count - 2), BinaryExpressionNode, out node))
                                {
                                    return true;
                                }
                                else return false;
                            }
                            else return false;
                        }
                        catch (ArgumentOutOfRangeException) 
                        {
                            Node LiteralExpressionNode = new Node() { TokenMatch = matches[0], Parent = node };
                            node.Children.Add(LiteralExpressionNode);
                            return true; 
                        }
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
                        try
                        {
                            if (matches[ParenDepth + 1].GroupingType == TokenType.Operator)
                            {
                                Node GroupingExpressionNode = new Node() { TokenMatch = matches[ParenDepth + 1], Parent = node };
                                node.Children.Add(GroupingExpressionNode);
                                if (Expression(matches.GetRange(1, ParenDepth - 1), GroupingExpressionNode, out node))
                                {    
                                    if (Expression(matches.GetRange(ParenDepth + 2, matches.Count - ParenDepth - 2), node, out node))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            if (Expression(matches.GetRange(1, ParenDepth - 1), node, out _))
                            {
                                return true;
                            }
                        }
                        break;
                    case TokenType.Sub:
                        if(Expression(matches.GetRange(1, matches.Count - 1), node, out _))
                            return true;
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

        public void ListInfo()
        {
            if (Parent != null)
            {
                Parent.ListInfo();
            }
            else
            {
                if (TokenMatch != null)    
                    Console.WriteLine($"{TokenMatch.TokenType}, {TokenMatch.Value}");
                ListChildInfo();
            }
        }

        public void ListChildInfo()
        {
            foreach (var child in Children)
            {
                if (TokenMatch != null && child.TokenMatch != null)
                    Console.WriteLine($"{child.TokenMatch.TokenType}, {child.TokenMatch.Value} parent: {TokenMatch.TokenType}, {TokenMatch.Value}");
                child.ListChildInfo();
            }
        }
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
                    //simplify all the operators to one operator group
                    if (match.TokenType == TokenType.Add || match.TokenType == TokenType.Sub || match.TokenType == TokenType.Mul || match.TokenType == TokenType.Div)
                        TokenMatchesOptimized[TokenMatchesOptimized.IndexOf(match)].GroupingType = TokenType.Operator;
                    //match.TokenType == TokenType.DoubleEqual || match.TokenType == TokenType.NotEqual || match.TokenType == TokenType.LesserThan || match.TokenType == TokenType.GreaterThan
                }
                Rules rules = new Rules(TokenMatchesOptimized);
                rules.VerifyMatches();
            }
        }
    }
}
