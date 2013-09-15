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
    public class PositionTests
    {
        [Test]
        public void DefaultValue_HasIndex0Line1Column1()
        {
            var pos = default(Position);

            pos.Index.Should().Be(0);
            pos.Line.Should().Be(1);
            pos.Column.Should().Be(1);
        }

        [Test]
        public void NegativeIndex_ThrowsException()
        {
            Action<int> act = index => new Position(index, 1, 1);

            foreach (var index in Enumerable.Range(-10, 10))
            {
                int idx = index;
                act.Invoking(a => a(idx)).ShouldThrow<ArgumentOutOfRangeException>();
            }
        }

        [Test]
        public void Line_WhenZeroOrLessThan_ThrowsException()
        {
            Action<int> act = line => new Position(0, line, 1);

            foreach (var line in Enumerable.Range(-10, 11))
            {
                int l = line;
                act.Invoking(a => a(l)).ShouldThrow<ArgumentOutOfRangeException>();
            }
        }

        [Test]
        public void Column_WhenZeroOrLessThan_ThrowsException()
        {
            Action<int> act = column => new Position(0, 1, column);

            foreach (var column in Enumerable.Range(-10, 11))
            {
                int c = column;
                act.Invoking(a => a(c)).ShouldThrow<ArgumentOutOfRangeException>();
            }
        }

        [Test]
        public void ProvidedValuesAreReturned()
        {
            var pos = new Position(45, 13, 4);

            pos.Index.Should().Be(45);
            pos.Line.Should().Be(13);
            pos.Column.Should().Be(4);
        }
    }
}
