using System;

namespace MonoDevelop.GorillaTools.Common
{
    public class ParserError
    {
        public String Message { get; set; }
        public UInt16 ForcedLine { get; set; }
        public UInt16 ForcedColumn { get; set; }

        public ParserError(string message, ushort line, ushort column)
        {
            Message = message;
            ForcedLine = line;
            ForcedColumn = column;
        }
    }
}
