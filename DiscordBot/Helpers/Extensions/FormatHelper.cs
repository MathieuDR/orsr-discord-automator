﻿using MathieuDR.Common.Formatters;

namespace DiscordBot.Helpers.Extensions;

public static class FormatHelper {
    private static readonly int[] Experiences = {
        0, 0, 83, 174, 276, 388, 512, 650, 801, 969, 1154, 1358, 1584, 1833, 2107, 2411, 2746, 3115, 3523, 3973,
        4470, 5018, 5624, 6291, 7028, 7842, 8740, 9730, 10824, 12031, 13363, 14833, 16456, 18247, 20224, 22406,
        24815, 27473, 30408, 33648, 37224, 41171, 45529, 50339, 55649, 61512, 67983, 75127, 83014, 91721, 101333,
        111945, 123660, 136594, 150872, 166636, 184040, 203254, 224466, 247886, 273742, 302288, 333804, 368599,
        407015, 449428, 496254, 547953, 605032, 668051, 737627, 814445, 899257, 992895, 1096278, 1210421, 1336443,
        1475581, 1629200, 1798808, 1986068, 2192818, 2421087, 2673114, 2951373, 3258594, 3597792, 3972294, 4385776,
        4842295, 5346332, 5902831, 6517253, 7195629, 7944614, 8771558, 9684577, 10692629, 11805606, 13034431, 14391160,
        15889109, 17542976, 19368992, 21385073, 23611006, 26068632, 28782069, 31777943, 35085654, 38737661, 42769801,
        47221641,
        52136869, 57563718, 63555443, 70170840, 77474828, 85539082, 94442737, 104273167, 115126838, 127110260, 140341028,
        154948977,
        171077457, 188884740, 200000000
    };

    public static string ToLevel(this Metric metric) {
        return metric.Value.ToLevel().ToString();
    }

    public static string FormattedExperience(this Metric metric) {
        return metric.Value.FormatNumber();
    }

    public static string FormattedRank(this Metric metric) {
        return FormatNumber((double)metric.Rank);
    }

    public static string FormatNumber(this double number, bool zeroAsStripe = false) {
        if (number >= 1) {
            return FormatNumber((long)number);
        }

        return number.ToString("N");
    }

    public static string FormatHours(this double number) {
        return $"{number - (number - (int)number)}:{TimeSpan.FromHours(number - (int)number).ToString(@"mm")}";
    }

    public static string FormatNumber(this long number, bool zeroAsStripe = false) {
        return NumberFormatter.FormatDecimal(number);
    }

    public static string FormatConditionally(this long number) {
        return NumberFormatter.FormatDecimal(number, true);
    }

    public static int ToLevel(this double experience) {
        return ToLevel((int)experience);
    }

    public static int ToLevel(this long experience) {
        int index;

        for (index = 0; index < Experiences.Length; index++) {
            if (index + 1 == Experiences.Length) {
                break;
            }

            if (Experiences[index + 1] > experience) {
                break;
            }
        }

        return index;
    }

    public static int ToExperience(this double level) {
        return ToExperience((int)level);
    }


    public static int ToExperience(this int level) {
        var index = level;

        if (index >= Experiences.Length) {
            return Experiences.Last();
        }

        return Experiences[index];
    }
    
    public static string Truncate(this string text, int maxLength, string suffix = "...") {
        if (text.Length <= maxLength) {
            return text;
        }
        
        var suffixLength = suffix.Length;

        return text[..(maxLength - suffixLength)] + suffix;
    }
}
