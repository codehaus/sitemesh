/*
 * Title:        FastPageParser
 * Description:
 *
 * This software is published under the terms of the OpenSymphony Software
 * License version 1.1, of which a copy has been included with this
 * distribution in the LICENSE.txt file.
 */

using System;
using System.Collections;
using System.IO;
using System.Text;

using SiteMesh;
using SiteMesh.Parser.Util;

namespace SiteMesh.Parser
{

	/**
	 * Very fast PageParser implementation for parsing HTML.
	 *
	 * <p>Produces FastPage.</p>
	 *
	 * @author <a href="mailto:salaman@qoretech.com">Victor Salaman</a>
	 * @version $Revision: 1.10 $
	 */
	public sealed class FastPageParser : IPageParser
	{
		private const int TOKEN_NONE = -0;
		private const int TOKEN_EOF = -1;
		private const int TOKEN_TEXT = -2;
		private const int TOKEN_TAG = -3;
		private const int TOKEN_COMMENT = -4;
		private const int TOKEN_CDATA = -5;
		private const int TOKEN_SCRIPT = -6;
		private const int TOKEN_DOCTYPE = -7;
		private const int TOKEN_EMPTYTAG = -8;

		private const int STATE_EOF = -1;
		private const int STATE_TEXT = -2;
		private const int STATE_TAG = -3;
		private const int STATE_COMMENT = -4;
		private const int STATE_TAG_QUOTE = -5;
		private const int STATE_CDATA = -6;
		private const int STATE_SCRIPT = -7;
		private const int STATE_DOCTYPE = -8;

		private const int TAG_STATE_NONE = 0;
		private const int TAG_STATE_HTML = -1;
		private const int TAG_STATE_HEAD = -2;
		private const int TAG_STATE_TITLE = -3;
		private const int TAG_STATE_BODY = -4;
		private const int TAG_STATE_XML = -6;
		private const int TAG_STATE_XMP = -7;

		// These hashcodes are hardcoded because swtich statements can only
		// switch on compile-time constants.
		// In theory it is possible for there to be a hashcode collision with
		// other HTML tags, however in practice it is *very* unlikely because
		// tags are generally only a few characters long and hence are likely
		// to produce unique values.

		private const int SLASH_XML_HASH = 1518984; // "/xml".hashCode();
		private const int XML_HASH = 118807; // "xml".hashCode();
		private const int SLASH_XMP_HASH = 1518988; // "/xmp".hashCode();
		private const int XMP_HASH = 118811; // "xmp".hashCode();
		private const int HTML_HASH = 3213227; // "html".hashCode();
		private const int SLASH_HTML_HASH = 46618714; // "/html".hashCode();
		private const int HEAD_HASH = 3198432; // "head".hashCode();
		private const int TITLE_HASH = 110371416; // "title".hashCode();
		private const int SLASH_TITLE_HASH = 1455941513; // "/title".hashCode();
		private const int PARAMETER_HASH = 1954460585; // "parameter".hashCode();
		private const int META_HASH = 3347973; // "meta".hashCode();
		private const int SLASH_HEAD_HASH = 46603919; // "/head".hashCode();
		private const int FRAMESET_HASH = -1644953643; // "frameset".hashCode();
		private const int FRAME_HASH = 97692013; // "frame".hashCode();
		private const int BODY_HASH = 3029410; // "body".hashCode();
		private const int SLASH_BODY_HASH = 46434897; // "/body".hashCode();
		private const int CONTENT_HASH = 951530617; // "content".hashCode();

		public IPage Parse(byte[] data)
		{
			return Parse(data, null);
		}

		public IPage Parse(byte[] data, string encoding)
		{
			StreamReader reader = null;

			if (encoding == null)
			{
				encoding = "utf-8";
			}
        
			reader = new StreamReader(new MemoryStream(data), Encoding.GetEncoding(encoding));

			FastPage page = InternalParse(new CharArrayReader(data));
			page.SetVerbatimPage(data);
			return page;
		}

