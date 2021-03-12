using System.Collections;
using System.Collections.Generic;

namespace LineCounter
{
    public interface IFileWrapper
    {
        bool FileExists(string filePath);

        IEnumerable<string> ReadAllLines(string filePath);
    }
}