using System;
using FluentAssertions;
using NUnit.Framework;

namespace Sprache.Playground.UnitTests
{
    [TestFixture]
    public class ContextTests
    {
        [Test]
        public void Context_WithZeroLengthInput_ReturnsExpectedValues()
        {
            string input = "";
            var ctx = new Context(input);

            ctx.Input.Should().Be(input);
            ctx.Position.Should().Be(new Position(0, 1, 1));
            ctx.Name.Should().Be(String.Empty);
            ctx.AtEnd.Should().BeTrue();
            ctx.Interleaving.Should().BeNull();
        }

        [Test]
        public void Context_WithNonZeroLengthInput_ReturnsExpectedValues()
        {
            string input = "Hello, World!";
            var ctx = new Context(input);

            ctx.Input.Should().Be(input);
            ctx.Position.Should().Be(new Position(0, 1, 1));
            ctx.Current.Should().Be('H');
            ctx.Name.Should().Be(String.Empty);
            ctx.AtEnd.Should().BeFalse();
            ctx.Interleaving.Should().BeNull();
        }

        [Test]
        public void Context_WithNullInput_ThrowsException()
        {
            Action act = () => new Context(null);

            act.Invoking(a => a()).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void Advance_ReturnsContext_WithNewPositionAndCurrent()
        {
            string input = "Hello, World!";
            var ctx = new Context(input);
            
            ctx = ctx.Advance();

            ctx.Position.Should().Be(new Position(1, 1, 2));
            ctx.Current.Should().Be('e');
        }

        [Test]
        public void AdvanceWithCount_ReturnsContext_WithNewPositionAndCurrent()
        {
            string input = "Hello, World!";
            var ctx = new Context(input);

            ctx = ctx.Advance(7);

            ctx.Position.Should().Be(new Position(7, 1, 8));
            ctx.Current.Should().Be('W');
        }

        [Test]
        public void Advance_WhenBuildingNewContext_RespectsNewLines()
        {
            string input = "Hello\r\nWorld!";
            var ctx = new Context(input).Advance(6);

            ctx.Position.Should().Be(new Position(6, 1, 7));

            ctx = ctx.Advance();

            ctx.Position.Should().Be(new Position(7, 2, 1));
        }

        [Test]
        public void AdvanceWithCount_WhenBuildingNewContext_RespectsNewLines()
        {
            string input = "Hello\r\nWorld!";
            var ctx = new Context(input);
            
            ctx = ctx.Advance(7);

            ctx.Position.Should().Be(new Position(7, 2, 1));
        }

        [Test]
        public void Advancing_ByNegativeCount_ThrowsException()
        {
            var ctx = new Context("Hello");
            ctx.Invoking(c => c.Advance(-2)).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Advancing_ByZero_ReturnsUnchangedContext()
        {
            var ctx = new Context("Hello");

            var newCtx = ctx.Advance(0);

            newCtx.Position.Should().Be(ctx.Position);
            newCtx.Input.Should().Be(ctx.Input);
            newCtx.Current.Should().Be(ctx.Current);
            newCtx.Name.Should().Be(ctx.Name);
            newCtx.AtEnd.Should().Be(ctx.AtEnd);
        }

        [Test]
        public void Advance_WhenAtEnd_ThrowsException()
        {
            var ctx = new Context("");

            ctx.Invoking(c => c.Advance()).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Advance_PastEndOfInput_ThrowsException()
        {
            var ctx = new Context("Hello");

            ctx.Invoking(c => c.Advance(10)).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WithInterleave_ReturnsContext_WithInterleaving()
        {
            var ctx = new Context("Hello");

            Parser<Object> interleaving = c => Result.WithValue<object>(42, "ultimate", c);
            ctx = ctx.WithInterleave(interleaving);

            ctx.Interleaving.Should().Be(interleaving);
        }

        [Test]
        public void WithInterleave_ReturnsContext_WithOtherValuesUnchanged()
        {
            var ctx = new Context("Hello").Advance(3);

            Parser<Object> interleaving = c => Result.WithValue<object>(42, "ultimate", c);
            var newCtx = ctx.WithInterleave(interleaving);

            newCtx.Input.Should().Be(ctx.Input);
            newCtx.Position.Should().Be(ctx.Position);
            newCtx.Current.Should().Be(ctx.Current);
            newCtx.Name.Should().Be(ctx.Name);
            newCtx.AtEnd.Should().Be(ctx.AtEnd);

            newCtx.Interleaving.Should().Be(interleaving);
        }

        [Test]
        public void ImplicitConvertion_FromStringToContext_Exists()
        {
            string input = "Hello";
            Context ctx = input;

            ctx.Input.Should().Be(input);
            ctx.Position.Should().Be(new Position(0, 1, 1));
            ctx.Current.Should().Be('H');
            ctx.Name.Should().Be(String.Empty);
            ctx.AtEnd.Should().BeFalse();
            ctx.Interleaving.Should().BeNull();

        }
    }
}
