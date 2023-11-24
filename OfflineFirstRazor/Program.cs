using Factory;
using Microsoft.Extensions.Hosting.WindowsServices;
using Service;
using CliWrap;

LoggerService.InitLogService();

var options = new WebApplicationOptions
{
	Args = args,
	ContentRootPath = WindowsServiceHelpers.IsWindowsService()
									 ? AppContext.BaseDirectory : default
};

string ServiceName = $"{Global.GlobalConfig.Instance.AppName} Service v{Global.GlobalConfig.Instance.AppVersion}";

if (args is { Length: 1 })
{
    try
    {
        string exeName = System.IO.Path.GetFileName(System.AppDomain.CurrentDomain.FriendlyName);

        string executablePath =
            Path.Combine(AppContext.BaseDirectory, exeName);

        if (args[0] is "/Install")
        {
            await Cli.Wrap("sc")
                .WithArguments(new[] { "create", ServiceName, $"binPath={executablePath}", "start=auto" })
                .ExecuteAsync();
        }
        else if (args[0] is "/Uninstall")
        {
            //Perform housekeep
            //Delete database
            System.IO.File.Delete(AppContext.BaseDirectory + Global.GlobalConfig.Instance.SqliteDatabaseName);
            //Remove Key
            CipherService.UninstallKey();

            await Cli.Wrap("sc")
                .WithArguments(new[] { "stop", ServiceName })
                .ExecuteAsync();
            await Cli.Wrap("sc")
                .WithArguments(new[] { "delete", ServiceName })
                .ExecuteAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSession();
builder.Services.AddMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHostedService<OlifService>();
builder.Host.UseWindowsService(options =>
{
    options.ServiceName = ServiceName;
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
