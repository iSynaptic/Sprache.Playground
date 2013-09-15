using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprache.Playground
{
    public static class Hashing
    {
        public static int MixJenkins32(Int32 input)
        {
            Int32 output = input;

            output += (output << 12);
            output ^= (output >> 22);
            output += (output << 4);
            output ^= (output >> 9);
            output += (output << 10);
            output ^= (output >> 2);
            output += (output << 7);
            output ^= (output >> 12);

            return output;
        }
    }
}
