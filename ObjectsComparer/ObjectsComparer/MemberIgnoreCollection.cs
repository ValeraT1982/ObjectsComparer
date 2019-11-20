using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ObjectsComparer.Exceptions;
using ObjectsComparer.Utils;

namespace ObjectsComparer
{
    internal class MemberIgnoreCollection
    {
        private class MemberIgnoreFilter
        {
            public Func<MemberInfo, bool> Filter { get; }

            public MemberIgnoreFilter(Func<MemberInfo, bool> filter)
            {
                Filter = filter;
            }
        }

        private readonly List<MemberIgnoreFilter> _ignoreFilters = new List<MemberIgnoreFilter>();

        public void AddMemberIgnore(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            _ignoreFilters.Add(new MemberIgnoreFilter(mi => mi.Equals(memberInfo)));
        }

        public void AddMemberIgnore(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _ignoreFilters.Add(new MemberIgnoreFilter(memberInfo => memberInfo.GetMemberType().Equals(type)));
        }

        public void AddComparer(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException($"{nameof(memberName)} cannot be null or empty");
            }

            _ignoreFilters.Add(new MemberIgnoreFilter(memberInfo => memberInfo.Name == memberName));
        }

        public void AddComparer(Func<MemberInfo, bool> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _ignoreFilters.Add(new MemberIgnoreFilter(filter));
        }

        public void Merge(MemberIgnoreCollection collection)
        {
            if (collection._ignoreFilters == null)
                return;

            foreach(MemberIgnoreFilter ignoreFilter in collection._ignoreFilters)
            {
                AddComparer(ignoreFilter.Filter);
            }
        }

        public bool DoesMemberPassFilters(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            foreach (MemberIgnoreFilter ignoreFilter in _ignoreFilters)
                if (ignoreFilter.Filter(member))
                    return true;

            return false;
        }
    }
}
