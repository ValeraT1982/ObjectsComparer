using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Exceptions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class ComparerOverridesCollection
    {
        private class ValueComparerWithFilter
        {
            public IValueComparer ValueComparer { get; }

            public Func<MemberInfo, bool> Filter { get; }

            public ValueComparerWithFilter(IValueComparer valueComparer, Func<MemberInfo, bool> filter)
            {
                Filter = filter;
                ValueComparer = valueComparer;
            }
        }

        private readonly Dictionary<MemberInfo, IValueComparer> _overridesByMember = new Dictionary<MemberInfo, IValueComparer>();
        private readonly Dictionary<Type, List<ValueComparerWithFilter>> _overridesByType = new Dictionary<Type, List<ValueComparerWithFilter>>();
        private readonly Dictionary<string, List<ValueComparerWithFilter>> _overridesByName = new Dictionary<string, List<ValueComparerWithFilter>>();

        public void AddComparer(MemberInfo memberInfo, IValueComparer valueComparer)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            if (valueComparer == null)
            {
                throw new ArgumentNullException(nameof(valueComparer));
            }

            if (_overridesByMember.ContainsKey(memberInfo))
            {
                throw new ValueComparerExistsException(memberInfo);
            }

            _overridesByMember[memberInfo] = valueComparer;
        }

        public void AddComparer(Type type, IValueComparer valueComparer,
            Func<MemberInfo, bool> filter = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (valueComparer == null)
            {
                throw new ArgumentNullException(nameof(valueComparer));
            }

            if (!_overridesByType.ContainsKey(type))
            {
                _overridesByType[type] = new List<ValueComparerWithFilter>();
            }

            _overridesByType[type].Add(new ValueComparerWithFilter(valueComparer, filter));
        }

        public void AddComparer(string memberName, IValueComparer valueComparer,
            Func<MemberInfo, bool> filter = null)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException($"{nameof(memberName)} cannot be null or empty");
            }

            if (valueComparer == null)
            {
                throw new ArgumentNullException(nameof(valueComparer));
            }

            if (!_overridesByName.ContainsKey(memberName))
            {
                _overridesByName[memberName] = new List<ValueComparerWithFilter>();
            }

            _overridesByName[memberName].Add(new ValueComparerWithFilter(valueComparer, filter));
        }

        public void Merge(ComparerOverridesCollection collection)
        {
            foreach (var overridePair in collection._overridesByMember)
            {
                AddComparer(overridePair.Key, overridePair.Value);
            }

            foreach (var overrideCollection in collection._overridesByType)
            {
                foreach (var overridePair in overrideCollection.Value)
                {
                    AddComparer(overrideCollection.Key, overridePair.ValueComparer, overridePair.Filter);
                }
            }

            foreach (var overrideCollection in collection._overridesByName)
            {
                foreach (var overridePair in overrideCollection.Value)
                {
                    AddComparer(overrideCollection.Key, overridePair.ValueComparer, overridePair.Filter);
                }
            }
        }

        public IValueComparer GetComparer(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            List<ValueComparerWithFilter> overridesByType;
            if (_overridesByType.TryGetValue(type, out overridesByType))
            {
                overridesByType = overridesByType.Where(o => o.Filter == null).ToList();

                if (overridesByType.Count > 1)
                {
                    throw new AmbiguousComparerOverrideResolutionException(type);
                }

                if (overridesByType.Count == 1)
                {
                    return overridesByType[0].ValueComparer;
                }
            }

            return null;
        }

        public IValueComparer GetComparer(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            List<ValueComparerWithFilter> overridesByType;
            if (_overridesByType.TryGetValue(memberInfo.GetMemberType(), out overridesByType))
            {
                overridesByType = overridesByType.Where(o => o.Filter == null || o.Filter(memberInfo)).ToList();

                if (overridesByType.Count > 1)
                {
                    throw new AmbiguousComparerOverrideResolutionException(memberInfo);
                }

                if (overridesByType.Count == 1)
                {
                    return overridesByType[0].ValueComparer;
                }
            }

            IValueComparer overrideByMemberInfo;
            if (_overridesByMember.TryGetValue(memberInfo, out overrideByMemberInfo))
            {
                return overrideByMemberInfo;
            }

            List<ValueComparerWithFilter> overridesByName;
            if (_overridesByName.TryGetValue(memberInfo.Name, out overridesByName))
            {
                overridesByName = overridesByName.Where(o => o.Filter == null || o.Filter(memberInfo)).ToList();

                if (overridesByName.Count > 1)
                {
                    throw new AmbiguousComparerOverrideResolutionException(memberInfo);
                }

                if (overridesByName.Count == 1)
                {
                    return overridesByName[0].ValueComparer;
                }
            }

            return null;
        }
    }
}
