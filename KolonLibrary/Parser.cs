using System;
using System.Collections.Generic;

namespace KolonLibrary
{
    public class Rules
    {
        private List<Statement> Statements;
        private List<Method> Blocks;
        private List<ListOfTokens> Matches;

        public Rules(ref List<ListOfTokens> Matches, List<Statement> Statements, List<Method> Blocks)
        {
            this.Matches = Matches;
            this.Statements = Statements;
            this.Blocks = Blocks;
        }

        #region verification
        public void VerifyMatches()
        {
            bool isBlock = false;
            Blocks.Add(new Method());
            for (int i = 0; i < Matches.Count; i++)
            {
                List<TokenMatch>? TokenMatches = Matches[i].matches;

                if (TokenMatches.Count == 0) continue;
                isBlock = Matches[i].isBlock;
                if(Matches[i].isBlock)
                    isBlock = true;
                else
                {
                    isBlock = false;
                    Blocks.Add(new Method());
                }
                List<TokenMatch> TokenMatchesOptimized = new List<TokenMatch>();
                TokenMatchesOptimized.AddRange(TokenMatches);

                foreach (var match in TokenMatches)
                {
                    //simplify all the operators to one operator group
                    if (match._TokenType == TokenType.Add || match._TokenType == TokenType.Sub || match._TokenType == TokenType.Mul || match._TokenType == TokenType.Div || match._TokenType == TokenType.Mod)
                        TokenMatchesOptimized[TokenMatchesOptimized.IndexOf(match)].GroupingType = TokenType.Operator;
                }

                TokenMatches = TokenMatchesOptimized;

                switch (TokenMatches[0]._TokenType)
                {
                    case TokenType.Int or TokenType.Bool:
                        if (TokenMatches[1]._TokenType == TokenType.Ident)
                        {
                            if (TokenMatches[2]._TokenType == TokenType.Equals)
                            {
                                string name = TokenMatches[1].Value;
                                if (TokenMatches[0]._TokenType == TokenType.Int)
                                {
                                    //add the variable to the list of statements
                                    if(!isBlock)
                                        Statements.Add(new VariableDeclare() { StatementType = StatementType.Variable, Value = new TokenMatch(), VariableType = TokenType.Int, Name = name, ValueMatch = TokenMatches.GetRange(3, TokenMatches.Count - 3)});
                                    else
                                    {
#pragma warning disable CS8600
                                        Method methodRef = Blocks[Blocks.Count - 1];
#pragma warning restore CS8600
#pragma warning disable CS8602
                                        methodRef.MethodStatements.Add(new VariableDeclare() { StatementType = StatementType.Variable, Value = new TokenMatch(), VariableType = TokenType.Int, Name = name, ValueMatch = TokenMatches.GetRange(3, TokenMatches.Count - 3) });
#pragma warning restore CS8602
                                    }
                                }
                                else if (TokenMatches[0]._TokenType == TokenType.Bool)
                                {
                                    if(!isBlock)
                                        Statements.Add(new VariableDeclare() { StatementType = StatementType.Variable, Value = new TokenMatch(), VariableType = TokenType.Bool, Name = name, ValueMatch = TokenMatches.GetRange(3, TokenMatches.Count - 3)});

                                    else
                                    {
#pragma warning disable CS8600
                                        Method methodRef = Blocks[Blocks.Count - 1];
#pragma warning restore CS8600
#pragma warning disable CS8602
                                        methodRef.MethodStatements.Add(new VariableDeclare() { StatementType = StatementType.Variable, Value = new TokenMatch(), VariableType = TokenType.Bool, Name = name, ValueMatch = TokenMatches.GetRange(3, TokenMatches.Count - 3)});
#pragma warning restore CS8602
                                    }
                                }
                            }
                        }
                        break;
                    case TokenType.IdentRef:
                        if (TokenMatches[1]._TokenType == TokenType.Equals || TokenMatches[1]._TokenType == TokenType.PlusEquals || TokenMatches[1]._TokenType == TokenType.MinusEquals || TokenMatches[1]._TokenType == TokenType.Decrement || TokenMatches[1]._TokenType == TokenType.Increment)
                        {
                            string name = TokenMatches[0].Value[1..];
                            if (!isBlock)
                            {
                                if (TokenMatches[1]._TokenType == TokenType.Increment)
                                    Statements.Add(new VariableChange() { Name = name, ValueMatch = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Add, Value = "+", GroupingType = TokenType.Operator }, new TokenMatch() { _TokenType = TokenType.IntValue, Value = "1" } } });
                                else if (TokenMatches[1]._TokenType == TokenType.Decrement)
                                    Statements.Add(new VariableChange() { Name = name, ValueMatch = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Sub, Value = "-", GroupingType = TokenType.Operator }, new TokenMatch() { _TokenType = TokenType.IntValue, Value = "1" } } });
                                else if (TokenMatches[1]._TokenType == TokenType.PlusEquals)
                                {
                                    List<TokenMatch> _match = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Add, Value = "-", GroupingType = TokenType.Operator } };
                                    _match.AddRange(TokenMatches.GetRange(2, TokenMatches.Count - 2));

