﻿using System;
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
                    if(TokenMatches[1].TokenType == TokenType.Ident)
                    {
                        if(TokenMatches[2].TokenType == TokenType.Equals)
                        {
                            //start the AST with 3 nodes: Int > Ident > Equals
                            Node VariableDeclareNode = new Node() { TokenMatch = TokenMatches[2] }; 
                            VariableDeclareNode.Parent = new Node() { TokenMatch = TokenMatches[1] };
                            VariableDeclareNode.Parent.Children.Add(VariableDeclareNode);
                            VariableDeclareNode.Parent.Parent = new Node() { TokenMatch = TokenMatches[0] };
                            VariableDeclareNode.Parent.Parent.Children.Add(VariableDeclareNode.Parent);
                            if (TokenMatches[0].TokenType == TokenType.Int)
                            {
                                //sort the expression so it follows the rules of bedmas
                                SetupOrder(TokenMatches.GetRange(3, TokenMatches.Count - 3), out TokenMatches);
                                Console.WriteLine(Expression(TokenMatches, VariableDeclareNode, out VariableDeclareNode));
                            }
                            else
                            {
                                if (TokenMatches[3].TokenType == TokenType.BoolValue)
                                {
                                    Node BoolNode = new Node() { TokenMatch = TokenMatches[3], Parent = VariableDeclareNode };
                                    VariableDeclareNode.Children.Add(BoolNode);
                                    VariableDeclareNode = BoolNode;
                                    Console.WriteLine("True");
                                }
                            }
                            VariableDeclareNode.ListInfo();
                        }
                    }
                    break;
                case TokenType.IdentRef:
                    if(TokenMatches[1].TokenType == TokenType.Equals)
                    {
                        Node VariableSetNode = new Node() { TokenMatch = TokenMatches[1] };
                        VariableSetNode.Parent = new Node() { TokenMatch = TokenMatches[0] };
                        VariableSetNode.Parent.Children.Add(VariableSetNode);
                        SetupOrder(TokenMatches.GetRange(2, TokenMatches.Count - 2), out TokenMatches);
                        Console.WriteLine(Expression(TokenMatches, VariableSetNode, out VariableSetNode));
                        VariableSetNode.ListInfo();
                    }
                    break;
                case TokenType.Print:
                    Node PrintNode = new Node() { TokenMatch = TokenMatches[0] };
                    SetupOrder(TokenMatches.GetRange(1, TokenMatches.Count - 1), out TokenMatches);
                    Console.WriteLine(Expression(TokenMatches, PrintNode, out PrintNode));
                    PrintNode.ListInfo();
                    break;
            }
        }

        #region bedmas fix
        /// <summary>
        /// Add parentheses so that the expression works with the bedmas rules
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="outMatches"></param>
        void SetupOrder (List<TokenMatch> matches, out List<TokenMatch> outMatches)
        {
            outMatches = matches;
            //list of all indexes where the operators that need to be grouped are, to group the substraction and addition operators away from the multiplication and division operators
            List<int[]> BracketOperatorIndex = new();
            //check if it's necessary to start a new group of operators, or just extend the current one
            bool locked = true;
            for (int i = 0; i < matches.Count; i++)
            {
                TokenMatch? match = matches[i];
                if(match.TokenType == TokenType.Add || match.TokenType == TokenType.Sub)
                {
                    //if locked, then add a new set of indexes to the list, containing the index of the current operator to group
                    if (locked)
                    {
                        BracketOperatorIndex.Add(new int[2] { i, i });
                        locked = false;
                    }
                    //if not locked, then change the second index of the group indexes to the latest operator found
                    else BracketOperatorIndex[BracketOperatorIndex.Count - 1][1] = i;
                    continue;
                }
                //if theres a operator that isn't sub or add, or a bracket, set locked to true so that the group is finished
                else if (match.TokenType == TokenType.OpeningParen || match.TokenType == TokenType.ClosingParen || match.TokenType == TokenType.Mul || match.TokenType == TokenType.Div)
                    locked = true;
            }

            //list of the indexes where the parentheses need to go to group the operators, and a bool to know if the the parenthese needs to be a opening parenthese, or a closing one
            List<(int, bool)> IndexInsertList = new();
            foreach (var index in BracketOperatorIndex)
            {
                //check if there is already brackets around the group
                if (index[0] > 1 && matches.Count - index[1] > 1 && matches[index[0] - 2].TokenType == TokenType.OpeningParen && matches[index[1] + 2].TokenType == TokenType.ClosingParen) continue;
                int[] IndexInsert = new int[2];
                //if the index before the first operator is a int value, then add the space to the IndexInsertList (gets added at the end)
                if(matches[index[0] - 1].TokenType == TokenType.IntValue || matches[index[0] - 1].TokenType == TokenType.IdentRef)
                {
                    IndexInsert[0] = index[0] - 1;
                }
                //if the index before the first operator is a closing bracket, then check where is the matching opening bracket, and add the index before that to the IndexInsertList (gets added at the end)
                else if (matches[index[0] - 1].TokenType == TokenType.ClosingParen)
                {
                    int depth = 0;
                    for (int i = index[0]; i != 0; i--)
                    {
                        TokenMatch? match = matches[i];
                        if (match.TokenType == TokenType.OpeningParen)
                        {
                            depth--;
                            if (depth == 0)
                            {
                                IndexInsert[0] = i - 1;
                            }
                        }
                        else if (match.TokenType == TokenType.ClosingParen) depth++;
                    }
                }

                //same as the check before, but reversed for the second operator (it could be the same operator as the first operator, as not all expressions have more than one operator)
                if(matches[index[1] + 1].TokenType == TokenType.IntValue || matches[index[1] + 1].TokenType  == TokenType.IdentRef)
                {
                    IndexInsert[1] = index[1] + 2;
                }
                else if (matches[index[1] + 1].TokenType == TokenType.OpeningParen)
                {
                    int depth = 0;
                    for (int i = index[1]; i < matches.Count; i++)
                    {
                        TokenMatch? match = matches[i];
                        if (match.TokenType == TokenType.ClosingParen)
                        {
                            depth++;
                            if (depth == 0)
                            {
                                IndexInsert[1] = i;
                            }
                        }
                        else if (match.TokenType == TokenType.OpeningParen) depth--;
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
                    matches.Insert(index.Item1, new TokenMatch() { TokenType = TokenType.ClosingParen, Value = ")", IsMatch = true });
                else
                    matches.Insert(index.Item1, new TokenMatch() { TokenType = TokenType.OpeningParen, Value = "(", IsMatch = true });
            }
        }
        #endregion bedmas fix

        #region expression check
        /// <summary>
        /// recursive function, checks the validity of an arithmetic expression
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        bool Expression(List<TokenMatch> matches, Node node, out Node outNode)
        {
            /*
            foreach (var match in matches)
            {
                Console.Write($"{match.TokenType}, ");
            }
            Console.WriteLine("\nExpresion method called\n");
            */
            outNode = node;
            if (matches.Count > 0)
            {
                //switch case for the first token, to check whether the function should validate a binary expression, or a grouping expression, etc..
                switch (matches[0].TokenType)
                {
                    case TokenType.IntValue or TokenType.IdentRef:
                        try
                        {
                            if (matches[1].GroupingType == TokenType.Operator)
                            {
                                Node BinaryExpressionNode = new Node() { TokenMatch = matches[1], Parent = node };
                                BinaryExpressionNode.Children.Add(new Node() { TokenMatch = matches[0], Parent = BinaryExpressionNode });
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
                            Node LiteralExpressionNode = new Node() { TokenMatch = matches[0], Parent = node };
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
                            if (match.TokenType == TokenType.ClosingParen)
                            {
                                ParenDepth--;
                                if (ParenDepth == 0)
                                {
                                    //instead of creating a new variable to hold the index of the closing parenthese, use the, now useless, ParenDepth variable
                                    ParenDepth = i;
                                    break;
                                }
                            }
                            else if (match.TokenType == TokenType.OpeningParen) ParenDepth++;
                        }
                        //check if the expresion between the parentheses is valid
                        try
                        {
                            if (matches[ParenDepth + 1].GroupingType == TokenType.Operator)
                            {
                                Node GroupingExpressionNode = new Node() { TokenMatch = matches[ParenDepth + 1], Parent = node };
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
                        if(Expression(matches.GetRange(1, matches.Count - 1), node, out _))
                            return true;
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }
        #endregion expression check

    }

    public class Node
    {
        public Node? Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public TokenMatch? TokenMatch { get; set; }

        public void ListInfo()
        {
            if (Parent != null)
            {
                Parent.ListInfo();
            }
            else
            {
                if (TokenMatch != null)    
                    Console.WriteLine($"{TokenMatch.TokenType}, {TokenMatch.Value}");
                ListChildInfo();
            }
        }

        public void ListChildInfo()
        {
            foreach (var child in Children)
            {
                if (TokenMatch != null && child.TokenMatch != null)
                    Console.WriteLine($"{child.TokenMatch.TokenType}, {child.TokenMatch.Value} parent: {TokenMatch.TokenType}, {TokenMatch.Value}");
                child.ListChildInfo();
            }
        }
    }

    public class Parser
    {
        //TODO : run a check somewhere to make sure that a line is not just an empty line, and actually contains tokens
        private List<List<TokenMatch>> TokenMatches = new List<List<TokenMatch>>();

        public Parser(List<List<TokenMatch>> TokenMatches)
        {
            this.TokenMatches = TokenMatches;
        }

        public void Parse()
        {
            foreach (var list in TokenMatches)
            {
                if (list.Count == 0) continue;
                List<TokenMatch> TokenMatchesOptimized = new List<TokenMatch>();
                TokenMatchesOptimized.AddRange(list);
                foreach (var match in list)
                {
                    //simplify all the operators to one operator group
                    if (match.TokenType == TokenType.Add || match.TokenType == TokenType.Sub || match.TokenType == TokenType.Mul || match.TokenType == TokenType.Div)
                        TokenMatchesOptimized[TokenMatchesOptimized.IndexOf(match)].GroupingType = TokenType.Operator;
                }
                Rules rules = new Rules(TokenMatchesOptimized);
                rules.VerifyMatches();
            }
        }
    }
}