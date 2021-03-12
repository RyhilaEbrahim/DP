using System;
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

            var fileLines = _wrapper.ReadAllLines(filePath);
            return fileLines.Count();
        }
    }
}