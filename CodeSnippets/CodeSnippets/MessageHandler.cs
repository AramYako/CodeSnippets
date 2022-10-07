using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CodeSnippets
{
    public class MessageHandler
    {

        private void MockAndVerifyHttpMessageHandler()
        {
            //Mock httpClient
            //Get posted content
            var MockHttpMessageHandler = new Mock<HttpMessageHandler>();

            string ActualHttpRequestContent = "";
            HttpRequestMessage ActualHttpRequestMessage = null;

            MockHttpMessageHandler
            .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>(
                    (httpRequestMessage, cancellationToken) =>
                    {
                        ActualHttpRequestContent = httpRequestMessage.Content
                           .ReadAsStringAsync()
                           .GetAwaiter()
                           .GetResult();

                        ActualHttpRequestMessage = httpRequestMessage;
                    })
                .Returns(
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(string.Empty)
                }));

            var httpClient = new HttpClient(MockHttpMessageHandler.Object);


            //Verify httpClient
            MockHttpMessageHandler.Protected().Verify("SendAsync", Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post
                && req.RequestUri == new Uri($"https://saintgobaindsigroupe4--pltqa.sandbox.testtesttest.salesforce.com/services/Soap/c/55.0/00D1/00511ZZZ")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
