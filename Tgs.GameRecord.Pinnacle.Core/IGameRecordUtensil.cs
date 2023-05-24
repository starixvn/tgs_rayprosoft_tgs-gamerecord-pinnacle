using Tgs.GameInterface.Pinnacle.Protos;
using Tgs.GameRecord.Pinnacle.Core.Models;

namespace Tgs.GameRecord.Pinnacle.Core;

public interface IGameRecordUtensil
{
    public ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GameRecordInfo source,
        CancellationToken cancellationToken = default);
}