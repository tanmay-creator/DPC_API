﻿namespace Transaction.API.Application.ModelDTOs.v1.Payment.Response.Registered
{
    public record Reg_CustomFields_Response_DTO
    {
        public string id { get; init; }
        public string value { get; init; }
    }
}
