using System.Collections;
using System.IO;
using System.Text;
using NUnit.Framework;
using SiteMesh.Parser;

namespace SiteMesh.Test.UnitTest
{
	/// <remarks>
	/// Test fixture for the HtmlPageParser.  Works with the 
	/// HtmlPageParserTestSuite to process a directory of test files.
	/// Each test file has labeled blocks for input and validation.
	/// </remarks>
	[TestFixture]
	public class HtmlPageParserTest
	{
		private IHtmlPage _page;
		private Hashtable _blocks;
		private string _file;

		[SetUp]
		public void SetUp()
		{
			// read blocks from input file.
			_blocks = ReadBlocks(_file);

			if (_blocks == null)
			{
				Assert.Fail("blocks not read");
			}

			// create PageParser and parse input block into HTMLPage object.
			string input = (string) _blocks["INPUT"];

			IPageParser parser = new FastPageParser();

			// hard coded ascii encoding, probably needs to be based upon content type
			Encoding utf8 = new UTF8Encoding();

			byte[] bytes = utf8.GetBytes(input.ToCharArray());

			_page = (IHtmlPage) parser.Parse(bytes, "utf-8");

		}

		[Test]
		public void TestTitle()
		{
			AssertBlock("TITLE", _page.Title);
		}

		[Test]
		public void TestBody()
		{
			StringWriter body = new StringWriter();
			_page.WriteBody(body);
			body.Flush();
			AssertBlock("BODY", body.ToString());
		}

		[Test]
		public void TestHead()
		{
			StringWriter head = new StringWriter();
			_page.WriteHead(head);
			head.Flush();
			AssertBlock("HEAD", head.ToString());
		}

		[Test]
		public void TestFullPage()
		{
			MemoryStream fullPage = new MemoryStream();
			_page.WritePage(fullPage);
			fullPage.Flush();
			AssertBlock("INPUT", fullPage.ToString());
		}

		private void AssertBlock(string blockName, string result)
		{
			string expected = (string) _blocks[blockName];
			expected = expected.Trim();
			result = result.Trim();
			Assert.AreEqual(_file + " : Block did not match", expected, result);
		}


		private Hashtable ReadBlocks(string filename)
		{
			Hashtable blocks = new Hashtable();
			StreamReader sr = File.OpenText(filename);
			string line = null;
			;
			string blockName = string.Empty;
			StringBuilder blockContents = new StringBuilder();
			line = sr.ReadLine();

			while (line != null)
			{
				if (line.StartsWith("~~~") && line.EndsWith("~~~"))
				{
					if (blockName != string.Empty)
					{
						blocks[blockName] = blockContents.ToString();
					}
					blockName = line.Substring(4, line.Length - 8);
					blockContents = new StringBuilder();
				}
				else
				{
					if (blockName != string.Empty)
					{
						blockContents.Append(line);
						blockContents.Append("\r\n");
					}
				}
				line = sr.ReadLine();
			}

			if (blockName != string.Empty)
			{
				blocks.Add(blockName, blockContents.ToString());
			}

			return blocks;
		}

		public string Context
		{
			get { return _file; }

			set { _file = value; }
		}

	}
}