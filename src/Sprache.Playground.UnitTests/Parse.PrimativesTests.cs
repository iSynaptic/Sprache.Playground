using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using NUnit.Framework;

namespace Sprache.Playground
{
    [TestFixture]
    public partial class ParseTests
    {
        [Test]
        public void Literal_WithEmptyInput()
        {
            var parser = Parse.Literal("Hello");

            var result = parser("");

            result.HasValue.Should().BeFalse();
            result.Description.Should().Be("literal");
            result.Errors.Should().HaveCount(1);
            result.Context.ConsumedTo.Should().Be(Position.Beginning);
            result.Context.ReadTo.Should().Be(Position.Beginning);

            var error = result.Errors.First();
            error.Position.Should().Be(Position.Beginning);
            error.Message.Should().Be("Unexpected: end of input");
        }

        [Test]
        public void Literal_WithParialMatch()
        {
            var parser = Parse.Literal("Hello");

            var result = parser("Help");

            result.HasValue.Should().BeFalse();
            result.Description.Should().Be("literal");
            result.Errors.Should().HaveCount(1);
            result.Context.ConsumedTo.Should().Be(Position.Beginning);
            result.Context.ReadTo.Should().Be(new Position(3, 1, 4));

            var error = result.Errors.First();
            error.Position.Should().Be(new Position(3, 1, 4));
            error.Message.Should().Be("Unexpected: character 'p'");
        }

        [Test]
        public void Literal_WithUnexpectedEndOfInput()
        {
            var parser = Parse.Literal("Hello, World!");

            var result = parser("Hello");

            result.HasValue.Should().BeFalse();
            result.Description.Should().Be("literal");
            result.Errors.Should().HaveCount(1);
            result.Context.ConsumedTo.Should().Be(Position.Beginning);
            result.Context.ReadTo.Should().Be(new Position(5, 1, 6));

            var error = result.Errors.First();
            error.Position.Should().Be(new Position(5, 1, 6));
            error.Message.Should().Be("Unexpected: end of input");
        }

        [Test]
        public void Literal_WithFullMatch()
        {
            var parser = Parse.Literal("Hello");

            var result = parser("Hello, World!");

            result.Value.Should().Be("Hello");
            result.HasValue.Should().BeTrue();
            result.Description.Should().Be("literal");
            result.Errors.Should().BeEmpty();
            result.Context.ConsumedTo.Should().Be(new Position(5, 1, 6));
            result.Context.ReadTo.Should().Be(new Position(5, 1, 6));
        }
    }
}
