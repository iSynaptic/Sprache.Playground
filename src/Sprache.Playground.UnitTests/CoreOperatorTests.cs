using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Sprache.Playground.UnitTests
{
    [TestFixture]
    public class CoreOperatorTests
    {
        [Test]
        public void InterleaveWith_WithMultipleInterleaves_Parses()
        {
            var parser = from greeting in Parse.String("Hello")
                from sep in Parse.String(",")
                from subject in Parse.String("World")
                from punc in Parse.String("!")
                select new {greeting, sep, subject, punc};

            parser = parser
                .InterleaveWith(Parse.String(" "))
                .InterleaveWith(Parse.String("\r"))
                .InterleaveWith(Parse.String("\n"));

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
            var parser = Parse.String("public").Or(
                         Parse.String("private")).DescribeAs("visibility modifier").Or(
                         Parse.String("class").DescribeAs("class keyword"));

            var result = parser("D");
            result.HasValue.Should().BeFalse();

            result.Errors.Count().Should().Be(1);
            var error = result.Errors.ElementAt(0);

            error.Message.Should().Be("Expected: visibility modifier or class keyword");
        }
    }
}
