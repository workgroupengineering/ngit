/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Linq;
using System.Text;
using NGit;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	/// <summary>Utility functions related to quoted string handling.</summary>
	/// <remarks>Utility functions related to quoted string handling.</remarks>
	public abstract class QuotedString
	{
		/// <summary>Quoting style that obeys the rules Git applies to file names</summary>
		public static readonly QuotedString.GitPathStyle GIT_PATH = new QuotedString.GitPathStyle
			();

		/// <summary>Quoting style used by the Bourne shell.</summary>
		/// <remarks>
		/// Quoting style used by the Bourne shell.
		/// <p>
		/// Quotes are unconditionally inserted during
		/// <see cref="Quote(string)">Quote(string)</see>
		/// . This
		/// protects shell meta-characters like <code>$</code> or <code>~</code> from
		/// being recognized as special.
		/// </remarks>
		public static readonly QuotedString.BourneStyle BOURNE = new QuotedString.BourneStyle
			();

		/// <summary>Bourne style, but permits <code>~user</code> at the start of the string.
		/// 	</summary>
		/// <remarks>Bourne style, but permits <code>~user</code> at the start of the string.
		/// 	</remarks>
		public static readonly QuotedString.BourneUserPathStyle BOURNE_USER_PATH = new QuotedString.BourneUserPathStyle
			();

		/// <summary>Quote an input string by the quoting rules.</summary>
		/// <remarks>
		/// Quote an input string by the quoting rules.
		/// <p>
		/// If the input string does not require any quoting, the same String
		/// reference is returned to the caller.
		/// <p>
		/// Otherwise a quoted string is returned, including the opening and closing
		/// quotation marks at the start and end of the string. If the style does not
		/// permit raw Unicode characters then the string will first be encoded in
		/// UTF-8, with unprintable sequences possibly escaped by the rules.
		/// </remarks>
		/// <param name="in">any non-null Unicode string.</param>
		/// <returns>a quoted string. See above for details.</returns>
		public abstract string Quote(string @in);

		/// <summary>Clean a previously quoted input, decoding the result via UTF-8.</summary>
		/// <remarks>
		/// Clean a previously quoted input, decoding the result via UTF-8.
		/// <p>
		/// This method must match quote such that:
		/// <pre>
		/// a.equals(dequote(quote(a)));
		/// </pre>
		/// is true for any <code>a</code>.
		/// </remarks>
		/// <param name="in">a Unicode string to remove quoting from.</param>
		/// <returns>the cleaned string.</returns>
		/// <seealso cref="Dequote(byte[], int, int)">Dequote(byte[], int, int)</seealso>
		public virtual string Dequote(string @in)
		{
			byte[] b = Constants.Encode(@in);
			return Dequote(b, 0, b.Length);
		}

		/// <summary>Decode a previously quoted input, scanning a UTF-8 encoded buffer.</summary>
		/// <remarks>
		/// Decode a previously quoted input, scanning a UTF-8 encoded buffer.
		/// <p>
		/// This method must match quote such that:
		/// <pre>
		/// a.equals(dequote(Constants.encode(quote(a))));
		/// </pre>
		/// is true for any <code>a</code>.
		/// <p>
		/// This method removes any opening/closing quotation marks added by
		/// <see cref="Quote(string)">Quote(string)</see>
		/// .
		/// </remarks>
		/// <param name="in">the input buffer to parse.</param>
		/// <param name="offset">first position within <code>in</code> to scan.</param>
		/// <param name="end">one position past in <code>in</code> to scan.</param>
		/// <returns>the cleaned string.</returns>
		public abstract string Dequote(byte[] @in, int offset, int end);

		/// <summary>Quoting style used by the Bourne shell.</summary>
		/// <remarks>
		/// Quoting style used by the Bourne shell.
		/// <p>
		/// Quotes are unconditionally inserted during
		/// <see cref="Quote(string)">Quote(string)</see>
		/// . This
		/// protects shell meta-characters like <code>$</code> or <code>~</code> from
		/// being recognized as special.
		/// </remarks>
		public class BourneStyle : QuotedString
		{
			public override string Quote(string @in)
			{
				StringBuilder r = new StringBuilder();
				r.Append('\'');
				int start = 0;
				int i = 0;
				for (; i < @in.Length; i++)
				{
					switch (@in[i])
					{
						case '\'':
						case '!':
						{
							r.AppendRange(@in, start, i);
							r.Append('\'');
							r.Append('\\');
							r.Append(@in[i]);
							r.Append('\'');
							start = i + 1;
							break;
						}
					}
				}
				r.AppendRange(@in, start, i);
				r.Append('\'');
				return r.ToString();
			}

			public override string Dequote(byte[] @in, int ip, int ie)
			{
				bool inquote = false;
				byte[] r = new byte[ie - ip];
				int rPtr = 0;
				while (ip < ie)
				{
					byte b = @in[ip++];
					switch (b)
					{
						case (byte)('\''):
						{
							inquote = !inquote;
							continue;
							goto case (byte)('\\');
						}

						case (byte)('\\'):
						{
							if (inquote || ip == ie)
							{
								r[rPtr++] = b;
							}
							else
							{
								// literal within a quote
								r[rPtr++] = @in[ip++];
							}
							continue;
							goto default;
						}

						default:
						{
							r[rPtr++] = b;
							continue;
							break;
						}
					}
				}
				return RawParseUtils.Decode(Constants.CHARSET, r, 0, rPtr);
			}
		}

		/// <summary>Bourne style, but permits <code>~user</code> at the start of the string.
		/// 	</summary>
		/// <remarks>Bourne style, but permits <code>~user</code> at the start of the string.
		/// 	</remarks>
		public class BourneUserPathStyle : QuotedString.BourneStyle
		{
			public override string Quote(string @in)
			{
				if (@in.Matches("^~[A-Za-z0-9_-]+$"))
				{
					// If the string is just "~user" we can assume they
					// mean "~user/".
					//
					return @in + "/";
				}
				if (@in.Matches("^~[A-Za-z0-9_-]*/.*$"))
				{
					// If the string is of "~/path" or "~user/path"
					// we must not escape ~/ or ~user/ from the shell.
					//
					int i = @in.IndexOf('/') + 1;
					if (i == @in.Length)
					{
						return @in;
					}
					return Sharpen.Runtime.Substring(@in, 0, i) + base.Quote(Sharpen.Runtime.Substring
						(@in, i));
				}
				return base.Quote(@in);
			}
		}

		/// <summary>Quoting style that obeys the rules Git applies to file names</summary>
		public sealed class GitPathStyle : QuotedString
		{
            /// <summary>
            /// Ordered by ascii value
            /// </summary>
			private static readonly char[] Whitespace = new char[] { 'a', 'b', 't', 'n', 'v', 'f', 'r' };

            /// <summary>
            /// Using rules from https://github.com/ethomson/libgit2/blob/apply/src/buffer.c#L862 and http://marc.info/?l=git&m=112927316408690&w=2
            /// </summary>
            public override string Quote(string instr)
            {
                var buf = Encoding.UTF8.GetBytes(instr);
                var indexToStartQuoting = GetIndexToStartQuoting(buf);

                if (buf.Length != 0 && buf.Length == indexToStartQuoting)
                {
                    return instr;
                }

                StringBuilder quoted = new StringBuilder(2 + buf.Length);
                quoted.Append('"');
                quoted.Append(Encoding.UTF8.GetString(buf, 0, indexToStartQuoting));
                for (int i = indexToStartQuoting; i < buf.Length; i++)
                {
                    AppendCharacter(buf, i, quoted);
                }

                quoted.Append('"');

                return quoted.ToString();
		    }

		    private static int GetIndexToStartQuoting(byte[] buf)
		    {
		        int startQuotingIndex = 0;

		        if (buf.ElementAtOrDefault(0) != '!')
		        {
		            for (; startQuotingIndex < buf.Length; startQuotingIndex++)
		            {
		                var thisElement = buf.ElementAt(startQuotingIndex);
		                if (thisElement == '"' || thisElement == '\\' ||
		                    thisElement < ' ' || thisElement > '~')
		                {
		                    break;
		                }
		            }
		        }
		        return startQuotingIndex;
		    }

		    private static void AppendCharacter(byte[] buf, int i, StringBuilder quoted)
		    {
                /* whitespace - use the map above, which is ordered by ascii value */
		        var thisElement = buf.ElementAt(i);
		        if (thisElement >= '\a' && thisElement <= '\r')
		        {
		            quoted.Append('\\');
		            quoted.Append(Whitespace[thisElement - '\a']);
		        }

		        /* double quote and backslash must be escaped */
		        else if (thisElement == '"' || thisElement == '\\')
		        {
		            quoted.Append('\\');
		            quoted.Append((char) thisElement);
		        }

		        /* escape anything unprintable as octal */
		        else if (thisElement != ' ' &&
		                 (thisElement < '!' || thisElement > '~'))
		        {
		            quoted.Append("\\");
		            quoted.Append(Convert.ToString(thisElement, 8).PadLeft(3, '0'));
		        }

		        /* yay, printable! */
		        else
		        {
		            quoted.Append((char) thisElement);
		        }
		    }

		    public override string Dequote(byte[] @in, int inPtr, int inEnd)
			{
				if (2 <= inEnd - inPtr && @in[inPtr] == '"' && @in[inEnd - 1] == '"')
				{
					return Dq(@in, inPtr + 1, inEnd - 1);
				}
				return RawParseUtils.Decode(Constants.CHARSET, @in, inPtr, inEnd);
			}

			private static string Dq(byte[] @in, int inPtr, int inEnd)
			{
				byte[] r = new byte[inEnd - inPtr];
				int rPtr = 0;
				while (inPtr < inEnd)
				{
					byte b = @in[inPtr++];
					if (b != '\\')
					{
						r[rPtr++] = b;
						continue;
					}
					if (inPtr == inEnd)
					{
						// Lone trailing backslash. Treat it as a literal.
						//
						r[rPtr++] = (byte)('\\');
						break;
					}
					switch (@in[inPtr++])
					{
						case (byte)('a'):
						{
							r[rPtr++] = unchecked((int)(0x07));
							continue;
							goto case (byte)('b');
						}

						case (byte)('b'):
						{
							r[rPtr++] = (byte)('\b');
							continue;
							goto case (byte)('f');
						}

						case (byte)('f'):
						{
							r[rPtr++] = (byte)('\f');
							continue;
							goto case (byte)('n');
						}

						case (byte)('n'):
						{
							r[rPtr++] = (byte)('\n');
							continue;
							goto case (byte)('r');
						}

						case (byte)('r'):
						{
							r[rPtr++] = (byte)('\r');
							continue;
							goto case (byte)('t');
						}

						case (byte)('t'):
						{
							r[rPtr++] = (byte)('\t');
							continue;
							goto case (byte)('v');
						}

						case (byte)('v'):
						{
							r[rPtr++] = unchecked((int)(0x0B));
							continue;
							goto case (byte)('\\');
						}

						case (byte)('\\'):
						case (byte)('"'):
						{
							r[rPtr++] = @in[inPtr - 1];
							continue;
							goto case (byte)('0');
						}

						case (byte)('0'):
						case (byte)('1'):
						case (byte)('2'):
						case (byte)('3'):
						{
							int cp = @in[inPtr - 1] - '0';
							for (int n = 1; n < 3 && inPtr < inEnd; n++)
							{
								byte c = @in[inPtr];
								if ('0' <= c && ((sbyte)c) <= '7')
								{
									cp <<= 3;
									cp |= c - '0';
									inPtr++;
								}
								else
								{
									break;
								}
							}
							r[rPtr++] = unchecked((byte)cp);
							continue;
							goto default;
						}

						default:
						{
							// Any other code is taken literally.
							//
							r[rPtr++] = (byte)('\\');
							r[rPtr++] = @in[inPtr - 1];
							continue;
							break;
						}
					}
				}
				return RawParseUtils.Decode(Constants.CHARSET, r, 0, rPtr);
			}

			public GitPathStyle()
			{
			}
			// Singleton
		}
	}
}
