var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Typed HttpClient pointing at the Web API.
builder.Services.AddHttpClient("Api", c =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"]
                  ?? "https://localhost:7080/";
    c.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customers}/{action=Enroll}/{id?}");

app.Run();
