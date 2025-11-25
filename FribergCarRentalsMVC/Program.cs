using FribergCarRentalsMVC.ApiClients;
using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.Options;
using FribergCarRentalsMVC.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
    opt.IdleTimeout = TimeSpan.FromHours(8);
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<JwtSessionHandler>();

//Auth-klient + service
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>((sp, http) =>
{
    var api = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    http.BaseAddress = new Uri(api.BaseUrl);
})
.AddHttpMessageHandler<JwtSessionHandler>(); 

builder.Services.AddScoped<IAuthService, AuthService>();

//Övriga ApiClients
builder.Services.AddHttpClient<ICarsApiClient, CarsApiClient>((sp, http) =>
{
    var api = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    http.BaseAddress = new Uri(api.BaseUrl);
})
.AddHttpMessageHandler<JwtSessionHandler>();

builder.Services.AddHttpClient<IBookingApiClient, BookingApiClient>((sp, http) =>
{
    var api = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    http.BaseAddress = new Uri(api.BaseUrl);
})
.AddHttpMessageHandler<JwtSessionHandler>();

builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>((sp, http) =>
{
    var api = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    http.BaseAddress = new Uri(api.BaseUrl);
})
.AddHttpMessageHandler<JwtSessionHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();   

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
