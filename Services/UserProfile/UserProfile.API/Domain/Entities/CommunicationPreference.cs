namespace UserProfile.API.Domain.Entities
{
    public class CommunicationPreference
    {
        public EmailCommunicationPreference EmailCommunicationPreference { get; set; } 
        public SmsCommunicationPreference SmsCommunicationPreference { get; set; }
    }
}
