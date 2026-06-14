using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Auth
{
    public class GenericResponseDto
    {
        public bool IsSuccess { get; set; }
        public required string Message { get; set; }
    }

    public class GenericResponseDto<T> : GenericResponseDto
    {
        public T? Data { get; set; }
    }
}