using System.Collections;
using System.Collections.Generic;
using NSubstitute;

namespace LineCounter.Tests.TestDataBuilder
{

    public class LineCounterTestBuilder 
    {
        private readonly IFileWrapper _fileWrapper;

        public LineCounterTestBuilder()
        {
            _fileWrapper = Substitute.For<IFileWrapper>();
        }

        public LineCounter Build()
        {
            return new LineCounter(_fileWrapper);
        }

        public LineCounterTestBuilder FileDoesNotExist(string filePath)
        {
            _fileWrapper.FileExists(filePath).Returns(false);
            return this;
        }

        public LineCounterTestBuilder FileDoesExist(string filePath)
        {
            _fileWrapper.FileExists(filePath).Returns(true);
            return this;
        }

        public LineCounterTestBuilder WithAllLinesFromFile(string filePath ,IEnumerable<string> returnedLines)
        {
            _fileWrapper.ReadAllLines(filePath).Returns(returnedLines);
            return this;
        }
    }
}