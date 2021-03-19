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
                Assert.That(result, Is.EqualTo(expected));
            }

            [TestFixture]
            public class GivenFileIsNotEmpty
            {
                [TestFixture]
                public class AndHasNoBlankLines
                {
                    [Test]
                    public void ShouldReturnLineCount()
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
                            .WithAllLinesFromFile(filePath, linesFromFile)
                            .Build();
                        //Act
                        var result = sut.CountLinesOfCode(filePath);
                        //Assert
                        Assert.That(result, Is.EqualTo(expected));
                    }
                }

                [Test]
                public void ShouldExcludeBlankLinesAndReturnLineCount()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 6;
                    var linesFromFile = new List<string>()
                        {
                            "Line1",
                             string.Empty,
                            "Line3",
                            "Line4",
                            "Line5",
                            "Line6",
                            "Line7"
                        };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }
                
                [Test]
                public void ShouldExcludeLinesThatAreContainedWithinABlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 4;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "/* First line of comment",
                        "Second line of comment",
                        "last line of comment */",
                        "Line5",
                        "Line6",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }     
                
                [Test]
                public void ShouldExcludeAllLinesThatAreContainedWithinABlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 5;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "/* First line of comment",
                        "Second line of comment",
                        "last line of comment */",
                        "Line5",
                        "Line6",
                        "/* First line of comment",
                        "last line of comment */",
                        "Line9",
                        "Line10"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }
                
                [Test]
                public void ShouldIncludeLinesThatContainValidText_AndADoubleForwardSlashComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 7;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "Some text // Some comment",
                        "Line3",
                        "Some other text //Some other comment",
                        "Line5",
                        "Line6",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }
                
                [Test]
                public void ShouldIncludeLinesThatContainValidText_AndACodeBlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 5;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "Some text /* First line of comment",
                        "Second line of comment",
                        "Last line of comment */",
                        "Line5",
                        "Line6",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }

                [Test]
                public void ShouldIncludeAllLinesThatHaveValidTextBeforeStartOfBlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 4;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "Some text /* First line of comment",
                        "Second line of comment",
                        "Last line of comment */",
                        "Some text again /* Some valid comment",
                        "Second line of valid comment",
                        "Last line of valid comment */",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }

                [Test]
                public void ShouldIncludeLinesThatContainValidText_AndIsAtEndOfCodeBlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 5;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "/* First line of comment",
                        "Second line of comment",
                        "Last line of comment */ Some valid code",
                        "Line5",
                        "Line6",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }


                [Test]
                public void ShouldExcludeLinesThatContainEmptySpace_AndIsAtEndOfCodeBlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 4;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "/* First line of comment",
                        "Second line of comment",
                        "Last line of comment */ ",
                        "Line5",
                        "Line6",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }

                [Test]
                public void ShouldExcludeLineThatIsOnlyABlockComment()
                {
                    //Arrange
                    var filePath = "test";
                    const int expected = 6;
                    var linesFromFile = new List<string>()
                    {
                        "Line1",
                        "/* First line of comment*/",
                        "Line3",
                        "Line4",
                        "Line5",
                        "Line6",
                        "Line7"
                    };
                    var sut = new LineCounterTestBuilder()
                        .FileDoesExist(filePath)
                        .WithAllLinesFromFile(filePath, linesFromFile)
                        .Build();
                    //Act
                    var result = sut.CountLinesOfCode(filePath);
                    //Assert
                    Assert.That(result, Is.EqualTo(expected));
                }

                [TestFixture]
                public class AndContainsASingleLineComment
                {
                    [TestCase("// */some text")]
                    [TestCase("// /* some other text")]
                    [TestCase("// some other text")]
                    [TestCase(" // some random text")]
                    public void ShouldExcludeLine(string singleLineComment)
                    {
                        //Arrange
                        var filePath = "test";
                        const int expected = 6;
                        var linesFromFile = new List<string>()
                        {
                            "Line1",
                            singleLineComment,
                            "Line3",
                            "Line4",
                            "Line5",
                            "Line6",
                            "Line7"
                        };
                        var sut = new LineCounterTestBuilder()
                            .FileDoesExist(filePath)
                            .WithAllLinesFromFile(filePath, linesFromFile)
                            .Build();
                        //Act
                        var result = sut.CountLinesOfCode(filePath);
                        //Assert
                        Assert.That(result, Is.EqualTo(expected));
                    }
                }
               
            }
        }
    }
}
