﻿namespace VendorConnect.API.Application.ModelDTOs.Payment.Response.Unregistered
{
    public record Unreg_ProcessorResponse_Response_DTO
    {
        public string responseCode { get; init; }
        public string authCode { get; init; }
        public string traceCode { get; init; }
    }
}
