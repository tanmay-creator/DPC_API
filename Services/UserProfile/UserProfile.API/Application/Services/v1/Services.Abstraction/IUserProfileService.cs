using UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request;

namespace UserProfile.API.Application.Services.v1.Services.Abstraction
{
    public interface IUserProfileService
    {
        //Task<HttpResponseMessage> GetUserProfile(Guid profile_id, CancellationToken cancellationToken);
        Task<HttpResponseMessage> CreateUserProfile<T>(UserProfile_Request_DTO user, string vendorCode, string lobCode);
        Task<HttpResponseMessage> GetUserProfile<T>(string profileId, string vendorCode, string lobCode);
        Task<HttpResponseMessage> UpdateUserProfile<T>(string profileId, UserProfile_Request_DTO userProfile_UpdateRequestDTO, string vendorCode, string lobCode);
        Task<HttpResponseMessage> UpdateCommunicationPreferences<T>(string profileId, CommunicationPreference_Request_DTO comPref_UpdateRequestDTO, string vendorCode, string lobCode);

    }
}
