using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms.VisualStyles;

namespace Interpreter
{
    class Interpreter
    {
        #region Helpers

        /// <summary> Returns true if source is a methodCall, passing details onto the methodCall argument, also returning the rest of the string</summary>
        private static bool IsMethod(string source, out MethodCall methodCall)
        {
            string nothing;
            return IsMethod(source, out methodCall, out nothing);
        }

        private static bool IsMethod(string source, out MethodCall methodCall, out string rest) //has kinda useless body
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

        private static Variable EvaluateSingle(string source, out MethodCall methodInfo) => IsMethod(source, out methodInfo) ? methodInfo.Evaluate() : Variable.Get(source);


        /// <summary>
        /// Returns a string containing everything between before lastIndex and its rightmost whitespace (or other character)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="ch"></param>
        /// <param name="lastIndex"></param>
        /// <returns></returns>
        private static string WrapUntil(string source, int lastIndex, char ch = ' ') => Substring(source, source.LastIndexOf(ch, lastIndex), lastIndex-1);

        private static string WrapFrom(string source, int index, char ch = ' ') => Substring(source, source.IndexOf(ch, index), index - 1);

        private static Variable Evaluate(string source)
        {
            string nothing;
            return Evaluate(source, out nothing);
        }

        private static Variable Evaluate(string source, out string rest)
        {
            //Remove parenthesis if expression is wrapped by them
            rest = "";
            MethodCall methodInfo;
            source = source.Trim(' ');


            if (source[0] == '(' && source[source.Length - 1] == ')')
                return Evaluate(Substring(source, 1, source.Length - 1));

            var splittedSource = source.Split(new [] {' '},3, StringSplitOptions.RemoveEmptyEntries);

            //Check if source contains a plain string
            if (source[0] == '\"')//plain strings will always start with a quote
            {
                var secondQuote = source.IndexOf('\"', 1); //find last quote index
                var firstExpression = '\"' + Substring(source, 1, secondQuote-1) + '\"'; //take plain string out
                splittedSource = new []{firstExpression}.Concat(source.Remove(0, firstExpression.Length).Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)).ToArray(); //glue the string back together
                if (splittedSource.Count() != 1) throw new Exception("Unhandled case");
                return new Text(Substring(splittedSource[0], 1, splittedSource[0].Length-2));//to remove quotes
            }

            //source can be a single variable or a method (empty or without space-separated arguments)
            if (splittedSource[0] == source)
                return EvaluateSingle(source, out methodInfo).Evaluate();

            //At this point there is a string of expressions which have to be evaluated, most likely containing operators
            //TODO: Perfect PEMDAS analysis
            #region PEMDAS analysis
            
            var e = source.IndexOf('^');
            var m = source.IndexOf('*');
            var d = source.IndexOf('/');
            var a = source.IndexOf('+');
            var s = source.IndexOf('-');
            
            //Evaluate parenthesis first, if any
            if (splittedSource[0][0] == '(')
            {
                //get closing parenthesis of this scope
                var scope = 1;
                var iEnding = 1;
                while (iEnding < source.Length)
                {
                    if (source[iEnding] == '(')
                        scope++;
                    if (source[iEnding] == ')')
                        scope--;
                    if (scope == 0)
                        break;

                    iEnding++;
                }
                var pString = Substring(source, 0, iEnding); //get p's expression definitions in source 
                splittedSource = source.Remove(0, pString.Length)
                    .Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries);

                //If this is the only operator-using expression or when this operator is superior, evaluate this block
                if (splittedSource.Count() < 3 || Operator.IsSuperior(splittedSource[0][0],
                    splittedSource[2].Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)[1][0]))
                    return Operator.Get(splittedSource[0], pString, splittedSource[1]).Evaluate();

