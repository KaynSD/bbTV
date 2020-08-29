using blaseball.db;

namespace blaseball.file {
	public interface IFileLoader
	{
		void SaveLog(IBlaseballDatabase database, string id, string json);
		void SetupStreamingAssets(IBlaseballDatabase database);
	}
}