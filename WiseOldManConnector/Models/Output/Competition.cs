﻿using System;
using System.Collections.Generic;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace WiseOldManConnector.Models.Output {
    public class Competition : IBaseConnectorOutput{
        public int Id { get; set; }
        public string Title { get; set; }
        public MetricType Metric { get; set; }
        public int Score { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int? GroupId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string Duration { get; set; }
        public int ParticipantCount { get; set; }
        public List<CompetitionParticipant> Participants { get; set; }
        private CompetitionLeaderboard _leaderboard;

        public CompetitionLeaderboard Leaderboard
        {
            get { return _leaderboard ??= new CompetitionLeaderboard(Participants, Metric); }
            
        }

    }
}