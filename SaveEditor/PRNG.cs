using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor
{
    public static class PRNG
    {
        private const uint factor = 0x41C64E6D;
        private const uint adder = 0x3039;
        public static uint rng_seed = 0;

        /// <summary>
        /// sub_55BC20()
        /// </summary>
        /// <param name="value"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static long sub_55BC20(uint value, ref uint seed)
        {
            long result;

            if ( value < 2 ) return 0;

            var temp = rng_seed;

            if ( seed > 0 )
            {
                seed = factor * seed + adder;
                rng_seed = temp;
                result = (seed >> 16) - (seed >> 16) / value * value;
            }
            else
            {
                rng_seed = factor * rng_seed + adder;
                result = (rng_seed >> 16) - (rng_seed >> 16) / value * value;
            }

            return result;
        }

    }
}
