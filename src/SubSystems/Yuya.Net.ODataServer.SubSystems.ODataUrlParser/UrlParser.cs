using System;
using System.Text.RegularExpressions;
using Yuya.Net.ODataServer.SubSystems.ODataUrlParser.QueryDetail;

namespace Yuya.Net.ODataServer.SubSystems.ODataUrlParser
{
	public class UrlParser : IUrlParser
	{
		private static readonly Regex _entityWithKeyFormat =
			new Regex("^([a-z][a-z0-9_-]+)(\\((.*?)\\))?$"
				, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

		private static readonly Regex _queryStringFormat =
			new Regex("^(\\$select|\\$filter|\\$top|\\$skip|@[a-z] [a-z0-9_]+)=(.+)$"
				, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

		public QueryDetail.QueryDetail ParseQuery(string urlString, ParseOptions options)
		{
			if (urlString == null) throw new ArgumentNullException(nameof(urlString));
			if (string.IsNullOrWhiteSpace(urlString)) throw new ArgumentException(nameof(urlString));

			var urlStart = $"{options.BaseUrl.TrimEnd('/')}/{options.ODataDirectory.Trim('/')}";

			if (!urlString.StartsWith(urlStart)) throw new ArgumentException("Wrong Format", nameof(urlString));

			Uri url;
			var result = new QueryDetail.QueryDetail(options);
			try
			{
				url = new Uri(urlString);
			}
			catch (UriFormatException exception)
			{
				throw new ArgumentException("Wrong Format", nameof(urlString), exception);
			}

			var firstSection = url.AbsoluteUri.Substring(urlStart.Length).Trim('/');
			if (!string.IsNullOrWhiteSpace(url.Query))
			{
				firstSection = firstSection.Replace(url.Query, "");
			}

			if (!firstSection.Contains("/"))
			{
				if (!WorkForEntitySection(result, firstSection)) throw new ArgumentException("Wrong entity format", nameof(urlString));
			}
			else
			{
				var entitySections = firstSection.Split('/');
				foreach (var s in entitySections)
				{
					if (!WorkForEntitySection(result, s)) throw new ArgumentException("Wrong entity format", nameof(urlString));
				}
			}

			var query = string.IsNullOrWhiteSpace(url.Query) ? null : url.Query.TrimStart('?');

			if (!string.IsNullOrWhiteSpace(query))
			{
				var parameters = query.Split('&');
				foreach (var parameter in parameters)
				{
					var match = _queryStringFormat.Match(parameter);
					if (!match.Success)
					{
						throw new ArgumentException("Wrong query format", nameof(urlString));
					}

					if (match.Groups[1].Value.StartsWith("$"))
					{
						int r;
						switch (match.Groups[1].Value)
						{
							case "$select":
								result.ColumnSelectors.AddRange(match.Groups[2].Value.Split(','));
								break;
							case "$top":
								if(int.TryParse(match.Groups[2].Value, out r))
								{
									result.Top = r;
								}
								else
								{
									throw new ArgumentException("wrong $top value", nameof(urlString));
								}
								break;
							case "$skip":
								if (int.TryParse(match.Groups[2].Value, out r))
								{
									result.Skip = r;
								}
								else
								{
									throw new ArgumentException("wrong $skip value", nameof(urlString));
								}
								break;
						}
					}
					else if (match.Groups[1].Value.StartsWith("@"))
					{

					}
				}
			}

			return result;
		}

		private static bool WorkForEntitySection(QueryDetail.QueryDetail result, string firstSection)
		{
			var match = _entityWithKeyFormat.Match(firstSection);
			if (match.Success)
			{
				if (match.Groups[2].Success
					&& !string.IsNullOrWhiteSpace(match.Groups[2].Value)
					&& match.Groups[3].Success
					&& !string.IsNullOrWhiteSpace(match.Groups[3].Value))
				{
					result.EntitySelectors.Add(new SingleEntitySelector(match.Groups[1].Value, match.Groups[3].Value));
				}
				else
				{
					result.EntitySelectors.Add(new ListEntitySelector(firstSection));
				}
				return true;
			}
			return false;
		}
	}
}