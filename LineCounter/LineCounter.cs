using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LineCounter
{
    public class LineCounter
    {
        private readonly IFileWrapper _wrapper;

        public LineCounter(IFileWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        public int CountLinesOfCode(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("Cannot read from an empty file");
            }

            if (!_wrapper.FileExists(filePath))
            {
                throw new FileNotFoundException("File not found.");
            }

            var fileLines = _wrapper.ReadAllLines(filePath)
                .Select(x => x.TrimStart())
                .Where(x => x != string.Empty && !x.StartsWith("//"));

            var finalFileLines = new List<string>();
            var inCommentBlock = false;

            foreach (var fileLine in fileLines)
            {
                var blockStartsOnLineWithValidCode = false;
                if (fileLine.StartsWith("/*"))
                {
                    inCommentBlock = true;
                }
                else
                {
                    if (fileLine.Contains("/*"))
                    {
                        inCommentBlock = true;
                        blockStartsOnLineWithValidCode = true;
                    }
                }
                
                if (!inCommentBlock || blockStartsOnLineWithValidCode)
                {
                    finalFileLines.Add(fileLine);
                }

                if (fileLine.EndsWith("*/"))
                {
                    inCommentBlock = false;
                }
            }

            return finalFileLines.Count();
        }
    }
}