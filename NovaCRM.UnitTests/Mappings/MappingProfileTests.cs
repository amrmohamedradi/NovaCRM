using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NovaCRM.Application.Mappings;

namespace NovaCRM.UnitTests.Mappings;

public class MappingProfileTests
{
    [Fact]
    public void AutoMapper_configuration_is_valid()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(_ => NullLoggerFactory.Instance);
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));
        var sp = services.BuildServiceProvider();

        var configProvider = sp.GetRequiredService<IConfigurationProvider>();
        configProvider.AssertConfigurationIsValid();
    }
}
