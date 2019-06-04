using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kompilator
{
    public class LLVMGenerator
    {
        static String header_text = "";
        static String main_text = "";
        public static int reg = 1;
        
        public static void scanf(String id)
        {
            main_text += "%" + reg + " = call i32 (i8*, ...) @__isoc99_scanf(i8* getelementptr inbounds ([3 x i8], [3 x i8]* @strs, i32 0, i32 0), i32* %" + id + ")\n";
            reg++;
        }

        public static void print(String text)
        {
            int str_len = text.Length;
            String str_type = "[" + (str_len + 2) + " x i8]";
            header_text += "@str" + reg + " = constant" + str_type + " c\"" + text + "\\0A\\00\"\n";
            main_text += "call i32 (i8*, ...) @printf(i8* getelementptr inbounds ( " + str_type + ", " + str_type + "* @str" + reg + ", i32 0, i32 0))\n";
            reg++;
        }

        public static void declare(String id)
        {
            main_text += "%" + id + " = alloca i32\n";
        }

        public static void assign(String id, String value)
        {
            main_text += "store i32 " + value + ", i32* %" + id + "\n";
        }

        public static void printf_i32(String id)
        {
            main_text += "%" + reg + " = load i32, i32* %" + id + "\n";
            reg++;
            main_text += "%" + reg + " = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strpi, i32 0, i32 0), i32 %" + (reg - 1) + ")\n";
            reg++;
        }

        public static void printf_double(String id)
        {
            main_text += "%" + reg + " = load double, double* %" + id + "\n";
            reg++;
            main_text += "%" + reg + " = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([4 x i8], [4 x i8]* @strpd, i32 0, i32 0), double %" + (reg - 1) + ")\n";
            reg++;
        }

        public static void declare_i32(String id)
        {
            main_text += "%" + id + " = alloca i32\n";
        }

        public static void declare_double(String id)
        {
            main_text += "%" + id + " = alloca double\n";
        }

        public static void declare_array(String id, int number)
        {
            main_text += "%" + id + " = alloca [" + number + " x i32]\n";
        }

        public static void assign_array(String id, String value, int number)
        {
            main_text += "store [" + number + " x i32]" + value + ", i32* %" + id + "\n";
        }

        public static void assign_i32(String id, String value)
        {
            main_text += "store i32 " + value + ", i32* %" + id + "\n";
        }

        public static void assign_double(String id, String value)
        {
            main_text += "store double " + value + ", double* %" + id + "\n";
        }


        public static void load_i32(String id)
        {
            main_text += "%" + reg + " = load i32, i32* %" + id + "\n";
            reg++;
        }

        public static void load_double(String id)
        {
            main_text += "%" + reg + " = load double, double* %" + id + "\n";
            reg++;
        }

        public static void add_i32(String val1, String val2)
        {
            main_text += "%" + reg + " = add i32 " + val1 + ", " + val2 + "\n";
            reg++;
        }

        public static void add_double(String val1, String val2)
        {
            main_text += "%" + reg + " = fadd double " + val1 + ", " + val2 + "\n";
            reg++;
        }

        public static void mult_i32(String val1, String val2)
        {
            main_text += "%" + reg + " = mul i32 " + val1 + ", " + val2 + "\n";
            reg++;
        }

        public static void mult_double(String val1, String val2)
        {
            main_text += "%" + reg + " = fmul double " + val1 + ", " + val2 + "\n";
            reg++;
        }

        public static void sitofp(String id)
        {
            main_text += "%" + reg + " = sitofp i32 " + id + " to double\n";
            reg++;
        }

        public static void fptosi(String id)
        {
            main_text += "%" + reg + " = fptosi double " + id + " to i32\n";
            reg++;
        }


        public static String generate()
        {
            String text;
            text = "declare i32 @printf(i8*, ...)\n";
            text += "declare i32 @__isoc99_scanf(i8*, ...)\n";
            text += "@strpi = constant [4 x i8] c\"%d\\0A\\00\"\n";
            text += "@strpd = constant [4 x i8] c\"%f\\0A\\00\"\n";
            text += "@strp = constant [4 x i8] c\"%d\\0A\\00\"\n";
            text += "@strs = constant [3 x i8] c\"%d\\00\"\n";
            text += header_text;
            text += "define i32 @main() nounwind{\n";
            text += main_text;
            text += "ret i32 0 }\n";
            return text;
        }
    }
}
