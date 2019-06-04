using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kompilator.CoombinedGrammarParser;
using static Kompilator.CoombinedGrammarLexer;
using System.IO;
using System.Diagnostics;

namespace Kompilator
{
    enum VarType { INT, REAL, VECTOR, STRING, UNKNOWN }

    class Value
    {
        public String name;
        public VarType type;
        public Value(String name, VarType type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class LLVMActions: CoombinedGrammarBaseListener
    {

        Dictionary<String, List<String>> ArrayMemory = new Dictionary<String, List<String>>();
        Dictionary<String, String> memory = new Dictionary<String, String>();
        Dictionary<String, VarType> variables = new Dictionary<String, VarType>();
        Stack<Value> stack = new Stack<Value>();
        String value;

        public override void ExitAssign(CoombinedGrammarParser.AssignContext ctx)
        {
            String ID = ctx.ID().GetText();
            Value v = stack.Pop();
            if (v.type == VarType.INT)
            {
                variables.Add(ID, v.type);
                LLVMGenerator.declare_i32(ID);
                LLVMGenerator.assign_i32(ID, v.name);
            }
            if (v.type == VarType.REAL)
            {
                variables.Add(ID, v.type);
                LLVMGenerator.declare_double(ID);
                LLVMGenerator.assign_double(ID, v.name);
            }
            if (v.type == VarType.STRING)
            {
                if (!memory.Any(m => m.Key == ID))
                {
                    memory.Add(ID, value);
                }
            }
            if (v.type == VarType.VECTOR)
            {
                if (!memory.Any(m => m.Key == ID))
                {
                    memory.Add(ID, value);
                }

                List<String> values = new List<string>();
                var tmp = v.name.Substring(1, v.name.Length - 2);
                int i = 0;
                foreach(var element in tmp.Split(','))
                {
                    variables.Add(ID + "_" + i + "_", VarType.INT);
                    values.Add(Int32.Parse(element).ToString());
                    LLVMGenerator.declare_i32(ID + "_" + i + "_");
                    LLVMGenerator.assign_i32(ID + "_" + i + "_", Int32.Parse(element).ToString());
                    i++;
                }
                ArrayMemory.Add(ID, values);                

            }
        }

        public override void ExitVector(CoombinedGrammarParser.VectorContext ctx)
        {
            stack.Push(new Value(ctx.GetText(), VarType.VECTOR));
        }

        public override void ExitProg(CoombinedGrammarParser.ProgContext ctx)
        {
            //Console.WriteLine(LLVMGenerator.generate());
            var filename = @"C:\Users\USER\Desktop\studia\MAGISTERSKIE\jezyki\PROJEKT_KONCOWY\przyklad.ll";
            using (FileStream fs = File.Create(filename))
            {
                // Add some text to file    
                Byte[] title = new UTF8Encoding(true).GetBytes(LLVMGenerator.generate());
                fs.Write(title, 0, title.Length);
            }
            string strCmdText;
            strCmdText = @"llc.exe przyklad.ll -o target.s";

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("cd ../../../..");
            cmd.StandardInput.WriteLine(strCmdText);
            cmd.StandardInput.WriteLine("clang target.s");
            cmd.StandardInput.WriteLine("a.exe");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }

        public override void ExitValue(CoombinedGrammarParser.ValueContext ctx)
        {
            if (ctx.ID() != null && !variables.Any(v => v.Key == ctx.ID().GetText()))
            {
                try
                {
                    value = memory[ctx.ID().GetText()];
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Nie można znaleźć zmiennej o nazwie: {ctx.ID().GetText()}");
                    Console.ForegroundColor = ConsoleColor.Black;
                }
            }
            if (ctx.STRING() != null)
            {
                String tmp = ctx.STRING().GetText();
                value = tmp.Substring(1, tmp.Length - 2);
                stack.Push(new Value(ctx.GetText(), VarType.STRING));
            }
            if (ctx.INT() != null)
            {                
                value = ctx.INT().GetText();
                //stack.Push(new Value(ctx.GetText(), VarType.INT));
            }
            if (ctx.vector() != null)
            {
                String tmp = ctx.GetText();
                value = tmp;
            }

        }

        public override void ExitInt(CoombinedGrammarParser.IntContext ctx)
        {
            stack.Push(new Value(ctx.INT().GetText(), VarType.INT));
        }

        public override void ExitReal(CoombinedGrammarParser.RealContext ctx)
        {
            stack.Push(new Value(ctx.REAL().GetText(), VarType.REAL));
        }

        public override void ExitPrint(CoombinedGrammarParser.PrintContext ctx)
        {
            var ID = ctx.value().ID();
            if (ID != null)
            {
                if (!variables.Any(v => v.Key == ID.GetText()))
                {
                    LLVMGenerator.print(value);

                    return;
                }

                VarType type = variables[ID.GetText()];
                if (type != VarType.UNKNOWN)
                {
                    if (type == VarType.INT)
                    {
                        LLVMGenerator.printf_i32(ID.GetText());
                    }
                    if (type == VarType.REAL)
                    {
                        LLVMGenerator.printf_double(ID.GetText());
                    }
                    if (type == VarType.REAL)
                    {
                        LLVMGenerator.printf_double(ID.GetText());
                    }
                    if (type == VarType.VECTOR)
                    {
                        LLVMGenerator.print("[ ");
                        foreach (var item in ArrayMemory[ID.GetText()])
                        {
                            LLVMGenerator.printf_i32(ID.GetText());
                            LLVMGenerator.print(", ");
                        }
                        LLVMGenerator.print("]");
                    }
                }
                else
                {
                    Console.WriteLine(ctx.start.Text, "unknown variable " + ID);
                }
            }
            else
            {
                //string
                LLVMGenerator.print(value);
            }
        }

        
        public override void ExitAdd(CoombinedGrammarParser.AddContext ctx)
        {
            Value v1 = stack.Pop();
            Value v2 = stack.Pop();
            if (v1.type == v2.type)
            {
                if (v1.type == VarType.INT)
                {
                    LLVMGenerator.add_i32(v1.name, v2.name);
                    stack.Push(new Value("%" + (LLVMGenerator.reg - 1), VarType.INT));
                }
                if (v1.type == VarType.REAL)
                {
                    LLVMGenerator.add_double(v1.name, v2.name);
                    stack.Push(new Value("%" + (LLVMGenerator.reg - 1), VarType.REAL));
                }
            }
            else
            {
                Console.WriteLine(ctx.start.Text, "add type mismatch");
            }
        }

        
        public override void ExitMult(CoombinedGrammarParser.MultContext ctx)
        {
            Value v1 = stack.Pop();
            Value v2 = stack.Pop();
            if (v1.type == v2.type)
            {
                if (v1.type == VarType.INT)
                {
                    LLVMGenerator.mult_i32(v1.name, v2.name);
                    stack.Push(new Value("%" + (LLVMGenerator.reg - 1), VarType.INT));
                }
                if (v1.type == VarType.REAL)
                {
                    LLVMGenerator.mult_double(v1.name, v2.name);
                    stack.Push(new Value("%" + (LLVMGenerator.reg - 1), VarType.REAL));
                }
            }
            else
            {
                Console.WriteLine(ctx.start.Text, "mult type mismatch");
            }
        }

        
        public override void ExitToint(CoombinedGrammarParser.TointContext ctx)
        {
            Value v = stack.Pop();
            LLVMGenerator.fptosi(v.name);
            stack.Push(new Value("%" + (LLVMGenerator.reg - 1), VarType.INT));
        }

        
        public override void ExitToreal(CoombinedGrammarParser.TorealContext ctx)
        {
            Value v = stack.Pop();
            LLVMGenerator.sitofp(v.name);
            stack.Push(new Value("%" + (LLVMGenerator.reg - 1), VarType.REAL));
        }

        public override void ExitRead(CoombinedGrammarParser.ReadContext ctx)
        {
            String ID = ctx.ID().GetText();
            if (!variables.Any(m => m.Key == ID))
            {
                memory.Add(ID, ID);
                LLVMGenerator.declare(ID);
            }
            LLVMGenerator.scanf(ID);
        }
    }
}
