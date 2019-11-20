﻿using System;
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
        private readonly List<ValueComparerWithFilter> _overridesByFilter = new List<ValueComparerWithFilter>();

        public void AddComparer(MemberInfo memberInfo, IValueComparer valueComparer)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            if (_overridesByMember.ContainsKey(memberInfo))
            {
                throw new ValueComparerExistsException(memberInfo);
            }

            _overridesByMember[memberInfo] = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
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

        public void AddComparer(Func<MemberInfo, bool> filter, IValueComparer valueComparer)
        {
            if (valueComparer == null)
            {
                throw new ArgumentNullException(nameof(valueComparer));
            }

            _overridesByFilter.Add(new ValueComparerWithFilter(valueComparer, filter));
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

            foreach (var overridePair in collection._overridesByFilter)
            {
                AddComparer(overridePair.Filter, overridePair.ValueComparer);
            }
        }

        public IValueComparer GetComparer(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!_overridesByType.TryGetValue(type, out var overridesByType))
            {
                return null;
            }

            overridesByType = overridesByType.Where(o => o.Filter == null).ToList();
            if (overridesByType.Count > 1)
            {
                throw new AmbiguousComparerOverrideResolutionException(type);
            }

            return overridesByType.Count == 1 ? overridesByType[0].ValueComparer : null;
        }

        public IValueComparer GetComparer(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            if (_overridesByMember.TryGetValue(memberInfo, out var overrideByMemberInfo))
            {
                return overrideByMemberInfo;
            }

            if (_overridesByName.TryGetValue(memberInfo.Name, out var overridesByName))
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

            if (_overridesByType.TryGetValue(memberInfo.GetMemberType(), out var overridesByType))
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

            var overridesByFilter = _overridesByFilter.Where(o => o.Filter == null || o.Filter(memberInfo)).ToList();

            if (overridesByFilter.Count > 1)
            {
                throw new AmbiguousComparerOverrideResolutionException(memberInfo);
            }

            if (overridesByFilter.Count == 1)
            {
                return overridesByFilter[0].ValueComparer;
            }

            return null;
        }

        public IValueComparer GetComparer(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            if (!_overridesByName.TryGetValue(memberName, out var overridesByName))
            {
                return null;
            }

            overridesByName = overridesByName.Where(o => o.Filter == null).ToList();
            if (overridesByName.Count > 1)
            {
                throw new AmbiguousComparerOverrideResolutionException(memberName);
            }

            return overridesByName.Count == 1 ? overridesByName[0].ValueComparer : null;
        }
    }
}
