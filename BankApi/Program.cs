using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BankApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankDatabase>(opt => opt.UseInMemoryDatabase("BanksAccounts"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankAPI", Version = "v1" });
});

builder.Services.AddTransient<IAmazonSimpleNotificationService>(sp =>
    new AmazonSimpleNotificationServiceClient());




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var snsTopicArn = app.Configuration.GetValue<string>("AwsConfig:Arn");



app.MapPost("/bank/withdraw", async (
    string accountId,
    decimal amount,
    BankDatabase dbContext,
    IAmazonSimpleNotificationService snsClient,
    ILogger<Program> logger) =>
{
    try
    {
        var account = await dbContext.BankAccounts.FindAsync(accountId);

        if (account == null)
        {
            logger.LogInformation("Account not found.");
            return Results.NotFound("Account not found.");
        }

        if (account.CurrentBalance >= amount)
        {

            account.CurrentBalance = account.CurrentBalance - amount;
            await dbContext.SaveChangesAsync();

            var eventMessage = new WithdrawalEvent(amount, accountId, "SUCCESSFUL").ToJson();

            await PublishMessage(eventMessage);
            logger.LogInformation("Message Published successfully");

            return Results.Ok("Withdrawal successful");
        }
        else
        {
            return Results.BadRequest("Insufficient funds for withdrawal");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during the withdrawal process.");
        return Results.Problem("An error occurred during the withdrawal process.");
    }

    async Task PublishMessage(string eventMessage)
    {
        var publishRequest = new PublishRequest
        {
            Message = eventMessage,
            TopicArn = snsTopicArn
        };

    await snsClient.PublishAsync(publishRequest);

    }
});

app.Run();





