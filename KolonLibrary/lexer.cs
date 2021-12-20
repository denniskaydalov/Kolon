using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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
        IdentRef,
        DoubleEqual,
        IntValue,
        BoolValue,
        PlusEquals,
        Increment,
        MinusEquals,
        Decrement,
        Equals,
        Add,
        Sub,
        Div,
        Mul,
        Mod,
        OpeningParen,
        ClosingParen,
        Operator,
        Comma,
        OpeningBrace,  
        ClosingBrace,  
        Empty, //for empty token match instances that need a tokentype
        Custom, //for custom data types
        Any,
        Function,
    }

    public class Lexer
    {
        //string for the lexer to tokenize
        private string[] InputStrings;
        //list of all token definitions, containing the TokenType and the regex pattern for the token type
        public List<TokenDef> TokenDefinitions = new List<TokenDef>();

        public Lexer(string[] InputStrings)
        {
            this.InputStrings = InputStrings;

            //add token definitions
            TokenDefinitions.Add(new TokenDef(TokenType.Int, "^(int) "));
            TokenDefinitions.Add(new TokenDef(TokenType.Bool, "^(bool) "));
            TokenDefinitions.Add(new TokenDef(TokenType.Function, "^(function) "));
            TokenDefinitions.Add(new TokenDef(TokenType.Comma, "^,"));
            TokenDefinitions.Add(new TokenDef(TokenType.DoubleEqual, "^(==)"));
            TokenDefinitions.Add(new TokenDef(TokenType.BoolValue, @"^(true)|^(false)|^(True)|^(False)"));
            TokenDefinitions.Add(new TokenDef(TokenType.Ident, "^[a-zA-Z_][a-zA-Z0-9_]*"));
            TokenDefinitions.Add(new TokenDef(TokenType.IdentRef, @"^\$[a-zA-Z_][a-zA-Z0-9_]*"));
            TokenDefinitions.Add(new TokenDef(TokenType.IntValue, @"^-?\d+"));
            TokenDefinitions.Add(new TokenDef(TokenType.PlusEquals, @"^(\+=)"));
            TokenDefinitions.Add(new TokenDef(TokenType.Increment, @"^(\+\+)"));
            TokenDefinitions.Add(new TokenDef(TokenType.MinusEquals, @"^(-=)"));
            TokenDefinitions.Add(new TokenDef(TokenType.Decrement, @"^(--)"));
            TokenDefinitions.Add(new TokenDef(TokenType.Equals, "^="));
            TokenDefinitions.Add(new TokenDef(TokenType.Add, @"^\+"));
            TokenDefinitions.Add(new TokenDef(TokenType.Sub, "^-"));
            TokenDefinitions.Add(new TokenDef(TokenType.Div, @"^\/"));
            TokenDefinitions.Add(new TokenDef(TokenType.Mul, @"^\*"));
            TokenDefinitions.Add(new TokenDef(TokenType.Mod, @"^\%"));
            TokenDefinitions.Add(new TokenDef(TokenType.OpeningParen, @"^\("));
            TokenDefinitions.Add(new TokenDef(TokenType.ClosingParen, @"^\)"));
            TokenDefinitions.Add(new TokenDef(TokenType.OpeningBrace, @"^\{"));
            TokenDefinitions.Add(new TokenDef(TokenType.ClosingBrace, @"^\}"));
        }

        /// <summary>
        /// tokenize input string
        /// </summary>
        public void Tokenize()
        {
            //token matches, nested list so that we automatically know each line break
            List<ListOfTokens> TokenMatchesList = new();

            bool isFunction = false;

            foreach (var InputString in InputStrings)
            {
                //tokens that have been matched
                ListOfTokens TokenMatches = new();

                //loop through the input string trying to find a regex match, when found, add the match to the TokenMatchesList
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
                                if(TokenMatch._TokenType == TokenType.OpeningBrace)
                                {
                                    isFunction = true;
                                }
                                else if(TokenMatch._TokenType == TokenType.ClosingBrace)
                                {
                                    TokenMatches.isBlock = true;
                                    isFunction = false;
                                }
                                else if(isFunction == true)
                                    TokenMatches.isBlock = true;
                                TokenMatches.matches.Add(TokenMatch);
                                //add the match's length to it so we don't have to loop through extra characters that we've already found a match for
                                i += TokenMatch.Value.Length - 1;
                                break;
                            }
                        }
                    }
                }

                TokenMatchesList.Add(TokenMatches);

                for(int i = 0; i < TokenMatches.matches.Count; i++)
                {
                    var match = TokenMatches.matches[i];
                }
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
                    _TokenType = TokenType,
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
        public TokenType _TokenType { get; set; }
        public string Value { get; set; } = string.Empty;
        public bool IsMatch { get; set; }
        public TokenType? GroupingType { get; set; }
    }

    public class ListOfTokens
    {
        public List<TokenMatch> matches { get; set; }= new();
        public bool isBlock { get; set; } = false;
        public StatementType type { get; set; }
    }
}
