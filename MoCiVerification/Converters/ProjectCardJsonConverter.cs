using System.Collections.Generic;
using MoCiVerification.Models;

namespace MoCiVerification.Converts;

public static class ProjectCardJsonConverter
{
    public static ProjectCardPrice ConvertFromServerFormat(ServerCardPrice serverData)
{
    return new ProjectCardPrice
    {
        StandardCard1 = ParseDecimalFromList(serverData.HourStandard),
        StandardCard2 = ParseDecimalFromList(serverData.DayStandard),
        StandardCard3 = ParseDecimalFromList(serverData.WeekStandard),
        StandardCard4 = ParseDecimalFromList(serverData.MonthStandard),
        StandardCard5 = ParseDecimalFromList(serverData.SeasonStandard),
        StandardCard6 = ParseDecimalFromList(serverData.HalfYearStandard),
        StandardCard7 = ParseDecimalFromList(serverData.YearStandard),
        StandardCard8 = ParseDecimalFromList(serverData.PermanentStandard),
        ProxyCard1 = ParseDecimalFromList(serverData.HourProxy),
        ProxyCard2 = ParseDecimalFromList(serverData.DayProxy),
        ProxyCard3 = ParseDecimalFromList(serverData.WeekProxy),
        ProxyCard4 = ParseDecimalFromList(serverData.MonthProxy),
        ProxyCard5 = ParseDecimalFromList(serverData.SeasonProxy),
        ProxyCard6 = ParseDecimalFromList(serverData.HalfYearProxy),
        ProxyCard7 = ParseDecimalFromList(serverData.YearProxy),
        ProxyCard8 = ParseDecimalFromList(serverData.PermanentProxy)
    };
}

public static ServerCardPrice ConvertToServerFormat(ProjectCardPrice config)
{
    return new ServerCardPrice
    {
        HourStandard = new List<string> { config.StandardCard1.ToString() },
        DayStandard = new List<string> { config.StandardCard2.ToString() },
        WeekStandard = new List<string> { config.StandardCard3.ToString() },
        MonthStandard = new List<string> { config.StandardCard4.ToString() },
        SeasonStandard = new List<string> { config.StandardCard5.ToString() },
        HalfYearStandard = new List<string> { config.StandardCard6.ToString() },
        YearStandard = new List<string> { config.StandardCard7.ToString() },
        PermanentStandard = new List<string> { config.StandardCard8.ToString() },
        HourProxy = new List<string> { config.ProxyCard1.ToString() },
        DayProxy = new List<string> { config.ProxyCard2.ToString() },
        WeekProxy = new List<string> { config.ProxyCard3.ToString() },
        MonthProxy = new List<string> { config.ProxyCard4.ToString() },
        SeasonProxy = new List<string> { config.ProxyCard5.ToString() },
        HalfYearProxy = new List<string> { config.ProxyCard6.ToString() },
        YearProxy = new List<string> { config.ProxyCard7.ToString() },
        PermanentProxy = new List<string> { config.ProxyCard8.ToString() }
    };
}

public static decimal ParseDecimalFromList(List<string> list)
{
    if (list == null || list.Count == 0 || string.IsNullOrEmpty(list[0])) 
        return 0m;
    
    return decimal.TryParse(list[0], out var result) ? result : 0m;
}
}