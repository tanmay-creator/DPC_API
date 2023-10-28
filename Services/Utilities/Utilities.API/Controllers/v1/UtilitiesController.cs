using Microsoft.Extensions.Options;
using System.Linq;

namespace Utilities.API.Controllers.v1
{
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        #region Variables
        private readonly string _ConcatValue = "-";
        private readonly string _ConcatLobEnrolledValue = "LOBEnrolled";
        private readonly string _ConcatPaymentValue = "Payment";
        private readonly string _ConcatCountryList = "CountryList";
        private readonly string _ConcatBankAccountTyep = "BankAccountType";
        private readonly string _ConcatStateList = "StateList";
        private readonly string _ConcatCityList = "CityList";
        private readonly string _ConcatErrorList = "APIErrorList";
        private readonly string _ConcatFileErrorList = "FileErrorList";
        private readonly string _IsActive = "Y";
        private readonly string _RequiredErrorType = "Required";
        private readonly double _APIErrorRedisTimeout;
        private readonly double _UIErrorRedisTimeout;
        private readonly double _FileErrorRedisTimeout;
        private readonly double _IsLobEnrolledRedisTimeout;
        private readonly double _LobPaymentMappingRedisTimeout;
        private readonly double _BankAccListRedisTimeout;
        private readonly double _CountryListRedisTimeout;
        private readonly double _StateListRedisTimeout;
        private readonly double _CityListRedisTimeout;
        #endregion

        private readonly ILogger<UtilitiesController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IRedisCache _redisCache;
        private readonly AppSettings _appSettings;

        public UtilitiesController(ILogger<UtilitiesController> logger, ApplicationDbContext db, IRedisCache redisCache, IOptionsSnapshot<AppSettings> appSettings)
        {
            _logger = logger;
            _db = db;
            _redisCache = redisCache;
            _appSettings = appSettings.Value;
            _APIErrorRedisTimeout = _appSettings.APIErrorRedisTimeout;
            _UIErrorRedisTimeout = _appSettings.UIErrorRedisTimeout;
            _FileErrorRedisTimeout = _appSettings.FileErrorRedisTimeout;
            _IsLobEnrolledRedisTimeout = _appSettings.IsLobEnrolledRedisTimeout;
            _LobPaymentMappingRedisTimeout = _appSettings.LobPaymentMappingRedisTimeout;
            _BankAccListRedisTimeout = _appSettings.BankAccListRedisTimeout;
            _CountryListRedisTimeout = _appSettings.CountryListRedisTimeout;
            _StateListRedisTimeout = _appSettings.StateListRedisTimeout;
            _CityListRedisTimeout = _appSettings.CityListRedisTimeout;
        }

        //[HttpGet]
        //[Route("LobSchemaNameTest")]
        //public void LobSchemaNameTest(string lobName) 
        //{
        //    // trial
        //    var met_sfd = "MET_SFD";
        //    var identifier = "LOBSchemaName_" + met_sfd;
        //    foreach (var lobschemaname in _appSettings)
        //    {

        //    }
        //}


