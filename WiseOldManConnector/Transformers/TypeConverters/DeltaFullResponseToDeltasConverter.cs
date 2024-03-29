﻿using AutoMapper;
using WiseOldManConnector.Models.API.Responses;
using WiseOldManConnector.Models.Output;

namespace WiseOldManConnector.Transformers.TypeConverters;

internal class DeltaFullResponseToDeltasConverter : ITypeConverter<DeltaFullResponse, Deltas> {
    public Deltas Convert(DeltaFullResponse source, Deltas destination, ResolutionContext context) {
        var deltas = context.Mapper.Map<IEnumerable<Deltas>>(source);

        if (deltas.Count() > 1) {
            throw new Exception("Too many deltas");
        }


        return destination;
    }
}
