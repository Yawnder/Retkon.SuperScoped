# Retkon SuperScoped

## Problem to solve
I often end up having a set of services that have to run in a context, for example, for a specific Tenant. I end up creating an `ITenantContext` and injecting this into everything that needs it, and setting the value for the tenant as early as I can near the Entrypoint of the process.

### What I don't like about the way I was going it before
* There is no implicit / automatic check to make sure the context has been set properly and implementing an "Ensure properly scope" is more tedious than it should.
* There is no guarantee the context won't be (erroneously) changed midway through the process.
* Some services might use the scope in their constructor, and that might have happened before the scope was set.

This library is meant as a way to enforce a flow surrounding this, making sure that the scope is set before anything might want to use it.

## Usage

Whatever was to _use_ the SuperScopes, the registration is the same.
```cs
var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<MyService>();
serviceCollection.AddSuperScope<MyScope>();
var serviceProvider = serviceCollection.BuildServiceProvider();

// A scope is required, duhhh.
var serviceScope = serviceProvider.CreateScope();
```

### Typical usage: SuperScoped Service through SuperScope

```cs
var superScopedService = serviceScope.ServiceProvider.GetRequiredService<SuperScoped<MyService>>();

// Initializes the scope and returns the service.
var service = superScopedService.SuperScope<MyScope>(s => s.Value = 1);
service.DoWork();

// Will work directly since that Scope has been initialized.
var otherService = serviceScope.ServiceProvider.GetRequiredService<MyOtherService>();
otherService.DoWork();
```

### Alternate usage: Manual Scoping

```cs
var superScope = serviceScope.ServiceProvider.GetRequiredService<SuperScope<MyScope>>();
superScope.WithScope(s => s.Value = 1);

// Will work directly since that Scope has been initialized.
var service = serviceScope.ServiceProvider.GetRequiredService<MyService>();
service.DoWork();
```
