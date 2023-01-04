

using System.Text;
using Microsoft.AspNetCore.Mvc;

const string CustomPolicyKey = "CustomPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: CustomPolicyKey, policy =>
    {
        policy.WithOrigins("https://dockerhost:7222")
            .AllowAnyHeader()
            .AllowCredentials()
            ;
    });
});

var app = builder.Build();

app.UseStaticFiles();

app.UseCors(CustomPolicyKey);

app.MapPost("token", ([FromBody]CreateTaskModel model) => {
    return Results.Ok(new { Token = $"token {model.UserName}"});
});

app.MapGet("/sse", async (ctx) => {
        
    ctx.Response.Headers.Add("Cache-Control", "no-cache");
    ctx.Response.Headers.Add("Content-Type", "text/event-stream");
    await ctx.Response.Body.FlushAsync();
    
    var str = new StringBuilder();

    for(var id = 0; id < 10; id++) {
        str.Clear();
        str.AppendLine($"id: {id}");
        str.AppendLine($"data: {id}: {DateTime.Now}");
        str.AppendLine();
        await ctx.Response.WriteAsync(str.ToString());

        await Task.Delay(2000);
    }


    str.Clear();
    str.AppendLine("id: -1");
    str.AppendLine($"data: exit");
    str.AppendLine();
    await ctx.Response.WriteAsync(str.ToString());

});

app.Run();
