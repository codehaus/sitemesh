using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;

namespace SiteMesh.Mapper
{
	/// <summary>
	///	The RobotDecoratorMapper will use the specified decorator when the requester
	///	is identified as a robot (also known as spider, crawler, ferret) of a search engine.
	///	
	/// <p>The name of this decorator should be supplied in the <code>decorator</code>
	/// property.</p>
	///	
	///	@author <a href="mailto:pathos@pandora.be">Mathias Bogaert</a>
	/// </summary>
	public sealed class RobotDecoratorMapper : AbstractDecoratorMapper
	{
		private String decoratorName = null;

		private const string ROBOT_KEY = "__sitemesh__robot";

		/// <summary>
		/// All known robot hosts (list can be found <a href="http://www.spiderhunter.com">here</a>).
		/// </summary>
		private static readonly string[] botHosts = new string[]
			{
				"alltheweb.com", "alta-vista.net", "altavista.com",
				"atext.com", "euroseek.net", "excite.com",
				"fast-search.net", "google.com", "googlebot.com",
				"infoseek.co.jp", "infoseek.com", "inktomi.com",
				"inktomisearch.com", "linuxtoday.com.au", "lycos.com",
				"lycos.com", "northernlight.com", "pa-x.dec.com"
			};

		/// <summary>
		/// All known robot user-agent headers (list can be found
		/// <a href="http://www.robotstxt.org/wc/active.html">here</a>).
		/// 
		/// <p>NOTE: To avoid bad detection:</p>
		/// 
		/// <ul>
		///  <li>Robots with ID of 2 letters only were removed</li>
		///  <li>Robot called "webs" were removed</li>
		///  <li>directhit was changed in direct_hit (its real id)</li>
		/// </ul>
		/// </summary>
		private static readonly string[] botAgents = new string[]
			{
				"acme.spider", "ahoythehomepagefinder", "alkaline", "appie", "arachnophilia",
				"architext", "aretha", "ariadne", "aspider", "atn.txt", "atomz", "auresys",
				"backrub", "bigbrother", "bjaaland", "blackwidow", "blindekuh", "bloodhound",
				"brightnet", "bspider", "cactvschemistryspider", "calif", "cassandra",
				"cgireader", "checkbot", "churl", "cmc", "collective", "combine", "conceptbot",
				"core", "cshkust", "cusco", "cyberspyder", "deweb", "dienstspider", "diibot",
				"direct_hit", "dnabot", "download_express", "dragonbot", "dwcp", "ebiness",
				"eit", "emacs", "emcspider", "esther", "evliyacelebi", "fdse", "felix",
				"ferret", "fetchrover", "fido", "finnish", "fireball", "fish", "fouineur",
				"francoroute", "freecrawl", "funnelweb", "gazz", "gcreep", "getbot", "geturl",
				"golem", "googlebot", "grapnel", "griffon", "gromit", "gulliver", "hambot",
				"harvest", "havindex", "hometown", "wired-digital", "htdig", "htmlgobble",
				"hyperdecontextualizer", "ibm", "iconoclast", "ilse", "imagelock", "incywincy",
				"informant", "infoseek", "infoseeksidewinder", "infospider", "inspectorwww",
				"intelliagent", "iron33", "israelisearch", "javabee", "jcrawler", "jeeves",
				"jobot", "joebot", "jubii", "jumpstation", "katipo", "kdd", "kilroy",
				"ko_yappo_robot", "labelgrabber.txt", "larbin", "legs", "linkscan",
				"linkwalker", "lockon", "logo_gif", "lycos", "macworm", "magpie", "mediafox",
				"merzscope", "meshexplorer", "mindcrawler", "moget", "momspider", "monster",
				"motor", "muscatferret", "mwdsearch", "myweb", "netcarta", "netmechanic",
				"netscoop", "newscan-online", "nhse", "nomad", "northstar", "nzexplorer",
				"occam", "octopus", "orb_search", "packrat", "pageboy", "parasite", "patric",
				"perignator", "perlcrawler", "phantom", "piltdownman", "pioneer", "pitkow",
				"pjspider", "pka", "plumtreewebaccessor", "poppi", "portalb", "puu", "python",
				"raven", "rbse", "resumerobot", "rhcs", "roadrunner", "robbie", "robi",
				"roverbot", "safetynetrobot", "scooter", "search_au", "searchprocess",
				"senrigan", "sgscout", "shaggy", "shaihulud", "sift", "simbot", "site-valet",
				"sitegrabber", "sitetech", "slurp", "smartspider", "snooper", "solbot",
				"spanner", "speedy", "spider_monkey", "spiderbot", "spiderman", "spry",
				"ssearcher", "suke", "sven", "tach_bw", "tarantula", "tarspider", "tcl",
				"techbot", "templeton", "titin", "titan", "tkwww", "tlspider", "ucsd",
				"udmsearch", "urlck", "valkyrie", "victoria", "visionsearch", "voyager",
				"vwbot", "w3index", "w3m2", "wanderer", "webbandit", "webcatcher", "webcopy",
				"webfetcher", "webfoot", "weblayers", "weblinker", "webmirror", "webmoose",
				"webquest", "webreader", "webreaper", "websnarf", "webspider", "webvac",
				"webwalk", "webwalker", "webwatch", "wget", "whowhere", "wmir", "wolp",
				"wombat", "worm", "wwwc", "wz101", "xget", "nederland.zoek"
			};

