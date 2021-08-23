using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KolonLibrary
{
    #region bad
    /*
    public enum Token
    {
        VarName,
        DoubleColon,
        VarType,
        IntValue,
        Equals
    }

    public class TokenDefinition
    {
        Regex pattern;
        Token token;

        public TokenDefinition(Token token, string pattern)
        {
            this.pattern = new Regex(pattern);
            this.token = token;
        }
    }

    public class lexer
    {
        public string InputString { private get; set; } = string.Empty;
        Dictionary<Token, string> Tokens = new Dictionary<Token, string>();

        public lexer()
        {
            Tokens.Add(Token.DoubleColon, @"^(::)");
            Tokens.Add(Token.VarType, @"^(int)|(string)");
            Tokens.Add(Token.IntValue, @"^\d*");
            Tokens.Add(Token.Equals, @"^=");
            Tokens.Add(Token.VarName, @"^[a-zA-Z0-9_]*");
        }

        public void Tokenize()
        {
            while (!string.IsNullOrWhiteSpace(InputString))
            {
                InputString.Trim();
                TokenMatch match = FindTokens(InputString);
                if (match.IsMatch == false)
                {
                    Console.WriteLine("match failed");
                }
            }
        }

        TokenMatch FindTokens(string InputString)
        {
            foreach (KeyValuePair<Token, string> pair in Tokens)
            {
                var match = new Regex(pair.Value).Match(InputString);
                if (match.Success)
                {
                    string _remainingText = string.Empty;
                    if (match.Length != InputString.Length)
                        _remainingText = InputString.Substring(match.Length).Trim();
                    this.InputString = _remainingText.Trim();   

                    System.Console.WriteLine($"{pair.Key.ToString()}, {match.Value}");

                    return new TokenMatch()
                    {
                        IsMatch = true,
                        token = pair.Key,
                        remainingText = _remainingText,
                        value = match.Value
                    };
                }
            }

            return new TokenMatch() { IsMatch = false };
        }
    }

    public class TokenMatch
    {
        public Token token { get; set; }
        public string value { get; set; }
        public string remainingText { get; set; }
        public bool IsMatch { get; set; }
    }
    */
    #endregion bad

    public enum TokenType
    {
        Int,
        Ident,
        DoubleColon,
        IntValue,
        Equals
    }

    public class lex
    {
        private string InputString = string.Empty;
        public List<TokenDef> TokenDefinitions = new List<TokenDef>();
        public List<TokenMatch> TokenMatches = new List<TokenMatch>();

        public lex(string InputString)
        {
            this.InputString = InputString;

            TokenDefinitions.Add(new TokenDef(TokenType.Int, "int"));
            TokenDefinitions.Add(new TokenDef(TokenType.Ident, "[a-zA-Z_][a-zA-Z0-9_]*", true));
            TokenDefinitions.Add(new TokenDef(TokenType.DoubleColon, "(::)"));
            TokenDefinitions.Add(new TokenDef(TokenType.IntValue, @"\d+", true));
            TokenDefinitions.Add(new TokenDef(TokenType.Equals, "="));
        }

        public void Tokenize()
        {
            string Token = string.Empty;

            foreach(char c in InputString + ' ')
            {
                if(char.IsWhiteSpace(c))
                {
                    foreach (var token in TokenDefinitions)
                    {
                        var TokenMatch = token.Match(Token);
                        if (TokenMatch.IsMatch)
                        {
                            TokenMatches.Add(TokenMatch);
                            break;
                        }
                    }
                    Token = string.Empty;
                    continue;
                }
                Token += c;
            }
            
            foreach(var token in TokenMatches)
            {
                Console.WriteLine($"{token.TokenType}, {token.value}");
            }
        }
    }

    public class TokenDef
    {
        private TokenType TokenType;
        private Regex regex;
        private bool HasValue;

        public TokenDef(TokenType TokenType, string regex, bool HasValue = false)
        {
            this.TokenType = TokenType;
            this.regex = new Regex(regex);
            this.HasValue = HasValue;
        }

        public TokenMatch Match(string Token)
        {
            var match = regex.Match(Token);
            if (match.Success)
            {
                return new TokenMatch()
                {
                    TokenType = TokenType,
                    value = HasValue ? match.Value : string.Empty,
                    IsMatch = true
                };
            }
            return new TokenMatch() { IsMatch = false };
        }
    }

    public class TokenMatch
    {
        public TokenType TokenType { get; set; }
        public string value { get; set; } = string.Empty;
        public bool IsMatch { get; set; }
    }
}
