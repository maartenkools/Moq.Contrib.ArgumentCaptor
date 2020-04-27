# Moq.Contrib.ArgumentCaptor

Provides an alternative to `It.IsAny<T>()` and `ItExp.IsAny<T>()` where you can capture the argument as well.

## Install

`Install-Package Moq.Contrib.ArgumentCaptor`

or `dotnet add package Moq.Contrib.ArgumentCaptor`

## Examples

### Basic usage
```csharp
var mock = new Mock<IMyInterface>();

mock.Object
    .Call("someValue");

// Verify IMyInterface.Call is called
var argumentCaptor = new ArgumentCaptor<string>();
mock.Verify(x => x.Call(argumentCaptor.Capture()));

// Assert the value that was passed to the Call method.
argumentCaptor.Should()
              .BeEquivalentTo(new[] {"someValue"});

// Or
argumentCaptor.Single()
              .Should()
              .Be("someValue");
```

### As a replacement to ItExpr.IsAny<T>()
```csharp
var mock = new Mock<BaseClass>();

mock.Object
    .Call("someValue");

// Verify BaseClass.CallImpl is called
var argumentCaptor = new ArgumentCaptor<HttpRequestMessage>();
mock.Protected()
    .Verify<>("CallImpl",
              Times.Once(),
              argumentCaptor.CaptureExpr());

// Assert the value that was passed to the CallImpl method.
argumentCaptor.Should()
              .BeEquivalentTo(new[] {"someValue"});
// Or
argumentCaptor.Single()
              .Should()
              .Be("someValue");
```

### Asserting values from multiple calls
```csharp
var mock = new Mock<IMyInterface>();

for (int i = 0; i < 2; ++i)
{
    mock.Object
        .Call($"someValue {i}");
}

// Verify IMyInterface.Call is called
var argumentCaptor = new ArgumentCaptor<string>();
mock.Verify(x => x.Call(argumentCaptor.Capture()), Times.EXactly(2));

// Assert the values that were passed to the Call method.
argumentCaptor.Should()
              .BeEquivalentTo(new[] 
                                {
                                    "someValue 0",
                                    "someValue 1",
                                });

// You can also assert each value individually
argumentCaptor[0].Should()
                 .Be("someValue 0");
argumentCaptor[1].Should()
                 .Be("someValue 1");

// Or in a loop
for (int i = 0; i < argumentCaptor.Count; ++i)
{
    argumentCaptor[i].Should()
                     .Be($"someValue {i}");
}
```