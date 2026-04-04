namespace DogoFinance.BusinessLogic.Layer.Response
{
    public class TodoItem
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty; // e.g., "BVN_VERIFY", "NIN_VERIFY", "PIN_SETUP", "KIN_ADD"
        public string Icon { get; set; } = string.Empty;
    }
}
