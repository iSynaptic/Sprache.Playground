using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprache.Playground
{
    public struct ResultInfo<T>
    {
        public ResultInfo(T value, string description, PositionRange position)
        {
            Value = value;
            Description = description;
            Position = position;
        }

        public T Value { get; }
        public string Description { get; }
        public PositionRange Position { get; }
    }
}
