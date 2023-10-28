﻿using Transactions.API.Domain.Exceptions;

namespace Transaction.API.Domain.Exceptions
{
    public class UnauthorizedException : Exception
    {
        #region Variables

        private const string DefaultMessage = "Entity does not exist.";
        public string errorCat { get; }
        public string vendor { get; }
        public string lobSchemaName { get; }
        public int EntityId { get; }
        #endregion

        protected UnauthorizedException(string message) : base(message)
        {
        }
        public UnauthorizedException(string vendorCode, string lobName) : base(vendorCode)
        {
            errorCat = ErrorCategory.Unauthorized.ToString();
            vendor = vendorCode;
            lobSchemaName = lobName;
        }
    }
}
