using Azure;
using Azure.Communication.Email;

using Models;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;

using System.ComponentModel;
using System.Globalization;
using System.Net;



namespace Plugins.SendMailPlugin;

public class Email
{
    /// Sends an email to the specified recipient.
    [Function("SendEmail")]
    [OpenApiOperation(operationId: "SendEmail", tags: new[] { "ExecuteFunction" }, Description = "Sends an email to the specified recipient.")]
    [OpenApiParameter(name: "input", Description = "The email to send the message to.", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "subject", Description = "The subject of the email.", Required = true, In = ParameterLocation.Query)]
    [OpenApiParameter(name: "body", Description = "The body of the email.", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns if the email was successfully sent.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public HttpResponseData SendEmail([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        var appSettings = AppSettings.LoadSettings();
        var emailServiceConnectionString = appSettings.EmailServiceConnectionString;

        var recipient = req.Query["input"];
        var subject = req.Query["subject"];
        var body = req.Query["body"];

        var emailClient = new EmailClient(emailServiceConnectionString);

        var result = emailClient.SendAsync(
            wait: Azure.WaitUntil.Completed,
            senderAddress: "DoNotReply@0b2fc93b-542b-45e6-9919-d8fedfbf7cc4.azurecomm.net",
            recipientAddress: recipient,
            subject: subject,
            htmlContent: body).Result;

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");

        response.WriteString("Email sent successfully.");
        
        return response;


    }
}