/*
 * Title:        FastPage
 * Description:
 *
 * This software is published under the terms of the OpenSymphony Software
 * License version 1.1, of which a copy has been included with this
 * distribution in the LICENSE.txt file.
 */

using System;
using System.Collections;
using System.IO;

namespace SiteMesh.Parser
{
	/**
	 * HTMLPage implementation produced by FastPageParser.
	 *
	 * @author <a href="mailto:salaman@qoretech.com">Victor Salaman</a>
	 * @version $Revision: 1.3 $
	 */

	public sealed class FastPage : AbstractHTMLPage
	{
		private string head;
		private string body;
		private bool frameSet;

		public FastPage(IDictionary sitemeshProps, IDictionary htmlProps, IDictionary metaProps, IDictionary bodyProps,
		                string title, string head, string body, bool frameSet)
		{
			this.head = head;
			this.body = body;
			this.frameSet = frameSet;
			AddAttributeList("", htmlProps);
			AddAttributeList("page.", sitemeshProps);
			AddAttributeList("body.", bodyProps);
			AddAttributeList("meta.", metaProps);
			AddProperty("title", title);
		}

		public override void WriteHead(TextWriter outWriter)
		{
			outWriter.Write(head);
		}

		public override void WriteBody(TextWriter outWriter)
		{
			outWriter.Write(body);
		}

		public override void WritePage(Stream outWriter)
		{
			outWriter.Write(pageData, 0, pageData.Length);
		}

		private void AddAttributeList(string prefix, IDictionary attributes)
		{
			if (attributes == null || attributes.Count == 0)
				return;

			String name, data;
			IDictionaryEnumerator i = attributes.GetEnumerator();

			while (i.MoveNext())
			{
				name = (String) i.Key;
				data = (String) i.Value;

				if (data != null && data.Trim().Length > 0)
				{
					AddProperty(prefix + name, data);
				}
			}
		}

		public void SetVerbatimPage(byte[] v)
		{
			this.pageData = v;
		}

		public override bool FrameSet
		{
			get { return frameSet; }
		}

		public override string Body
		{
			get { return body; }
		}

	}

}