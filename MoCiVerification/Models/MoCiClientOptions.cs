using System;
using System.Net.Http;

namespace MoCiVerification.Models;

public class MoCiClientOptions
{
    public string ServerAddress { get; set; } = "";
    public bool IgnoreCertificateErrors { get; set; } = false;
    public Version RequestVersion { get; set; } = System.Net.HttpVersion.Version11;
    public HttpVersionPolicy VersionPolicy { get; set; } = HttpVersionPolicy.RequestVersionOrHigher;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    
    
    
    
}