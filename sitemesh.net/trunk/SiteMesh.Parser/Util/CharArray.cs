using System;

namespace SiteMesh.Parser.Util
{
	/**
	 * A leaner, meaner version of StringBuffer.
	 * <p/>
	 * It provides basic functionality to handle dynamically-growing
	 * char arrays as quickly as possible. This class is not threadsafe.
	 * 
	 * @author Chris Miller
	 */

	public class CharArray
	{
		private int size = 0;
		private byte[] buffer;

		// These properties allow us to specify a substring within the character array
		// that we can perform comparisons against. This is here purely for performance -
		// the comparisons are at the heart of the FastPageParser loop and any speed increase
		// we can get at this level has a huge impact on performance.
		private int subStrStart = 0;
		private int subStrLen = 0;

		/**
		 * Constructs a CharArray that is initialized to the specified size.
		 *
		 * Do not pass in a negative value because there is no bounds checking!
		 */

		public CharArray(int size)
		{
			buffer = new byte[size];
		}

		/**
		 * Returns a String represenation of the character array.
		 */

		public override string ToString()
		{
			return Convert.ToBase64String(buffer, 0, size);
		}

		/**
		 * Returns the character that is at the specified position in the array.
		 *
		 * There is no bounds checking on this method so be sure to pass in a
		 * sensible value.
		 */

		public char CharAt(int pos)
		{
			return Convert.ToChar(buffer[pos]);
		}

		/**
		 * Changes the size of the character array to the value specified.
		 *
		 * If the new size is less than the current size, the data in the
		 * internal array will be truncated. If the new size is &lt;= 0,
		 * the array will be reset to empty (but, unlike StringBuffer, the
		 * internal array will NOT be shrunk). If the new size is &gt the
		 * current size, the array will be padded out with null characters
		 * (<tt>'&#92;u0000'</tt>).
		 *
		 * @param newSize the new size of the character array
		 */

		public void setLength(int newSize)
		{
			if (newSize < 0)
			{
				newSize = 0;
			}

			if (newSize <= size)
			{
				size = newSize;
			}
			else
			{
				if (newSize >= buffer.Length)
					Grow(newSize);
				// Pad the array
				for (; size < newSize; size++)
					buffer[size] = Convert.ToByte('\0');
			}
		}

		/**
		 * Returns the current length of the character array.
		 */

		public int Length
		{
			get { return size; }
		}

		/**
		 * Appends an existing CharArray on to this one.
		 *
		 * Passing in a <tt>null</tt> CharArray will result in a <tt>NullPointerException</tt>.
		 */

		public CharArray append(CharArray chars)
		{
			return append(chars.buffer, chars.size);
		}

		/**
		 * Appends the supplied characters to the end of the array.
		 */

		public CharArray append(byte[] chars)
		{
			return append(chars, chars.Length);
		}

		private CharArray append(byte[] chars, int length)
		{
			int requiredSize = length + size;
			if (requiredSize >= buffer.Length)
				Grow(requiredSize);
			Array.Copy(chars, 0, buffer, size, length);
			size = requiredSize;
			return this;
		}

		/**
		 * Appends a single character to the end of the character array.
		 */

		public CharArray append(char c)
		{
			if (buffer.Length == size)
				Grow(0);
			buffer[size++] = Convert.ToByte(c);
			return this;
		}

		/**
		 * Appends the supplied string to the end of this character array.
		 *
		 * Passing in a <tt>null</tt> string will result in a <tt>NullPointerException</tt>.
		 */

		public CharArray append(String str)
		{
			int requiredSize = str.Length + size;
			if (requiredSize >= buffer.Length)
				Grow(requiredSize);

			for (int i = 0; i < str.Length; i++)
				buffer[size + i] = Convert.ToByte(str[i]);

			size = requiredSize;
			return this;
		}

		/**
		 * Returns a substring from within this character array.
		 *
		 * Note that NO range checking is performed!
		 */

		public String substring(int begin, int end)
		{
			return Convert.ToBase64String(buffer, begin, end - begin);
		}

		/**
		 * Allows an arbitrary substring of this character array to be specified.
		 * This method should be called prior to calling {@link #compareLowerSubstr(String)}
		 * to set the range of the substring comparison.
		 *
		 * @param begin the starting offset into the character array.
		 * @param end the ending offset into the character array.
		 */

		public void setSubstr(int begin, int end)
		{
			subStrStart = begin;
			subStrLen = end - begin;
		}

		/**
		 * Returns the substring that was specified by the {@link #setSubstr(int, int)} call.
		 */

		public String getLowerSubstr()
		{
			const byte shifter = 32;
			for (int i = subStrStart; i < subStrStart + subStrLen; i++)
			{
				int val = buffer[i] | shifter;
				buffer[i] = (byte) val;
			}
			return Convert.ToBase64String(buffer, subStrStart, subStrLen);
		}

		/**
		 * This compares a substring of this character array (as specified
		 * by the {@link #setSubstr(int, int)} method call) with the supplied
		 * string. The supplied string <em>must</em> be lowercase, otherwise
		 * the comparison will fail.
		 */

		public bool compareLowerSubstr(String lowerStr)
		{
			// Range check
			if (lowerStr.Length != subStrLen || subStrLen <= 0)
				return false;

			for (int i = 0; i < lowerStr.Length; i++)
			{
				// | 32 converts from ASCII uppercase to ASCII lowercase
				if ((buffer[subStrStart + i] | 32) != lowerStr[i])
					return false;
			}
			return true;
		}

		/**
		 * Returns the hashcode for a <em>lowercase</em> version of the array's substring
		 * (as set by the {@link #setSubstr(int, int)} method).
		 *
		 * This uses the same calculation as the <tt>String.hashCode()</tt> method
		 * so that it remains compatible with the hashcodes of normal strings.
		 */

		public int substrHashCode()
		{
			int hash = 0;
			int offset = subStrStart;
			for (int i = 0; i < subStrLen; i++)
			{
				hash = 31*hash + (buffer[offset++] | 32);
			}
			return hash;
		}

		/**
		 * Compares the supplied uppercase string with the contents of
		 * the character array, starting at the offset specified.
		 *
		 * This is a specialized method to help speed up the FastPageParser
		 * slightly.
		 * <p/>
		 * The supplied string is assumed to contain only uppercase ASCII
		 * characters. The offset indicates the offset into the character
		 * array that the comparison should start from.
		 * <p/>
		 * If (and only if) the supplied string and the relevant portion of the
		 * character array are considered equal, this method will return <tt>true</tt>.
		 */

		public bool compareLower(String lowerStr, int offset)
		{
			// Range check
			if (offset < 0 || offset + lowerStr.Length > size)
				return false;

			for (int i = 0; i < lowerStr.Length; i++)
			{
				// | 32 converts from ASCII uppercase to ASCII lowercase
				if ((buffer[offset + i] | 32) != lowerStr[i])
					return false;
			}
			return true;
		}

		/**
		 * Grows the internal array by either ~100% or minSize (whichever is larger),
		 * up to a maximum size of Integer.MAX_VALUE.
		 */

		private void Grow(int minSize)
		{
			int newCapacity = (buffer.Length + 1)*2;
			if (newCapacity < 0)
			{
				newCapacity = int.MaxValue;
			}
			else if (minSize > newCapacity)
			{
				newCapacity = minSize;
			}
			byte[] newBuffer = new byte[newCapacity];
			Array.Copy(buffer, 0, newBuffer, 0, size);
			buffer = newBuffer;
		}

		public void clear()
		{
			size = 0;
		}
	}
}