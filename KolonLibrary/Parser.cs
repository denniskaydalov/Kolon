using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KolonLibrary
{
    public class Rules
    {

    }

    class Node
    {
        public Node? Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public TokenMatch? TokenMatch { get; set; }
    }

    class Parser
    {
        private List<List<TokenMatch>> TokenMatches = new List<List<TokenMatch>>();

        public Parser(List<List<TokenMatch>> TokenMatches)
        {
            this.TokenMatches = TokenMatches;
        }

        public void Parse()
        {
            foreach (var list in TokenMatches)
            {
                //TODO - give list to rules to view if it fits in the rules
                foreach (var match in list)
                {
                    Console.WriteLine($"{match.TokenType}, {match.Value}");
                }
            }
        }
    }
}
