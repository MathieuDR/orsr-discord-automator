using RestSharp;
using WiseOldManConnector.Helpers;
using WiseOldManConnector.Interfaces;
using WiseOldManConnector.Models;
using WiseOldManConnector.Models.API.Responses;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace WiseOldManConnector.Api;

internal class RecordConnector : BaseConnecter, IWiseOldManRecordApi {
    public RecordConnector(IServiceProvider provider) : base(provider) {
        Area = "records";
    }

    protected override string Area { get; }


    public Task<ConnectorCollectionResponse<Record>> View(MetricType metric, Period period) {
        return QueryRecords(metric, period);
    }

    public Task<ConnectorCollectionResponse<Record>> View(MetricType metric, Period period, PlayerType playerType) {
        return QueryRecords(metric, period, playerType);
    }


    private async Task<ConnectorCollectionResponse<Record>> QueryRecords(MetricType metric, Period period,
        PlayerType? playerType = null) {
        var request = GetNewRestRequest("/leaderboard");
        
        request.AddOrUpdateParameter("metric", metric.ToEnumMemberOrDefault());

        if (playerType.HasValue) {
            request.AddOrUpdateParameter("playerType", playerType.Value.ToEnumMemberOrDefault());
        }

        request.AddOrUpdateParameter("period", period.ToEnumMemberOrDefault());
        var queryResponse = await ExecuteCollectionRequest<WOMRecord>(request);
        var response = GetResponse<WOMRecord, Record>(queryResponse);

        //foreach (Record record in response.Data) {
        //    record.Period = period;
        //}

        return response;
    }
}
