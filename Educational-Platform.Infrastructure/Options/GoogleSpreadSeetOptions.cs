namespace Educational_Platform.Infrastructure.Options
{
	public class GoogleSpreadSeetOptions
	{
		public string SheetId { get; set; }
		public string ReadOnlyGridName { get; set; }
		public string WriteOnlyGridName { get; set; }
		public string AppName { get; set; }
        public int GridId { get; set; }
	}

}
