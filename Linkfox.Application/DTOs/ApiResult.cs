using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkFox.Application.DTOs
{
    /// <summary>
    /// This class is used to wrap the result. 
    /// </summary>
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public static Result<T> Ok(T data) => new Result<T> { Success = true, Data = data };
        public static Result<T> Fail(string errorCode, string message) =>
            new Result<T> { Success = false, ErrorCode = errorCode, ErrorMessage = message };
    }
}
