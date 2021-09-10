using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KolonLibrary
{
    public class AST
    {
        public static TokenMatch ResolveExpressionValue(Node node)
        {
            if (node.TokenMatch != null && (node.TokenMatch.GroupingType == TokenType.Operator || node.TokenMatch.TokenType == TokenType.IntValue))
            {
                try
                {
                    return new TokenMatch() { Value = CalculateIntExpression(node).ToString(), IsMatch = true, TokenType = TokenType.IntValue };
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            return new TokenMatch();
        }

        private static int CalculateIntExpression(Node node)
        {
            if (node.TokenMatch != null)
            {
                //Console.WriteLine(node.TokenMatch.TokenType);
                if (node.TokenMatch.GroupingType == TokenType.Operator)
                {
                    if (node.Children.Count == 2)
                    {
                        int leftNumber, rightNumber;
                        leftNumber = CalculateIntExpression(node.Children[0]);
                        rightNumber = CalculateIntExpression(node.Children[1]);

                        switch (node.TokenMatch.TokenType)
                        {
                            case TokenType.Add:
                                return leftNumber + rightNumber;
                            case TokenType.Sub:
                                return leftNumber - rightNumber;
                            case TokenType.Div:
                                return leftNumber / rightNumber;
                            case TokenType.Mul:
                                return leftNumber * rightNumber;
                        }
                    }
                }
                else if (node.TokenMatch.TokenType == TokenType.IntValue)
                {
                    return Convert.ToInt32(node.TokenMatch.Value);
                }
                else if (node.TokenMatch.TokenType == TokenType.IdentRef)
                {
                    Console.WriteLine("hi");
                    Runtime runtime = Runtime.GetInstance();
                    List<string> RuntimeVariableNames = (from variables in runtime.variables select variables.Name).ToList();
                    if(RuntimeVariableNames.Contains(node.TokenMatch.Value))
                    {
                        try
                        {
                            return int.Parse(runtime.variables[RuntimeVariableNames.IndexOf(node.TokenMatch.Value)].Value.Value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            return 0;
        }
    }

    public class Statement { }

    public class Variable : Statement
    {
        public TokenType VariableType { get; set; }
        public StatementType StatementType { get; set; }
        public string Name { get; set; } = string.Empty;
        public TokenMatch Value { get; set; } = new();
        public Node ValueNode { get; set; } = new();
    }

    public class MethodCall : Statement
    {
        public List<List<TokenMatch>> Arguments { get; set; } = new();
        public StatementType StatementType { get; set; }
    }


    public enum StatementType
    {
        Variable,
        Print,
        CustomMethod,
    }
}
