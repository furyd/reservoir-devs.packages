using System;

namespace ReservoirDevs.Models.Result
{
    public class ResultModel : ResultModelBase
    {
        public ResultModel() { }

        public ResultModel(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}
