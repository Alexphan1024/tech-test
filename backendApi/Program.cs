var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// For Testing ONlY
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.MapControllers();

//For Testing ONLY
app.UseCors("AllowAll");

app.UseCors();
app.Run();
