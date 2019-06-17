namespace Yuya.Net.ODataServer.SubSystems.ODataUrlParser.QueryDetail
{
	public class SingleEntitySelector : EntitySelector
	{
		public SingleEntitySelector(string entityName, string key) : base(entityName)
		{
			Key = key;
		}

		public string Key { get; }

	}
}