using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KolonLibrary
{
    public class Runtime
    {
        #region Singleton
        private Runtime() { }

#pragma warning disable CS8618
        private static Runtime _instance;
#pragma warning restore CS8618

        public static Runtime GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Runtime();
            }
            return _instance;
        }
        #endregion Singleton

        public List<Statement> statements = new();
        public List<Variable> variables = new();

        public void Run()
        {
            foreach (var statement in statements)
            {
                if (statement is Variable)
                {
                    variables.Add((Variable) statement);
                }
                else if(statement is MethodCall)
                {
#pragma warning disable CS8600 
                    MethodCall _statement = statement as MethodCall;
#pragma warning restore CS8600 
#pragma warning disable CS8602
                    switch (_statement.StatementType)
#pragma warning restore CS8602 
                    {
                        case StatementType.CustomMethod:
                            break;
                        case StatementType.Print:
                            try
                            {
                                List<TokenMatch> matches;
                                Node node = new() { TokenMatch = new() { TokenType = TokenType.Empty, Value = string.Empty} };
                                Rules.SetupOrder(_statement.Arguments[0], out matches);
                                Rules.Expression(matches.GetRange(1, matches.Count - 1), node, out node);
                                Console.WriteLine(AST.ResolveExpressionValue(node.GetRootNode().Children[0]).Value);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            break;
                    }
                }
            }
            foreach (var variable in variables)
            {
                Console.WriteLine($"{variable.Name}, {variable.Value.Value}");
            }
        }
    }
}
