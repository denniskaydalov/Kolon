using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KolonLibrary
{
    public class Statement 
    {
        public void ResolveExpressionValue(Node node)
        {
            if(node.TokenMatch != null && (node.TokenMatch.GroupingType == TokenType.Operator || node.TokenMatch.TokenType == TokenType.IntValue))
            {
                Console.WriteLine(CalculateIntExpression(node));
            }
        }

        private int CalculateIntExpression(Node node)
        {
            if (node.TokenMatch != null && node.TokenMatch.GroupingType == TokenType.Operator)
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
            else if (node.TokenMatch != null && node.TokenMatch.TokenType == TokenType.IntValue)
            {
                return Convert.ToInt32(node.TokenMatch.Value);
            }
            return 0;
        }
    }

    public class MethodCall : Statement
    { 
        List<Node> Arguments = new();
        StatementType? Type;

        public MethodCall(List<Node> Arguments, StatementType Type)
        {
            this.Arguments = Arguments;
            this.Type = Type;
        }

        public void ResolveArgumentValues()
        {
            foreach (var Argument in Arguments)
            {
                ResolveExpressionValue(Argument.GetRootNode());
            }
        }
    }

    public enum StatementType
    {
        Print,
        CustomMethod,
    }
}
