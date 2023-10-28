﻿namespace VendorConnect.API.Application.ModelDTOs.FundingAccount.Request
{
    public record BillingAddress_Request_DTO
    {
        public string[] lines { get; init; }
        public string city { get; init; }
        public string regionCode { get; init; }
        public string postalCode { get; init; }
        public string countryCode { get; init; }
    }
}
