using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime.Tree;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Kompilator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string input = "";
                StringBuilder text = new StringBuilder();
                Console.WriteLine("Input the program.");

                // to type the EOF character and end the input: use CTRL+D, then press <enter>
                while ((input = Console.ReadLine()) != "\u0004")
                {
                    text.AppendLine(input);
                }

                AntlrInputStream inputStream = new AntlrInputStream(text.ToString());
                CoombinedGrammarLexer grammarLexer = new CoombinedGrammarLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(grammarLexer);
                CoombinedGrammarParser grammarParser = new CoombinedGrammarParser(commonTokenStream);

                //CoombinedGrammarParser.ProgContext progContext = grammarParser.prog();

                IParseTree tree = grammarParser.prog();
                var walker = new ParseTreeWalker();
                walker.Walk(new LLVMActions(), tree);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
