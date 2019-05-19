using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kompilator.CoombinedGrammarParser;

namespace Kompilator
{
    public class SpeakVisitor : CoombinedGrammarBaseVisitor<object>
    {
        //public List<SpeakLine> Lines = new List<SpeakLine>();

        //public override object VisitStat(StatContext context)
        //{
        //    NameContext name = context.GetText();
        //    OpinionContext opinion = context.opinion();

        //    SpeakLine line = new SpeakLine() { Person = name.GetText(), Text = opinion.GetText().Trim('"') };
        //    Lines.Add(line);

        //    return line;
        //}
    }

}