        [HttpGet]
        [Route("/v{version:apiVersion}/api-error-list/{vendor-code}/api-errors/{lob-code}")]
        public async Task<IActionResult> GetAllAPIErrorDetails([FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            if (String.IsNullOrEmpty(vendorCode))
            {
                return BadRequest();
            }
            string apiErrorDetailKey = String.Concat(_ConcatErrorList, _ConcatValue, lobCode, _ConcatValue, vendorCode);
            string errorDetails = null;
            if (await _redisCache.isKeyExist(apiErrorDetailKey))
            {
                var cachedErrorData = _redisCache.GetCacheData(apiErrorDetailKey);

                if (cachedErrorData != null)
                {
                    errorDetails = cachedErrorData;
                    return Ok(errorDetails);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                //store error_category in enum
                var errorList = _db.T_VND_ERR_LKUP
                                   .Join(_db.T_VND_LKUP, vndMst => vndMst.VND_ID, parTbl => parTbl.VND_ID,
                                    (tb1, tb2) => new
                                    {
                                        ErrorSource = tb1.ERR_SRC.Trim(),
                                        ErrorCode = tb1.ERR_CD.Trim(),
                                        ErrorCategory = tb1.ERR_CTGY.Trim(),
                                        ErrorDescription = tb1.ERR_DESC.Trim(),
                                        DPCErrorCode = tb1.DPC_ERR_CD.Trim(),
                                        DPCErrorKind = tb1.DPC_ERR_KIND.Trim(),
                                        DPCErrorDescription = tb1.DPC_ERR_DESC.Trim(),
                                        VendorID = tb1.VND_ID,
                                        VendorCode = tb2.VND_CD.Trim(),
                                        ErrorActive = tb1.Row_Act_Ind.Trim()
                                    })
                                    .Where
                                    (
                                        err =>
                                        (err.VendorCode.Trim() == vendorCode.Trim()) &&
                                        (err.ErrorSource.Trim() == "api")
                                    )
                                    .ToList();
                if (errorList != null)
                {
                    errorDetails = JsonConvert.SerializeObject(errorList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_APIErrorRedisTimeout);
                    await _redisCache.SetCacheDataAsync(apiErrorDetailKey, errorDetails, expirationTime);

                    return Ok(errorDetails);

                }

                return BadRequest();

            }
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/api-error-details/{vendor-code}/api-errors/{error-category}/{lob-code}")]
        public async Task<IActionResult> GetAPIErrorDetails([FromRoute(Name = "error-category")] string errorCategory, [FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            IList<ErrorDetails> errorList = null;

            if (String.IsNullOrEmpty(vendorCode) || String.IsNullOrEmpty(errorCategory))
            {
                return BadRequest();
            }

            var errorDetails = GetAllAPIErrorDetails(vendorCode, lobCode);

            if (errorDetails == null || errorDetails.Result == null)
            {
                return BadRequest();
            }

            var errors = ((OkObjectResult)errorDetails.Result).Value.ToString();
            if (errors != null)
            {
                var err = JsonConvert.DeserializeObject<IList<ErrorDetails>>(errors);

                errorList = err.Where(errCat => errCat.ErrorCategory.Trim().ToLower() == errorCategory.Trim().ToLower()).ToList();
            }

            return Ok(errorList);
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/error-code-details/{vendor-error-code}/{vendor-code}/{lob-code}")]
        public IList<ErrorDetails> GetErrorCodeDetails([FromRoute(Name = "vendor-error-code")] string vendorErrorCode, [FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            IList<ErrorDetails> errorList = null;

            var errorDetails = GetAllAPIErrorDetails(vendorCode, lobCode);

            var errors = ((OkObjectResult)errorDetails.Result).Value.ToString();
            if (errors != null)
            {
                var err = JsonConvert.DeserializeObject<IList<ErrorDetails>>(errors);

                errorList = err.Where(errCat => errCat.ErrorCode.ToLower() == vendorErrorCode.ToLower()).ToList();
            }

            return errorList;
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/file-error-list/{vendor-code}/file-errors/{lob-code}")]
        public async Task<IActionResult> GetAllFileErrorDetails([FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            if (String.IsNullOrEmpty(vendorCode))
            {
                return BadRequest();
            }
            string fileErrorDetailKey = String.Concat(_ConcatFileErrorList, _ConcatValue, lobCode, _ConcatValue, vendorCode);
            string errorDetails = null;
            if (await _redisCache.isKeyExist(fileErrorDetailKey))
            {
                var cachedErrorData = _redisCache.GetCacheData(fileErrorDetailKey);

                if (cachedErrorData != null)
                {
                    errorDetails = cachedErrorData;
                    return Ok(errorDetails);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                //store error_category in enum
                var errorList = _db.T_VND_ERR_LKUP
                                   .Join(_db.T_VND_LKUP, vndMst => vndMst.VND_ID, parTbl => parTbl.VND_ID,
                                    (tb1, tb2) => new
                                    {
                                        ErrorSource = tb1.ERR_SRC.Trim(),
                                        ErrorCode = tb1.ERR_CD.Trim(),
                                        ErrorCategory = tb1.ERR_CTGY.Trim(),
                                        ErrorDescription = tb1.ERR_DESC.Trim(),
                                        DPCErrorCode = tb1.DPC_ERR_CD.Trim(),
                                        DPCErrorDescription = tb1.DPC_ERR_DESC.Trim(),
                                        VendorID = tb1.VND_ID,
                                        VendorCode = tb2.VND_CD.Trim(),
                                        ErrorActive = tb1.Row_Act_Ind.Trim()
                                    })
                                    .Where
                                    (
                                        err =>
                                        (err.VendorCode == vendorCode) &&
                                        (err.ErrorSource == "File")
                                    )
                                    .ToList();
                if (errorList != null)
                {
                    errorDetails = JsonConvert.SerializeObject(errorList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_FileErrorRedisTimeout);
                    await _redisCache.SetCacheDataAsync(fileErrorDetailKey, errorDetails, expirationTime);

                    return Ok(errorDetails);

                }

                return BadRequest();

            }
        }


        [HttpGet]
        [Route("/v{version:apiVersion}/ui-error-list/{vendor-code}/ui-errors/{lob-code}")]
        public async Task<IActionResult> GetAllUIErrorDetails([FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            if (String.IsNullOrEmpty(vendorCode))
            {
                return BadRequest();
            }
            string uiErrorDetailKey = String.Concat(_ConcatFileErrorList, _ConcatValue, lobCode, _ConcatValue, vendorCode);
            string errorDetails = null;
            if (await _redisCache.isKeyExist(uiErrorDetailKey))
            {
                var cachedErrorData = _redisCache.GetCacheData(uiErrorDetailKey);

                if (cachedErrorData != null)
                {
                    errorDetails = cachedErrorData;
                    return Ok(errorDetails);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                //store error_category in enum
                var errorList = _db.T_VND_ERR_LKUP
                                   .Join(_db.T_VND_LKUP, vndMst => vndMst.VND_ID, parTbl => parTbl.VND_ID,
                                    (tb1, tb2) => new
                                    {
                                        ErrorSource = tb1.ERR_SRC.Trim(),
                                        ErrorCode = tb1.ERR_CD.Trim(),
                                        ErrorCategory = tb1.ERR_CTGY.Trim(),
                                        ErrorDescription = tb1.ERR_DESC.Trim(),
                                        DPCErrorCode = tb1.DPC_ERR_CD.Trim(),
                                        DPCErrorDescription = tb1.DPC_ERR_DESC.Trim(),
                                        VendorID = tb1.VND_ID,
                                        VendorCode = tb2.VND_CD.Trim(),
                                        ErrorActive = tb1.Row_Act_Ind.Trim()
                                    })
                                    .Where
                                    (
                                        err =>
                                        (err.VendorCode == vendorCode) &&
                                        (err.ErrorSource == "UI")
                                    )
                                    .ToList();
                if (errorList != null)
                {
                    errorDetails = JsonConvert.SerializeObject(errorList);
                    var expirationTimee = DateTimeOffset.Now.AddMinutes(_UIErrorRedisTimeout);
                    await _redisCache.SetCacheDataAsync(uiErrorDetailKey, errorDetails, expirationTimee);

                    return Ok(errorDetails);

                }

                return BadRequest();

            }
        }


        [HttpGet]
        [Route("/v{version:apiVersion}/lob-configuration-details/{vendor-code}/{lob-code}")]
        public async Task<IActionResult> IsLobEnrolled([FromRoute(Name = "lob-code")] string lobCode, [FromRoute(Name = "vendor-code")] string vendorCode)
        {
            //string? isActive = string.Empty;
            string lobKey = String.Concat(_ConcatLobEnrolledValue, _ConcatValue, lobCode.Trim(), _ConcatValue, vendorCode.Trim());
            string lobDetails = null;
            //IList<DPCBillerMaster> dpcMasterList = null;

            if (String.IsNullOrEmpty(lobCode.Trim()) || String.IsNullOrEmpty(vendorCode.Trim()))
            {
                return BadRequest();
            }

            if (await _redisCache.isKeyExist(lobKey))
            {
                var cachedBillerData = _redisCache.GetCacheData(lobKey);

                if (cachedBillerData != null)
                {
                    lobDetails = cachedBillerData;
                    return Ok(lobDetails);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var lobDetailsList = _db.T_LOB_LKUP
                           .Join(_db.T_LOB_VND_MAP, lobLkup => lobLkup.LOB_ID, vendLkp => vendLkp.LOB_ID,
                           (tb1, tb2) => new
                           {
                               LobMst = tb1,
                               LobVendMap = tb2
                           })
                           .Join(_db.T_VND_LKUP, vndMst => vndMst.LobVendMap.VND_ID, parTbl => parTbl.VND_ID,
                           (tb1, tb2) => new
                           {
                               LobMster = tb1.LobMst,
                               VendMst = tb2
                           })
                           .Join(_db.T_LOB_CPBLTY_MAP, parTbl => parTbl.LobMster.LOB_ID, lobCap => lobCap.LOB_ID,
                           (tb1, tb2) => new
                           {
                               LobMst = tb1.LobMster,
                               LobCapMst = tb2,
                               VendCapMst = tb1.VendMst
                           })
                           .Join(_db.T_DPC_CPBLTY_LKUP, parTbl => parTbl.LobCapMst.CPBLTY_ID, capMst => capMst.CPBLTY_ID,
                           (tb1, tb2) => new
                           {
                               LobId = tb1.LobCapMst.LOB_ID,
                               LobCode = tb1.LobMst.LOB_CD.Trim(),
                               LobDesc = tb1.LobMst.LOB_Desc.Trim(),
                               VendorId = tb1.VendCapMst.VND_ID,
                               VendorCode = tb1.VendCapMst.VND_CD.Trim(),
                               LobCapID = tb2.CPBLTY_ID,
                               LobCapCode = tb2.CPBLTY_CD.Trim(),
                               LobCapDesc = tb2.CPBLTY_DESC.Trim(),
                               LobCapFrequency = tb2.CPBLTY_FREQ.Trim(),
                               LobActive = tb1.LobMst.Row_Act_Ind.Trim(),
                               VendActive = tb1.VendCapMst.Row_Act_Ind.Trim(),
                               LobCapMstActive = tb1.LobCapMst.Row_Act_Ind.Trim(),
                               CapActive = tb2.Row_Act_Ind.Trim()
                           })
                           .Where(
                                    lobDet => lobDet.LobCode == lobCode.Trim() &&
                                    lobDet.VendorCode == vendorCode.Trim() &&
                                    lobDet.LobActive == _IsActive &&
                                    lobDet.VendActive == _IsActive &&
                                    lobDet.LobCapMstActive == _IsActive &&
                                    lobDet.CapActive == _IsActive
                                 )
                           .ToList();

                if (lobDetailsList != null)
                {
                    lobDetails = JsonConvert.SerializeObject(lobDetailsList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_IsLobEnrolledRedisTimeout);

                    await _redisCache.SetCacheDataAsync(lobKey, lobDetails, expirationTime);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(lobDetails);

        }

        [HttpGet]
        [Route("/v{version:apiVersion}/payment-method-type/{vendor-code}/{lob-code}")]
        public async Task<IActionResult> GetLobPaymentMapping([FromRoute(Name = "lob-code")] string lobCode, [FromRoute(Name = "vendor-code")] string vendorCode)
        {

            string paymentMappingKey = String.Concat(_ConcatPaymentValue, _ConcatValue, lobCode, _ConcatValue, vendorCode);
            string paymentMappingDetails = null;

            //await IsBillerActive(billerId, lobSchemaName);

            if (String.IsNullOrEmpty(lobCode.Trim()) || String.IsNullOrEmpty(vendorCode.Trim()))
            {
                return BadRequest();
            }

            if (await _redisCache.isKeyExist(paymentMappingKey))
            {
                var cachedPaymentMapData = _redisCache.GetCacheData(paymentMappingKey);

                if (cachedPaymentMapData != null)
                {
                    paymentMappingDetails = cachedPaymentMapData;
                    return Ok(paymentMappingDetails);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var paymentMethodList = _db.T_LOB_LKUP
                           .Join(_db.T_LOB_VND_MAP, lobLkup => lobLkup.LOB_ID, vendLkp => vendLkp.LOB_ID,
                           (tb1, tb2) => new
                           {
                               LobMst = tb1,
                               LobVendMap = tb2
                           })
                            .Join(_db.T_VND_LKUP, vndMst => vndMst.LobVendMap.VND_ID, parTbl => parTbl.VND_ID,
                            (tb1, tb2) => new
                            {
                                LobMster = tb1.LobMst,
                                LobVendMap = tb1.LobVendMap,
                                VendMst = tb2
                            })
                            .Join(_db.T_LOB_VND_PYMT_MAP, lvMap => lvMap.LobVendMap.LOB_VND_MAP_ID, lvPay => lvPay.LOB_VND_MAP_ID,
                            (tb1, tb2) => new
                            {
                                lobVenPymt = tb2,
                                LobMstNew = tb1.LobMster,
                                VenMst = tb1.VendMst
                            })
                             .Join(_db.T_PYMT_TYP_LKUP, lvMap => lvMap.lobVenPymt.PYMT_TYP_ID, lvPay => lvPay.PYMT_TYP_ID,
                            (tb1, tb2) => new
                            {
                                PaymentCode = tb2.PYMT_TYP_CD.Trim(),
                                PaymentDesc = tb2.PYMT_TYP_DESC.Trim(),
                                PaymentMethodActive = tb2.Row_Act_Ind.Trim(),
                                LobCode = tb1.LobMstNew.LOB_CD.Trim(),
                                VendorCode = tb1.VenMst.VND_CD.Trim(),
                                LobActive = tb1.LobMstNew.Row_Act_Ind.Trim(),
                                VendActive = tb1.VenMst.Row_Act_Ind.Trim()
                            })
                            .Where(
                                    lobDet => lobDet.LobCode == lobCode.Trim() &&
                                    lobDet.VendorCode == vendorCode.Trim() &&
                                    lobDet.LobActive == _IsActive &&
                                    lobDet.VendActive == _IsActive &&
                                    lobDet.PaymentMethodActive == _IsActive


                                 )
                            .ToList();

                //var paymentMethodList = record.Select(payMethod => payMethod.paymentMethod).ToList();


                //DPCBillerMasterService dpcBillerMasterService = new DPCBillerMasterService(_dpcBillerMaster, _dpcBillerPaymentMapping);

                //var paymentMethodList = dpcBillerMasterService.GetBillerPaymentMethod(billerId);

                if (paymentMethodList != null)
                {
                    paymentMappingDetails = JsonConvert.SerializeObject(paymentMethodList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(30.0);

                    await _redisCache.SetCacheDataAsync(paymentMappingKey, paymentMappingDetails, expirationTime);
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok(paymentMappingDetails);
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/payment-sub-types/{payment-method-type}/{lob-code}")]
        public async Task<IActionResult> GetBankAccountList([FromRoute(Name = "payment-method-type")] string paymentMethodType, [FromRoute(Name = "lob-code")] string lobCode)
        {
            var bankAccountKey = String.Concat(_ConcatBankAccountTyep, _ConcatValue, lobCode, _ConcatValue, paymentMethodType);

            string bankAccountType = null;

            if (await _redisCache.isKeyExist(bankAccountKey))
            {
                var cachedBankAccountType = _redisCache.GetCacheData(bankAccountKey);

                if (cachedBankAccountType != null)
                {
                    bankAccountType = cachedBankAccountType;
                    return Ok(cachedBankAccountType);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var bankAccountTypeList = _db.T_PYMT_TYP_LKUP
                           .Join(_db.T_PYMT_SBTYP_MAP, pymLkp => pymLkp.PYMT_TYP_ID, pymSubTyp => pymSubTyp.PYMT_TYP_ID,
                           (tb1, tb2) => new
                           {
                               PymtLkp = tb1,
                               PymtSubtypeLkp = tb2
                           })
                            .Join(_db.T_PYMT_SBTYP_LKUP, subType => subType.PymtSubtypeLkp.PYMT_SBTYP_ID, parTbl => parTbl.PYMT_SBTYP_ID,
                            (tb1, tb2) => new
                            {
                                PaymentTypeCode = tb1.PymtLkp.PYMT_TYP_CD.Trim(),
                                PaymentSubTypeCode = tb2.PYMT_SBTYP_CD.Trim(),
                                PaymentSubTypeDesc = tb2.PYMT_SBTYP_DESC.Trim(),
                                PaymentSubTypeDsplVal = tb2.PYMT_SBTYP_DSPL_VAL.Trim(),
                                PaymentTypeActive = tb1.PymtLkp.Row_Act_Ind.Trim(),
                                PaymentSubTypeActive = tb2.Row_Act_Ind.Trim()
                            })
                            .Where(
                                    payType => payType.PaymentTypeCode == paymentMethodType.Trim() &&
                                    payType.PaymentTypeActive == _IsActive &&
                                    payType.PaymentSubTypeActive == _IsActive
                                 )
                            .ToList();


                if (bankAccountTypeList != null)
                {
                    bankAccountType = JsonConvert.SerializeObject(bankAccountTypeList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_BankAccListRedisTimeout);
                    await _redisCache.SetCacheDataAsync(bankAccountKey, JsonConvert.SerializeObject(bankAccountTypeList), expirationTime);

                }
                else
                {
                    BadRequest("BadRequest");
                }
            }

            return Ok(bankAccountType);
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/countries/{lob-code}")]
        public async Task<IActionResult> GetCountryList([FromRoute(Name = "lob-code")] string lobCode)
        {
            var countryListKey = String.Concat(_ConcatCountryList, _ConcatValue, lobCode);
            string countryMaster = null;

            if (await _redisCache.isKeyExist(countryListKey))
            {
                var cachedCountryList = _redisCache.GetCacheData(countryListKey);

                if (cachedCountryList != null)
                {
                    countryMaster = cachedCountryList;
                    return Ok(countryMaster);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var countryList = _db.T_CNTRY_LKUP
                                  .Select
                                   (
                                        cntry => new { cntry.CNTRY_ID, cntry.CNTRY_CD, cntry.CNTRY_DESC, cntry.Row_Act_Ind }
                                   )
                                  .Where
                                   (
                                        cntryLkp => cntryLkp.Row_Act_Ind == _IsActive
                                   )
                                  .ToList();

                if (countryList != null)
                {
                    countryMaster = JsonConvert.SerializeObject(countryList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_CountryListRedisTimeout);
                    await _redisCache.SetCacheDataAsync(countryListKey, countryMaster, expirationTime);
                }
                else
                {
                    throw new BadRequestException("BadRequest");
                }
            }

            return Ok(countryMaster);
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/countries/{lob-code}/{country-code}/states")]
        public async Task<IActionResult> GetSateList([FromRoute(Name = "country-code")] string countryCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            var stateListKey = String.Concat(_ConcatStateList, _ConcatValue, countryCode, _ConcatValue, lobCode);
            string stateMaster = null;

            if (await _redisCache.isKeyExist(stateListKey))
            {
                var cachedStateList = _redisCache.GetCacheData(stateListKey);

                if (cachedStateList != null)
                {
                    stateMaster = cachedStateList;
                    return Ok(stateMaster);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var stateList = _db.T_CNTRY_LKUP
                                .Join(_db.T_CNTRY_ST_MAP, cntryLkp => cntryLkp.CNTRY_ID, cntryStMap => cntryStMap.CNTRY_ID,
                                (tb1, tb2) => new
                                {
                                    CntryLkp = tb1,
                                    CntryStMap = tb2
                                })
                                .Join(_db.T_CNTRY_ST_LKUP, cntryLkp => cntryLkp.CntryStMap.CNTRY_ST_ID, stateLkp => stateLkp.CNTRY_ST_ID,
                                (tb1, tb2) => new
                                {
                                    CountryCode = tb1.CntryLkp.CNTRY_CD.Trim(),
                                    StateID = tb1.CntryStMap.CNTRY_ST_ID,
                                    StateCode = tb2.CNTRY_ST_CD.Trim(),
                                    StateCategory = tb2.CNTRY_ST_CTGY.Trim(),
                                    StateDescription = tb2.CNTRY_ST_DESC.Trim(),
                                    ZipCodeRange = tb2.ZIP_CD_RNG.Trim(),
                                    StateActive = tb2.Row_Act_Ind.Trim()
                                })
                                .Where
                                (
                                    stateLkp => stateLkp.StateActive.Trim() == _IsActive &&
                                                stateLkp.CountryCode.Trim() == countryCode
                                )
                                .ToList();

                if (stateList != null)
                {
                    stateMaster = JsonConvert.SerializeObject(stateList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_StateListRedisTimeout);
                    await _redisCache.SetCacheDataAsync(stateListKey, stateMaster,expirationTime);
                }
                else
                {
                    throw new BadRequestException("BadRequest");
                }
            }

            return Ok(stateMaster);
        }

        [HttpGet]
        [Route("/v{version:apiVersion}/countries/{lob-code}/{country-code}/states/{state-code}/cities")]
        public async Task<IActionResult> GetCityList([FromRoute(Name = "country-code")] string countryCode, [FromRoute(Name = "state-code")] string stateCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            var cityListKey = String.Concat(_ConcatCityList, _ConcatValue, stateCode, _ConcatValue, lobCode);
            string cityMaster = null;

            if (await _redisCache.isKeyExist(cityListKey))
            {
                var cachedCityList = _redisCache.GetCacheData(cityListKey);

                if (cachedCityList != null)
                {
                    cityMaster = cachedCityList;
                    return Ok(cityMaster);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var cityList = _db.T_CNTRY_ST_LKUP
                                .Join(_db.T_ST_CITY_MAP, stateLkp => stateLkp.CNTRY_ST_ID, cntryStMap => cntryStMap.CNTRY_ST_ID,
                                (tb1, tb2) => new
                                {
                                    CntryStMap = tb1,
                                    StCityMap = tb2
                                })
                                .Join(_db.T_CITY_LKUP, cntryLkp => cntryLkp.StCityMap.CITY_ID, cityLkp => cityLkp.CITY_ID,
                                (tb1, tb2) => new
                                {
                                    StateID = tb1.StCityMap.CNTRY_ST_ID,
                                    StateCode = tb1.CntryStMap.CNTRY_ST_CD.Trim(),
                                    CityID = tb2.CITY_ID,
                                    CityCode = tb2.CITY_CD.Trim(),
                                    CityDesc = tb2.CITY_DESC.Trim(),
                                    CityCategory = tb2.CITY_CTGY.Trim(),
                                    CityActive = tb2.Row_Act_Ind.Trim()
                                })
                                .Where
                                (
                                    cityLkp => cityLkp.CityActive.Trim() == _IsActive &&
                                               cityLkp.StateCode.Trim() == stateCode
                                )
                                .ToList();

                if (cityList != null)
                {
                    cityMaster = JsonConvert.SerializeObject(cityList);
                    var expirationTime = DateTimeOffset.Now.AddMinutes(_CityListRedisTimeout);
                    await _redisCache.SetCacheDataAsync(cityListKey, cityMaster,expirationTime);
                }
                else
                {
                    throw new BadRequestException("BadRequest");
                }
            }

            return Ok(cityMaster);
        }

        [HttpPost]
        [Route("/v{version:apiVersion}/validate-inbound-request/{vendor-code}/{lob-code}")]
        public async Task<IActionResult> ValidateRequest([FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode, [FromQuery(Name = "schema-name")] string schemaName)
        {
            var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            var jsonRequest = await reader.ReadToEndAsync();
            var jsondoc = JsonDocument.Parse(jsonRequest);
            var errors = ValidateInboundRequest.ValidateRequest(jsondoc, schemaName, _logger);
            //var validationErrors = ValidationErrors(errors);
            if (errors != null && errors.Count > 0)
            {
                return GetValidationErrorList(errors, vendorCode, lobCode);
            }
            else
            {
                return Ok();
            }
            //return validationErrors;
        }

        /// <summary>
        /// Get the validate error list
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="vendorCode"></param>
        /// <param name="lobSchemaName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("validate/get-error-details")]
        public IActionResult GetValidationErrorList(IList<ValidationError> errors, string vendorCode, string lobSchemaName)
        {
            #region Variables
            string _validationError = "ValidationError";
            List<string> lstValues = new List<string>();
            #endregion

            var errorDetails = GetAllAPIErrorDetails(vendorCode, lobSchemaName);
            var errorValue = ((OkObjectResult)errorDetails.Result).Value;
            var errorList = JsonConvert.DeserializeObject<IList<ErrorDetails>>(errorValue.ToString());

            var parentErrorList = errorList != null ?
                                  errorList.Where(f => f.ErrorCategory.Trim() == _validationError).FirstOrDefault() :
                                  null;

            DPCErrorModel rootError = new DPCErrorModel();
            Error error = new Error();
            error.status = StatusCodes.Status422UnprocessableEntity;
            error.code = parentErrorList != null ? parentErrorList.DPCErrorCode : string.Empty;
            error.type = parentErrorList != null ? parentErrorList.ErrorCategory : string.Empty;
            error.description = parentErrorList != null ? parentErrorList.DPCErrorDescription : string.Empty;

            errors.ToList().ForEach(err =>
            {
                lstValues = err.Value as List<string>;
                if (lstValues == null)
                {
                    lstValues = new List<string>();
                    lstValues.Add(err.Path);
                }

                var childErrList = errorList!
                                   .Where(
                                            f => (f.ErrorCategory.Trim() == err.ErrorType.ToString())
                                    );

                error.details.AddRange(lstValues.Select(errSchemaDet => new Details
                {
                    kind = childErrList.Select(x => x.DPCErrorKind).FirstOrDefault(),

                    target = (String.IsNullOrEmpty(err.Value.ToString()) ? errSchemaDet : String.Concat(err.Schema.Title, ".", errSchemaDet)),

                    message = new Message()
                    {
                        code = childErrList.Select(x => x.DPCErrorCode).FirstOrDefault(),
                        description = childErrList.Select(x => x.DPCErrorDescription).FirstOrDefault(),
                    }
                }));
            });

            rootError.error = error;

            return UnprocessableEntity(rootError);
        }

        /// <summary>
        /// Get the validate error list
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="vendorCode"></param>
        /// <param name="lobSchemaName"></param>
        /// <returns></returns>               
        [HttpPost]
        [Route("/v{version:apiVersion}/validation-error-list/vendor/{vendor-code}/{lob-code}")]
        public async Task<IActionResult> FormatVendorError([FromRoute(Name = "vendor-code")] string vendorCode, [FromRoute(Name = "lob-code")] string lobCode)
        {
            #region Variables
            string _validationError = "ValidationError";
            List<string> lstValues = new List<string>();
            string parentCode = String.Empty;
            string details = String.Empty;
            JArray jsonDetailArr = null;
            #endregion

            #region Read the request from body
            _logger.LogInformation("Start reading the vendor format error");
            var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false);
            var jsonRequest = await reader.ReadToEndAsync();
            _logger.LogInformation("End of reading the vendor format error");
            #endregion

            #region Read Vendor Error Details
            _logger.LogInformation("Read the error details");
            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(jsonRequest);


            parentCode = jsonElement.GetProperty("error").GetProperty("message").GetProperty("code").ToString();
            details = jsonElement.GetProperty("error").GetProperty("details").ToString();
            jsonDetailArr = JArray.Parse(details.ToString());

            if (parentCode == String.Empty || jsonDetailArr.Count == 0)
            {
                throw new NotFoundException("NotFound");
            }
            #endregion

            #region Create DPC Error Format
            _logger.LogInformation("Get Error Code Details for error code {0},VendorCode {1}, LobCode {2} ", parentCode, vendorCode, lobCode);
            var errorCode = GetErrorCodeDetails(parentCode, vendorCode, lobCode);

            DPCErrorModel rootError = new DPCErrorModel();
            Error error = new Error();
            error.status = StatusCodes.Status422UnprocessableEntity;
            error.code = errorCode.Select(c => c.DPCErrorCode).FirstOrDefault()!;
            error.type = _validationError;
            error.description = errorCode.Select(c => c.DPCErrorDescription).FirstOrDefault()!;
            List<Details> lstDetails = null;

            jsonDetailArr.ToList().ForEach(x =>
            {
                var errorData = GetErrorCodeDetails(Convert.ToString(((JValue)x["message"]!["code"]!).Value)!, vendorCode, lobCode);
                var detailKind = Convert.ToString(errorData.Select(cd => cd.DPCErrorKind).FirstOrDefault());
                var detailTarget = ((JValue)x["target"]!).Value;
                var detailcode = Convert.ToString(errorData.Select(cd => cd.DPCErrorCode).FirstOrDefault());
                var detaildesc = Convert.ToString(errorData.Select(cd => cd.DPCErrorDescription).FirstOrDefault());

                error.details.AddRange(lstDetails = new List<Details>()
                {
                   new Details
                   {
                        kind = Convert.ToString(detailKind),
                        target = Convert.ToString(detailTarget),
                        message = new Message
                        {
                            code = detailcode,
                            description = detaildesc
                        }
                   }
                });


            });

            rootError.error = error;
            #endregion

            return UnprocessableEntity(rootError);
        }


    }
}


