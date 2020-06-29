﻿using System.Collections.Generic;
using AutoMapper;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace WiseOldManConnector.Transformers.Resolvers {
    internal class MetricToDictionaryResolver : IValueResolver<Models.API.Responses.Models.Snapshot, Snapshot, Dictionary<MetricType, Metric>> {
        public Dictionary<MetricType, Metric> Resolve(Models.API.Responses.Models.Snapshot source, Snapshot destination, Dictionary<MetricType, Metric> destMember, ResolutionContext context) {
            var result = new Dictionary<MetricType, Metric>() {
                {MetricType.Overall, context.Mapper.Map<Metric>(source.Overall)},
                {MetricType.Attack, context.Mapper.Map<Metric>(source.Attack)},
                {MetricType.Defence, context.Mapper.Map<Metric>(source.Defence)},
                {MetricType.Strength, context.Mapper.Map<Metric>(source.Strength)},
                {MetricType.Hitpoints, context.Mapper.Map<Metric>(source.Hitpoints)},
                {MetricType.Ranged, context.Mapper.Map<Metric>(source.Ranged)},
                {MetricType.Prayer, context.Mapper.Map<Metric>(source.Prayer)},
                {MetricType.Magic, context.Mapper.Map<Metric>(source.Magic)},
                {MetricType.Cooking, context.Mapper.Map<Metric>(source.Cooking)},
                {MetricType.Woodcutting, context.Mapper.Map<Metric>(source.Woodcutting)},
                {MetricType.Fletching, context.Mapper.Map<Metric>(source.Fletching)},
                {MetricType.Fishing, context.Mapper.Map<Metric>(source.Fishing)},
                {MetricType.Firemaking, context.Mapper.Map<Metric>(source.Firemaking)},
                {MetricType.Crafting, context.Mapper.Map<Metric>(source.Crafting)},
                {MetricType.Smithing, context.Mapper.Map<Metric>(source.Smithing)},
                {MetricType.Mining, context.Mapper.Map<Metric>(source.Mining)},
                {MetricType.Herblore, context.Mapper.Map<Metric>(source.Herblore)},
                {MetricType.Agility, context.Mapper.Map<Metric>(source.Agility)},
                {MetricType.Thieving, context.Mapper.Map<Metric>(source.Thieving)},
                {MetricType.Slayer, context.Mapper.Map<Metric>(source.Slayer)},
                {MetricType.Farming, context.Mapper.Map<Metric>(source.Farming)},
                {MetricType.Runecrafting, context.Mapper.Map<Metric>(source.Runecrafting)},
                {MetricType.Hunter, context.Mapper.Map<Metric>(source.Hunter)},
                {MetricType.Construction, context.Mapper.Map<Metric>(source.Construction)},
                {MetricType.LeaguePoints, context.Mapper.Map<Metric>(source.LeaguePoints)},
                {MetricType.BountyHunterHunter, context.Mapper.Map<Metric>(source.BountyHunterHunter)},
                {MetricType.BountyHunterRogue, context.Mapper.Map<Metric>(source.BountyHunterRogue)},
                {MetricType.ClueScrollsAll, context.Mapper.Map<Metric>(source.ClueScrollsAll)},
                {MetricType.ClueScrollsBeginner, context.Mapper.Map<Metric>(source.ClueScrollsBeginner)},
                {MetricType.ClueScrollsEasy, context.Mapper.Map<Metric>(source.ClueScrollsEasy)},
                {MetricType.ClueScrollsMedium, context.Mapper.Map<Metric>(source.ClueScrollsMedium)},
                {MetricType.ClueScrollsHard, context.Mapper.Map<Metric>(source.ClueScrollsHard)},
                {MetricType.ClueScrollsElite, context.Mapper.Map<Metric>(source.ClueScrollsElite)},
                {MetricType.ClueScrollsMaster, context.Mapper.Map<Metric>(source.ClueScrollsMaster)},
                {MetricType.LastManStanding, context.Mapper.Map<Metric>(source.LastManStanding)},
                {MetricType.AbyssalSire, context.Mapper.Map<Metric>(source.AbyssalSire)},
                {MetricType.AlchemicalHydra, context.Mapper.Map<Metric>(source.AlchemicalHydra)},
                {MetricType.BarrowsChests, context.Mapper.Map<Metric>(source.BarrowsChests)},
                {MetricType.Bryophyta, context.Mapper.Map<Metric>(source.Bryophyta)},
                {MetricType.Callisto, context.Mapper.Map<Metric>(source.Callisto)},
                {MetricType.Cerberus, context.Mapper.Map<Metric>(source.Cerberus)},
                {MetricType.ChambersOfXeric, context.Mapper.Map<Metric>(source.ChambersOfXeric)},
                {MetricType.ChambersOfXericChallengeMode, context.Mapper.Map<Metric>(source.ChambersOfXericChallengeMode)},
                {MetricType.ChaosElemental, context.Mapper.Map<Metric>(source.ChaosElemental)},
                {MetricType.ChaosFanatic, context.Mapper.Map<Metric>(source.ChaosFanatic)},
                {MetricType.CommanderZilyana, context.Mapper.Map<Metric>(source.CommanderZilyana)},
                {MetricType.CorporealBeast, context.Mapper.Map<Metric>(source.CorporealBeast)},
                {MetricType.CrazyArchaeologist, context.Mapper.Map<Metric>(source.CrazyArchaeologist)},
                {MetricType.DagannothPrime, context.Mapper.Map<Metric>(source.DagannothPrime)},
                {MetricType.DagannothRex, context.Mapper.Map<Metric>(source.DagannothRex)},
                {MetricType.DagannothSupreme, context.Mapper.Map<Metric>(source.DagannothSupreme)},
                {MetricType.DerangedArchaeologist, context.Mapper.Map<Metric>(source.DerangedArchaeologist)},
                {MetricType.GeneralGraardor, context.Mapper.Map<Metric>(source.GeneralGraardor)},
                {MetricType.GiantMole, context.Mapper.Map<Metric>(source.GiantMole)},
                {MetricType.GrotesqueGuardians, context.Mapper.Map<Metric>(source.GrotesqueGuardians)},
                {MetricType.Hespori, context.Mapper.Map<Metric>(source.Hespori)},
                {MetricType.KalphiteQueen, context.Mapper.Map<Metric>(source.KalphiteQueen)},
                {MetricType.KingBlackDragon, context.Mapper.Map<Metric>(source.KingBlackDragon)},
                {MetricType.Kraken, context.Mapper.Map<Metric>(source.Kraken)},
                {MetricType.Kreearra, context.Mapper.Map<Metric>(source.Kreearra)},
                {MetricType.KrilTsutsaroth, context.Mapper.Map<Metric>(source.KrilTsutsaroth)},
                {MetricType.Mimic, context.Mapper.Map<Metric>(source.Mimic)},
                {MetricType.Nightmare, context.Mapper.Map<Metric>(source.Nightmare)},
                {MetricType.Obor, context.Mapper.Map<Metric>(source.Obor)},
                {MetricType.Sarachnis, context.Mapper.Map<Metric>(source.Sarachnis)},
                {MetricType.Scorpia, context.Mapper.Map<Metric>(source.Scorpia)},
                {MetricType.Skotizo, context.Mapper.Map<Metric>(source.Skotizo)},
                {MetricType.TheGauntlet, context.Mapper.Map<Metric>(source.TheGauntlet)},
                {MetricType.TheCorruptedGauntlet, context.Mapper.Map<Metric>(source.TheCorruptedGauntlet)},
                {MetricType.TheatreOfBlood, context.Mapper.Map<Metric>(source.TheatreOfBlood)},
                {MetricType.ThermonuclearSmokeDevil, context.Mapper.Map<Metric>(source.ThermonuclearSmokeDevil)},
                {MetricType.TzkalZuk, context.Mapper.Map<Metric>(source.TzkalZuk)},
                {MetricType.TztokJad, context.Mapper.Map<Metric>(source.TztokJad)},
                {MetricType.Venenatis, context.Mapper.Map<Metric>(source.Venenatis)},
                {MetricType.Vetion, context.Mapper.Map<Metric>(source.Vetion)},
                {MetricType.Vorkath, context.Mapper.Map<Metric>(source.Vorkath)},
                {MetricType.Wintertodt, context.Mapper.Map<Metric>(source.Wintertodt)},
                {MetricType.Zalcano, context.Mapper.Map<Metric>(source.Zalcano)},
                {MetricType.Zulrah, context.Mapper.Map<Metric>(source.Zulrah)}
            };

            return result;
        }
    }
}