namespace UserProfile.API.Domain.Exceptions
{
    public class UserProfileRequestTimedOut : Exception
    {
        public UserProfileRequestTimedOut(Guid profileId)
           : base($"The Request to the user profile API timed out")
        {
        }
    }
}