                //Now the right side expression has to be evaluated before this expression can be evaluated
                var rightExpression = Evaluate(splittedSource[1]);
                return Operator.Get(splittedSource[0], pString, rightExpression.Value.ToString()).Evaluate();
            }

            //Evaluate EMDAS

            Func<string, int, Variable> f =
                (x, i) =>
                    Operator.Get(x, Substring(source, 0, i - 1), Substring(source, i + 1, source.Length - 1)).Evaluate();

            if (e >= 0)
                return f("^", e);
            if (m >= 0)
                return f("*", m);
            if (d >= 0)
                return f("/", d);
            if (a >= 0)
                return f("+", a);
            if (s >= 0)
                return f("-", s);
            
            #endregion

            //Now pemdas analysis is over and the only possibility left is a method call
            if (IsMethod(source, out methodInfo))
                return methodInfo.Evaluate();


            throw new Exception("Unknown case");
        }

        ///<summary>Adds one to the endIndex and returns string between startIndex and endIndex</summary>
        private static string Substring(string str, int startIndex, int endIndex) => str.Substring(startIndex, endIndex - startIndex+1);

        private static IEnumerable<int> GetCharIndices(string str, char ch)
        {
            for (var i = 0; i < str.Length; i++)
                if (str[i] == ch)
                    yield return i;
        }

        private static IEnumerable<Tuple<int, int>> GetCharIndices(string str, char first, char second)
        {
            var l1 = GetCharIndices(str, first).ToArray();
            var l2 = GetCharIndices(str, second).ToArray();
            return l1.Select((x, i) => new Tuple<int, int>(x, l2[i]));
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


        /// <summary>
        /// Removes the newline character at the end of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string BeheadString(string str)
        {
            int endIndex = str.IndexOf('\n');
            return endIndex == -1 ? "" : str.Remove(0, endIndex + 1);
        }

        #endregion Helpers

        #region Operators

        abstract class Operator : IExpression
        {
            private readonly string Op;
            private readonly string Left;
            private readonly string Right;

            private Variable _vLeft, _vRight;

            private static string PriorityList { get; } = "-+/*^";

            protected Operator(string op,string left, string right)
            {
                Op = op;
                Left = left;
                Right = right;
            }

            public Variable Evaluate()
            {
                _vLeft = _vLeft ?? Interpreter.Evaluate(Left);
                _vRight = _vRight?? Interpreter.Evaluate(Right);

                if(_vLeft is Text | _vRight is Text)
                    if (Op == "+")
                        return new Text(string.Concat((string)_vLeft.Value, (string)_vRight.Value));
                    else
                        throw new InvalidExpressionException("");

                if ((_vLeft is Number | _vLeft is Decimal) && (_vRight is Number | _vRight is Decimal))
                {
                    double result;
                    var left = Convert.ToDouble(_vLeft.Value);
                    var right = Convert.ToDouble(_vRight.Value);
                    switch (Op)
                    {
                        case "+":
                            result = left + right;
                            break;
                        case "-":
                            result = left - right;
                            break;
                        case "*":
                            result = left*right;
                            break;
                        case "/":
                            result = left/right;
                            break;
                        case "^":
                            result = Math.Pow(left, right);
                            break;
                        default: throw new InvalidExpressionException("");
                    }
                    return result < double.Epsilon
                        ? (Variable) new Number(Convert.ToInt32(result))
                        : new Decimal(result);
                }
                throw new NotSupportedException("Unsupported case");
            }

            public static Operator Get(string op, string left, string right)
            {
                switch (op)
                {
                    case "+": return new Addition(left, right);
                    case "-": return new Subtraction(left, right);
                    case "*": return new Multiplication(left, right);
                    case "/": return new Division(left, right);
                    case "^": return new Exponent(left, right);
                    default: throw new NotSupportedException($"Operator {op} not found.");
                }
            }

            /// <summary>
            /// Returns true if firstOperator is superior to secondOperator
            /// </summary>
            public static bool IsSuperior(char firstOperator, char secondOperator)
            {
                var i1 = 0;
                while (PriorityList[i1] != firstOperator)
                    i1++;
                
                var i2 = i1;
                while (PriorityList[i2] != secondOperator && i2 < PriorityList.Length)
                    i2++;

                return i2 <= i1;
            }
        }

        class Addition : Operator
        {
            public Addition(string left, string right) : base("+", left, right)
            {
            }
        }

        class Subtraction : Operator
        {
            public Subtraction(string left, string right) : base("-",left, right)
            {
            }
        }

        class Multiplication : Operator
        {
            public Multiplication(string left, string right) : base("*",left, right)
            {
            }
        }

        class Division : Operator
        {
            public Division(string left, string right) : base("/", left, right)
            {
            }
        }

        class Exponent : Operator
        {
            public Exponent(string left, string right) : base("^", left, right) { }
        }

        #endregion Operators

        #region Variables

        class Variable : IExpression
        {
            private string Name;
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
                if (_programVariables.ContainsKey(obj))
                    return _programVariables[obj];
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

            public override string ToString() => Convert.ToString(Value);
        }

        class Number : Variable
        {
            public Number(string name, int value) : base(name, value)
            {
            }

            public Number(int value) : base(value)
            {
            }
        }

        class Decimal : Variable
        {
            public Decimal(string name, double value) : base(name, value)
            {
            }

            public Decimal(double value) : base(value)
            {
            }
        }

        class Text : Variable
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

        interface IExpression
        {
            Variable Evaluate();
        }

        class ReturnStmt : IExpression
        {
            private readonly string _returnValue;

            public ReturnStmt(string val)
            {
                _returnValue = val;
            }

            public Variable Evaluate() => Interpreter.Evaluate(_returnValue);

            public override string ToString() => "return " + _returnValue;
        }

        class MethodDeclaration : IExpression
        {
            public string Name { get; }
            private string[] Parameters { get; }
            public IEnumerable<IExpression> Body { get; }
            private List<string> localVariables; 

            public MethodDeclaration(string name, string[] parameters, IEnumerable<IExpression> body)
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public Variable Evaluate()
            {
                _programMethods.Add(Name, this);
                return null;
            }

            public Variable Call(Variable[] args)
            {
                //Reserve space for local variables
                localVariables = new List<string>();
                
                //Load arguments into program
                for (var i = 0; i < Parameters.Length; i++)
                    _programVariables.Add(Parameters[i], args[i]);

                //Evaluate function
                Variable returnVariable = null;
                foreach (var expression in Body)
                {
                    if(expression is VariableDeclaration)
                        localVariables.Add( ((VariableDeclaration)expression).Name );

                    if (expression is ReturnStmt)
                    {
                        returnVariable = expression.Evaluate();
                        break;
                    }

                    expression.Evaluate();
                }

                //Remove arguments and local variables from program
                localVariables.ForEach(x => _programVariables.Remove(x));
                Parameters.ToList().ForEach(x => _programVariables.Remove(x));
                return returnVariable;
            }

            public override string ToString() => string.Format("fun {0}({1}) { {2}}", Name, Parameters, Body.Select(expr => expr.ToString() + ";\n"));
        }

        class MethodCall : IExpression
        {
            public string TargetMethod { get; }
            public IEnumerable<string> Arguments { get; }
            
            public MethodCall(string methodName, IEnumerable<string> argumentNames)
            {
                TargetMethod = methodName;
                Arguments = argumentNames;
            }

            public Variable Evaluate() => _programMethods[TargetMethod].Call(Arguments.Select(Variable.Get).ToArray());

            public static MethodCall Empty() => new MethodCall(null, null);

            public override string ToString() => string.Format("{0}({1})", TargetMethod, Arguments);
        }

        class VariableDeclaration : IExpression
        {
            public string Name { get; }
            public string Value { get; }

            public VariableDeclaration(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public Variable Evaluate()
            {
                _programVariables.Add(Name, Variable.CreateFromVariable(Name, Interpreter.Evaluate(Value)));
                return null;
            }

            public override string ToString() => ($"var {Name} = {Value}");
        }

        class VariableAssignment : IExpression
        {
            public string TargetName { get; }
            public string SourceName { get; }

            public VariableAssignment(string target, string source)
            {
                TargetName = target;
                SourceName = source;
            }

            public Variable Evaluate()
            {
                _programVariables[TargetName].Value = Interpreter.Evaluate(SourceName).Value;
                return null;
            }

            public override string ToString() => ($"{TargetName} = {SourceName}");
        }

        #endregion Expressions

        #region Semantics

        private static VariableDeclaration MakeVarDecl(string[] splittedExpression) =>
            new VariableDeclaration(splittedExpression[1], string.Join(" ", splittedExpression.Skip(3)));

        private static VariableAssignment MakeVarAss(string[] splittedExpression) =>
            new VariableAssignment(splittedExpression[0], splittedExpression.Skip(2).Aggregate( (x,y) => x + " " + y));

        private static MethodDeclaration MakeMethodDecl(string signature, string body)
        {
            IEnumerable<string> args;
            var methodName = ExtractMethodNameAndArguments(signature, out args);
            var functionBody = body.Substring(body.IndexOf("{", StringComparison.Ordinal) + 1, body.LastIndexOf("}", StringComparison.Ordinal) - 1); //dec & inc to avoid brackets
            return new MethodDeclaration(methodName, args.ToArray(), MakeProgram(new List<IExpression>(), functionBody));
        }

        private static MethodCall MakeMethodCall(string expression)
        {
            IEnumerable<string> args;
            var methodName = ExtractMethodNameAndArguments(expression, out args);
            return new MethodCall(methodName, args);
        }
        
        private static ReturnStmt MakeReturnStmt(string expression) => new ReturnStmt(expression.Split(new []{' '}, 2, StringSplitOptions.RemoveEmptyEntries)[1]);

        private static IEnumerable<IExpression> MakeProgram(List<IExpression> programCode, string programText)
        {
            if (programText.Length == 0)
                return programCode;

            programText = programText.TrimStart('\t', ' ', '\n');

            var curLine = programText.Split(';').First();
            var curLineSplitted = curLine.Split(' ').ToArray();

            //Do the parser work
            if (curLine[0] == '#')
                return MakeProgram(programCode, programText.Remove(0, programText.IndexOf('\n')));

            if (!curLineSplitted.Any())
                throw new ArgumentException("Unhandled case");

            if (curLineSplitted[0] == "sub")
            {
                //Extract function body
                var iStart = programText.IndexOf("{", StringComparison.Ordinal);
                var iEnd = programText.IndexOf("}", iStart, StringComparison.Ordinal) + 1; //inc to wrap around when taking substring

                return MakeProgram(AddExpressionToList(programCode, MakeMethodDecl(curLine, programText.Substring(iStart, iEnd - iStart))), programText.Remove(0, iEnd));
            }
            if (curLineSplitted[0] == "var" && curLineSplitted[2] == "=")
                return MakeProgram(AddExpressionToList(programCode, MakeVarDecl(curLineSplitted)), BeheadString(programText));
            if (curLineSplitted.Count() > 1 && curLineSplitted[1] == "=")
                return MakeProgram(AddExpressionToList(programCode, MakeVarAss(curLineSplitted)), BeheadString(programText));
            if (curLineSplitted[0] == "return")
                return MakeProgram(AddExpressionToList(programCode, MakeReturnStmt(curLine)),BeheadString(programText));
            if (curLine.Contains("(") && curLine.Contains(")"))
                return MakeProgram(AddExpressionToList(programCode, MakeMethodCall(curLine)), BeheadString(programText));

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

            public bool IsSet() => (LineNumber >= 0);


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
        private static ProgramError CheckLineEndings(string code)
        {
            var scannableCode = code.Replace(" ", "").Replace('\t'.ToString(), "").Replace(new [] {'\n','n'}.ToString(),'\n'.ToString());
            var acceptedLineEndings = new[] {';', '}', '{', '\n', '\t'};
            for (int lineNumber = 0, lastLineEnding = 0, lineEnding = scannableCode.IndexOf('\n'); 
                lineEnding != -1; 
                lastLineEnding = lineEnding, lineEnding = scannableCode.IndexOf('\n', lastLineEnding+1), lineNumber++)
                if (!acceptedLineEndings.Contains(scannableCode[lineEnding - 1]) && (scannableCode[lastLineEnding + 1] != '#'))
                    return new ProgramError.LineEndingError(lineNumber,Substring(scannableCode, lastLineEnding, lineEnding));
            return ProgramError.Empty();
        }
        
        /// <summary>
        /// Checks whether all functions have a body.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static ProgramError CheckEmptyFunctions(string code)
        {
            var scannableCode = code.Replace(" ", "").Replace('\t'.ToString(), "").Replace('\n'.ToString(), "");
            for (int lineNumber = 0, firstBracket = scannableCode.IndexOf('{'), secondBracket = scannableCode.IndexOf('}', firstBracket), functionNameStart = scannableCode.LastIndexOf("sub", firstBracket, StringComparison.Ordinal) + 3;
                firstBracket != -1;
                firstBracket = scannableCode.IndexOf('{', secondBracket), secondBracket = firstBracket >= 0 ? scannableCode.IndexOf('}', firstBracket) : 0, lineNumber++
                )
                if(secondBracket - firstBracket == 1)
                    return new ProgramError.EmptyFunctionError(lineNumber, Substring(scannableCode, functionNameStart, firstBracket-1));
            return ProgramError.Empty();
        }

        private static bool RunCodeInspection(string code, out ProgramError exceptionInfo)
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
        
        private static string _inputCode;
        private static List<IExpression> _programCode;
        private static ProgramError _programStartupError;
        private static Dictionary<string, Variable> _programVariables;
        private static Dictionary<string, MethodDeclaration> _programMethods;

        public static IEnumerable<KeyValuePair<string,string>> GetProgramVariables() => _programVariables.Select(x => new KeyValuePair<string, string>(x.Key, Convert.ToString(x.Value)));

        public static IEnumerable<KeyValuePair<string, int>> GetProgramMethods() => _programMethods.Select(
            x => new KeyValuePair<string, int>(x.Value.Name, x.Value.Body.Count()));

        public static IEnumerable<string> GetMethodBody(string methodName) => _programMethods[methodName].Body.Select(x => x.ToString());

        private static ProgramError Init(string code, out bool success)
        {
            //Code analysis
            _inputCode = code;
            _programStartupError = ProgramError.Empty();
            success = RunCodeInspection(_inputCode, out _programStartupError);
            if(!success)
                return _programStartupError;

            //Initialize virtual machine
            _programMethods = new Dictionary<string, MethodDeclaration>();
            _programVariables = new Dictionary<string, Variable>();
            _programCode = new List<IExpression>(MakeProgram(new List<IExpression>(), code));
            return null;
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
            if (_ip == _programCode.Count) return false;
            _programCode[_ip].Evaluate();
            _ip++;
            return true;
        }

        public static void ExitDebug()
        {
            _ip = _programCode.Count;
        }

        private static bool Interpret(string code, out ProgramError errorBuffer)
        {
            bool success;
            errorBuffer = Init(code, out success);
            if (!success)
                return false;

            _programCode.ForEach(x => x.Evaluate());
            return true;
        }

        public static ProgramError Start(string code, string action)
        {
            switch (action)
            {
                case "interpret":
                    return Interpret(code, out _programStartupError) ? null : _programStartupError;
                case "debug":
                    return StartDebug(code, out _programStartupError) ? null : _programStartupError;
                default:
                    return ProgramError.Empty();
            }
        }

        #endregion Main Program Management
    }
}