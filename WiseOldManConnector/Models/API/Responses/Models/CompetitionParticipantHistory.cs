﻿using System;
using Newtonsoft.Json;

namespace WiseOldManConnector.Models.API.Responses.Models {
    internal class CompetitionParticipantHistory {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
}