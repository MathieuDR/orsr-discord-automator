﻿using System.Threading.Tasks;
using WiseOldManConnector.Models;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace WiseOldManConnector.Interfaces {
    public interface IWiseOldManPlayerApi {
        #region search

        Task<ConnectorCollectionResponse<Player>> Search(string username);

        #endregion

        #region track

        Task<ConnectorResponse<Player>> Track(string username);

        #endregion

        #region view

        Task<ConnectorResponse<Player>> View(string username);
        Task<ConnectorResponse<Player>> View(int id);

        #endregion

        #region assert

        Task<ConnectorResponse<MessageResponse>> AssertPlayerType(string username);
        Task<ConnectorResponse<MessageResponse>> AssertDisplayName(string username);

        #endregion

        #region import

        Task<ConnectorResponse<MessageResponse>> Import(string username);

        #endregion

        #region competitions

        Task<ConnectorCollectionResponse<Competition>> Competitions(int id);
        Task<ConnectorCollectionResponse<Competition>> Competitions(string username);

        #endregion

        #region achievements

        Task<ConnectorCollectionResponse<Achievement>> Achievements(int id);
        Task<ConnectorCollectionResponse<Achievement>> Achievements(int id, bool includeMissing);
        Task<ConnectorCollectionResponse<Achievement>> Achievements(string username);
        Task<ConnectorCollectionResponse<Achievement>> Achievements(string username, bool includeMissing);

        #endregion

        #region snapshots

        Task<ConnectorResponse<Snapshots>> Snapshots(int id);
        Task<ConnectorResponse<Snapshots>> Snapshots(int id, Period period);
        Task<ConnectorResponse<Snapshots>> Snapshots(string username);
        Task<ConnectorResponse<Snapshots>> Snapshots(string username, Period period);

        #endregion


        #region gained

        Task<ConnectorCollectionResponse<Delta>> Gained(int id);
        Task<ConnectorResponse<Delta>> Gained(int id, Period period);
        Task<ConnectorCollectionResponse<Delta>> Gained(string username);
        Task<ConnectorResponse<Delta>> Gained(string username, Period period);

        #endregion

        #region records

        Task<ConnectorCollectionResponse<Record>> Records(int id);
        Task<ConnectorCollectionResponse<Record>> Records(int id, MetricType metric);
        Task<ConnectorCollectionResponse<Record>> Records(int id, Period period);
        Task<ConnectorCollectionResponse<Record>> Records(int id, MetricType metric, Period period);

        Task<ConnectorCollectionResponse<Record>> Records(string username);
        Task<ConnectorCollectionResponse<Record>> Records(string username, MetricType metric);
        Task<ConnectorCollectionResponse<Record>> Records(string username, Period period);
        Task<ConnectorCollectionResponse<Record>> Records(string username, MetricType metric, Period period);

        #endregion
    }
}