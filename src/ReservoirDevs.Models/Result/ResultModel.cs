using System;

namespace ReservoirDevs.Models.Result
{
    public record ResultModel
    {
        public Exception Exception { get; init; }

        public bool IsSuccessful => Exception == null;
    }
}
