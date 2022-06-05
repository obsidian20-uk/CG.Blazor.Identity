
var builder = WebApplication.CreateBuilder(args);

// This adds a sample database.
builder.Services.AddDbContext<SampleDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );

// This adds our blazor identity library.
builder.AddBlazorIdentity<IdentityUser>()
    .AddEntityFrameworkStores<SampleDbContext>();

// This adds a sample email sender.
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // This makes sure we have a database at runtime.
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
        db.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// This adds our blazor identity middlewear.
app.UseBlazorIdentity<IdentityUser>();

app.Run();
