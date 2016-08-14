using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Interpreter
{
    internal class Interpreter
    {
        public interface IToken { }

        #region Helpers

        /// <summary> Returns true if source is a methodCall, passing details onto the methodCall argument, also specifying the rest of the string in</summary>
        private static bool IsMethod(string source, out MethodCall methodCall, out string rest)
        {
            rest = source;
            methodCall = null;
            var firstEvidence = source.Split(' ')[0].IndexOf('(');
            if (firstEvidence == -1)
                return false;

            var secondEvidence = source.IndexOf(')');
            if(secondEvidence == -1)
                throw new ArgumentException($"Something's wrong here");
            
            IEnumerable<string> arguments;
            var methodName = ExtractMethodNameAndArguments(source, out arguments, out rest);
            methodCall = new MethodCall(methodName, arguments);
            return true;
        }

        private static Variable Evaluate(string source)
        {
            string x;
            return Evaluate(source,  out x);
        }
        
        private static Variable Evaluate(string source, out string rest)
        {
            MethodCall methodInfo;
            var splittedSource = source.Split(new [] {' '},3, StringSplitOptions.RemoveEmptyEntries);

            //DEBUG
            if (splittedSource[0].Contains("somefun"))
                rest = "";

            //Check if source contains a plain string
            if (source[0] == '\"')//plain strings will always start with a quote
            {
                var secondQuote = source.IndexOf('\"', 1); //find last quote index
                var firstExpression = '\"' + Substring(source, 1, secondQuote-1) + '\"'; //take plain string out
                splittedSource = new []{firstExpression}.Concat(source.Remove(0, firstExpression.Length).Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)).ToArray();
                if (splittedSource.Count() == 1)
                {
                    rest = "";
                    return Variable.Get(splittedSource[0]);
                }
                throw new Exception("Unhandled case");
            }

            //source can be a single variable or a method (empty or without space-separated arguments)
            if (splittedSource[0] == source)
                return IsMethod(source, out methodInfo, out rest) ? methodInfo.Evaluate() : Variable.Get(splittedSource[0]);

            //ensure first expression is a method. Evaluate method and evaluate right expression (if there is any)
            if (IsMethod(source, out methodInfo, out rest))
            {
                if(rest.Length > 0)
                    throw new Exception("Checkup");
                return methodInfo.Evaluate();
            }

            //ensure first expression is not a method and evaluate left and right expression with operator
            if (!IsMethod(splittedSource[0], out methodInfo, out rest))
                return Operator.Get(splittedSource[1], Evaluate(splittedSource[0]), Evaluate(splittedSource[2])).Evaluate();
            
            throw new Exception("I don't know what to do now!");
        }

        ///<summary>Adds one to the endIndex and returns string between startIndex and endIndex</summary>
        private static string Substring(string str, int startIndex, int endIndex) => str.Substring(startIndex, endIndex - startIndex+1);

        private static IEnumerable<int> FindAllChars(string str, char subStr)
        {
            for (var i = 0; i < str.Length; i++)
                if (str[i] == subStr)
                    yield return i;
        }

        private static IEnumerable<Tuple<int, int>> FindAllStringIndexes(string str, string substr)
        {
            var tLast = str.IndexOf(substr, 0, StringComparison.Ordinal);
            while (tLast != -1)
            {
                yield return new Tuple<int, int>(tLast, tLast+substr.Length);
                tLast = str.IndexOf(substr, tLast, StringComparison.Ordinal);
            }
        }

        private static string ExtractMethodNameAndArguments(string signature, out IEnumerable<string> arguments)
        {
            string x;
            return ExtractMethodNameAndArguments(signature, out arguments, out x);
        }

        private static string ExtractMethodNameAndArguments(string signature, out IEnumerable<string> arguments, out string rest)
        {
            //Extract method name
            var iEnd = signature.IndexOf("(", StringComparison.Ordinal);
            var iStart = signature.LastIndexOf(" ", iEnd, StringComparison.Ordinal) + 1;
            iStart = iStart == -1 ? 0 : iStart; //When its a plain methodCall, there won't be any whitespaces
            var methodName = Substring(signature, iStart, iEnd-1);

            //Extract arguments
            iStart = iEnd + 1; //inc to mark starting position of first argument
            iEnd = signature.IndexOf(")", iStart, StringComparison.Ordinal);
            arguments = Substring(signature, iStart, iEnd-1).Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace(" ", "")).ToArray();
            rest = Substring(signature, iEnd + 1, signature.Length-1);
            return methodName;
        }

        private static List<IExpression> AddExpressionToList(List<IExpression> listToBeUpdated,
            IExpression expressionToBeAdded)
        {
            listToBeUpdated.Add(expressionToBeAdded);
            return listToBeUpdated;
        }

        private static string BeheadString(string str)
        {
            int endIndex = str.IndexOf('\n');
            return endIndex == -1 ? "" : str.Remove(0, endIndex + 1);
        }

        #endregion Helpers

        #region Operators

        public abstract class Operator : IExpression
        {
            public IExpression Left, Right;

            protected Operator(IExpression left, IExpression right)
            {
                Left = left;
                Right = right;
            }

            public abstract Variable Evaluate();

            public static Operator Get(string op, IExpression left, IExpression right)
            {
                switch (op)
                {
                    case "+": return new Addition(left, right);
                    case "-": return new Subtraction(left, right);
                    case "*": return new Multiplication(left, right);
                    case "/": return new Division(left, right);
                    default: throw new NotSupportedException($"Operator {op} not found.");
                }
            }
        }

        public class Addition : Operator
        {
            public Addition(IExpression left, IExpression right) : base(left, right)
            {
            }

            public override Variable Evaluate()
            {
                var l = Left.Evaluate();
                var r = Right.Evaluate();
                if (l is Number && r is Number)
                    return Variable.Get(((int)l.Value + (int)r.Value).ToString(CultureInfo.CurrentCulture));
                if ((l is Number | l is Decimal) && (r is Number | r is Decimal))
                    return Variable.Get(((double)l.Value + (double)r.Value).ToString(CultureInfo.CurrentCulture));
                if (l is Text && r is Text)
                    return Variable.Get('\"' + l.Value.ToString() + r.Value.ToString() + '\"' );
                throw new InvalidExpressionException("Expressions cannot be added!");
            }
        }

        public class Subtraction : Operator
        {
            public Subtraction(IExpression left, IExpression right) : base(left, right)
            {
            }

            public override Variable Evaluate()
            {
                var l = Left.Evaluate();
                var r = Right.Evaluate();
                if (l is Text | r is Text)
                    throw new InvalidExpressionException("Expressions cannot be divided!");
                var result = Convert.ToDouble(l.Value) - Convert.ToDouble(r.Value);
                if ((Math.Abs(result % 1)) < double.Epsilon)
                    return new Number(Convert.ToInt32(result));
                return new Decimal(result);
            }
        }

        public class Multiplication : Operator
        {
            public Multiplication(IExpression left, IExpression right) : base(left, right)
            {
            }

            public override Variable Evaluate()
            {
                var l = Left.Evaluate();
                var r = Right.Evaluate();
                if (l is Text | r is Text)
                    throw new InvalidExpressionException("Expressions cannot be divided!");
                var result = Convert.ToDouble(l.Value) * Convert.ToDouble(r.Value);
                if ((Math.Abs(result % 1)) < double.Epsilon)
                    return new Number(Convert.ToInt32(result));
                return new Decimal(result);
            }
        }

        public class Division : Operator
        {
            public Division(IExpression left, IExpression right) : base(left, right)
            {
            }

            public override Variable Evaluate()
            {
                var l = Left.Evaluate();
                var r = Right.Evaluate();
                if (l is Text | r is Text)
                    throw new InvalidExpressionException("Expressions cannot be divided!");
                var result = Convert.ToDouble(l.Value) / Convert.ToDouble(r.Value);
                if ((Math.Abs(result % 1)) < double.Epsilon)
                    return new Number(Convert.ToInt32(result));
                return new Decimal(result);
            }
        }

        #endregion Operators

        #region Variables

        public class Variable : IExpression
        {
            public string Name;
            public object Value;

            protected Variable(Variable variable)
            {
                Name = variable.Name;
                Value = variable.Value;
            }

            protected Variable(object value)
            {
                Name = null;
                Value = value;
            }

            protected Variable(string name, object value)
            {
                Name = name;
                Value = value;
            }

            /// <summary>
            /// Returns a ProgramVariable if obj is a name, else return a loose variable
            /// </summary>
            public static Variable Get(string obj)
            {
                if (ProgramVariables.ContainsKey(obj))
                    return ProgramVariables[obj];
                if (obj.Contains("\""))
                    return new Text(obj.Substring(1, obj.Length - 2)); //to remove quotes
                if (obj.Contains("."))
                    return new Decimal(double.Parse(obj));
                return new Number(int.Parse(obj));
            }

            public static Variable CreateFromVariable(string name, Variable obj)
            {
                obj.Name = name;
                return obj;
            }

            public Variable Evaluate() => this;
        }

        public class Number : Variable
        {
            public Number(string name, int value) : base(name, value)
            {
            }

            public Number(int value) : base(value)
            {
            }
        }

        public class Decimal : Variable
        {
            public Decimal(string name, double value) : base(name, value)
            {
            }

            public Decimal(double value) : base(value)
            {
            }
        }

        public class Text : Variable
        {
            public Text(string name, string value) : base(name, value)
            {
            }

            public Text(string value) : base(value)
            {
            }
        }

        #endregion Variables

        #region Expressions

        public interface IExpression : IToken
        {
            Variable Evaluate();
        }

        public class ReturnStmt : IExpression
        {
            public string ReturnValue;

            public ReturnStmt(string val)
            {
                ReturnValue = val;
            }

            public Variable Evaluate() => Interpreter.Evaluate(ReturnValue);
        }

        public class MethodDeclaration : IExpression
        {
            public string Name;
            public string[] Parameters { get; }
            public IEnumerable<IExpression> Body { get; }

            public MethodDeclaration(string name, string[] parameters, IEnumerable<IExpression> body)
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public Variable Evaluate()
            {
                ProgramMethods.Add(Name, this);
                return null;
            }

            public void Call(Variable[] args, out Variable returnVariable)
            {
                //Load arguments into program
                for (var i = 0; i < Parameters.Length; i++)
                    ProgramVariables.Add(Parameters[i], args[i]);

                //Evaluate function
                returnVariable = null;
                foreach (var expression in Body)
                {
                    if (expression is ReturnStmt)
                    {
                        returnVariable = expression.Evaluate();
                        break;
                    }

                    expression.Evaluate();
                }

                //Remove arguments from program
                Parameters.ToList().ForEach(x => ProgramVariables.Remove(x));
            }

            public override string ToString() => string.Format("fun {0}({1}) { {2}}", Name, Parameters, Body.Select(expr => expr.ToString() + ";\n"));
        }

        public class MethodCall : IExpression
        {
            public string TargetMethod;
            public IEnumerable<string> Arguments;
            public Variable ReturnVariable;
            
            public MethodCall(string methodName, IEnumerable<string> argumentNames)
            {
                TargetMethod = methodName;
                Arguments = argumentNames;
            }

            public Variable Evaluate()
            {
                ProgramMethods[TargetMethod].Call(Arguments.Select(Variable.Get).ToArray(), out ReturnVariable);
                return ReturnVariable;
            }

            public static MethodCall Empty()
            {
                return new MethodCall(null, null);
            }

            public override string ToString() => string.Format("{0}({1})", TargetMethod, Arguments);
        }

        public class VariableDeclaration : IExpression
        {
            public string Name, Value;

            public VariableDeclaration(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public Variable Evaluate()
            {
                ProgramVariables.Add(Name, Variable.CreateFromVariable(Name, Interpreter.Evaluate(Value)));
                return null;
            }

            public override string ToString() => ($"var {Name} = {Value}");
        }

        public class VariableAssignment : IExpression
        {
            public string TargetName;
            public string SourceName;

            public VariableAssignment(string target, string source)
            {
                TargetName = target;
                SourceName = source;
            }

            public Variable Evaluate()
            {
                ProgramVariables[TargetName].Value = Interpreter.Evaluate(SourceName).Value;
                return null;
            }

            public override string ToString() => ($"{TargetName} = {SourceName}");
        }

        #endregion Expressions

        #region Semantics

        public static VariableDeclaration MakeVarDecl(string[] splittedExpression) =>
            new VariableDeclaration(splittedExpression[1], string.Join(" ", splittedExpression.Skip(3)));

        public static VariableAssignment MakeVarAss(string[] splittedExpression) =>
            new VariableAssignment(splittedExpression[0], splittedExpression.Skip(2).Aggregate( (x,y) => x + " " + y));

        public static MethodDeclaration MakeMethodDecl(string signature, string body)
        {
            IEnumerable<string> args;
            var methodName = ExtractMethodNameAndArguments(signature, out args);
            var functionBody = body.Substring(body.IndexOf("{", StringComparison.Ordinal) + 1, body.LastIndexOf("}", StringComparison.Ordinal) - 1); //dec & inc to avoid brackets
            return new MethodDeclaration(methodName, args.ToArray(), MakeProgram(new List<IExpression>(), functionBody));
        }

        public static MethodCall MakeMethodCall(string expression)
        {
            IEnumerable<string> args;
            var methodName = ExtractMethodNameAndArguments(expression, out args);
            return new MethodCall(methodName, args);
        }
        
        public static ReturnStmt MakeReturnStmt(string expression) => new ReturnStmt(expression.Split(new []{' '}, 2, StringSplitOptions.RemoveEmptyEntries)[1]);

        public static IEnumerable<IExpression> MakeProgram(List<IExpression> programCode, string programText)
        {
            if (programText.Length == 0)
                return programCode;
            if (IllegalStartingCharacters.Contains(programText[0]))
                return MakeProgram(programCode, programText.Remove(0, 1));

            var curLine = programText.Split(';').First();
            var curLineSplitted = curLine.Split(' ').ToArray();
            if (!curLineSplitted.Any()) curLineSplitted = null;

            //Do the parser work
            if (curLineSplitted?[0] == "sub")
            {
                //Extract function body
                var iStart = programText.IndexOf("{", StringComparison.Ordinal);
                var iEnd = programText.IndexOf("}", iStart, StringComparison.Ordinal) + 1; //inc to wrap around when taking substring

                return MakeProgram(AddExpressionToList(programCode, MakeMethodDecl(curLine, programText.Substring(iStart, iEnd - iStart))), programText.Remove(0, iEnd));
            }
            if (curLineSplitted?[0] == "var" && curLineSplitted?[2] == "=")
                return MakeProgram(AddExpressionToList(programCode, MakeVarDecl(curLineSplitted)), BeheadString(programText));
            if (curLine.Contains("(") && curLine.Contains(")"))
                return MakeProgram(AddExpressionToList(programCode, MakeMethodCall(curLine)), BeheadString(programText));
            if (curLineSplitted?[1] == "=")
                return MakeProgram(AddExpressionToList(programCode, MakeVarAss(curLineSplitted)), BeheadString(programText));
            if (curLineSplitted?[0] == "return")
                return MakeProgram(AddExpressionToList(programCode, MakeReturnStmt(curLine)),BeheadString(programText));
            throw new NotSupportedException($"Unhandled line of code: {curLine}");
        }

        #endregion Tokenizer Methods

        #region CodeInspection

        public class ProgramError
        {
            public int LineNumber;
            public string CodeLine;
            public string ErrorInfo;

            public ProgramError(int lineNumber, string codeLine)
            {
                LineNumber = lineNumber;
                CodeLine = codeLine;
                ErrorInfo = "";
            }
            
            public ProgramError(int lineNumber, string codeLine, string errorInfo)
            {
                LineNumber = lineNumber;
                CodeLine = codeLine;
                ErrorInfo = errorInfo;
            }

            public static ProgramError Empty() => new ProgramError(-1, "", "Unknown");

            public bool IsSet() => (this.LineNumber >= 0);


            public class LineEndingError : ProgramError
            {
                public LineEndingError(int lineNumber, string codeline) : base(lineNumber, codeline, "Line is invalidly ended") { }
            }

            public class EmptyFunctionError : ProgramError
            {
                public EmptyFunctionError(int lineNumber, string functionSignature) : base(lineNumber, functionSignature, "Function has no body") { }
            }
        }

        /// <summary>
        /// Checks whether all code lines are validly ended.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ProgramError CheckLineEndings(string code)
        {
            var scannableCode = code.Replace(" ", "");
            var acceptedLineEndings = new[] {';', '}', '{', '\n', '\t'};
            for (int lineNumber = 0, lastLineEnding = 0, lineEnding = scannableCode.IndexOf('\n'); 
                lineEnding != -1; 
                lastLineEnding = lineEnding, lineEnding = scannableCode.IndexOf('\n', lastLineEnding+1), lineNumber++)
                if (!acceptedLineEndings.Contains(scannableCode[lineEnding - 1]))
                    return new ProgramError.LineEndingError(lineNumber,Substring(scannableCode, lastLineEnding, lineEnding));
            return ProgramError.Empty();
        }


        /// <summary>
        /// Checks whether all functions have a body.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ProgramError CheckEmptyFunctions(string code)
        {
            var scannableCode = code.Replace(" ", "").Replace('\t'.ToString(), "").Replace('\n'.ToString(), "");
            for (int lineNumber = 0, firstBracket = scannableCode.IndexOf('{'), secondBracket = scannableCode.IndexOf('}', firstBracket), functionNameStart = scannableCode.LastIndexOf("sub", firstBracket, StringComparison.Ordinal) + 3;
                firstBracket != -1;
                firstBracket = scannableCode.IndexOf('{', secondBracket), secondBracket = scannableCode.IndexOf('}'), lineNumber++
                )
                if(secondBracket - firstBracket == 1)
                    return new ProgramError.EmptyFunctionError(lineNumber, Substring(scannableCode, functionNameStart, firstBracket-1));
            return ProgramError.Empty();
        }

        public static bool RunCodeInspection(string code, out ProgramError exceptionInfo)
        {
            exceptionInfo = CheckLineEndings(code);
            if (exceptionInfo.IsSet())
                return false;

            exceptionInfo = CheckEmptyFunctions(code);
            if (exceptionInfo.IsSet())
                return false;

            return true;
        }

        #endregion

        #region Main Program Management

        /** Programming rules:
         * - Variables are declared using the 'var' keyword
         * - Variables are assigned using the '=' operator
         * - Decimal variables are signed by the '.' sign
         * - Methods are declared using the 'fun' keyword, using the '()' operator with parameters inside, with it's body between '{}'
         * - Methods are called using the '()' operator with arguments separated by ','
         *
         * Variable Declaration:
         * var <variable_name> = <future_value>
         *
         * Variable Assignment:
         * <variable_name> = <future_value>
         *
         * MethodDeclaration Declaration:
         * fun <function_name> ( <parameter_name> , ... ) { var x = 5; }
         *
         */

        public static char[] IllegalStartingCharacters = { '\n', '\t', ' ' };
        public static string InputCode;
        public static ProgramError ProgramStartupError;
        public static Dictionary<string, Variable> ProgramVariables;
        public static Dictionary<string, MethodDeclaration> ProgramMethods;
        public static List<IExpression> ProgramCode;

        private static ProgramError Init(string code, out bool success)
        {
            //Code analysis
            InputCode = code;
            ProgramStartupError = ProgramError.Empty();
            success = RunCodeInspection(InputCode, out ProgramStartupError);
            
            //Initialize virtual machine
            ProgramMethods = new Dictionary<string, MethodDeclaration>();
            ProgramVariables = new Dictionary<string, Variable>();
            ProgramCode = new List<IExpression>(MakeProgram(new List<IExpression>(), code));
            return ProgramStartupError;
        }

        private static int _ip = -1;

        private static bool StartDebug(string code, out ProgramError errorBuffer)
        {
            _ip = 0;
            bool success;
            errorBuffer = Init(code, out success);
            return success;
        }

        public static bool StepDebug()
        {
            if (_ip == ProgramCode.Count) return false;
            ProgramCode[_ip].Evaluate();
            _ip++;
            return true;
        }

        public static void ExitDebug()
        {
            _ip = ProgramCode.Count;
        }

        private static bool Interpret(string code, out ProgramError errorBuffer)
        {
            bool success;
            errorBuffer = Init(code, out success);
            if (!success)
                return false;

            ProgramCode.ForEach(x => x.Evaluate());
            return true;
        }

        public static ProgramError Start(string code, string action)
        {
            switch (action)
            {
                case "interpret":
                    return Interpret(code, out ProgramStartupError) ? null : ProgramStartupError;
                case "debug":
                    return StartDebug(code, out ProgramStartupError) ? null : ProgramStartupError;
                default:
                    return ProgramError.Empty();
            }
        }

        #endregion Main Program Management
    }
}