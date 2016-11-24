using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Multinet.Utils
{

    /// <summary>
    /// This class contains a set of helper methods to array of bits manipulation.
    /// </summary>
    public sealed class BitArrayUtils
    {
        /// <summary>
        /// This method get a real number in range [0.0, 1.0]. The real number
        /// is stored in variable <code>value</code>. The variable  <code>value</code> is 
        /// of type double, but it is assumed that the value stored in X is in the range [0,1].
        /// The actual value provided is stored as an unsigned 32-bit integer.
        /// </summary>
        /// <param name="value">Normalized real value to be converted in a bit array.</param>
        /// <returns>A 32-bit array that corresponds to the actual number in <code>value</code></returns>
        public static BitArray ToBitArray(double value)
        {
            uint lvalue = (uint)(value *  UInt32.MaxValue);
            return ToBitArray(lvalue);
        }

        /// <summary>
        /// This method get an unsigned int in range [0, MAX_VALUE]. The unsigned value
        /// is stored in variable <code>value</code>. The variable  <code>value</code> is 
        /// of type <code>uint</code>.
        /// The actual value provided is stored as an unsigned 32-bit integer.
        /// </summary>
        /// <param name="value">Value to be stored.</param>
        /// <returns>Array of bits that corresponds to value in variable <code>value</code>.</returns>
        public static BitArray ToBitArray(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return new BitArray(bytes);
        }

        /// <summary>
        /// This method get an unsigned long in range [0, MAX_VALUE]. The unsigned value
        /// is stored in variable <code>value</code>. The variable  <code>value</code> is 
        /// of type <code>ulong</code>.
        /// The actual value provided is stored as an unsigned 64-bit integer.
        /// </summary>
        /// <param name="value">Value to be stored.</param>
        /// <returns>Array of bits that corresponds to value in variable <code>value</code>.</returns>
        public static BitArray ToBitArray(ulong lvalue)
        {
            byte[] bytes = BitConverter.GetBytes(lvalue);

            return new BitArray(bytes);
        }
        
        /// <summary>
        /// Convert a bit array in a double value with 32 bits of precision.
        /// </summary>
        /// <param name="bitarray">Array of bits to be converted.</param>
        /// <returns>Normalized real value found. Returned value is in range [0.0, 1.0].
        /// Value 0.0 corresponds to unsigned integer value 0; value 1.0, corresponds 
        /// to unsigned value MAX_UINT_VALUE, MAX_UINT_VALUE is a maximum value of variable
        /// of type uint.</returns>
        public static double ToNDouble(BitArray bitarray)
        {
            double sum = 0.0;
            for (int i = 0; i < bitarray.Length; i++)
            {
                int bit = (bitarray.Get(i) ? 1 : 0);
                sum += System.Math.Pow(2, i) * bit;
            }
            return sum / UInt32.MaxValue;
        }

        public static uint ToUInt32(BitArray bitarray)
        {
            long sum = 0;
            for (int i = 0; i < bitarray.Length; i++)
            {
                int bit = (bitarray.Get(i) ? 1 : 0);
                sum += (long)System.Math.Pow(2, i) * bit;
            }
            return (uint)sum;
        }

        public static BitArray DeepClone(BitArray bitarray)
        {
            BitArray copy = new BitArray(bitarray.Length);
            for (int i = 0; i < bitarray.Length; i++)
            {
                copy.Set(i, bitarray[i]);
            }
            return copy;
        }

    }
}