                                    Statements.Add(new VariableChange() { Name = name, ValueMatch = _match} );
                                }
                                else if (TokenMatches[1]._TokenType == TokenType.MinusEquals)
                                {
                                    List<TokenMatch> _match = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Sub, Value = "-", GroupingType = TokenType.Operator } };
                                    _match.AddRange(TokenMatches.GetRange(2, TokenMatches.Count - 2));

                                    Statements.Add(new VariableChange() { Name = name, ValueMatch = _match} );
                                }
                                else Statements.Add(new VariableChange() { Name = name, ValueMatch = TokenMatches.GetRange(2, TokenMatches.Count - 2) });
                            }

                            else
                            {
#pragma warning disable CS8600
                                Method methodRef = Blocks[Blocks.Count - 1];
#pragma warning restore CS8600
#pragma warning disable CS8602
                                if (TokenMatches[1]._TokenType == TokenType.Increment)
                                    methodRef.MethodStatements.Add(new VariableChange() { Name = name, ValueMatch = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Add, Value = "+", GroupingType = TokenType.Operator }, new TokenMatch() { _TokenType = TokenType.IntValue, Value = "1" } } });
                                else if (TokenMatches[1]._TokenType == TokenType.Decrement)
                                    methodRef.MethodStatements.Add(new VariableChange() { Name = name, ValueMatch = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Sub, Value = "-", GroupingType = TokenType.Operator }, new TokenMatch() { _TokenType = TokenType.IntValue, Value = "1" } } });
                                else if (TokenMatches[1]._TokenType == TokenType.PlusEquals)
                                {
                                    List<TokenMatch> _match = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Add, Value = "-", GroupingType = TokenType.Operator } };
                                    _match.AddRange(TokenMatches.GetRange(2, TokenMatches.Count - 2));

                                    methodRef.MethodStatements.Add(new VariableChange() { Name = name, ValueMatch = _match });
                                }
                                else if (TokenMatches[1]._TokenType == TokenType.MinusEquals)
                                {
                                    List<TokenMatch> _match = new List<TokenMatch> { new TokenMatch() { _TokenType = TokenType.IdentRef, Value = $"${name}" }, new TokenMatch() { _TokenType = TokenType.Sub, Value = "-", GroupingType = TokenType.Operator } };
                                    _match.AddRange(TokenMatches.GetRange(2, TokenMatches.Count - 2));

                                    methodRef.MethodStatements.Add(new VariableChange() { Name = name, ValueMatch = _match });
                                }
                                else methodRef.MethodStatements.Add(new VariableChange() { Name = name, ValueMatch = TokenMatches.GetRange(2, TokenMatches.Count - 2) });
