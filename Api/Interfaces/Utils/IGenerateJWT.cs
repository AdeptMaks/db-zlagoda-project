using Api.Features.Shared;

namespace Api.Interfaces.Utils;

public interface IGenerateJWT
{
    string Generate(string username, EmpoloyeeRole empoloyeeRole);
}