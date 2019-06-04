using System;
using System.Linq;
using Antlr4.Runtime;
using Kompilator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class ParserTest
    {
        private CoombinedGrammarParser Setup(string text)
        {
            AntlrInputStream inputStream = new AntlrInputStream(text);
            CoombinedGrammarLexer speakLexer = new CoombinedGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            CoombinedGrammarParser speakParser = new CoombinedGrammarParser(commonTokenStream);

            return speakParser;
        }

        [TestMethod]
        public void TestPrint()
        {
            CoombinedGrammarParser parser = Setup("print \"ala ma kota\"");

            CoombinedGrammarParser.StatContext context = parser.stat();
            //SpeakVisitor visitor = new SpeakVisitor();
            //visitor.Visit(context);

            Assert.AreEqual("ala ma kota", context.stop.Text);
        }

        //[TestMethod]
        //public void TestLine()
        //{
        //    CoombinedGrammarParser parser = Setup("john says \"hello\" \n");

        //    CoombinedGrammarParser.LineContext context = parser.line();
        //    SpeakVisitor visitor = new SpeakVisitor();
        //    SpeakLine line = (SpeakLine)visitor.VisitLine(context);

        //    Assert.AreEqual("john", line.Person);
        //    Assert.AreEqual("hello", line.Text);
        //}

        //[TestMethod]
        //public void TestWrongLine()
        //{
        //    CoombinedGrammarParser parser = Setup("john sayan \"hello\" \n");

        //    var context = parser.line();

        //    Assert.IsInstanceOfType(context, typeof(CoombinedGrammarParser.LineContext));
        //    Assert.AreEqual("john", context.name().GetText());
        //    Assert.IsNull(context.SAYS());
        //    Assert.AreEqual("johnsayan\"hello\"\n", context.GetText());
        //}
    }
}
