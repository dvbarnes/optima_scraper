using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace optima_tracking.Data
{
    public interface IUnitDatastore
    {
        Task SaveAsync(UnitData unitData, CancellationToken token = default);
        Task<IReadOnlyList<UnitData>> ReadAllDataAsync(CancellationToken token = default);
    }
}