		private FastPage InternalParse(TextReader reader)
		{
			CharArray _buffer    = new CharArray(4096);
			CharArray _body      = new CharArray(4096);
			CharArray _head      = new CharArray(512);
			CharArray _title     = new CharArray(128);
			IDictionary _htmlProperties     = null;
			IDictionary _metaProperties     = new Hashtable(6);
			IDictionary _sitemeshProperties = new Hashtable(6);
			IDictionary _bodyProperties     = null;

			CharArray _currentTaggedContent = new CharArray(1024);
			string _contentTagId = null;
			bool tagged = false;

			bool _frameSet = false;

			int _state = STATE_TEXT;
			int _tokenType = TOKEN_NONE;
			int _pushBack = 0;
			int _comment = 0;
			int _quote = 0;
			bool hide = false;

			int state = TAG_STATE_NONE;
			int laststate = TAG_STATE_NONE;
			bool doneTitle = false;

			// This tag object gets reused each iteration.
			Tag tagObject = new Tag();

			while (_tokenType != TOKEN_EOF)
			{
				if(tagged)
				{
					if(_tokenType == TOKEN_TAG || _tokenType == TOKEN_EMPTYTAG)
					{
						if(_buffer==null || _buffer.Length == 0)
						{
							_tokenType=TOKEN_NONE;
							continue;
						}

						if (parseTag(tagObject, _buffer) == null) continue;

						if (_buffer.compareLowerSubstr("/content"))   // Note that the '/' survives the | 32 operation
						{
							tagged = false;
							if(_contentTagId != null)
							{
								state = TAG_STATE_NONE;
								_sitemeshProperties.Add(_contentTagId, _currentTaggedContent.ToString());
								_currentTaggedContent.setLength(0);
								_contentTagId = null;
							}
						}
						else
						{
							_currentTaggedContent.append('<').append(_buffer).append('>');
						}
					}
					else
					{
						if(_buffer.Length > 0) _currentTaggedContent.append(_buffer);
					}
				}
				else
				{
					if(_tokenType == TOKEN_TAG || _tokenType == TOKEN_EMPTYTAG)
					{
						if(_buffer==null || _buffer.Length == 0)
						{
							_tokenType=TOKEN_NONE;
							continue;
						}

						if(parseTag(tagObject, _buffer) == null) 
						{
							_tokenType=TOKEN_TEXT;
							continue;
						}

						int tagHash = _buffer.substrHashCode();

						if(state == TAG_STATE_XML || state == TAG_STATE_XMP)
						{
							writeTag(state, laststate, hide, _head, _buffer, _body);
							if( (state == TAG_STATE_XML && tagHash == SLASH_XML_HASH)
								||(state == TAG_STATE_XMP && tagHash == SLASH_XMP_HASH) )
							{
								state = laststate;
							}
						}
						else
						{
							bool doDefault = false;
							switch (tagHash) 
							{
								case HTML_HASH:
									if (!_buffer.compareLowerSubstr("html")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									state = TAG_STATE_HTML;
									_htmlProperties = parseProperties(tagObject, _buffer).properties;
									break;
								case HEAD_HASH:
									if (!_buffer.compareLowerSubstr("head")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									state = TAG_STATE_HEAD;
									break;
								case XML_HASH:
									if (!_buffer.compareLowerSubstr("xml")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									laststate = state;
									writeTag(state, laststate, hide, _head, _buffer, _body);
									state = TAG_STATE_XML;
									break;
								case XMP_HASH:
									if (!_buffer.compareLowerSubstr("xmp")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									laststate = state;
									writeTag(state, laststate, hide, _head, _buffer, _body);
									state = TAG_STATE_XMP;
									break;
								case TITLE_HASH:
									if (!_buffer.compareLowerSubstr("title")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									if (doneTitle)
									{
										hide = true;
									}
									else
									{
										laststate = state;
										state = TAG_STATE_TITLE;
									}
									break;
								case SLASH_TITLE_HASH:
									if (!_buffer.compareLowerSubstr("/title")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									if (doneTitle)
									{
										hide = false;
									}
									else
									{
										doneTitle = true;
										state = laststate;
									}
									break;
								case PARAMETER_HASH:
									if (!_buffer.compareLowerSubstr("parameter")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									parseProperties(tagObject, _buffer);
									string name = (string) tagObject.properties["name"];
									string val = (string) tagObject.properties["value"];

									if (name != null && val != null)
									{
										_sitemeshProperties.Add(name, val);
									}
									break;
								case META_HASH:
									if (!_buffer.compareLowerSubstr("meta")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									CharArray metaDestination = state == TAG_STATE_HEAD ? _head : _body;
									metaDestination.append('<');
									metaDestination.append(_buffer);
									metaDestination.append('>');
									parseProperties(tagObject, _buffer);
									name = (string) tagObject.properties["name"];
									val = (string) tagObject.properties["content"];

									if (name == null)
									{
										string httpEquiv = (string) tagObject.properties["http-equiv"];

										if (httpEquiv != null)
										{
											name = "http-equiv." + httpEquiv;
										}
									}

									if (name != null && val != null)
									{
										_metaProperties.Add(name, val);
									}
									break;
								case SLASH_HEAD_HASH:
									if (!_buffer.compareLowerSubstr("/head")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									state = TAG_STATE_HTML;
									break;
								case FRAME_HASH:
									if (!_buffer.compareLowerSubstr("frame")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									_frameSet = true;
									break;
								case FRAMESET_HASH:
									if (!_buffer.compareLowerSubstr("frameset")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									_frameSet = true;
									break;
								case BODY_HASH:
									if (!_buffer.compareLowerSubstr("body")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									if (_tokenType == TOKEN_EMPTYTAG)
									{
										state = TAG_STATE_BODY;
									}
									_bodyProperties = parseProperties(tagObject, _buffer).properties;
									break;
								case CONTENT_HASH:
									if (!_buffer.compareLowerSubstr("content")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									state = TAG_STATE_NONE;
									IDictionary props = parseProperties(tagObject, _buffer).properties;
									if (props != null)
									{
										tagged = true;
										_contentTagId = (string) props["tag"];
									}
									break;
								case SLASH_XMP_HASH:
									if (!_buffer.compareLowerSubstr("/xmp")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									hide = false;
									break;
								case SLASH_BODY_HASH:
									if (!_buffer.compareLowerSubstr("/body")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									state = TAG_STATE_NONE;
									hide = true;
									break;
								case SLASH_HTML_HASH:
									if (!_buffer.compareLowerSubstr("/html")) 
									{ // skip any accidental hash collisions
										doDefault = true;
										break;
									}
									state = TAG_STATE_NONE;
									hide = true;
									break;
								default:
									doDefault = true;
									break;
							}
							if (doDefault)
								writeTag(state, laststate, hide, _head, _buffer, _body);
						}
					}
					else if (!hide)
					{
						if (_tokenType == TOKEN_TEXT)
						{
							if (state == TAG_STATE_TITLE)
							{
								_title.append(_buffer);
							}
							else if (shouldWriteToHead(state, laststate))
							{
								_head.append(_buffer);
							}
							else
							{
								_body.append(_buffer);
							}
						}
						else if (_tokenType == TOKEN_COMMENT)
						{
							CharArray commentDestination = shouldWriteToHead(state, laststate) ? _head : _body;
							commentDestination.append("<!--");
							commentDestination.append(_buffer);
							commentDestination.append("-->");
						}
						else if (_tokenType == TOKEN_CDATA)
						{
							CharArray commentDestination = state == TAG_STATE_HEAD ? _head : _body;
							commentDestination.append("<![CDATA[");
							commentDestination.append(_buffer);
							commentDestination.append("]]>");
						}
						else if (_tokenType == TOKEN_SCRIPT)
						{
							CharArray commentDestination = state == TAG_STATE_HEAD ? _head : _body;
							commentDestination.append('<');
							commentDestination.append(_buffer);
						}
					}
				}
				_buffer.setLength(0);

			start:
				while (true)
				{
					int c;

					if(_pushBack != 0)
					{
						c = _pushBack;
						_pushBack = 0;
					}
					else
					{
						try
						{
							c = reader.Read();
						}
						catch(IOException)
						{
							_tokenType = TOKEN_EOF;
							goto start;
						}
					}

					if(c < 0)
					{
						int tmpstate = _state;
						_state = STATE_EOF;

						if(_buffer.Length > 0 && tmpstate == STATE_TEXT)
						{
							_tokenType = TOKEN_TEXT;
							goto start;
						}
						else
						{
							_tokenType = TOKEN_EOF;
							goto start;
						}
					}

					switch(_state)
					{
						case STATE_TAG:
						{
							int buflen = _buffer.Length;

							if(c == '>')
							{
								if (_buffer.Length > 1 && _buffer.CharAt(_buffer.Length - 1) == '/')
								{
									_tokenType = TOKEN_EMPTYTAG;
								}
								else
								{
									_tokenType = TOKEN_TAG;
								}
								_state = STATE_TEXT;
								goto start;
							}
							else if(c == '/')
							{
								_buffer.append('/');
							}
							else if(c == '<' && buflen == 0)
							{
								_buffer.append("<<");
								_state = STATE_TEXT;
							}
							else if(c == '-' && buflen == 2 && _buffer.CharAt(1) == '-' && _buffer.CharAt(0) == '!')
							{
								_buffer.setLength(0);
								_state = STATE_COMMENT;
							}
							else if(c == '[' && buflen == 7 && _buffer.CharAt(0) == '!' && _buffer.CharAt(1) == '[' &&  _buffer.compareLower("cdata", 2))
							{
								_buffer.setLength(0);
								_state = STATE_CDATA;
							}
							else if((c == 'e' || c == 'E') && buflen == 7 && _buffer.CharAt(0) == '!' && _buffer.compareLower("doctyp", 1))
							{
								_buffer.append((char)c);
								_state = STATE_DOCTYPE;
							}
							else if((c == 'T' || c == 't') && buflen == 5 && _buffer.compareLower("scrip", 0))
							{
								_buffer.append((char)c);
								_state = STATE_SCRIPT;
							}

							else if(c == '"' || c == '\'')
							{
								_quote = c;
								_buffer.append(( char ) c);
								_state = STATE_TAG_QUOTE;
							}
							else
							{
								_buffer.append(( char ) c);
							}
						}
							break;

						case STATE_TEXT:
						{
							if(c == '<')
							{
								_state = STATE_TAG;
								if(_buffer.Length > 0)
								{
									_tokenType = TOKEN_TEXT;
									goto start;
								}
							}
							else
							{
								_buffer.append(( char ) c);
							}
						}
							break;

						case STATE_TAG_QUOTE:
						{
							if(c == '>')
							{
								_pushBack = c;
								_state = STATE_TAG;
							}
							else
							{
								_buffer.append(( char ) c);
								if(c == _quote)
								{
									_state = STATE_TAG;
								}
							}
						}
							break;

						case STATE_COMMENT:
						{
							if(c == '>' && _comment >= 2)
							{
								_buffer.setLength(_buffer.Length - 2);
								_comment = 0;
								_state = STATE_TEXT;
								_tokenType = TOKEN_COMMENT;
								goto start;
							}
							else if(c == '-')
							{
								_comment++;
							}
							else
							{
								_comment = 0;
							}

							_buffer.append(( char ) c);
						}
							break;

						case STATE_CDATA:
						{
							if(c == '>' && _comment >= 2)
							{
								_buffer.setLength(_buffer.Length - 2);
								_comment = 0;
								_state = STATE_TEXT;
								_tokenType = TOKEN_CDATA;
								goto start;
							}
							else if(c == ']')
							{
								_comment++;
							}
							else
							{
								_comment = 0;
							}

							_buffer.append(( char ) c);
						}
							break;

						case STATE_SCRIPT:
						{
							_buffer.append((char) c);
							if (c == '<')
							{
								_comment = 0;
							}
							else if ((c == '/' && _comment == 0)
								||((c == 's' || c == 'S' ) && _comment == 1)
								||((c == 'c' || c == 'C' ) && _comment == 2)
								||((c == 'r' || c == 'R' ) && _comment == 3)
								||((c == 'i' || c == 'I' ) && _comment == 4)
								||((c == 'p' || c == 'P' ) && _comment == 5)
								||((c == 't' || c == 'T' ) && _comment == 6)
								)
							{
								_comment++;
							}
							else if(c == '>' && _comment >= 7)
							{
								_comment = 0;
								_state = STATE_TEXT;
								_tokenType = TOKEN_SCRIPT;
								goto start;
							}
						}
							break;

						case STATE_DOCTYPE:
						{
							_buffer.append((char) c);
							if (c == '>')
							{
								_state = STATE_TEXT;
								_tokenType = TOKEN_DOCTYPE;
								goto start;
							}
							else 
							{
								_comment = 0;
							}
						}
							break;
					}
				}
			}

			// Help the GC
			_currentTaggedContent = null;
			_buffer = null;

			return new FastPage(_sitemeshProperties,
				_htmlProperties,
				_metaProperties,
				_bodyProperties,
				_title.ToString().Trim(),
				_head.ToString().Trim(),
				_body.ToString().Trim(),
				_frameSet);
		}

		private static void writeTag(int state, int laststate, bool hide, CharArray _head, CharArray _buffer, CharArray _body) 
		{
			if (!hide)
			{
				if (shouldWriteToHead(state, laststate))
				{
					_head.append('<').append(_buffer).append('>');
				}
				else
				{
					_body.append('<').append(_buffer).append('>');
				}
			}
		}

		private static bool shouldWriteToHead(int state, int laststate)
		{
			return state == TAG_STATE_HEAD
				||(laststate == TAG_STATE_HEAD && (state == TAG_STATE_XML || state == TAG_STATE_XMP));
		}

		/**
		 * Populates a {@link Tag} object using data from the supplied {@link CharArray}.
		 *
		 * The supplied tag parameter is reset and reused - this avoids excess object
		 * creation which hwlps performance.
		 *
		 * @return the same tag instance that was passed in, except it will be populated
		 * with a new <tt>name</tt> value (and the corresponding <tt>nameEndIdx</tt> value).
		 * However if the tag contained nathing but whitespace, this method will return
		 * <tt>null</tt>.
		 */
		private Tag parseTag(Tag tag, CharArray buf)
		{
			int len = buf.Length;
			int idx = 0;
			int begin;

			// Skip over any leading whitespace in the tag
			while (idx < len && Char.IsWhiteSpace(buf.CharAt(idx))) idx++;

			if(idx == len) return null;

			// Find out where the non-whitespace characters end. This will give us the tag name.
			begin = idx;
			while (idx < len && !Char.IsWhiteSpace(buf.CharAt(idx))) idx++;

			// Mark the tag name as a substring within the buffer. This allows us to perform
			// a substring comparison against it at a later date
			buf.setSubstr(begin, buf.CharAt(idx - 1) == '/' ? idx - 1 : idx);

			// Remember where the name finishes so we can pull out the properties later if need be
			tag.nameEndIdx = idx;

			return tag;
		}

		/**
		 * This is called when we need to extract the properties for the tag from the tag's HTML.
		 * We only call this when necessary since it has quite a lot of overhead.
		 *
		 * @param tag the tag that is currently being processed. This should be the
		 * tag that was returned as a result of a call to {@link #parseTag(FastPageParser.Tag, CharArray)}
		 * (ie, it has the <tt>name</tt> and <tt>nameEndIdx</tt> fields set correctly for the
		 * tag in question. The <tt>properties</tt> field can be in an undefined state - it
		 * will get replaced regardless).
		 * @param buffer a <tt>CharArray</tt> containing the entire tag that is being parsed.
		 * @return the same tag instance that was passed in, only it will now be populated
		 * with any properties that were specified in the tag's HTML.
		 */
		private static Tag parseProperties(Tag tag, CharArray buffer)
		{
			int len = buffer.Length;
			int idx = tag.nameEndIdx;

			// Start with an empty hashmap. A new HashMap is lazy-created if we happen to find any properties
			tag.properties = new Hashtable();
			int begin;
			while (idx < len)
			{
				// Skip forward to the next non-whitespace character
				while (idx < len && Char.IsWhiteSpace(buffer.CharAt(idx))) idx++;

				if(idx == len) continue;

				begin = idx;
				if(buffer.CharAt(idx) == '"')
				{
					idx++;
					while (idx < len && buffer.CharAt(idx) != '"') idx++;
					if(idx == len) continue;
					idx++;
				}
				else if(buffer.CharAt(idx) == '\'')
				{
					idx++;
					while (idx < len && buffer.CharAt(idx) != '\'') idx++;
					if(idx == len) continue;
					idx++;
				}
				else
				{
					while (idx < len && !Char.IsWhiteSpace(buffer.CharAt(idx)) && buffer.CharAt(idx) != '=') idx++;
				}

				// Mark the substring. This is the attribute name
				buffer.setSubstr(begin, idx);

				if(idx < len && Char.IsWhiteSpace(buffer.CharAt(idx)))
				{
					while (idx < len && Char.IsWhiteSpace(buffer.CharAt(idx))) idx++;
				}

				if(idx == len || buffer.CharAt(idx) != '=') continue;

				idx++;

				if(idx == len) continue;

				if(buffer.CharAt(idx) == ' ')
				{
					while (idx < len && Char.IsWhiteSpace(buffer.CharAt(idx))) idx++;
					if(idx == len || (buffer.CharAt(idx) != '"' && buffer.CharAt(idx) != '"')) continue;
				}

				begin = idx;
				int end;
				if(buffer.CharAt(idx) == '"')
				{
					idx++;
					begin = idx;
					while (idx < len && buffer.CharAt(idx) != '"') idx++;
					if(idx == len) continue;
					end = idx;
					idx++;
				}
				else if(buffer.CharAt(idx) == '\'')
				{
					idx++;
					begin = idx;
					while (idx < len && buffer.CharAt(idx) != '\'') idx++;
					if(idx == len) continue;
					end = idx;
					idx++;
				}
				else
				{
					while (idx < len && !Char.IsWhiteSpace(buffer.CharAt(idx))) idx++;
					end = idx;
				}
				// Extract the name and value as string objects and add them to the property map
				string name = buffer.getLowerSubstr();
				string value = buffer.substring(begin, end);

				tag.AddProperty(name, value);
			}
			return tag;
		}

		private class Tag
		{
			// The index where the name string ends. This is used as the starting
			// offet if we need to continue processing to find the tag's properties
			public int nameEndIdx = 0;

			// This holds a map of the various properties for a particular tag.
			// This map is only populated when required - normally it will remain empty
			public IDictionary properties = null;

			/**
			 * Adds a name/value property pair to this tag. Each property that is
			 * added represents a property that was parsed from the tag's HTML.
			 */
			public void AddProperty(string name, string data)
			{
				if(properties == null)
				{
					properties = new Hashtable(8);
				}
				properties.Add(name, data);
			}
		}
	}
}