using Microsoft.Extensions.DependencyInjection;
using Retkon.SuperScoped.Exceptions;
using Retkon.SuperScoped.Tests.TestClasses;

namespace Retkon.SuperScoped.Tests;

[TestClass]
public class SuperScopedTest
{
    private IServiceScope serviceScope = null!;

    public void TestInitialize(Action<IServiceCollection> testRegistrations)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<List<string>>();
        serviceCollection.AddSuperScope<MyScope>();
        testRegistrations(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        this.serviceScope = serviceProvider.CreateScope();
    }

    [TestMethod]
    public void SuperScoped_Integrated_Basic()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceChild>>();
        Assert.IsEmpty(result);

        var myServiceChild = sut1.SuperScope<MyScope>(s => s.Value = 1);
        Assert.HasCount(1, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);

        myServiceChild.Poke();
        Assert.HasCount(2, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceChild::Poke", result[1]);
    }

    [TestMethod]
    public void SuperScoped_Integrated_ParentChild()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
            services.AddScoped<MyServiceParent>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceParent>>();
        Assert.IsEmpty(result);

        var myServiceParent = sut1.SuperScope<MyScope>(s => s.Value = 1);
        Assert.HasCount(2, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);

        myServiceParent.Poke();
        Assert.HasCount(3, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);
        Assert.AreEqual("MyServiceParent::Poke", result[2]);
    }

    [TestMethod]
    public void SuperScoped_Integrated_ParentChild_ScopingOnChild()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
            services.AddScoped<MyServiceParent>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceParent>>();
        var sut2 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceChild>>();
        Assert.IsEmpty(result);

        var myServiceChild = sut2.SuperScope<MyScope>(s => s.Value = 1);
        var myServiceParent = sut1.SuperScope<MyScope>();
        Assert.HasCount(2, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);

        myServiceParent.Poke();
        Assert.HasCount(3, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);
        Assert.AreEqual("MyServiceParent::Poke", result[2]);
    }

    [TestMethod]
    public void SuperScoped_Integrated_ParentChild_SameScoping()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
            services.AddScoped<MyServiceParent>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceParent>>();
        var sut2 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceChild>>();
        Assert.IsEmpty(result);

        var myServiceParent = sut1.SuperScope<MyScope>(s => s.Value = 1);
        var myServiceChild = sut1.SuperScope<MyScope>(s => s.Value = 1);
        Assert.HasCount(2, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);

        myServiceParent.Poke();
        Assert.HasCount(3, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);
        Assert.AreEqual("MyServiceParent::Poke", result[2]);
    }

    [TestMethod]
    public void SuperScoped_Integrated_ParentChild_DifferentScoping()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
            services.AddScoped<MyServiceParent>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceParent>>();
        var sut2 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceChild>>();
        Assert.IsEmpty(result);

        var myServiceParent = sut1.SuperScope<MyScope>(ss => ss.Value = 1);
        try
        {
            var myServiceChild = sut1.SuperScope<MyScope>(s => s.Value = 2);
            Assert.Fail("Shouldn't reach this.");
        }
        catch (SuperScopeFixedException) { }
    }

    [TestMethod]
    public void SuperScoped_Integrated_ParentChild_NoScoping()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
            services.AddScoped<MyServiceParent>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceParent>>();
        var sut2 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceChild>>();
        Assert.IsEmpty(result);

        try
        {
            var myServiceParent = sut1.SuperScope<MyScope>();
            Assert.Fail("Shouldn't reach this.");
        }
        catch (SuperScopeNotFixedException) { }
    }

    [TestMethod]
    public void SuperScoped_Integrated_Child_IgnoringSuperScoping()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
        });

        // Act / Assert
        try
        {
            _ = this.serviceScope.ServiceProvider.GetRequiredService<MyServiceChild>();
            Assert.Fail("Shouldn't reach this.");
        }
        catch (SuperScopeNotFixedException) { }
    }

    [TestMethod]
    public void SuperScoped_Integrated_ParentChild_ChildIgnoringSuperScoping()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
            services.AddScoped<MyServiceParent>();
        });

        var result = this.serviceScope.ServiceProvider.GetRequiredService<List<string>>();

        // Act / Assert
        var sut1 = this.serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyServiceParent>>();
        Assert.IsEmpty(result);

        var myServiceParent = sut1.SuperScope<MyScope>(s => s.Value = 1);
        Assert.HasCount(2, result);
        Assert.AreEqual("MyServiceChild::ctor", result[0]);
        Assert.AreEqual("MyServiceParent::ctor", result[1]);

        myServiceParent.Poke();
        Assert.HasCount(3, result);
        Assert.AreEqual("MyServiceParent::Poke", result[2]);

        var myServiceChild = this.serviceScope.ServiceProvider.GetRequiredService<MyServiceChild>();
        myServiceChild.Poke();
        Assert.HasCount(4, result);
        Assert.AreEqual("MyServiceChild::Poke", result[3]);
    }

    [TestMethod]
    public void SuperScoped_Integrated_Child_ManualScope()
    {
        // Arrange
        this.TestInitialize(services =>
        {
            services.AddScoped<MyServiceChild>();
        });

        // Act / Assert
        var superScope = this.serviceScope.ServiceProvider.GetRequiredService<SuperScope<MyScope>>();
        superScope.WithScope(s => s.Value = 1);

        var myServiceChild = this.serviceScope.ServiceProvider.GetRequiredService<MyServiceChild>();
    }

}