#pragma warning restore CS8602
                            }
                        }
                        break;
                    case TokenType.Ident:
                        MethodCall method = new();
                        method.name = TokenMatches[0].Value;
                        int argumentIndex = 0;
                        //gather all argument tokens into a list, seperating each argument 
                        foreach (var match in TokenMatches.GetRange(1, TokenMatches.Count - 1))
                        {
                            if (match._TokenType != TokenType.Comma)
                                try { method.Arguments[argumentIndex].Add(match); }
                                catch (Exception)
                                {
                                    method.Arguments.Add(new List<TokenMatch>());
                                    method.Arguments[argumentIndex].Add(match);
                                }
                            else argumentIndex++;
                        }
                        if (TokenMatches[0].Value == "print")
                            method.StatementType = StatementType.Print;
                        else method.StatementType = StatementType.CustomMethod;
                        if(!isBlock)
                            Statements.Add(method);
                        else
                        {
#pragma warning disable CS8600
                            Method methodRef = Blocks[Blocks.Count - 1];
#pragma warning restore CS8600
#pragma warning disable CS8602
                            methodRef.MethodStatements.Add(method);
#pragma warning restore CS8602
                        }
                        break;
                    case TokenType.Function:
                        if (TokenMatches[1]._TokenType == TokenType.Ident)
                        {
                            if(TokenMatches[2]._TokenType == TokenType.OpeningParen && TokenMatches[3]._TokenType == TokenType.ClosingParen)
                            {     
                                isBlock = true;
                                Blocks.Add(new Method(){ name = TokenMatches[1].Value});
                            }
                        }
                        break;

                }
            }
        }
        #endregion verification

        #region bedmas fix
        /// <summary>
        /// Add parentheses so that the expression works with the bedmas rules
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="outMatches"></param>
        public static void SetupOrder (List<TokenMatch> matches, out List<TokenMatch> outMatches)
        {
            outMatches = matches;
            //list of all indexes where the operators that need to be grouped are, to group the substraction and addition operators away from the multiplication and division operators
            List<int[]> BracketOperatorIndex = new();
            //variable to check if it's necessary to start a new group of operators, or just extend the current one
            bool locked = true;
            for (int i = 0; i < matches.Count; i++)
            {
                TokenMatch? match = matches[i];
                if(match._TokenType == TokenType.Div || match._TokenType == TokenType.Mul)
                {
                    //if locked, then add a new set of indexes to the list, containing the index of the current operator to group
                    if (locked)
                    {
                        BracketOperatorIndex.Add(new int[2] { i, i });
                        locked = false;
                    }
                    //if not locked, then change the second index of the group indexes to the latest operator found
                    else BracketOperatorIndex[BracketOperatorIndex.Count - 1][1] = i;
                }
                //if theres a operator that isn't sub or add, or a bracket, set locked to true so that the group is finished
                else if (match._TokenType == TokenType.OpeningParen || match._TokenType == TokenType.ClosingParen || match._TokenType == TokenType.Add || match._TokenType == TokenType.Sub)
                    locked = true;
            }
            //list of the indexes where the parentheses need to go to group the operators, and a bool to know if the the parenthese needs to be a opening parenthese, or a closing one
            List<(int, bool)> IndexInsertList = new();
            foreach (var index in BracketOperatorIndex)
            {
                //check if there is already brackets around the group
                if (index[0] > 1 && matches.Count - index[1] > 1 && matches[index[0] - 2]._TokenType == TokenType.OpeningParen && matches[index[1] + 2]._TokenType == TokenType.ClosingParen) continue;
                int[] IndexInsert = new int[2];
                //if the index before the first operator is a int value, then add the index to the IndexInsertList (gets added at the end)
                if(matches[index[0] - 1]._TokenType == TokenType.IntValue || matches[index[0] - 1]._TokenType == TokenType.IdentRef)
                {
                    IndexInsert[0] = index[0] - 1;
                }
                //if the index before the first operator is a closing bracket, then check where is the matching opening bracket, and add the index before that to the IndexInsertList (gets added at the end)
                else if (matches[index[0] - 1]._TokenType == TokenType.ClosingParen)
                {
                    int depth = 0;
                    for (int i = index[0]; i != 0; i--)
                    {
                        TokenMatch? match = matches[i];
                        if (match._TokenType == TokenType.OpeningParen)
                        {
                            depth--;
                            if (depth == 0)
                            {
                                IndexInsert[0] = i - 1;
                            }
                        }
                        else if (match._TokenType == TokenType.ClosingParen) depth++;
                    }
                }

                //same as the check before, but reversed for the second operator (it could be the same operator as the first operator, as not all expressions have more than one operator)
                if(matches[index[1] + 1]._TokenType == TokenType.IntValue || matches[index[1] + 1]._TokenType  == TokenType.IdentRef)
                {
                    IndexInsert[1] = index[1] + 2;
                }
                else if (matches[index[1] + 1]._TokenType == TokenType.OpeningParen)
                {
                    int depth = 0;
                    for (int i = index[1]; i < matches.Count; i++)
                    {
                        TokenMatch? match = matches[i];
                        if (match._TokenType == TokenType.ClosingParen)
                        {
                            depth++;
                            if (depth == 0)
                            {
                                IndexInsert[1] = i;
                                break;
                            }
                        }
                        else if (match._TokenType == TokenType.OpeningParen) depth--;
                    }
                }

                IndexInsertList.Add((IndexInsert[0], false));
                IndexInsertList.Add((IndexInsert[1], true));
            }

            //sort the list so that when adding the parentheses, it won't mess up the indexes of the other parentheses that need to be added
            IndexInsertList.Sort();

            //loop through the list backwards, to not interfere with the previous parentheses that need to be added
            for (int i = IndexInsertList.Count - 1; i >= 0; i--)
            {
                (int, bool) index = IndexInsertList[i];
                if (index.Item2)
                    matches.Insert(index.Item1, new TokenMatch() { _TokenType = TokenType.ClosingParen, Value = ")", IsMatch = true });
                else
                    matches.Insert(index.Item1, new TokenMatch() { _TokenType = TokenType.OpeningParen, Value = "(", IsMatch = true });
            }
        }
        #endregion bedmas fix

        #region expression check
        /// <summary>
        /// recursive function, checks the validity of an arithmetic expression
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool Expression(List<TokenMatch> matches, Node node, out Node outNode)
        {
            outNode = node;
            if (matches.Count > 0)
            {
                //switch case for the first token, to check whether the function should validate a binary expression, or a grouping expression, etc..
                switch (matches[0]._TokenType)
                {
                    case TokenType.IntValue or TokenType.IdentRef:
                        try
                        {
                            if (matches[1].GroupingType == TokenType.Operator)
                            {
                                Node BinaryExpressionNode = new Node() { _TokenMatch = matches[1], Parent = node };
                                BinaryExpressionNode.Children.Add(new Node() { _TokenMatch = matches[0], Parent = BinaryExpressionNode });
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
                            Node LiteralExpressionNode = new Node() { _TokenMatch = matches[0], Parent = node };
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
                            if (match._TokenType == TokenType.ClosingParen)
                            {
                                ParenDepth--;
                                if (ParenDepth == 0)
                                {
                                    //instead of creating a new variable to hold the index of the closing parenthese, use the, now useless, ParenDepth variable
                                    ParenDepth = i;
                                    break;
                                }
                            }
                            else if (match._TokenType == TokenType.OpeningParen) ParenDepth++;
                        }
                        //check if the expresion between the parentheses is valid
                        try
                        {
                            if (matches[ParenDepth + 1].GroupingType == TokenType.Operator)
                            {
                                Node GroupingExpressionNode = new Node() { _TokenMatch = matches[ParenDepth + 1], Parent = node };
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
                        Node NegativeExpression = new Node() { _TokenMatch = new TokenMatch() { _TokenType = TokenType.Mul, Value = "*", GroupingType = TokenType.Operator, IsMatch = true }, Parent = node};
                        NegativeExpression.Children.Add(new Node() { _TokenMatch = new TokenMatch() { _TokenType = TokenType.IntValue, Value = "-1", IsMatch = true }, Parent = NegativeExpression });
                        node.Children.Add(NegativeExpression);
                        if (Expression(matches.GetRange(1, matches.Count - 1), NegativeExpression, out node))
                        {
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }
        #endregion expression check


        public static bool BoolExpression(List<TokenMatch> matches, Node node, out Node outNode)
        {
            outNode = node;
            if(matches.Count > 0)
            {
                switch(matches[0]._TokenType)
                {
                    case TokenType.BoolValue or TokenType.IdentRef:
                        Node LiteralExpressionNode = new Node() { _TokenMatch = matches[0], Parent = node };
                        node.Children.Add(LiteralExpressionNode);
                        return true; 
                    default:
                        break;
                }
            }
            return false;
        }
    }

    public class Node
    {
        public Node? Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public TokenMatch? _TokenMatch { get; set; }

        public void ListInfo()
        {
            if (Parent != null)
            {
                Parent.ListInfo();
            }
            else
            {
                if (_TokenMatch != null)    
                    Console.WriteLine($"{_TokenMatch._TokenType}, {_TokenMatch.Value}");
                ListChildInfo();
            }
        }

        public void ListChildInfo()
        {
            foreach (var child in Children)
            {
                if (_TokenMatch != null && child._TokenMatch != null)
                    Console.WriteLine($"{child._TokenMatch._TokenType}, {child._TokenMatch.Value} parent: {_TokenMatch._TokenType}, {_TokenMatch.Value}");
                child.ListChildInfo();
            }
        }

        public Node GetRootNode()
        {
            if (Parent != null)
            {
                return Parent.GetRootNode();
            }
            else return this;
        }

	public TokenType GetValueType(Node node)
	{
	    if(node._TokenMatch != null)
	    {
	        switch (node._TokenMatch._TokenType )
	        {
		    case TokenType.Add or TokenType.Div or TokenType.Sub or TokenType.Mul:
			if(node.Children[0] != null)
			{
			    return(GetValueType(node.Children[0]));
			}
		        break;
		    case TokenType.IntValue or TokenType.BoolValue:
			return node._TokenMatch._TokenType;
	        }
	    }
	    return TokenType.Empty;
	}
    }

    public class Parser
    {

        //TODO: add support for boolean values
        //TODO: add support for floats
        //TODO: add support for comments
        //TODO: add support for strings
        //TODO: add support for options between print and printline
        //TODO: add support for debugging
        //TODO: add support for memory management 
        //TODO: check if a variable already exists when trying to declare it

        private List<ListOfTokens>  TokenMatches = new();
        private Runtime Runtime = Runtime.GetInstance();

        public Parser(List<ListOfTokens> TokenMatches)
        {
            this.TokenMatches = TokenMatches;
        }

        public void Parse()
        {
            Rules rules = new Rules(ref TokenMatches, Runtime.statements, Runtime.blocks);
            rules.VerifyMatches();
            Runtime.Run(Runtime.statements);
        }
    }
}
