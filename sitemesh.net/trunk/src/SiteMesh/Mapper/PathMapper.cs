/*
 * Title:        PathMapper
 * Description:
 *
 * This software is published under the terms of the OpenSymphony Software
 * License version 1.1, of which a copy has been included with this
 * distribution in the LICENSE.txt file.
 */

using System.Collections;
using System.Text.RegularExpressions;
/**
 * The PathMapper is used to map file patterns to keys, and find an approriate
 * key for a given file path. Wildcard patterns are also supported, using
 * any combination of * and ?.
 *
 * <h3>Example</h3>
 *
 * <blockquote><code>
 * PathMapper pm = new PathMapper();<br>
 * <br>
 * pm.put("one","/");<br>
 * pm.put("two","/mydir/*");<br>
 * pm.put("three","*.xml");<br>
 * pm.put("four","/myexactfile.html");<br>
 * pm.put("five","/*\/admin/*.??ml");<br>
 * <br>
 * String result1 = pm.get("/mydir/myfile.xml"); // returns "two";<br>
 * String result2 = pm.get("/mydir/otherdir/admin/myfile.html"); // returns "five";<br>
 * </code></blockquote>
 *
 * @author 
 * @version $Revision: 1.2 $
 */

public sealed class PathMapper
{
	private IDictionary mappings = new Hashtable();

	public void Add(string key, string pattern)
	{
		if (key != null)
		{
			mappings.Add(pattern, key);
		}
	}


	public string Item(string path)
	{
		if (path == null)
		{
			path = "/";
		}
		string mapped = FindKey(path, mappings);
		if (mapped == null)
		{
			return null;
		}
		return (string) mappings[mapped];
	}

	// Find exact key in mappings.
	private string FindKey(string path, IDictionary mappings)
	{
		string result = findExactKey(path, mappings);
		if (result == null)
		{
			result = findComplexKey(path, mappings);
		}
		if (result == null)
		{
			result = findDefaultKey(mappings);
		}

		return result;
	}


	// Check if path matches exact pattern ( /blah/blah.aspx ). 
	private string findExactKey(string path, IDictionary mappings)
	{
		if (mappings.Contains(path))
		{
			return path;
		}

		return null;
	}

	private string findComplexKey(string path, IDictionary mappings)
	{
		string result = null;

		foreach (string key in mappings.Keys)
		{
			if ((key.Length > 1 && (key.IndexOf("?") != -1)
				|| (key.IndexOf("*") != -1) && match(key, path, false)))
			{
				if (result == null || key.Length > result.Length)
				{
					result = key;
				}
			}
		}
		return result;
	}

	// Look for root pattern ( / )
	private string findDefaultKey(IDictionary mappings)
	{
		string[] defaultKeys = {"/", "*", "/*"};

		foreach (string key in defaultKeys)
		{
			if (mappings.Contains(key))
			{
				return key;
			}
		}

		return null;
	}

	private bool match(string pattern, string str, bool isCaseSensitive)
	{
		// more special characters may need to be added to these, but they pass the tests
		pattern = pattern.Replace("*", @"[a-zA-Z0-9/\-\.\s_]*");
		pattern = pattern.Replace("?", @"[a-zA-Z0-9/\-\.\s_]{1,1}");

		// instantiate a new Regex object.
		Regex rex = new Regex(pattern);

		// Find a single match in the input string
		Match m = rex.Match(str);
		return m.Success;
	}
}