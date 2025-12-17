using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using SoulFlow.Data;

using SoulFlow.Models;



var builder = WebApplication.CreateBuilder(args);



builder.Services.Configure<IdentityOptions>(options =>

{

    options.User.AllowedUserNameCharacters =

        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+abcçdefgðhýijklmnoöprsþtuüvyzABCÇDEFGÐHIÝJKLMNOÖPRSÞTUÜVYZ";



    options.User.RequireUniqueEmail = true;

});





var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>

    options.UseSqlServer(connectionString));



builder.Services.AddIdentity<AppUser, IdentityRole>()

    .AddEntityFrameworkStores<ApplicationDbContext>()

    .AddDefaultTokenProviders();



builder.Services.AddControllersWithViews();



var app = builder.Build();





if (!app.Environment.IsDevelopment())

{

    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();

}



app.UseHttpsRedirection();

app.UseStaticFiles();



app.UseRouting();



app.UseAuthentication();

app.UseAuthorization();



app.MapControllerRoute(

    name: "default",

    pattern: "{controller=Home}/{action=Index}/{id?}");





using (var scope = app.Services.CreateScope())

{

    var services = scope.ServiceProvider;

    try

    {



        await SoulFlow.Data.DbSeeder.SeedRolesAndAdminAsync(services);

    }

    catch (Exception ex)

    {



        Console.WriteLine("Admin oluþturulurken bir hata oluþtu: " + ex.Message);

    }

}



app.Run(); 