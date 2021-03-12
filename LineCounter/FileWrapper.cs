using System.Collections.Generic;
using System.IO;

namespace LineCounter
{
    public class FileWrapper : IFileWrapper
    {
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public IEnumerable<string> ReadAllLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }
    }
}