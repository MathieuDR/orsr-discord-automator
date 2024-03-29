﻿using AutoMapper;
using WiseOldManConnector.Models.API.Responses;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace WiseOldManConnector.Transformers.TypeConverters;

internal class WOMGroupTopMemberToDeltaMemberConverter : ITypeConverter<WOMGroupDeltaMember, DeltaMember> {
    public DeltaMember Convert(WOMGroupDeltaMember source, DeltaMember destination, ResolutionContext context) {
        destination = new DeltaMember();

        destination.Player = context.Mapper.Map<PlayerResponse, Player>(source.Player);
        destination.Delta = new Delta {
            DeltaType = DeltaType.Experience,
            Gained = source.Gained
        };

        destination.EndTime = source.EndDate;
        destination.StartTime = source.StartDate;

        return destination;
    }
}

// internal class LeaderboardMemberToHighscoreMember : ITypeConverter<LeaderboardMember, HighscoreMember> {
//     public HighscoreMember Convert(LeaderboardMember source, HighscoreMember destination, ResolutionContext context) {
//         destination = new HighscoreMember();
//
//         destination.Player = context.Mapper.Map<PlayerResponse, Player>(source.Player);
//         destination.Delta = new Delta() {
//             DeltaType = DeltaType.Experience,
//             Gained = source.Gained
//         };
//
//         destination.EndTime = source.EndDate;
//         destination.StartTime= source.StartDate;
//
//         return destination;
//     }
// }
