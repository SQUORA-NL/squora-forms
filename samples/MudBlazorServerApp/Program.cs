using MudBlazor.Services;
using Radzen;
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
// the Radzen components of the framework switcher need these (dialogs, tooltips, notifications)
builder.Services.AddRadzenComponents();
builder.Services.AddSquoraForms(cfg => 
                cfg.UseDefaultMudBlazorComponents()
                .RegisterComponent(ComponentType.GroupLayout, "Collapsable", typeof(CollapsableGroup))
                .RegisterComponent(ComponentType.TemplateContent, "contactcard", typeof(ContactCard)));

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
