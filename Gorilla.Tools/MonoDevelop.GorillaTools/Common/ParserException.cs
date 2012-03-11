using System;
using System.Collections.Generic;

namespace MonoDevelop.GorillaTools.Common
{
    public class ParserException : Exception
    {
        public List<ParserError> Errors { get; set; }

        public ParserException(ParserError error)
        {
            Errors = new List<ParserError> { error };
        }
    }
}
