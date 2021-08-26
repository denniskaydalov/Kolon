using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KolonLibrary
{
    /// <summary>
    /// enum of all possible tokens
    /// </summary>
    public enum TokenType
    {
        Int,
        Bool,
        Ident,
        Print,
        DoubleEqual,
        IntValue,
        BoolValue,
        Equals,
        Add,
        Sub,
        Div,
        Mul,
        GreaterThan,
        LesserThan,
        OpeningParen,
        ClosingParen,
        NotEqual,
        NewLine
    }

    public class Lexer
    {
        //string for the lexer to tokenize
        private string[] InputString;
        //list of all token definitions, containing the TokenType and the regex pattern for the token type
        public List<TokenDef> TokenDefinitions = new List<TokenDef>();

        public Lexer(string[] InputString)
        {
            this.InputString = InputString;

            //add token definitions
            TokenDefinitions.Add(new TokenDef(TokenType.Int, "^(int) "));
            TokenDefinitions.Add(new TokenDef(TokenType.Bool, "^(bool) "));
            TokenDefinitions.Add(new TokenDef(TokenType.Print, "^(print) "));
            TokenDefinitions.Add(new TokenDef(TokenType.DoubleEqual, "^(==)"));
            TokenDefinitions.Add(new TokenDef(TokenType.Ident, "^[a-zA-Z_][a-zA-Z0-9_]*"));
            TokenDefinitions.Add(new TokenDef(TokenType.IntValue, @"^\d+"));
            TokenDefinitions.Add(new TokenDef(TokenType.BoolValue, @"^(true)|^(false)"));
            TokenDefinitions.Add(new TokenDef(TokenType.Equals, "^="));
            TokenDefinitions.Add(new TokenDef(TokenType.Add, @"^\+"));
            TokenDefinitions.Add(new TokenDef(TokenType.Sub, "^-"));
            TokenDefinitions.Add(new TokenDef(TokenType.Div, @"^\/"));
            TokenDefinitions.Add(new TokenDef(TokenType.Mul, @"^\*"));
            TokenDefinitions.Add(new TokenDef(TokenType.GreaterThan, "^>"));
            TokenDefinitions.Add(new TokenDef(TokenType.LesserThan, "^<"));
            TokenDefinitions.Add(new TokenDef(TokenType.OpeningParen, @"^\("));
            TokenDefinitions.Add(new TokenDef(TokenType.ClosingParen, @"^\)"));
            TokenDefinitions.Add(new TokenDef(TokenType.NotEqual, @"^(!=)"));
        }

        /// <summary>
        /// tokenize input string
        /// </summary>
        public void Tokenize()
        {
            List<List<TokenMatch>> TokenMatchesList = new List<List<TokenMatch>>();

            foreach (var InputString in InputString)
            {
                //tokens that have been matched
                List<TokenMatch> TokenMatches = new List<TokenMatch>();

                for (int i = 0; i < InputString.Length; i++)
                {
                    if (char.IsWhiteSpace(InputString[i])) continue;
                    else
                    {
                        foreach (var token in TokenDefinitions)
                        {
                            var TokenMatch = token.Match(InputString[i..]);
                            if (TokenMatch.IsMatch)
                            {
                                TokenMatches.Add(TokenMatch);
                                i += TokenMatch.Value.Length - 1;
                                break;
                            }
                        }
                    }
                }
                TokenMatchesList.Add(TokenMatches);
            }
            
            Parser parser = new Parser(TokenMatchesList);
            parser.Parse();
        }
    }

    public class TokenDef
    {
        private TokenType TokenType;
        private Regex regex;

        public TokenDef(TokenType TokenType, string regex)
        {
            this.TokenType = TokenType;
            this.regex = new Regex(regex);
        }

        //Match function, if the match is a success, return a TokenMatch with the information
        public TokenMatch Match(string Token)
        {
            var match = regex.Match(Token);
            if (match.Success)
            {
                return new TokenMatch()
                {
                    TokenType = TokenType,
                    //conditional statement to only return a value if the Token has a value
                    Value = match.Value,
                    IsMatch = true
                };
            }
            return new TokenMatch() { IsMatch = false};
        }
    }

    public class TokenMatch
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsMatch { get; set; }
    }
}