using Azure;
using Azure.Communication.Email;

using Models;

using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

using System.ComponentModel;
using System.Globalization;


namespace Plugins.SendMailPlugin;

public class Email
{
    /// Sends an email to the specified recipient.
    [SKFunction, Description("Sends an email to the specified recipient.")]
    [SKParameter("recipient", "The email to send the message to.")]
    [SKParameter("subject", "The subject of the email.")]
    [SKParameter("body", "The body of the email.")]
    [OpenApiOperation(operationId: "SendEmail", tags: new[] { "ExecuteFunction" }, Description = "Adds two numbers.")]
    [OpenApiParameter(name: "number1", Description = "The first number to add", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "number2", Description = "The second number to add", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the sum of the two numbers.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")] 
    public string SendEmail(SKContext context)
    {
        var appSettings = AppSettings.LoadSettings();
        var emailServiceConnectionString = appSettings.EmailServiceConnectionString;

        var recipient = context.Variables["recipient"];
        var subject = context.Variables["subject"];
        var body = context.Variables["body"];

        var emailClient = new EmailClient(emailServiceConnectionString); 

        var result = emailClient.SendAsync(
            wait: Azure.WaitUntil.Completed,
            senderAddress: "johnsontseng@microsoft.com",
            recipientAddress: recipient,
            subject: subject, 
            htmlContent: body).Result;

        return "send";

        
    }
}