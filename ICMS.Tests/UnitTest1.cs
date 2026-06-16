using ICMS.Application;
using ICMS.Application.Interfaces.Services;
using ICMS.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace ICMS.Tests;

public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UnitTest1(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void TestLocalizer()
    {
        var assembly = typeof(MessagesResource).Assembly;
        var names = assembly.GetManifestResourceNames();
        var namesStr = string.Join(", ", names);

        using var scope = _factory.Services.CreateScope();
        var localizer = scope.ServiceProvider.GetRequiredService<ILocalizer>();

        var titleAr = "";
        var titleEn = "";
        var prevCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("ar");
            titleAr = localizer["NewFieldVisitAssignmentTitle"];
            
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            titleEn = localizer["NewFieldVisitAssignmentTitle"];

            Assert.Equal("تعيين زيارة ميدانية جديدة", titleAr);
            Assert.Equal("New Field Visit Assignment", titleEn);
        }
        finally
        {
            CultureInfo.CurrentUICulture = prevCulture;
        }
    }
}
