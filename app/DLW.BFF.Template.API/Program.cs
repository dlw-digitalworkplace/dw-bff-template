using DLW.BFF.Template.Core.Extensions;

// Create the Web Application builder and add the required services
var builder = WebApplication.CreateBuilder(args);

// Add the API controllers
builder.Services.AddControllers();

// Add the Swagger services
builder.Services.AddSwagger(builder.Configuration, typeof(Program).Assembly.GetName().Name);

// Add services to the container.
builder.Services.AddOAuthTokenAuthentication(builder.Configuration);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger(typeof(Program).Assembly.GetName().Name);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
