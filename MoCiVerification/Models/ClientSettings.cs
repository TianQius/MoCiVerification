using System;
using System.IO;
using Newtonsoft.Json;

namespace MoCiVerification.Models;

public class ClientSettings
{
    public string JsonPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), "MoCi.json");
    
    public readonly object SearchRecipient = new();

    public string CurrentProjectName { get; set; }
    public string CurrentProjectAnnouncement { get; set; }
    
    public string CurrentVersion { get; set; }
    public string CurrentVersionAnnouncement { get; set; }
    public string CurrentVersionData { get; set; }
    public string CurrentCustomDataKey { get; set; }
    public string CurrentCustomDataValue { get; set; }
    public string CurrentCustomDataMask { get; set; }
    public string CurrentCustomDataGetWay { get; set; }
    public bool UseNodeServer { get; set; } = true;
    public bool IsAuto { get; set; } = false;
    
    public ProjectCardPrice? CurrentProjectCardPrice { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; } 
    
    public string? ClientLicense { get; set; }
    public string AppVersion { get; set; } = "4.0.0beta";
    public string? GlobalMessage { get; set; }
    
    public void LoadFromJson()
    {
        if (!File.Exists(JsonPath)) return;

        try
        {
            var json = File.ReadAllText(JsonPath);
            var config = JsonConvert.DeserializeObject<AssestBase>(json);
            if (config != null)
            {
                UserName = config.UserName;
                Password = config.PassWord; 
                IsAuto = config.IsAuto;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载配置失败: {ex.Message}");
        }
    }

    public void SaveToJson()
    {
        var config = new AssestBase()
        {
            UserName = UserName,
            PassWord = Password,
            IsAuto = IsAuto,
            UseNodeServer =UseNodeServer
        };

        try
        {
            File.WriteAllText(JsonPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存配置失败: {ex.Message}");
        }
    }
}



public class AssestBase
{
    
    public string UserName
    {
        get;
        set;
    }
    public string PassWord
    {
        get;
        set;
    }
    public bool IsAuto
    {
        get;
        set;
    }
    public bool UseNodeServer 
    { 
        get; 
        set; 
    }
    
}