﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WiseOldManConnector.Models.API.Responses.Models;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace WiseOldManConnector.Models.API.Responses {
    internal class DeltaFullResponse : BaseResponse {
        public DeltaResponse Day { get; set; }
        public DeltaResponse Year { get; set; }
        public DeltaResponse Month { get; set; }
        public DeltaResponse Week { get; set; }
    }

    internal class DeltaResponse : BaseResponse {
        public Period Period { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string Interval { get; set; }

        [JsonProperty("data")]
        public DeltaMetrics Metrics{ get; set; }

        public List<DeltaInfo> DeltaInfoList {
            get {
                return Metrics.AllDeltaInfos;
            }
        }

        public DeltaInfo DeltaInfoForType(MetricType type) {
            return Metrics.InfoFromType(type);
        }
    }
}