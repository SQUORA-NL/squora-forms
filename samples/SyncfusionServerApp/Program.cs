using Squora.Forms.Blazor.Extensions;
using Squora.Forms.Blazor.SyncfusionComponents.Extensions;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Register Syncfusion Blazor services.
// To use a licensed version, register your key before builder.Build():
//   Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY");
builder.Services.AddSyncfusionBlazor();


// Register SquoraForms with the Syncfusion component provider.
builder.Services.AddSquoraForms(cfg => cfg.UseDefaultSyncfusionComponents());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
