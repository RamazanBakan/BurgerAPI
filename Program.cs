using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication10.Data;
using WebApplication10.Interface;
using WebApplication10.Middlewares;
using WebApplication10.Repository;
using WebApplication10.Service;

var builder = WebApplication.CreateBuilder(args);

// 📌 Add services to the container
builder.Services.AddControllers();

// 📌 Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 📌 Add dependency injection
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<EmailService>(); // EmailService added
//builder.Services.AddSingleton<RabbitMqPublisher>(); // ✅ Publisher'ı ekle
//builder.Services.AddSingleton<RabbitMqConsumer>(); // 📌 Consumer eklendi

// 📌 Add Authentication and JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// 📌 Add CORS policy to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 📌 Configure Database Context
var conStr = builder.Configuration.GetConnectionString("MySqlConStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(conStr, ServerVersion.AutoDetect(conStr));
});

// 📌 Configure JSON serialization to handle cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 📌 Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 📌 Enable CORS
app.UseCors("AllowFrontend");

// 📌 Add Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// 📌 Map controllers
app.MapControllers();

// 📌 Start RabbitMQ Consumer (Siparişleri dinlemek için)
//var consumer = app.Services.GetRequiredService<RabbitMqConsumer>(); 
//Task.Run(async () => await consumer.ConsumeOrdersAsync()); 

app.Run();
