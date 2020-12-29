using System;
using System.ComponentModel.DataAnnotations;

namespace ReservoirDevs.Idempotence.DataTransferObjects
{
    public class IdempotenceTokenDTO
    {
        [Key]
        public string Token { get; set; }
        
        public DateTime Created { get; set; }
    }
}