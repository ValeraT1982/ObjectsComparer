using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using ObjectsComparer.Tests.Utils;
using System.Reflection;
using System.Diagnostics;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    internal class ComparisonContextTests
    {
        [Test]
        public void ComparisonContextMember_Member_Correct_MemberName()
        {
            var ctxMember = new ComparisonContextMember(name: "Property1");
            Assert.AreEqual("Property1", ctxMember.Name);
            Assert.AreEqual(null, ctxMember.Info);
        }

        [Test]
        public void ComparisonContextMember_Member_Correct_Member()
        {
            var memberInfo = typeof(Address).GetMember(nameof(Address.Country)).Single();
            var ctxMember = new ComparisonContextMember(memberInfo, memberInfo.Name);
            Assert.AreEqual(nameof(Address.Country), ctxMember.Info.Name);
            Assert.AreEqual(nameof(Address.Country), ctxMember.Name);
        }

        [Test]
        public void CustomComparisonContext()
        {            
            var settings = new ComparisonSettings();
            var rootCtx = ComparisonContextProvider.CreateRootContext();

            settings.ConfigureComparisonContext((currentContex, options) =>
            {
                options.UseComparisonContextFactory(ctxMember => new CustomComparisonContext(ctxMember, rootCtx));
            });
            
            var ctx = ComparisonContextProvider.CreateContext(settings, rootCtx, "Property1");

            Assert.AreEqual("Property1", ctx.Member.Name);
            Assert.IsTrue(ctx.GetType() == typeof(CustomComparisonContext));
            Assert.IsTrue(ctx.Ancestor == rootCtx);
        }

        [Test]
        public void CustomComparisonContextMember()
        {
            var settings = new ComparisonSettings();
            var rootCtx = ComparisonContextProvider.CreateRootContext();

            settings.ConfigureComparisonContext((currentContex, options) =>
            {
                options.UseComparisonContextMemberFactory(defaultMember => new CustomComparisonContextMember(defaultMember.Name));
            });
            
            var ctx = ComparisonContextProvider.CreateContext(settings, rootCtx, "Property1");

            Assert.AreEqual("Property1", ctx.Member.Name);
            Assert.AreEqual(null, ctx.Member.Info);
            Assert.IsTrue(ctx.Member.GetType() == typeof(CustomComparisonContextMember));
            Assert.IsTrue(ctx.Ancestor == rootCtx);
        }

        [Test]
        public void ComparisonContextException()
        {
            var factory = new CustomComparersFactory();
            var comparer = factory.GetObjectsComparer<string>();
            var rootCtx = ComparisonContextProvider.CreateRootContext();

            var diffs =  comparer.CalculateDifferences("hello", "hi", rootCtx).ToArray();
        }

        [Test]
        public void EnumerateConditional()
        {
            var list = new List<int> { 6, 8, 79, 3, 45, 9 };

            var enumerateConditional = EnumerateConditionalExt(
                list,
                moveNextItem: () => true,
                completed: () => Debug.WriteLine("Completed"));

            foreach (var item in enumerateConditional)
            {
                Debug.WriteLine(item);
            }
        }

        protected IEnumerable<T> EnumerateConditionalExt<T>(IEnumerable<T> enumerable, Func<bool> moveNextItem, Action completed = null)
        {
            var enumerator = enumerable.GetEnumerator();

            while (moveNextItem())
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                else
                {
                    completed?.Invoke();
                    break;
                }
            }
        }
    }

    class CustomComparersFactory : ComparersFactory
    {
        public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, BaseComparer parentComparer = null)
        {
            if (typeof(T) != typeof(string))
            {
                return base.GetObjectsComparer<T>(settings, parentComparer);
            }

            return (IComparer<T>)new CustomStringComparer(settings, parentComparer, this);
        }
    }

    class CustomStringComparer : AbstractComparer<string>
    {
        public CustomStringComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(string obj1, string obj2)
        {
            throw new NotImplementedException();
        }
    }

    class CustomComparisonContext : ComparisonContextBase
    {
        public CustomComparisonContext(IComparisonContextMember member = null, IComparisonContext ancestor = null) : base(member, ancestor)
        {

        }
    }

    class CustomComparisonContextMember : IComparisonContextMember
    {
        public CustomComparisonContextMember(string memberName)
        {
            Name = memberName;
        }
        public MemberInfo Info => null;

        public string Name { get; }
    }
}
