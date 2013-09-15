using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Sprache.Playground
{
    [TestFixture]
    public partial class ParseTests
    {
        [Test]
        public void InterleaveWith_WithMultipleInterleaves_Parses()
        {
            var parser = from greeting in Parse.Literal("Hello")
                         from sep in Parse.Literal(",")
                         from subject in Parse.Literal("World")
                         from punc in Parse.Literal("!")
                         select new {greeting, sep, subject, punc};

            parser = parser
                .InterleaveWith(Parse.Literal(" "))
                .InterleaveWith(Parse.Literal("\r"))
                .InterleaveWith(Parse.Literal("\n"));

            var result = parser(@"  

            Hello

,
   World

     !


    ");

            result.HasValue.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var value = result.Value;

            value.greeting.Should().Be("Hello");
            value.sep.Should().Be(",");
            value.subject.Should().Be("World");
            value.punc.Should().Be("!");
        }

        [Test]
        public void Or_WithNoResults_UsesDescriptionToBuildErrorMessage()
        {
            var parser = Parse.Literal("public").Or(
                Parse.Literal("private")).DescribeAs("visibility modifier").Or(
                    Parse.Literal("class").DescribeAs("class keyword"));

            var result = parser("D");
            result.HasValue.Should().BeFalse();

            result.Errors.Count().Should().Be(1);
            var error = result.Errors.ElementAt(0);

            error.Message.Should().Be("Expected: visibility modifier or class keyword");
        }
    }
}
