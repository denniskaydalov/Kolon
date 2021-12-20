using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<Method> blocks = new ();
        public List<Statement> statements = new();
        public List<VariableDeclare> variables = new();

        public void Run(List<Statement> Statements)
        {
            foreach (var statement in Statements)
            {
                if (statement is VariableDeclare)
                {
                    //resolve the value, then add the variable to the variable list
#pragma warning disable CS8600
                    VariableDeclare _variable = statement as VariableDeclare;
#pragma warning restore CS8600
#pragma warning disable CS8602
                    _variable.Value = AST.ResolveExpressionValue(_variable.ValueMatch, _variable.VariableType);
#pragma warning restore CS8602
                    variables.Add(_variable);
                }
                else if (statement is VariableChange)
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    VariableChange _variable = statement as VariableChange;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    List<string> RuntimeVariableNames = (from variables in variables select variables.Name).ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (RuntimeVariableNames.Contains(_variable.Name))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    {
                        try
                        {
                            TokenType valueType = variables[RuntimeVariableNames.IndexOf(_variable.Name)].Value._TokenType;
                            if(valueType == TokenType.IntValue) valueType = TokenType.Int;
                            else if(valueType == TokenType.BoolValue) valueType = TokenType.Bool;
                            variables[RuntimeVariableNames.IndexOf(_variable.Name)].Value.Value = AST.ResolveExpressionValue(_variable.ValueMatch, valueType).Value;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{_variable.Name} does not exist in the current context");
                        Environment.Exit(1);
                    }
                }
                else if(statement is MethodCall)
                {
#pragma warning disable CS8600 
                    MethodCall _method = statement as MethodCall;
#pragma warning restore CS8600 
#pragma warning disable CS8602
                    switch (_method.StatementType)
#pragma warning restore CS8602 
                    {
                        case StatementType.CustomMethod:
                            foreach(var block in blocks)
                            {
                                Method tempMethod = block;

                                if (tempMethod.name == _method.name)
                                {
                                    Run(tempMethod.MethodStatements);
                                }
                            }
                            break;
                        case StatementType.Print:
                            if (_method.Arguments.Count == 1)
                            {
                                try
                                {
                                    Console.WriteLine(AST.ResolveExpressionValue(_method.Arguments[0]).Value);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Print only accepts one argument");
                                Environment.Exit(1);
                            }
                            break;
                    }
                }
            }
        }
    }
}
