using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using MonoDevelop.Core;
using MonoDevelop.GorillaTools.Common;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

namespace MonoDevelop.GorillaTools.Generators
{
    public class EcsFileGenerator : SingleFileGenerator
    {
        public static Regex Tag = new Regex("<%(=|!)?(.*)%>");
        public static Regex Endline = new Regex("[\r]?\n");

        readonly List<int> _endlines = new List<int>();
        readonly List<EcsTag> _tags = new List<EcsTag>();

        public override FilePath Generate(ProjectFile file)
        {
            string baseDir = file.Project.BaseDirectory;
            string @namespace = String.Join(" ; ", file.ExtendedProperties);
            string classname = file.Name.Substring(baseDir.Length + 1);
            classname = classname.Substring(0, classname.IndexOf('.')).Replace('\\', '_');
            //file.Project;

            CodeDomProvider codeProvider = new CSharpCodeProvider();
            var code = new StringBuilder();

            _tags.Clear();
            _endlines.Clear();

            string input = File.ReadAllText(file.FilePath);
            
            code.AppendLine("using System;");
            code.AppendLine("using System.Text;");
            code.AppendLine("using Gibbon.Http;");

            if (input.StartsWith("using "))
            {
                int index = input.IndexOf('\n');
                string[] usings = input.Substring(6, index - 6).Split(',');
                input = input.Substring(index + 1);

                foreach (var @using in usings)
                {
                    var _using = @using.Trim();
                    if (!(_using == "System" || _using == "System.Text"))
                        code.AppendLine("using " + _using + ";");
                }
            }
            
            code.AppendLine();

            code.AppendLine("/*");

            foreach (var item in file.Project.GetConfigurations())
            {
                code.AppendLine(item);
            }

            code.AppendLine("*/");

            code.AppendLine("namespace Gorilla.Rendered.Ecs //");
            code.AppendLine("{");
            code.AppendLine("\tpublic class " + classname);
            code.AppendLine("\t{");
            code.AppendLine("\t\tpublic static string Render(Dictionary<string, object> @params)");
            code.AppendLine("\t\t{");
            code.AppendLine("\t\t\t var ___output = new StringBuilder();");

            MatchCollection matches = Endline.Matches(input);
            foreach (Match match in matches)
                if (match.Success)
                    _endlines.Add(match.Index);

            matches = Tag.Matches(input);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    bool flag = match.Value.StartsWith("<%=");
                    _tags.Add(new EcsTag
                                  {
                                      Start = match.Index,
                                      End = match.Length + match.Index,
                                      Value =
                                          (flag
                                               ? match.Value.Substring(3, match.Length - 5)
                                               : match.Value.Substring(2, match.Length - 4)),
                                      Output = flag
                                  });
                }
            }

            int offset = 0;
            foreach (var tag in _tags)
            {
                code.AppendLine("\t\t\t___output.Append(\"" +
                            input.Substring(offset, tag.Start - offset).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t","\\t") +
                            "\");");

                if (tag.Output)
                {
                    tag.CodeStart = code.Length + "\t___output.Append((".Length;
                    code.Append("\t\t\t___output.Append((" + tag.Value + ").ToString());");
                    tag.CodeEnd = code.Length - ").ToString());".Length;

                    code.Append("//tag[" + tag.Start + ", " + tag.End + "]->[" + tag.CodeStart + ", " + tag.CodeEnd + "]\r\n");
                }
                else
                {
                    tag.CodeStart = code.Length;
                    code.Append(tag.Value + ";");
                    tag.CodeEnd = code.Length - 1;

                    code.Append("//tag[" + tag.Start + ", " + tag.End + "]->[" + tag.CodeStart + ", " + tag.CodeEnd + "]\r\n");
                }

                offset = tag.End;
            }

            code.AppendLine("\t\t\t___output.Append(\"" +
                            input.Substring(offset, input.Length - offset).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") +
                            "\");");

            code.AppendLine("\t\t\t return ___output.ToString();");

            code.Append("\t\t}\r\n\t}\r\n}");
            
            FilePath output = file.FilePath.ChangeExtension(".ecs." + codeProvider.FileExtension);
            File.WriteAllText(output.FullPath, code.ToString());

            return output;
        }

        public override void HandleException(Exception ex, ProjectFile file, SingleFileCustomToolResult result)
        {
            if (ex is ParserException)
            {
                var sfpex = (ParserException)ex;

                if (sfpex.Errors == null || sfpex.Errors.Count == 0)
                {
                    result.UnhandledException = ex;
                }
                else
                {
                    var compilerErrors = new CompilerErrorCollection();

                    foreach (var error in sfpex.Errors)
                    {
                        var compilerError = new CompilerError(file.Name, error.ForcedLine, error.ForcedColumn, "0", error.Message);
                        compilerErrors.Add(compilerError);
                    }

                    result.Errors.AddRange(compilerErrors);
                }
            }
            else
            {
                result.UnhandledException = ex;
            }
        }

    }

    public class EcsTag
    {
        public int Start;
        public int End;

        public int CodeStart;
        public int CodeEnd;

        public String Value;
        public bool Output;
    }
}
