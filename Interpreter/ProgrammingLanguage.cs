using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Interpreter
{
    class Interpreter
    {
        public interface IToken { }

        #region Helpers

        private static Variable GetMethodCallResultOrVariable(string source)
        {
            source = source.Replace(";", "");
            if (!source.Contains("("))
                return Variable.Get(source);

            var methodNameAndArgs = ExtractMethodNameAndArguments(source);
            var tCall = new MethodCall(methodNameAndArgs.Item1, methodNameAndArgs.Item2);
            return Variable.Get(tCall.ReturnVariable.ToString());

        }

        private static string Substring(string str, int startIndex, int endIndex)
                    => str.Substring(startIndex, endIndex - startIndex + 1);

        private static IEnumerable<int> FindAll(string str, char subStr)
        {
            for (var i = 0; i < str.Length; i++)
                if (str[i] == subStr)
                    yield return i;
        }

        private static Tuple<string, IEnumerable<string>> ExtractMethodNameAndArguments(string signature)
        {
            //Extract method name
            var iEnd = signature.IndexOf("(", StringComparison.Ordinal);
            var iStart = signature.LastIndexOf(" ", iEnd, StringComparison.Ordinal) + 1;
            iStart = iStart == -1 ? 0 : iStart;
            var methodName = signature.Substring(iStart, iEnd - iStart);

            //Extract arguments
            iStart = iEnd + 1; //inc to mark starting position of first argument
            iEnd = signature.IndexOf(")", iStart, StringComparison.Ordinal);
            var argumentNames = signature.Substring(iStart, iEnd - iStart);
            var delimiterIndexes = FindAll(argumentNames, ',').ToArray();
            var argNameList = new List<string>();

            //check if there's only one argument, else harvest them all
            if (iStart != iEnd && argumentNames.Length > 0)
                return new Tuple<string, IEnumerable<string>>(methodName, new[] { argumentNames });

            for (var i = 0; i < delimiterIndexes.Count() - 1; i++)
                argNameList.Add(i == 0
                    ? Substring(argumentNames, 0, delimiterIndexes[i] - 1)
                    : Substring(argumentNames, delimiterIndexes[i] + 1, delimiterIndexes[i + 1] - 1));

            return new Tuple<string, IEnumerable<string>>(methodName, argNameList);
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


        #endregion
        
        #region Variables

        public class Variable : IToken
        {
            //TODO: A variable has no name unless it's kept within ProgramVariables

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
        }

        public class Number : Variable
        {
            public Number(string name, int value) : base(name, value) { }
            public Number(int value) : base(value) { }
        }

        public class Decimal : Variable
        {
            public Decimal(string name, double value) : base(name, value) { }
            public Decimal(double value) : base(value) { }
        }

        public class Text : Variable
        {
            public Text(string name, string value) : base(name, value) { }
            public Text(string value) : base(value) { }
        }

        #endregion

        #region Expressions

        public interface IExpression : IToken
        {
            void Execute();
        }
        
        public class ReturnStmt : IExpression
        {
            public Variable ReturnValue;
            
            public ReturnStmt(Variable val)
            {
                ReturnValue = val;
            }

            public void Execute() =>
                ProgramReturnVariable = ReturnValue;
            
        }

        public class Method : Variable, IExpression
        {
            public string[] Parameters { get; }
            public IEnumerable<IExpression> Body { get; }

            public Method(string name, string[] parameters, IEnumerable<IExpression> body) : base(name, null)
            {
                Parameters = parameters;
                Body = body;
            }

            public void Execute() =>
                ProgramMethods.Add(Name, this);
            

            public void Call(Variable[] args, out Variable returnVariable)
            {
                //Load arguments into program
                for(var i = 0; i < Parameters.Length; i++)
                    ProgramVariables.Add(Parameters[i], args[i]);

                //Execute function
                returnVariable = null;
                foreach (var expression in Body)
                {
                    expression.Execute();
                    if (expression is ReturnStmt)
                        returnVariable = ProgramReturnVariable;
                        break;
                }
                
                //Remove arguments from program
                Parameters.ToList().ForEach(x => ProgramVariables.Remove(x));
            }
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

            public void Execute() => ProgramMethods[TargetMethod].Call(Arguments.Select(Variable.Get).ToArray(), out ReturnVariable);
        }

        public class VariableDeclaration : IExpression
        {
            public Variable LocalVariable;

            public VariableDeclaration(string name, string value)
            {
                LocalVariable = Variable.CreateFromVariable(name, GetMethodCallResultOrVariable(value));
            }
            
            public void Execute() => ProgramVariables.Add(LocalVariable.Name, LocalVariable);
            
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

            public void Execute() => ProgramVariables[TargetName].Value = GetMethodCallResultOrVariable(SourceName).Value;
            
        }

        #endregion

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
         * Method Declaration:
         * fun <function_name> ( <parameter_name> , ... ) { var x = 5; }
         * 
         */

        #region Tokenizer Methods

        public static VariableDeclaration MakeVarDecl(string[] splittedExpression) =>
            new VariableDeclaration(splittedExpression[1], splittedExpression[3].Replace(";", ""));
        

        public static VariableAssignment MakeVarAss(string[] splittedExpression) =>
            new VariableAssignment(splittedExpression[0], splittedExpression[2]);
        

        public static Method MakeMethodDecl(string signature, string body)
        {
            var methodInfo = ExtractMethodNameAndArguments(signature);
            var functionBody = body.Substring(body.IndexOf("{", StringComparison.Ordinal) + 1, body.LastIndexOf("}", StringComparison.Ordinal) - 1); //dec & inc to avoid brackets
            return new Method(methodInfo.Item1, methodInfo.Item2.ToArray(), MakeProgram(new List<IExpression>(), functionBody));
        }

        public static MethodCall MakeMethodCall(string expression)
        {
            var methodInfo = ExtractMethodNameAndArguments(expression);
            return new MethodCall(methodInfo.Item1, methodInfo.Item2);
        }

        public static IEnumerable<IExpression> MakeProgram(List<IExpression> programCode, string programText)
        {
            //Debugging purposes
            if(programCode.Count == 2)
                System.Threading.Thread.Sleep(1);

            var splittedProgramText = programText.Split('\n');
            var curLine = splittedProgramText[0];
            if(curLine == "")
                if (splittedProgramText.Count() >= 2)
                {
                    programText = programText.Remove(0, 1);
                    curLine = splittedProgramText[1];
                }
                else
                    return programCode;

            var curLineSplitted = curLine.Split(' ').Where(x => x != "").ToArray();
            if (!curLineSplitted.Any()) curLineSplitted = null;
            
            //Do the parser work
            if (curLineSplitted?[0] == "sub")
            {
                //Extract function body
                var iStart = programText.IndexOf("{", StringComparison.Ordinal);
                var iEnd = programText.IndexOf("}", iStart, StringComparison.Ordinal) + 1; //inc to wrap around when taking substring
                var functionBody = programText.Substring(iStart, iEnd - iStart);

                //Update programCode parameter for next recursive call
                return MakeProgram(AddExpressionToList(programCode, MakeMethodDecl(curLine, functionBody)), programText.Remove(0, iEnd));
            }
            if (curLineSplitted?[0] == "var")
                return MakeProgram(AddExpressionToList(programCode, MakeVarDecl(curLineSplitted)), BeheadString(programText));
            if(curLine.Contains("(") && curLine.Contains(")"))
                return MakeProgram(AddExpressionToList(programCode, MakeMethodCall(curLine)), BeheadString(programText));
            if (curLineSplitted?[1] == "=")
                return MakeProgram(AddExpressionToList(programCode, MakeVarAss(curLineSplitted)), BeheadString(programText));

            throw new NotSupportedException($"Unhandled line of code: {curLine}");
        }

#endregion

        #region Main Program Management

        public static string InputCode;
        public static Variable ProgramReturnVariable;
        public static Dictionary<string, Variable> ProgramVariables;
        public static Dictionary<string, Method> ProgramMethods;
        public static List<IExpression> ProgramCode;
        
        private static void Init(string code)
        {
            InputCode = code;
            ProgramMethods = new Dictionary<string, Method>();
            ProgramVariables = new Dictionary<string, Variable>();
            ProgramCode = new List<IExpression>(MakeProgram(new List<IExpression>(), code));
        }

        private static int _ip;
        public static void StartDebug(string code)
        {
            Init(code);
            _ip = 0;
        }

        /// <summary>
        /// Executes the next expression in code.
        /// </summary>
        /// <returns>Returns a tuple containing with a bool containing whether execution was done and the text </returns>
        public static bool StepDebug()
        {
            if (_ip == ProgramCode.Count) return false;
            ProgramCode[_ip].Execute();
            _ip++;
            return true;
        }

        public static void ExitDebug()
        {
            _ip = ProgramCode.Count;
        }

        public static void Interpret(string code)
        {
            Init(code);
            foreach (var expression in ProgramCode)
                expression.Execute();
        }

        #endregion
    }
}