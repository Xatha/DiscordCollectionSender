using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilsLibrary
{
    public static class ListUtils
    {
        public static bool AnyOrFalse<T>(this List<T> list) => list?.Any() ?? default;

    }
}
