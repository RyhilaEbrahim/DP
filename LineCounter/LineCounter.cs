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

            var cleanedFileLines = GetCleanedFileLinesFrom(filePath);
            var validCodeFileLines = GetValidCodeLines(cleanedFileLines);

            return validCodeFileLines.Count();
        }

        private IEnumerable<string> GetCleanedFileLinesFrom(string filePath)
        {
            return _wrapper.ReadAllLines(filePath)
                .Select(line => line.Trim());
        }

        private static IEnumerable<string> GetValidCodeLines(IEnumerable<string> fileLines)
        {
            var validCodeLines = new List<string>();
            var inCommentBlock = false;

            foreach (var line in fileLines)
            {
                var isSingleCommentLine = line.StartsWith("//");
                if (string.IsNullOrEmpty(line) || isSingleCommentLine)
                {
                    continue;
                }

                validCodeLines = GetLinesExcludingCommentBlocks(line, ref inCommentBlock, validCodeLines);
            }

            return validCodeLines;
        }

        private static List<string> GetLinesExcludingCommentBlocks(string line, ref bool inCommentBlock, List<string> validCodeLines)
        {
            var blockStartsOnLineWithValidCode = false;
            if (line.StartsWith("/*"))
            {
                inCommentBlock = true;
            }
            else
            {
                if (line.Contains("/*"))
                {
                    inCommentBlock = true;
                    blockStartsOnLineWithValidCode = true;
                }
            }

            if (!inCommentBlock || blockStartsOnLineWithValidCode)
            {
                validCodeLines.Add(line);
            }

            if (line.EndsWith("*/"))
            {
                inCommentBlock = false;
            }
            else
            {
                if (line.Contains("*/"))
                {
                    inCommentBlock = false;
                    validCodeLines.Add(line);
                }
            }

            return validCodeLines;
        }
    }
}