using System.Runtime.InteropServices;
using Api.Features.Auth;

namespace Api.Interfaces.Utils;

public interface IGenerateJWT
{
    string Generate(CreateEmployeeRequest input);
}