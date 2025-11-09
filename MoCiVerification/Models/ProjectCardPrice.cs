using System.Collections.Generic;
using Newtonsoft.Json;

namespace MoCiVerification.Models;

public class ProjectCardPrice
{
    [JsonProperty("时卡标准")]
    public decimal StandardCard1 { get; set; }
    [JsonProperty("天卡标准")]
    public decimal StandardCard2 { get; set; }
    [JsonProperty("周卡标准")]
    public decimal StandardCard3 { get; set; }
    [JsonProperty("月卡标准")]
    public decimal StandardCard4 { get; set; }
    [JsonProperty("季卡标准")]
    public decimal StandardCard5 { get; set; }
    [JsonProperty("半年卡标准")]
    public decimal StandardCard6 { get; set; }
    [JsonProperty("年卡标准")]
    public decimal StandardCard7 { get; set; }
    [JsonProperty("永久卡标准")]
    public decimal StandardCard8 { get; set; }
    [JsonProperty("时卡代理")]
    public decimal ProxyCard1 { get; set; }
    [JsonProperty("天卡代理")]
    public decimal ProxyCard2 { get; set; }
    [JsonProperty("周卡代理")]
    public decimal ProxyCard3 { get; set; }
    [JsonProperty("月卡代理")]
    public decimal ProxyCard4 { get; set; }
    [JsonProperty("季卡代理")]
    public decimal ProxyCard5 { get; set; }
    [JsonProperty("半年卡代理")]
    public decimal ProxyCard6 { get; set; }
    [JsonProperty("年卡代理")]
    public decimal ProxyCard7 { get; set; }
    [JsonProperty("永久卡代理")]
    public decimal ProxyCard8 { get; set; }
}
public class ServerCardPrice
{
    [JsonProperty("时卡标准")]
    public List<string> HourStandard { get; set; }
    
    [JsonProperty("天卡标准")]
    public List<string> DayStandard { get; set; }
    
    [JsonProperty("周卡标准")]
    public List<string> WeekStandard { get; set; }
    
    [JsonProperty("月卡标准")]
    public List<string> MonthStandard { get; set; }
    
    [JsonProperty("季卡标准")]
    public List<string> SeasonStandard { get; set; }
    
    [JsonProperty("半年卡标准")]
    public List<string> HalfYearStandard { get; set; }
    
    [JsonProperty("年卡标准")]
    public List<string> YearStandard { get; set; }
    
    [JsonProperty("永久卡标准")]
    public List<string> PermanentStandard { get; set; } 
    
    [JsonProperty("时卡代理")]
    public List<string> HourProxy { get; set; }
    
    [JsonProperty("天卡代理")]
    public List<string> DayProxy { get; set; } 
    
    [JsonProperty("周卡代理")]
    public List<string> WeekProxy { get; set; }
    
    [JsonProperty("月卡代理")]
    public List<string> MonthProxy { get; set; } 
    
    [JsonProperty("季卡代理")]
    public List<string> SeasonProxy { get; set; } 
    
    [JsonProperty("半年卡代理")]
    public List<string> HalfYearProxy { get; set; } 
    
    [JsonProperty("年卡代理")]
    public List<string> YearProxy { get; set; } 
    
    [JsonProperty("永久卡代理")]
    public List<string> PermanentProxy { get; set; } 
}