		public override void Init(IDictionary properties, IDecoratorMapper parent)
		{
			base.Init(properties, parent);
			decoratorName = (string) properties["decorator"];
		}

		public override IDecorator GetDecorator(HttpRequest request, IPage page)
		{
			IDecorator result = null;

			if (decoratorName != null && IsBot(request))
			{
				result = GetNamedDecorator(request, decoratorName);
			}

			return result == null ? base.GetDecorator(request, page) : result;
		}

		/// <summary>
		/// Check if the current request came from  a robot (also known as spider, crawler, ferret)
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static bool IsBot(HttpRequest request)
		{
			if (request == null)
				return false;

			// force creation of a session
			HttpSessionState session = HttpContext.Current.Session;

			if (Boolean.FalseString.Equals(session[ROBOT_KEY]))
			{
				return false;
			}
			else if (Boolean.TrueString.Equals(session[ROBOT_KEY]))
			{
				// a key was found in the session indicating it is a robot
				return true;
			}
			else
			{
				if ("robots.txt".IndexOf(request.Url.AbsoluteUri) != -1)
				{
					// there is a specific request for the robots.txt file, so we assume
					// it must be a robot (only robots request robots.txt)

					// set a key in the session, so the next time we don't have to manually
					// detect the robot again
					session[ROBOT_KEY] = Boolean.TrueString;
					return true;
				}
				else
				{
					string userAgent = request.Headers["User-Agent"];

					if (userAgent != null && userAgent.Trim().Length > 2)
					{
						// first check for common user-agent headers, so that we can speed
						// this thing up, hopefully clever spiders will not send a fake header
						if (userAgent.IndexOf("MSIE") != -1 || userAgent.IndexOf("Gecko") != -1 // MSIE and Mozilla
							|| userAgent.IndexOf("Opera") != -1 || userAgent.IndexOf("iCab") != -1 // Opera and iCab (mac browser)
							|| userAgent.IndexOf("Konqueror") != -1 || userAgent.IndexOf("KMeleon") != -1 // Konqueror and KMeleon
							|| userAgent.IndexOf("4.7") != -1 || userAgent.IndexOf("Lynx") != -1) // NS 4.78 and Lynx
						{
							// indicate this session is not a robot
							session[ROBOT_KEY] = Boolean.FalseString;
							return false;
						}

						for (int i = 0; i < botAgents.Length; i++)
						{
							if (userAgent.IndexOf(botAgents[i]) != -1)
							{
								// set a key in the session, so the next time we don't have to manually
								// detect the robot again
								session[ROBOT_KEY] = Boolean.TrueString;
								return true;
							}
						}
					}

					// detect the robot from the host or user-agent
					string remoteHost = request.UserHostName; // requires one DNS lookup

					// if the DNS server didn't return a hostname, getRemoteHost returns the
					// IP address, which is ignored here (the last char is checked, because some
					// remote hosts begin with the IP)
					if (remoteHost != null && remoteHost.Length > 0 && remoteHost[remoteHost.Length - 1] > 64)
					{
						for (int i = 0; i < botHosts.Length; i++)
						{
							if (remoteHost.IndexOf(botHosts[i]) != -1)
							{
								// set a key in the session, so the next time we don't have to manually
								// detect the robot again
								session[ROBOT_KEY] = Boolean.TrueString;
								return true;
							}
						}
					}

					// remote host and user agent are not in the predefined list,
					// so it must be an unknown robot or not a robot

					// indicate this session is not a robot
					session[ROBOT_KEY] = Boolean.FalseString;
					return false;
				}
			}
		}
	}
}