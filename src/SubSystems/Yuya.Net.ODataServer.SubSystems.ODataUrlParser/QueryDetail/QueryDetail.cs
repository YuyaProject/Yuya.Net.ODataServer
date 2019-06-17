using System;
using System.Collections.Generic;
using System.Text;

namespace Yuya.Net.ODataServer.SubSystems.ODataUrlParser.QueryDetail
{
	public class QueryDetail
	{

		public QueryDetail(ParseOptions parseOptions)
		{
			ParseOptions = parseOptions;
		}

		public ParseOptions ParseOptions { get; }

		public List<EntitySelector> EntitySelectors { get; } = new List<EntitySelector>();
		public List<string> ColumnSelectors { get; } = new List<string>();

		public int? Top { get; set; }
		public int? Skip { get; set; }
	}
}
