﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using WiseOldManConnector.Helpers;
using WiseOldManConnector.Interfaces;
using WiseOldManConnector.Models;
using WiseOldManConnector.Models.API.Responses;
using WiseOldManConnector.Models.API.Responses.Models;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;
using Record = WiseOldManConnector.Models.Output.Record;
using Snapshot = WiseOldManConnector.Models.Output.Snapshot;

namespace WiseOldManConnector.Api {
    internal class PlayerConncetor : BaseConnecter, IWiseOldManPlayerApi {
        public PlayerConncetor(IServiceProvider provider) : base(provider) {
            Area = "players";
        }

        protected override string Area { get; }

        #region search

        public async Task<ConnectorCollectionResponse<Player>> Search(string username) {
            var request = GetNewRestRequest("search");
            request.AddParameter("username", username);

            var result = await ExecuteCollectionRequest<SearchResponse>(request);
            return GetResponse<SearchResponse, Player>(result);
        }

        #endregion

        #region track

        public async Task<ConnectorResponse<Player>> Track(string username) {
            var request = GetNewRestRequest("track");
            request.Method = Method.POST;
            username = username.ToLowerInvariant();
            request.AddJsonBody(new {username});

            PlayerResponse result = await ExecuteRequest<PlayerResponse>(request);
            return GetResponse<Player>(result);
        }

        #endregion

        #region import
        public async Task<ConnectorResponse<MessageResponse>> Import(string username) {
            var request = GetNewRestRequest("import");
            request.Method = Method.POST;
            request.AddJsonBody(new {username});

            var result = await ExecuteRequest<WOMMessageResponse>(request);
            return GetResponse<MessageResponse>(result);
        }
        #endregion

        #region competitions

        public async Task<ConnectorCollectionResponse<Competition>> Competitions(int id) {
            var request = GetNewRestRequest("/{id}/competitions");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            IEnumerable<CompetitionResponse> result = await ExecuteCollectionRequest<CompetitionResponse>(request);
            return GetResponse<CompetitionResponse, Competition>(result);
        }

        public async Task<ConnectorCollectionResponse<Competition>> Competitions(string username) {
            var request = GetNewRestRequest("/username/{username}/competitions");
            request.AddParameter("username", username, ParameterType.UrlSegment);

            var result = await ExecuteCollectionRequest<CompetitionResponse>(request);
            return GetResponse<CompetitionResponse, Competition>(result);
        }

        #endregion

        #region asserting

        public async Task<ConnectorResponse<PlayerType>> AssertPlayerType(string username) {
            var request = GetNewRestRequest("/assert-type");
            request.Method = Method.POST;
            request.AddJsonBody(new {username});

            var result = await ExecuteRequest<AssertPlayerTypeResponse>(request);
            return GetResponse<PlayerType>(result);
        }

        public async Task<ConnectorResponse<string>> AssertDisplayName(string username) {
            var request = GetNewRestRequest("/assert-name");
            request.Method = Method.POST;
            request.AddJsonBody(new {username});

            var result = await ExecuteRequest<AssertDisplayNameResponse>(request);
            return GetResponse<string>(result);
        }

        #endregion

        #region achievements

        public Task<ConnectorCollectionResponse<Achievement>> Achievements(int id) {
            return Achievements(id, false);
        }

        public async Task<ConnectorCollectionResponse<Achievement>> Achievements(int id, bool includeMissing) {
            var request = GetNewRestRequest("/{id}/achievements");
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("includeMissing", includeMissing);

            var result = await ExecuteCollectionRequest<WOMAchievement>(request);
            return GetResponse<WOMAchievement, Achievement>(result);
        }

        public Task<ConnectorCollectionResponse<Achievement>> Achievements(string username) {
            return Achievements(username, false);
        }

        public async Task<ConnectorCollectionResponse<Achievement>> Achievements(string username, bool includeMissing) {
            var request = GetNewRestRequest("/username/{username}/achievements");
            request.AddParameter("username", username, ParameterType.UrlSegment);
            request.AddParameter("includeMissing", includeMissing);

            var result = await ExecuteCollectionRequest<WOMAchievement>(request);
            return GetResponse<WOMAchievement, Achievement>(result);
        }

        #endregion

        #region snapshots

        public async Task<ConnectorResponse<Snapshots>> Snapshots(int id) {
            var request = GetNewRestRequest("/{id}/snapshots"); 
            request.AddParameter("id", id, ParameterType.UrlSegment);
            
            var result = await ExecuteRequest<SnapshotsResponse>(request);


            return GetResponse<Snapshots>(result);
        }

        public async Task<ConnectorCollectionResponse<Snapshot>> Snapshots(int id, Period period) {
            var request = GetNewRestRequest("/{id}/snapshots");
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("period", period.GetEnumValueNameOrDefault());

            var result = await ExecuteCollectionRequest<WOMSnapshot>(request);
            return GetResponse<WOMSnapshot, Snapshot>(result);
        }

