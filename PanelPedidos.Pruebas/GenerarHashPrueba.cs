using Xunit;
using Xunit.Abstractions;

namespace PanelPedidos.Pruebas;

public class GenerarHashPrueba
{
    private readonly ITestOutputHelper _output;

    public GenerarHashPrueba(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void GenerarHash()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(
            "AdminPrueba123",
            workFactor: 12);

        _output.WriteLine(hash);
    }
}