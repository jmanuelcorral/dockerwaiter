using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dockerwaiter.unittests.Utils
{
    public static class TestExtensions
    {
        public static Task<IEnumerable<T>> BoxInsideTask<T>(this IEnumerable<T> collection)
        {
            return Task.FromResult(collection);
        }
    }
}
