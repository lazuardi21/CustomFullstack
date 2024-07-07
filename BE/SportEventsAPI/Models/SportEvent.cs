namespace SportEventsAPI.Models
{
    public class SportEvent
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public Organizer Organizer { get; set; }
        public int OrganizerId { get; internal set; }
    }

}
