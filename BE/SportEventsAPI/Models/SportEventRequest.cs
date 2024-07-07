namespace SportEventsAPI.Models
{
    public class CreateSportEventRequest
    {
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public int OrganizerId { get; set; }
    }

    public class UpdateSportEventRequest
    {
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public int OrganizerId { get; set; }
    }
}
