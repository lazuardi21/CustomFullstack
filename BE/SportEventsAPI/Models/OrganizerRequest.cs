namespace SportEventsAPI.Models
{
    public class CreateOrganizerRequest
    {
        public string OrganizerName { get; set; }
        public string ImageLocation { get; set; }
    }

    public class UpdateOrganizerRequest
    {
        public string OrganizerName { get; set; }
        public string ImageLocation { get; set; }
    }

}
