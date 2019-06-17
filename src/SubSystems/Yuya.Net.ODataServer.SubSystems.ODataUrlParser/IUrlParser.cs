using Yuya.Net.ODataServer.SubSystems.ODataUrlParser.QueryDetail;

namespace Yuya.Net.ODataServer.SubSystems.ODataUrlParser
{
	public interface IUrlParser
	{
		QueryDetail.QueryDetail ParseQuery(string urlString, ParseOptions options);
	}
}