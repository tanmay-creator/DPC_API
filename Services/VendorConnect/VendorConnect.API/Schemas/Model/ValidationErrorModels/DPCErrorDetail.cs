﻿namespace VendorConnect.API.Schemas.Model.ValidationErrorModels
{
    public class DPCErrorDetail
    {
        public string ERROR_CODE { get; set; }
        public string ERROR_CATEGORY { get; set; }
        public string ERROR_DESCRIPTION { get; set; }
        public string DPC_ERROR_CODE { get; set; }
        public string DPC_ERROR_DESCRIPTION { get; set; }
    }
}
