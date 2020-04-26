# Moq.Contrib.ArgumentCaptor

Provides an alternative to `It.IsAny<T>()` and `ItExp.IsAny<T>()` where you can capture the argument as well.

## Install

`Install-Package Moq.Contrib.ArgumentCaptor`

or `dotnet add package Moq.Contrib.ArgumentCaptor`

## Examples

### Basic usage
```csharp
var mock = new Mock<IMyInterface>();

var sut = new SystemUnderTest(mock.Object);
sut.CallMethod();

var argumentCaptor = new ArgumentCaptor<string>();
// Verify IMyInterface.InterfaceMethod is called
mock.Verify(x => x.InterfaceMethod(argumentCaptor.Capture()));

// Assert that the argument to IMyInterface.InterfaceMethod is equal to expectedValue
argumentCaptor.Value
              .Should()
			  .BeEqualTo("expectedValue");
```

### As a replacement to ItExpr.IsAny<T>()
```csharp
var configuration = new Mock<IConfiguration>();
configuration.Setup(x => x["AccessToken"])
			 .Returns("mockAccessToken");

var httpMessageHandler = new Mock<HttpMessageHandler>();
httpMessageHandler.Protected()
				  .Setup<Task<HttpResponseMessage>>("SendAsync",
												   ItExpr.IsAny<HttpRequestMessage>(),
												   ItExpr.IsAny<CancellationToken>())
				  .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent)));

var httpClientAdapter = new HttpClientAdapter(configuration.Object,
											  httpMessageHandler.Object);
await httpClientAdapter.GetAsync(new Uri("https://unit.test/api/get"))
					   .ConfigureAwait(false);

var requestMessageCaptor = new ArgumentCaptor<HttpRequestMessage>();
httpMessageHandler.Protected()
				  .Verify<Task<HttpResponseMessage>>("SendAsync",
													Type.EmptyTypes,
													Times.Once(),
													requestMessageCaptor.CaptureExpr(),
													ItExpr.IsAny<CancellationToken>());

requestMessageCaptor.Value
					.Headers
					.Should()
					.BeEquivalentTo(new Dictionary<string, IEnumerable<string>>
									{
										["Authorization"] = new[] { "Bearer mockAccessToken" },
										["Accept"] = new[] { "application/json" }
									});
```

### Using a custom predicate to match arguments
```csharp

var mock = new Mock<IMyInterface>();

var sut = new SystemUnderTest(mock.Object);
sut.CallMethod();

var firstArgCaptor = new ArgumentCaptor<string>(x => x.StartsWith("frst"));
var secondArgCaptor = new ArgumentCaptor<string>(x => x.StartsWith("scnd"));
// Verify IMyInterface.InterfaceMethod is called
mock.Verify(x => x.InterfaceMethod(firstArgCaptor.Capture(), secondArgCaptor.Capture()));

firstArgCaptor.Value
              .Should()
			  .BeEqualTo("frstValue");
secondArgCaptor.Value
               .Should()
			   .BeEqualTo("scndValue");
```