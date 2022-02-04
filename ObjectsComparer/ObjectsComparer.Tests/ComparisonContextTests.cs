using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using ObjectsComparer.Tests.Utils;
using System.Reflection;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    internal class ComparisonContextTests
    {
        [Test]
        public void ComparisonContextMember_Member_Correct_MemberName()
        {
            var ctxMember = ComparisonContextMember.Create("Property1");
            Assert.AreEqual("Property1", ctxMember.Name);
            Assert.AreEqual(null, ctxMember.Info);
        }

        [Test]
        public void ComparisonContextMember_Member_Correct_Member()
        {
            var memberInfo = typeof(Address).GetMember(nameof(Address.Country)).Single();
            var ctxMember = ComparisonContextMember.Create(memberInfo);
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
