﻿using System;
using System.Collections.Generic;
using AutoMapper;
using WiseOldManConnector.Models.API.Responses;
using WiseOldManConnector.Models.API.Responses.Models;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;
using WiseOldManConnector.Transformers.Resolvers;
using WiseOldManConnector.Transformers.TypeConverters;
using Metric = WiseOldManConnector.Models.Output.Metric;


namespace WiseOldManConnector.Transformers {
    internal static class Configuration {
        public static Mapper GetMapper() {
            var config = new MapperConfiguration(cfg => {
                //cfg.CreateMap<string, MetricType>().ConvertUsing<StringToMetricTypeConverter>();
                cfg.CreateMap<PlayerResponse, Player>();
                cfg.CreateMap<WiseOldManConnector.Models.API.Responses.Models.Metric, Metric>();
                cfg.CreateMap<Models.API.Responses.Models.WOMSnapshot, Snapshot>()
                    .ForMember(dest => dest.AllMetrics, opt => opt.MapFrom<MetricToDictionaryResolver>());

                cfg.CreateMap<WOMCompetition, Competition>()
                    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartsAt))
                    .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndsAt))
                    .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedAt))
                    .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.ParticipantCount));

                //cfg.CreateMap<SearchResponse, Player>();
                cfg.CreateMap<WOMGroup, Group>();
                cfg.CreateMap<WOMGroupDeltaMember, DeltaMember>().ConvertUsing<WOMGroupTopMemberToDeltaMemberConverter>();

                cfg.CreateMap<AssertPlayerTypeResponse, PlayerType>()
                    .ConvertUsing<AssertPlayerTypeResponseToPlayerTypeConverter>();
                

                cfg.CreateMap<AssertDisplayNameResponse, string>().ConvertUsing<AssertDisplayNameResponseToStringConverter>();
                


                cfg.CreateMap<Models.API.Responses.Models.WOMAchievement, Achievement>()
                    .ForMember(dest => dest.AchievedAt, opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Type))
                    .ForMember(dest => dest.IsMissing, opt => opt.MapFrom(src => src.Missing));

                cfg.CreateMap<SnapshotsResponse, Snapshots>();

                cfg.CreateMap<DeltaResponse, Deltas>()
                    .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.StartsAt))
                    .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.EndsAt))
                    .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.Period))
                    .ForMember(dest => dest.DeltaMetrics, opt => opt.MapFrom(src => src.Metrics));

                cfg.CreateMap<DeltaFullResponse, IEnumerable<Deltas>>()
                    .ConvertUsing<DeltaFullResponseToCollectionOfDeltasConverter>();
                cfg.CreateMap<DeltaFullResponse, Deltas>().ConvertUsing<DeltaFullResponseToDeltasConverter>();


                cfg.CreateMap<DeltaMetrics, Dictionary<MetricType, DeltaMetric>>()
                    .ConvertUsing<DeltaMetricsToDeltaDictionaryConverter>();

                cfg.CreateMap<WOMDeltaMetric, DeltaMetric>()
                    .ForMember(dest => dest.Deltas, opt => opt.MapFrom<WOMDeltaToDeltaDictionaryResolver>());

                cfg.CreateMap<WOMDelta, Delta>();
                cfg.CreateMap<string, PlayerType>().ConvertUsing<StringToPlayerTypeConverter>();
                cfg.CreateMap<string, PlayerBuild>().ConvertUsing<StringToPlayerBuildConverter>();
                cfg.CreateMap<string, GroupRole?>().ConvertUsing<StringToGroupRoleConvertor>();

                cfg.CreateMap<WOMRecord, Record>()
                    .ForMember(dest => dest.MetricType, opt => opt.MapFrom(src => src.MetricType))
                    .ForMember(dest => dest.UpdateDateTime, opt => opt.MapFrom(src => src.UpdatedAt))
                    .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.Player));

                cfg.CreateMap<RecordResponse, IEnumerable<Record>>().ConvertUsing<RecordResponseToRecordCollectionConverter>();

                cfg.CreateMap<WOMMessageResponse, MessageResponse>();

                cfg.CreateMap<IEnumerable<WOMGroupDeltaMember>, DeltaLeaderboard>()
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src));

                cfg.CreateMap<LeaderboardMember, Player>();
                cfg.CreateMap<LeaderboardMember, Metric>();

                cfg.CreateMap<IEnumerable<LeaderboardMember>, HighscoreLeaderboard>()
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src));

                cfg.CreateMap<LeaderboardMember, HighscoreMember>()
                    .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src))
                    .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src));

                cfg.CreateMap<IEnumerable<LeaderboardMember>, HighscoreLeaderboard>()
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src));

                cfg.CreateMap<LeaderboardMember, HighscoreMember>()
                    .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src))
                    .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src));

                cfg.CreateMap<IEnumerable<LeaderboardMember>, RecordLeaderboard>()
                    .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src));

                cfg.CreateMap<LeaderboardMember, Record>()
                    .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src))
                    .ForMember(dest => dest.UpdateDateTime, opt => opt.MapFrom(src => src.UpdatedAt));

                cfg.CreateMap<LeaderboardMember, Player>();

                cfg.CreateMap<LeaderboardMember, Metric>();

                cfg.CreateMap<StatisticsResponse, Statistics>()
                    .ForMember(dest => dest.Maxed200MExpPlayers, opt => opt.MapFrom(src => src.Maxed200msCount))
                    .ForMember(dest => dest.MaxedCombatPlayers, opt => opt.MapFrom(src => src.MaxedCombatCount))
                    .ForMember(dest => dest.MaxedTotalPlayers, opt => opt.MapFrom(src => src.MaxedTotalCount));
            });

            return new Mapper(config);
        }
    }
}