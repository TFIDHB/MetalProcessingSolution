using Application.Extensions;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using MetalProcessingSolution.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await AdminSeeder.SeedAsync(db, builder.Configuration);
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

var externalPath = Path.Combine(app.Environment.ContentRootPath, "..", "ExternalUploads");
if (!Directory.Exists(externalPath)) Directory.CreateDirectory(externalPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(externalPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();