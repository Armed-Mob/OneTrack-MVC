using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneTrack.Data;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


// Retrieve the connection string for the Logging Database from the configuration
var loggerConnectionString = builder.Configuration.GetConnectionString("LoggerConnection");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");

if (string.IsNullOrEmpty(loggerConnectionString))
{
    throw new ArgumentNullException(nameof(loggerConnectionString), "Connection string 'LoggerConnection' is missing."); 
}

if (string.IsNullOrEmpty(identityConnectionString))
{
    throw new ArgumentNullException(nameof(identityConnectionString), "Connection string 'IdentityConnection' is missing.");
}

// Define column options for logging to SQL Server
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn { ColumnName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 128 },
        new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 256 },
    }
};

// Define SQL Server Sink Options
var sinkOptions = new MSSqlServerSinkOptions
{
    TableName = "Logs",
    AutoCreateSqlDatabase = true,
    AutoCreateSqlTable = true,
};

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: loggerConnectionString,
        sinkOptions: sinkOptions,
        columnOptions: columnOptions
    )
    .CreateLogger();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(identityConnectionString));

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
    

// Configure cookie settings within Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // For GDPR compliance
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting up the application");

    var app = builder.Build();

    app.Use(async (context, next) =>
    {
        Log.Information($"Handling request: {context.Request.Path}");
        Log.ForContext("UserName", context.User.Identity?.Name ?? "Anonymous")
        .ForContext("RequestPath", context.Request.Path)
        .Information("Request Information");

        await next.Invoke();

        Log.Information($"Finished handling request.");
    });

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    // Log.Fatal is a serilog method to log fatal error
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    // Ensure the log is flushed and closed properly
    Log.CloseAndFlush();
}