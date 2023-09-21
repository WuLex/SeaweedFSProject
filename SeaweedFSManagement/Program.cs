using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SeaweedFSManagement.Models;
using SeaweedFSManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// ע��IHttpClientFactory
builder.Services.AddHttpClient();

var Configuration= builder.Configuration;

// �������ݿ�������
builder.Services.AddDbContext<RocketDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

// ע��SeaweedFS�ļ�����
builder.Services.AddScoped<SeaweedFileService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
