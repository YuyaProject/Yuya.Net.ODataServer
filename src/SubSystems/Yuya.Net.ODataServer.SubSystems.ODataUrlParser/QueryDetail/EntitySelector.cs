namespace Yuya.Net.ODataServer.SubSystems.ODataUrlParser.QueryDetail
{
	public abstract class EntitySelector
	{
		protected EntitySelector(string entityName)
		{
			EntityName = entityName;
		}

		public string EntityName { get; }

	}
}