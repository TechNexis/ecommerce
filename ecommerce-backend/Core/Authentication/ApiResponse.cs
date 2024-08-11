using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Authentication
{
    public  class ApiResponse
    {
        public int Status { get; set; } //= string.Empty;
        public string Message { get; set; }=string.Empty;
        public ApiResponse(int statusCode, string message = "")
        {
            Status = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have mode",
                401 => "Authorized,you are not",
                404 => "Resource found,it was not",
                500 => "Errors are the path to the dark side. Errors lead to anger",
                _ => ""
            };

        }
    }
}
