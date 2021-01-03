using System;

namespace ReservoirDevs.Models.Result
{
    public abstract class ResultModelBase
    {
        public Exception Exception { get; protected set; }

        public bool IsSuccessful => Exception == null;
    }
}
