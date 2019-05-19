using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kompilator.CoombinedGrammarParser;
using static Kompilator.CoombinedGrammarLexer;

namespace Kompilator
{
    public class LLVMActions: CoombinedGrammarBaseListener
    {
        Dictionary<String, String> memory = new Dictionary<String, String>();
        String value;

        public override void ExitAssign(CoombinedGrammarParser.AssignContext ctx)
        {
            String tmp = ctx.STRING().GetText();
            tmp = tmp.Substring(1, tmp.Length - 1);
            memory.Add(ctx.ID().GetText(), tmp);
        }

        public override void ExitProg(CoombinedGrammarParser.ProgContext ctx)
        {
            Console.WriteLine(LLVMGenerator.generate());
        }

        public override void ExitValue(CoombinedGrammarParser.ValueContext ctx)
        {
            if (ctx.ID() != null)
            {
                value = memory[ctx.ID().GetText()];
            }
            if (ctx.STRING() != null)
            {
                String tmp = ctx.STRING().GetText();
                value = tmp.Substring(1, tmp.Length - 1);
            }
        }

        public override void ExitPrint(CoombinedGrammarParser.PrintContext ctx)
        {
            LLVMGenerator.print(value);
        }
    }
}
