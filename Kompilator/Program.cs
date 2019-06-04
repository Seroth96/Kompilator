using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                string pattern = "#include\\s*(<[^>]+|\"[^\"]+\")";

                var path = @"C:\Users\USER\Desktop\studia\MAGISTERSKIE\jezyki\PROJEKT_KONCOWY\";

                // The RegexOptions are optional to this call, we will go into more detail about
                // them below.
                while ((input = Console.ReadLine()) != "\u0004")
                {
                    var comment = input.IndexOf(@"//");
                    Match result = Regex.Match(input, pattern);
                    if ( comment >= 0)
                    {
                        text.AppendLine(input.Substring(0, comment - 1));
                    }
                    else if (result.Success)
                    {
                        try
                        {
                            using (StreamReader sr = File.OpenText(path + result.Groups[1].Value.Trim('\"')))
                            {
                                string line = "";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    var commentF = line.IndexOf(@"//");
                                    Match resultF = Regex.Match(line, pattern);
                                    if (commentF >= 0)
                                    {
                                        text.AppendLine(line.Substring(0, commentF - 1));
                                    }
                                    else
                                    {
                                        text.AppendLine(line);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Nie można znaleźć pliku o nazwie {result.Groups["name"].Value}");
                            Console.ForegroundColor = ConsoleColor.Black;
                            return;
                        }                        
                    }
                    else
                    {
                        text.AppendLine(input);
                    }
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
                Console.ReadKey();
            }
        }
    }
}
