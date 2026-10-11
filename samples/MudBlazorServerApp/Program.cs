using MudBlazor.Services;
using MudBlazorServerApp.Data;
using Squora.Forms.Blazor.Extensions;
using Squora.Forms.Blazor.MudBlazorComponents.Extensions;
using Squora.Forms.Common.Enums;
using MudBlazorServerApp.Shared.FormExtentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<OnboardingService>();
builder.Services.AddMudServices();
builder.Services.AddSquoraForms(cfg => 
                cfg.UseDefaultMudBlazorComponents()
                .RegisterComponent(ComponentType.GroupLayout, "Collapsable", typeof(CollapsableGroup)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
