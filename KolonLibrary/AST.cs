using System;
using System.Collections.Generic;
using System.Linq;

namespace KolonLibrary
{
    public class AST
    {
        public static TokenMatch ResolveExpressionValue(List<TokenMatch> matches, TokenType type = TokenType.Any)
        {
            TokenType matchesType = GetExpressionType(matches);
            if(matchesType != TokenType.Empty && (matchesType == type || type == TokenType.Any))
            {
                switch(matchesType)
                {
                    case TokenType.IntValue or TokenType.Int:
                        Rules.SetupOrder(matches, out matches);
                        Node ExpressionNode = new Node() {_TokenMatch = new TokenMatch() {_TokenType = TokenType.Empty}};
                        if(Rules.Expression(matches, ExpressionNode, out ExpressionNode))
                        {
                            return new TokenMatch() { Value = CalculateIntExpression(ExpressionNode.Children[0]).ToString(), IsMatch = true, _TokenType = TokenType.IntValue };
                        }
                        else return new TokenMatch();
                    case TokenType.BoolValue or TokenType.Bool:
                        Node BoolNode = new Node() {_TokenMatch = new TokenMatch() {_TokenType = TokenType.Empty}};
                        if(Rules.BoolExpression(matches, BoolNode, out BoolNode))
                        {
                            return new TokenMatch() { Value = CalculateBoolValue(BoolNode.Children[0]), IsMatch = true, _TokenType = TokenType.BoolValue };
                        }
                        else return new TokenMatch();
                }
            }
            else if(matchesType != TokenType.Empty)
            {
                Console.WriteLine("A type safety error has occurred");
                Environment.Exit(1);
            }
            return new TokenMatch();
        }

        private static TokenType GetExpressionType(List<TokenMatch> matches)
        {
            foreach(var match in matches)
            {
                if(match._TokenType == TokenType.IntValue)
                {
                    return TokenType.Int; 
                }
            else if(match._TokenType == TokenType.BoolValue)
            {
                return TokenType.Bool;
            }
            else if(match.GroupingType == TokenType.Operator || match._TokenType == TokenType.OpeningParen || match._TokenType == TokenType.ClosingParen)
            {
                continue;
            }
            else if(match._TokenType == TokenType.IdentRef)
            {
                        Runtime runtime = Runtime.GetInstance();
                        //get list of all variable names
                        List<string> RuntimeVariableNames = (from variables in runtime.variables select variables.Name).ToList();
                        //check if the variable name that the user entered exists in the current context, the range operator selects the name of the variable, ignore the $ sign
                        if (RuntimeVariableNames.Contains(match.Value[1..]))
                        {
                            try
                            {
                                //parse the value, if it's an int
                                return runtime.variables[RuntimeVariableNames.IndexOf(match.Value[1..])].VariableType;
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            Console.WriteLine($"{match.Value.Substring(1)} does not exist in the current context");
                            //exit the application with an error
                            Environment.Exit(1);
                        }
            }
            else
            {
                Console.WriteLine($"{match._TokenType} is an unkown type");
                Environment.Exit(1);
            }
	    }
	    return TokenType.Empty;
        }

        private static string CalculateBoolValue(Node node)
        {
            if(node._TokenMatch != null)
            {
                if(node._TokenMatch._TokenType == TokenType.BoolValue)
                {
                    return node._TokenMatch.Value;
                }
                else if(node._TokenMatch._TokenType == TokenType.IdentRef)
                {
                    Runtime runtime = Runtime.GetInstance();
                    //get list of all variable names
                    List<string> RuntimeVariableNames = (from variables in runtime.variables select variables.Name).ToList();
                    //check if the variable name that the user entered exists in the current context, the range operator selects the name of the variable, ignore the $ sign
                    if (RuntimeVariableNames.Contains(node._TokenMatch.Value[1..]))
                    {
                        try
                        {
                            return runtime.variables[RuntimeVariableNames.IndexOf(node._TokenMatch.Value[1..])].Value.Value;
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                    Console.WriteLine($"{node._TokenMatch.Value.Substring(1)} does not exist in the current context");
                    //exit the application with an error
                    Environment.Exit(1);
                    }
                }
            }
#pragma warning disable CS8602
            Console.WriteLine($"{node._TokenMatch._TokenType} is not regonized");
#pragma warning restore CS8602
            Environment.Exit(1);
            return string.Empty;
        }

        private static int CalculateIntExpression(Node node)
        {
            if (node._TokenMatch != null)
            {
                //Console.WriteLine(node.TokenMatch.TokenType);
                if (node._TokenMatch.GroupingType == TokenType.Operator)
                {
                    if (node.Children.Count == 2)
                    {
                        int leftNumber, rightNumber;
                        leftNumber = CalculateIntExpression(node.Children[0]);
                        rightNumber = CalculateIntExpression(node.Children[1]);

                        switch (node._TokenMatch._TokenType)
                        {
                            case TokenType.Add:
                                return leftNumber + rightNumber;
                            case TokenType.Sub:
                                return leftNumber - rightNumber;
                            case TokenType.Div:
                                return leftNumber / rightNumber;
                            case TokenType.Mul:
                                return leftNumber * rightNumber;
                            case TokenType.Mod:
                                return leftNumber % rightNumber;
                        }
                    }
                }
                else if (node._TokenMatch._TokenType == TokenType.IntValue)
                {
                    return Convert.ToInt32(node._TokenMatch.Value);
                }
                else if (node._TokenMatch._TokenType == TokenType.IdentRef)
                {
                    Runtime runtime = Runtime.GetInstance();
                    //get list of all variable names
                    List<string> RuntimeVariableNames = (from variables in runtime.variables select variables.Name).ToList();
                    //check if the variable name that the user entered exists in the current context, the range operator selects the name of the variable, ignore the $ sign
                    if (RuntimeVariableNames.Contains(node._TokenMatch.Value[1..]))
                    {
                        try
                        {
                            //parse the value, if it's an int
                            return int.Parse(runtime.variables[RuntimeVariableNames.IndexOf(node._TokenMatch.Value[1..])].Value.Value);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"value of {node._TokenMatch.Value[1..]} is not an int");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{node._TokenMatch.Value.Substring(1)} does not exist in the current context");
                        //exit the application with an error
                        Environment.Exit(1);
                    }
                }
            }
            return 0;
        }
    }

    public class Statement { }
    
    public class Block { }

    public class Method : Block
    {
        public List<Statement> MethodStatements { get; set; } = new();
        public string name { get; set; } = String.Empty;
    }

    public class MethodCreate : Statement 
    {
        public string name { get; set; } = String.Empty;
    }
    
    public class VariableDeclare : Statement
    {
        public TokenType VariableType { get; set; }
        public StatementType StatementType { get; set; }
        public string Name { get; set; } = string.Empty;
        public TokenMatch Value { get; set; } = new();
        //node of the value so the value can be resolved at runtime
        public List<TokenMatch> ValueMatch { get; set; } = new();
    }

    public class MethodCall : Statement
    {
        //list of arguments so they can be resolved at runtime
        public List<List<TokenMatch>> Arguments { get; set; } = new();
        public StatementType StatementType { get; set; }
        public string name { get; set; } = String.Empty;
    }

    public class VariableChange : Statement
    {
        public string Name { get; set; } = string.Empty;
        public List<TokenMatch> ValueMatch { get; set; } = new();
    }

    public enum StatementType
    {
        Variable,
        Print,
        CustomMethod,
    }
}
