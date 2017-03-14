using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    public class EnumerablesComparer : IEnumerablesComparer
    {
        private readonly ComparisonSettings _settings;
        private readonly IObjectsDataComparer _parentComparer;

        public EnumerablesComparer(ComparisonSettings settings, IObjectsDataComparer parentComparer = null)
        {
            _settings = settings;
            _parentComparer = parentComparer;
        }

        public IEnumerable<Difference> Compare(object obj1, object obj2)
        {
            if (!_settings.EmptyAndNullEnumerablesEqual &&
                (obj1 == null || obj2 == null) && obj1 != obj2)
            {
                yield return new Difference("[]", obj1?.ToString() ?? string.Empty, obj2?.ToString() ?? string.Empty);
                yield break;
            }

            obj1 = obj1 ?? Enumerable.Empty<object>();
            obj2 = obj2 ?? Enumerable.Empty<object>();

            if (!obj1.GetType().InheritsFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(obj1));
            }

            if (!obj2.GetType().InheritsFrom(typeof(IEnumerable)))
            {
                throw new ArgumentException(nameof(obj2));
            }

            var array1 = ((IEnumerable)obj1).Cast<object>().ToArray();
            var array2 = ((IEnumerable)obj2).Cast<object>().ToArray();

            if (array1.Length != array2.Length)
            {
                yield return new Difference("[]", array1.Length.ToString(), array2.Length.ToString());
                yield break;
            }

            for (var i = 0; i < array2.Length; i++)
            {
                if (array1[i].GetType() != array2[i].GetType())
                {
                    yield return new Difference($"[{i}]", array1[i] + string.Empty, array2[i] + string.Empty);
                    continue;
                }

                var comparer = ObjectsDataComparer<object>.CreateComparer(array1[i].GetType(), _settings, _parentComparer);

                foreach (var failure in comparer.CalculateDifferences(array1[i], array2[i]))
                {
                    yield return failure.InsertPath($"[{i}]");
                }
            }
        }
    }
}
