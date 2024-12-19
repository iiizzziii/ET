namespace ET.Api.Services;

public interface IIpService
{
    Task<string> GetIpCountryCode(string ipAddress);
}