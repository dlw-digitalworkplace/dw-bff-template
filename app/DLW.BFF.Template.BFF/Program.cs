using DLW.BFF.Template.BFF.Extensions;

// Create the Web Application builder and add the required services
var builder = WebApplication.CreateBuilder(args);

// Add the API controllers
builder.Services.AddControllers();

// Add the Swagger services
builder.Services.AddSwagger(builder.Configuration);

// Add authentication services
builder.Services.AddAuthentication(builder.Configuration);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwaggerSetup();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");
app.Run();
