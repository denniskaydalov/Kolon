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
                case TokenType.Int or TokenType.Bool:
                    VariableCheck();
                    break;
            }
        }

        void VariableCheck()
        {
            if (TokenMatches[1].TokenType == TokenType.Ident)
            {
                Console.WriteLine(TokenMatches.GetRange(3, TokenMatches.Count - 3).Select(p => p.TokenType).ToList().SequenceEqual(Expression(TokenMatches.GetRange(3, TokenMatches.Count - 3).Select(p => p.TokenType).ToList())));
            }
        }

        List<TokenType> Expression(List<TokenType> TokenMatches)
        {
            try
            {
                List<TokenType> GroupingList = new List<TokenType>() { TokenType.OpeningParen };
                int depth = 0;
                for (int i = 0; i < TokenMatches.Count; i++)
                {
                    TokenType match = TokenMatches[i];
                    if (match == TokenType.OpeningParen) depth++;
                    else if (match == TokenType.ClosingParen) depth--;
                    else if (match == TokenType.Operator && depth == 0)
                    {
                        depth = i;
                    }
                }
                GroupingList.AddRange(TokenMatches.GetRange(1, (TokenMatches.Count - 2) - depth));
                GroupingList.Add(TokenType.ClosingParen);

                List<TokenType> BinaryList = new List<TokenType>();
                try
                {
                    BinaryList.AddRange(TokenMatches.GetRange(0, TokenMatches.IndexOf(TokenType.Operator)));
                    BinaryList.Add(TokenType.Operator);
                    BinaryList.AddRange(TokenMatches.GetRange(TokenMatches.IndexOf(TokenType.Operator) + 1, TokenMatches.Count - TokenMatches.IndexOf(TokenType.Operator) - 1));
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

                if (TokenMatches.SequenceEqual(new List<TokenType>() { TokenType.IntValue }) || TokenMatches.SequenceEqual(new List<TokenType>() { TokenType.BoolValue })) return TokenMatches;
                if (TokenMatches.SequenceEqual(GroupingList)) return TokenMatches;
                if (TokenMatches.SequenceEqual(BinaryList)) return TokenMatches;

                return new List<TokenType>();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<TokenType>(); }
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
