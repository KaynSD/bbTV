namespace blaseball.ui {
	/// <summary>
	/// A visual logger for the blaseball.
	/// </summary>
	public interface IUILogger
	{
		/// <summary>
		/// Log a line of text to the terminal user display
		/// </summary>
		/// <param name="line">Text to display</param>
		void Log(string line);
		
		/// <summary>
		/// Open the Logging window
		/// </summary>
		void Open();
		/// <summary>
		/// Close the Logging window (without clearing)
		/// </summary>
		void Close();
	}
}