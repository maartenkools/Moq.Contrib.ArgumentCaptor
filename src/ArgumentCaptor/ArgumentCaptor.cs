using Moq.Protected;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moq.Contrib.ArgumentCaptor
{
    public class ArgumentCaptor<TValue> : IReadOnlyList<TValue>
    {
        private readonly IList<TValue> values = new List<TValue>(5);

        public TValue Capture()
        {
            return It.Is<TValue>(value => SaveValue(value));
        }

        public Expression CaptureExpr()
        {
            return ItExpr.Is<TValue>(value => SaveValue(value));
        }

        private bool SaveValue(TValue value)
        {
	        this.values.Add(value);
            return true;
        }

        public int Count => this.values.Count;
        
        public TValue this[int index] => this.values[index];

        public IEnumerator<TValue> GetEnumerator() => this.values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
