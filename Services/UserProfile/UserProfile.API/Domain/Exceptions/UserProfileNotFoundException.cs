namespace UserProfile.API.Domain.Exceptions
{
    public class UserProfileServiceUnavailableException : Exception
    {
        public UserProfileServiceUnavailableException(Guid profileId)
           : base($"The user with the profileid {profileId} could not be retrieved.")
        {
        }
    }
}
