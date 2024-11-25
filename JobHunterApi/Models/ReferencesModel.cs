namespace JobHunterApi.Models
{
    public class ReferencesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Organization { get; set; } =string.Empty;
        public string Email { get; set; }=string.Empty;
        public string ?Link { get; set; }=string.Empty;
        public string ?Position { get; set; }=string.Empty;

    }
}