namespace SportEventsAPI.Models
{
    public class CreateSportEventResponse
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public int OrganizerId { get; set; }
    }

}
