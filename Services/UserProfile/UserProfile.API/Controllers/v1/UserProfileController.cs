using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using UserProfile.API.Application.ModelDTOs.v1.UserProfile.Request;
using UserProfile.API.Application.ModelDTOs.v1.UserProfile.Response;
using UserProfile.API.Application.Services.v1.Services.Abstraction;
using UserProfile.API.Domain.Entities;
using UserProfile.API.Domain.Exceptions;
using UserProfile.API.UserProfile_Schema.Model.ValidationErrorModels;

namespace UserProfile.API.Controllers.v1
{
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    public class UserProfileController : Controller
    {
        #region Variables

        private readonly IUserProfileService _userprofileService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<UserProfileController> _logger;

        #region UserProfileVariables
        private HttpResponseMessage _userProfileReqValidation;
        private HttpResponseMessage _userProfileResValidation;
        private HttpResponseMessage _userProfileResponse;
        private string _userProfileReqValidationContent;
        private string _userProfileResValidationContent;
        private string _userProfileResponseContent;
        private const string _userProfileCreate_ReqSchemaName = "UserProfile/UserProfileCreate_RequestSchema.json";
        private const string _userProfileCreate_ResSchemaName = "UserProfile/UserProfileCreate_ResponseSchema.json";
        private const string _userProfileUpdate_ReqSchemaName = "UserProfile/UserProfileUpdate_RequestSchema.json";
        private const string _commPrefUpdate_ReqSchemaName = "UserProfile/ComPrefUpdate_Schema.json";
        private const string _commPrefUpdate_ResSchemaName = "UserProfile/ComPrefUpdate_Schema.json";
        #endregion

        #endregion

        public AppSettings _appSettings { get; }

        public UserProfileController(IUserProfileService userprofileService, IUtilityService utilityService, ILogger<UserProfileController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _userprofileService = userprofileService;
            _logger = logger;
            _utilityService = utilityService;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        #region Create User Profile

        [HttpPost]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active/{biller-id}/{lob-name}")]
        [Route("CreateUserProfile")]

        public async Task<IActionResult> CreateUserProfile()
        {

            _logger.LogInformation("UserProfileController :: Request for creating user profile  has started for the user");

            // Get header values
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                string userProfileBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                userProfileBody = await reader.ReadToEndAsync();
                if(userProfileBody == null)
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }
                JObject data = JObject.Parse(userProfileBody);

                // Request validation
                _userProfileReqValidation = await _utilityService.ValidateRequestResponse(userProfileBody, _userProfileCreate_ReqSchemaName, vendorCode, lobCode);
                _userProfileReqValidationContent = await _userProfileReqValidation.Content.ReadAsStringAsync();

                if (_userProfileReqValidation.StatusCode == HttpStatusCode.OK)
                {
                    UserProfile_Request_DTO userProfileRequest_Dto = JsonConvert.DeserializeObject<UserProfile_Request_DTO>(userProfileBody);
                    _userProfileResponse = await _userprofileService.CreateUserProfile<HttpResponseMessage>(userProfileRequest_Dto, vendorCode, lobCode);

                    _userProfileResponseContent = await _userProfileResponse.Content.ReadAsStringAsync();
                    if (_userProfileResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileResponseContent);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    
                    if (_userProfileResponseContent == null)
                    {
                        throw new NotFoundException(vendorCode, lobCode);
                    }

                    // Successful user profile creation
                    if (_userProfileResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Response validation
                        _userProfileResValidation = await _utilityService.ValidateRequestResponse(_userProfileResponseContent, _userProfileCreate_ResSchemaName, vendorCode, lobCode);
                        _userProfileResValidationContent = await _userProfileResValidation.Content.ReadAsStringAsync();

                        if (_userProfileResValidation.StatusCode == HttpStatusCode.OK)
                        {
                            UserProfile_Response_DTO _userProfileResponse_Dto = JsonConvert.DeserializeObject<UserProfile_Response_DTO>(_userProfileResponseContent);
                            _logger.LogInformation("UserProfileController::User Profile created successfully.");
                            return new ObjectResult(_userProfileResponse_Dto);
                        }
                        //else if (_userProfileResValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        //{
                        //    RootError resValidationErrors = JsonConvert.DeserializeObject<RootError>(_userProfileResValidationContent);
                        //    return StatusCode(422, resValidationErrors);
                        //}
                        else
                        {

                            DPCErrorModel errorContent = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileResValidationContent);
                            var errorResponse = new ObjectResult(errorContent);
                            errorResponse.StatusCode = errorContent.error.status;
                            return errorResponse;

                        }
                    }
                    else
                    {
                        throw new BadRequestException(vendorCode, lobCode);
                    }
                }
                // Request is not valid
                else
                {
                    DPCErrorModel reqValidationErrors = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileReqValidationContent);
                    return StatusCode(422, reqValidationErrors);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(vendorCode, lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Get User Profile

        [HttpGet]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active/{biller-id}/{lob-name}")]
        [Route("GetUserProfile/{profile_id}")]
        public async Task<IActionResult> GetUserProfile(string profile_id)
        {

            _logger.LogInformation("UserProfileController::GetUserProfile called for profile id : {profile_id}  ", profile_id);

            // Get header values
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                _userProfileResponse = await _userprofileService.GetUserProfile<HttpResponseMessage>(profile_id, vendorCode, lobCode);
                _userProfileResponseContent = await _userProfileResponse.Content.ReadAsStringAsync();


                if (_userProfileResponseContent == null)
                {
                    throw new NotFoundException(vendorCode, lobCode);
                }
                if (_userProfileResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("UserProfileController::User profile fetched successfully");
                    UserProfile_Response_DTO userProfile = JsonConvert.DeserializeObject<UserProfile_Response_DTO>(_userProfileResponseContent);
                    return Ok(userProfile);
                }
                else
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(vendorCode, lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion


        #region Update User Profile

        [HttpPatch]
        //[Route("api/v{version:apiVersion}/profiles/{profile_id}")]
        //[Route("api/v{version:apiVersion}/[controller]/billers/active/{biller-id}/{lob-name}")]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(string profileId)
        {
           
                _logger.LogInformation("UserProfileController::UpdateUserProfile called for given profile id : ");

                // Get header values
                var vendorCode = Request.Headers["Vendor-Code"];
                var lobCode = Request.Headers["Lob-Code"];
            try { 
                string userProfileBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                userProfileBody = await reader.ReadToEndAsync();
                if(userProfileBody  == null)
                {
                    throw new BadRequestException(vendorCode,lobCode);
                }
                // Request validation
                _userProfileReqValidation = await _utilityService.ValidateRequestResponse(userProfileBody, _userProfileUpdate_ReqSchemaName, vendorCode, lobCode);
                _userProfileReqValidationContent = await _userProfileReqValidation.Content.ReadAsStringAsync();

                if (_userProfileReqValidation.StatusCode == HttpStatusCode.OK)
                {
                    UserProfile_Request_DTO userProfileRequest_Dto = JsonConvert.DeserializeObject<UserProfile_Request_DTO>(userProfileBody);
                    _userProfileResponse = await _userprofileService.UpdateUserProfile<HttpResponseMessage>(profileId, userProfileRequest_Dto, vendorCode, lobCode);
                    _userProfileResponseContent = await _userProfileResponse.Content.ReadAsStringAsync();

                    if (_userProfileResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileResponseContent);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }
                    if (_userProfileResponseContent == null)
                    {
                        throw new NotFoundException(vendorCode,lobCode);
                    }
                    // Successful user profile creation
                    if (_userProfileResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Response validation
                        _userProfileResValidation = await _utilityService.ValidateRequestResponse(_userProfileResponseContent, _userProfileCreate_ResSchemaName, vendorCode, lobCode);
                        _userProfileResValidationContent = await _userProfileResValidation.Content.ReadAsStringAsync();

                        if (_userProfileResValidation.StatusCode == HttpStatusCode.OK)
                        {
                            UserProfile_Response_DTO _userProfileResponse_Dto = JsonConvert.DeserializeObject<UserProfile_Response_DTO>(_userProfileResponseContent);
                            _logger.LogInformation("UserProfileController::User Profile updated successfully.");
                            return new ObjectResult(_userProfileResponse_Dto);
                        }
                        //else if (_userProfileResValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        //{
                        //    var errors = JsonConvert.DeserializeObject<RootError>(_userProfileResValidationContent);
                        //    return StatusCode(422, errors);
                        //}
                        else
                        {

                            DPCErrorModel errorContent = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileResValidationContent);
                            var errorResponse = new ObjectResult(errorContent);
                            errorResponse.StatusCode = errorContent.error.status;
                            return errorResponse;
                        }
                    }
                    else
                    {
                        var errorResponse = new ObjectResult(_userProfileResponseContent);
                        errorResponse.StatusCode = (int)_userProfileResponse.StatusCode;
                        return errorResponse; ;
                    }
                }
                // Request is not valid
                else
                {
                    DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileReqValidationContent);
                    return StatusCode(422, errors);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(vendorCode, lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update Communication Preferences

        [HttpPatch]
        //[Route("api/v{version:apiVersion}/profiles/{profile_id}")]
        //[Route("/v{version:apiVersion}/user-profile/communication-preferences/{profile-id}/{kind}")]
        [Route("UpdateCommunicationPreferences/{profile-id}/{kind}")]
        public async Task<IActionResult> UpdateCommunicationPreferences([FromRoute(Name = "profile-id")] string profileId, [FromRoute(Name = "kind")] string kind)
        {

            _logger.LogInformation("UserProfileController::Update communication preferences called for given profile id and kind: ");

            // Get header values
            var vendorCode = Request.Headers["Vendor-Code"];
            var lobCode = Request.Headers["Lob-Code"];
            try
            {
                string commPrefBody = string.Empty;
                var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
                commPrefBody = await reader.ReadToEndAsync();
                if(commPrefBody == null)
                {
                    throw new BadRequestException(vendorCode, lobCode);
                }
                // Request validation
                _userProfileReqValidation = await _utilityService.ValidateRequestResponse(commPrefBody, _commPrefUpdate_ReqSchemaName, vendorCode, lobCode);
                _userProfileReqValidationContent = await _userProfileReqValidation.Content.ReadAsStringAsync();

                if (_userProfileReqValidation.StatusCode == HttpStatusCode.OK)
                {
                    CommunicationPreference_Request_DTO comPrefRequest = JsonConvert.DeserializeObject<CommunicationPreference_Request_DTO>(commPrefBody);
                    _userProfileResponse = await _userprofileService.UpdateCommunicationPreferences<HttpResponseMessage>(profileId, comPrefRequest, vendorCode, lobCode);
                    _userProfileResponseContent = await _userProfileResponse.Content.ReadAsStringAsync();

                    if (_userProfileResponse.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        DPCErrorModel errorDetails = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileResponseContent);
                        var formatedRes = new ObjectResult(errorDetails);
                        formatedRes.StatusCode = errorDetails.error.status;
                        return formatedRes;
                    }

                    if (_userProfileResponseContent == null)
                    {
                        throw new NotFoundException(vendorCode, lobCode);
                    }
                    // Successful user profile creation
                    if (_userProfileResponse.StatusCode == HttpStatusCode.OK)
                    {
                        // Response validation
                        _userProfileResValidation = await _utilityService.ValidateRequestResponse(_userProfileResponseContent, _commPrefUpdate_ResSchemaName, vendorCode, lobCode);
                        _userProfileResValidationContent = await _userProfileResValidation.Content.ReadAsStringAsync();

                        if (_userProfileResValidation.StatusCode == HttpStatusCode.OK)
                        {
                            CommunicationPreference_Response_DTO comPrefResponse = JsonConvert.DeserializeObject<CommunicationPreference_Response_DTO>(_userProfileResponseContent);
                            _logger.LogInformation("UserProfileController::User Profile updated successfully.");
                            return new ObjectResult(comPrefResponse);
                        }
                        //else if (_userProfileResValidation.StatusCode == HttpStatusCode.UnprocessableEntity)
                        //{
                        //    var errors = JsonConvert.DeserializeObject<RootError>(_userProfileResValidationContent);
                        //    return StatusCode(422, errors);
                        //}
                        else
                        {
                            DPCErrorModel errorContent = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileResValidationContent);
                            var errorResponse = new ObjectResult(errorContent);
                            errorResponse.StatusCode = errorContent.error.status;
                            return errorResponse;

                        }
                    }
                    else
                    {
                       throw new BadRequestException(vendorCode, lobCode);
                    }
                }
                // Request is not valid
                else
                {
                    DPCErrorModel errors = JsonConvert.DeserializeObject<DPCErrorModel>(_userProfileReqValidationContent);
                    return StatusCode(422, errors);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceNotAvailableException(vendorCode, lobCode);
            }
            catch (JsonReaderException ex)
            {
                throw new BadRequestException(vendorCode, lobCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
