namespace FundingAccount.API.Application.ModelDTOs.v1.FundingAccount.Response.Registered
{
    public record Reg_GetAllFA_ResponseDTO
    {
        public int TotalCount { get; init; }
        public int Offset { get; init; }
        public int Count { get; init; }
        public List<Reg_Data_Response_DTO> Data { get; init; }
    }
}
