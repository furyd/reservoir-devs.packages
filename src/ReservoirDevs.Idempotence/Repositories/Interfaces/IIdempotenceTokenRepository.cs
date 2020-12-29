using System.Threading.Tasks;
using ReservoirDevs.Idempotence.DataTransferObjects;

namespace ReservoirDevs.Idempotence.Repositories.Interfaces
{
    /// <summary>
    /// Interface to handle idemopotence token storage
    /// </summary>
    public interface IIdempotenceTokenRepository
    {
        /// <summary>
        /// Creates a new token
        /// </summary>
        /// <param name="token"></param>
        Task Create(string token);

        /// <summary>
        /// Retrieves a token
        /// </summary>
        /// <param name="token">Token value</param>
        /// <returns>Null, or an object representing the token</returns>
        Task<IdempotenceTokenDTO> Retrieve(string token);
    }
}