﻿namespace Transaction.API.Application.ModelDTOs.v1.Payment.Request.Registered
{
    public record Reg_Originator_Request_DTO
    {
        public string id { get; init; }
        public string kind { get; init; }
    }
}