        public async Task<ConnectorResponse<Snapshots>> Snapshots(string username) {
            var request = GetNewRestRequest("/username/{username}/snapshots");
            request.AddParameter("username", username, ParameterType.UrlSegment);

            var result = await ExecuteRequest<SnapshotsResponse>(request);
            return GetResponse<Snapshots>(result);
        }

        public async Task<ConnectorCollectionResponse<Snapshot>> Snapshots(string username, Period period) {
            var request = GetNewRestRequest("/username/{username}/snapshots");
            request.AddParameter("username", username, ParameterType.UrlSegment);
            request.AddParameter("period", period.GetEnumValueNameOrDefault());

            var result = await ExecuteCollectionRequest<WOMSnapshot>(request);
            return GetResponse<WOMSnapshot, Snapshot>(result);
        }

        #endregion

        #region gained

        public async Task<ConnectorCollectionResponse<Deltas>> Gained(int id) {
            var request = GetNewRestRequest("/{id}/gained");
            request.AddParameter("id", id, ParameterType.UrlSegment);
            
            var result = await ExecuteRequest<DeltaFullResponse>(request);
            return GetCollectionResponse<Deltas>(result);
        }

        public async Task<ConnectorResponse<Deltas>> Gained(int id, Period period) {
            var request = GetNewRestRequest("/{id}/gained");
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("period", period.GetEnumValueNameOrDefault());

            var result = await ExecuteRequest<DeltaResponse>(request);
            return GetResponse<Deltas>(result);
        }

        public async Task<ConnectorCollectionResponse<Deltas>> Gained(string username) {
            var request = GetNewRestRequest("/username/{username}/gained");
            request.AddParameter("username", username, ParameterType.UrlSegment);

            var result = await ExecuteRequest<DeltaFullResponse>(request);
            return GetCollectionResponse<Deltas>(result);
        }

        public async Task<ConnectorResponse<Deltas>> Gained(string username, Period period) {
            var request = GetNewRestRequest("/username/{username}/gained");
            request.AddParameter("username", username, ParameterType.UrlSegment);
            request.AddParameter("period", period.GetEnumValueNameOrDefault());

            var result = await ExecuteRequest<DeltaResponse>(request);
            return GetResponse<Deltas>(result);
        }

        #endregion

        #region records

        public Task<ConnectorCollectionResponse<Record>> Records(int id) {
            return QueryRecords(id);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(int id, MetricType metric) {
            return QueryRecords(id, metric);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(int id, Period period) {
            return QueryRecords(id, null, period);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(int id, MetricType metric, Period period) {
            return QueryRecords(id, metric, period);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(string username) {
            return QueryRecords(username);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(string username, MetricType metric) {
            return QueryRecords(username, metric);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(string username, Period period) {
            return QueryRecords(username, null, period);
        }

        public Task<ConnectorCollectionResponse<Record>> Records(string username, MetricType metric, Period period) {
            return QueryRecords(username, metric, period);
        }

        private async Task<ConnectorCollectionResponse<Record>> QueryRecords(string username, MetricType? metric = null, Period? period = null) {
            var request = GetNewRestRequest("/username/{username}/records");
            request.AddParameter("username", username, ParameterType.UrlSegment);

            if (metric.HasValue) {
                request.AddParameter("metric", metric.Value.GetEnumValueNameOrDefault());
            }

            if (period.HasValue) {
                request.AddParameter("period", period.Value.GetEnumValueNameOrDefault());
            }

            var result = await ExecuteCollectionRequest<WOMRecord>(request);
            return GetResponse<WOMRecord, Record>(result);
        }

        private async Task<ConnectorCollectionResponse<Record>> QueryRecords(int id, MetricType? metric = null, Period? period = null) {
            var request = GetNewRestRequest("/{id}/records");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            if (metric.HasValue) {
                request.AddParameter("metric", metric.Value.GetEnumValueNameOrDefault());
            }

            if (period.HasValue) {
                request.AddParameter("period", period.Value.GetEnumValueNameOrDefault());
            }

            var result = await ExecuteCollectionRequest<WOMRecord>(request);
            return GetResponse<WOMRecord, Record>(result);
        }

        #endregion

        #region view

        public async Task<ConnectorResponse<Player>> View(string username) {
            var request = GetNewRestRequest("username/{username}");
            request.AddParameter("username", username, ParameterType.UrlSegment);
            request.Method = Method.GET;

            var result = await ExecuteRequest<PlayerResponse>(request);
            return GetResponse<Player>(result);
        }

        public async Task<ConnectorResponse<Player>> View(int id) {
            var request = GetNewRestRequest("{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var result = await ExecuteRequest<PlayerResponse>(request);
            return GetResponse<Player>(result);
        }

        #endregion
    }
}