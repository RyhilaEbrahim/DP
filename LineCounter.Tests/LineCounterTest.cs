using System;
using System.Collections.Generic;
using System.IO;
using LineCounter.Tests.TestDataBuilder;
using NUnit.Framework;

namespace LineCounter.Tests
{
    [TestFixture]
    public class LineCounterTest
    {
        [TestFixture]
        public class CountLinesOfCode
        {
            [TestCase("")]
            [TestCase(null)]
            public void GivenFilePathIsEmpty_ShouldThrowException(string filePath)
            {
                //Arrange
                var sut = new LineCounterTestBuilder().Build();
                //Act & //Assert
                Assert.Throws<InvalidOperationException>(() => sut.CountLinesOfCode(filePath), "Cannot read from an empty file");
            }

            [Test]
            public void GivenFileDoesNotExist_ShouldThrowException()
            {
                //Arrange
                var filePath = "test";
                var sut = new LineCounterTestBuilder()
                                .FileDoesNotExist(filePath)
                                .Build();
                //Act & //Assert
                Assert.Throws<FileNotFoundException>(() => sut.CountLinesOfCode(filePath), "File not found.");
            }

            [Test]
            public void GivenFileIsEmpty_ShouldReturnZero()
            {
                //Arrange
                var filePath = "test";
                const int expected = 0;
                var sut = new LineCounterTestBuilder()
                    .FileDoesExist(filePath)
                    .Build();
                //Act
                var result = sut.CountLinesOfCode(filePath);
                //Assert
                Assert.That(result,Is.EqualTo(expected));
            }

            [Test]
            public void GivenFileIsNotEmpty_ShouldReturnLineCount()
            {
                //Arrange
                var filePath = "test";
                const int expected = 5;
                var linesFromFile = new List<string>()
                {
                    "Line1",
                    "Line2",
                    "Line3",
                    "Line4",
                    "Line5"
                };
                var sut = new LineCounterTestBuilder()
                    .FileDoesExist(filePath)
                    .WithAllLinesFromFile(filePath,linesFromFile)
                    .Build();
                //Act
                var result = sut.CountLinesOfCode(filePath);
                //Assert
                Assert.That(result, Is.EqualTo(expected));
            }
        }
    }
}
