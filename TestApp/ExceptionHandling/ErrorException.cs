using System;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.ExceptionHandling
{
    public class ErrorException : Exception
    {
        public ErrorModel Error { get; set; }

        public ErrorException(ErrorCode errorCode, string message)
        {
            Error = new ErrorModel
            {
                ErrorCode = errorCode,
                Message = message
            };
        }
    }
}
