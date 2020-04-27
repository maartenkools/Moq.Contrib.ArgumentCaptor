using FluentAssertions;
using System.Linq;
using Moq.Protected;
using Xunit;

namespace Moq.Contrib.ArgumentCaptor.Tests
{
	public class ArgumentCaptorTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(5)]
		public void ArgumentCaptor_Capture_Captures_Argument(int invocations)
		{
			var @interface = new Mock<IInterface>();

			Invoke(@interface.Object, invocations);

			var argumentCaptor = new ArgumentCaptor<string>();
			@interface.Verify(x => x.Call(argumentCaptor.Capture()));

			argumentCaptor.Count
			              .Should()
			              .Be(invocations, $"because there should be {invocations} invocation(s)");
			argumentCaptor.Should()
			              .BeEquivalentTo(Enumerable.Range(0, invocations)
			                                        .Select(x => $"Invocation {x}"));

			for(var i = 0; i < invocations; ++i)
			{
				argumentCaptor[i].Should()
				                 .Be($"Invocation {i}");
			}
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(5)]
		public void ArgumentCaptor_CaptureExpr_Captures_Argument(int invocations)
		{
			var instance = new Mock<BaseClass>();

			Invoke(instance.Object, invocations);

			var argumentCaptor = new ArgumentCaptor<string>();
			instance.Protected()
			        .Verify("CallImpl",
			                Times.Exactly(invocations),
			                argumentCaptor.CaptureExpr());

			argumentCaptor.Count
			              .Should()
			              .Be(invocations, $"because there should be {invocations} invocation(s)");
			argumentCaptor.Should()
			              .BeEquivalentTo(Enumerable.Range(0, invocations)
			                                        .Select(x => $"Invocation {x}"));

			for (var i = 0; i < invocations; ++i)
			{
				argumentCaptor[i].Should()
				                 .Be($"Invocation {i}");
			}
		}

		#region Nested types

		private static void Invoke(IInterface instance, int count)
		{
			foreach (var i in Enumerable.Range(0,  count))
			{
				instance.Call($"Invocation {i}");
			}
		}

		internal interface IInterface
		{
			void Call(string value);
		}

		internal abstract class BaseClass : IInterface
		{
			public void Call(string value) => CallImpl(value);
			protected abstract void CallImpl(string value);
		}

		#endregion
	}
}
