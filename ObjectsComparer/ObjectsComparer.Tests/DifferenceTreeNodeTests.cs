using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using ObjectsComparer.Tests.Utils;
using System.Reflection;
using System.Diagnostics;
using ObjectsComparer.Utils;
using ObjectsComparer.DifferenceTreeExtensions;
using ObjectsComparer.Exceptions;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    internal class DifferenceTreeNodeTests
    {
        [Test]
        public void DifferenceTreeNodeMember_Member_Correct_MemberName()
        {
            var treeNodeMember = new DifferenceTreeNodeMember(name: "Property1");
            Assert.AreEqual("Property1", treeNodeMember.Name);
            Assert.AreEqual(null, treeNodeMember.Info);
        }

        [Test]
        public void DifferenceTreeNodeMember_Member_Correct_Member()
        {
            var memberInfo = typeof(Address).GetMember(nameof(Address.Country)).Single();
            var treeNodeMember = new DifferenceTreeNodeMember(memberInfo, memberInfo.Name);
            Assert.AreEqual(nameof(Address.Country), treeNodeMember.Info.Name);
            Assert.AreEqual(nameof(Address.Country), treeNodeMember.Name);
        }

        [Test]
        public void CustomDifferenceTreeNode()
        {            
            var settings = new ComparisonSettings();
            var rootNode = DifferenceTreeNodeProvider.CreateRootNode();

            settings.ConfigureDifferenceTree((currentNode, options) =>
            {
                options.UseDifferenceTreeNodeFactory(treeNodeMember => new CustomDifferenceTreeNode(treeNodeMember, rootNode));
            });
            
            var treeNode = DifferenceTreeNodeProvider.CreateNode(settings, rootNode, "Property1");

            Assert.AreEqual("Property1", treeNode.Member.Name);
            Assert.IsTrue(treeNode.GetType() == typeof(CustomDifferenceTreeNode));
            Assert.IsTrue(treeNode.Ancestor == rootNode);
        }

        [Test]
        public void CustomDifferenceTreeNodeMember()
        {
            var settings = new ComparisonSettings();
            var rootNode = DifferenceTreeNodeProvider.CreateRootNode();

            settings.ConfigureDifferenceTree((currentContex, options) =>
            {
                options.UseDifferenceTreeNodeMemberFactory(defaultMember => new CustomDifferenceTreeNodeMember(defaultMember.Name));
            });
            
            var treeNode = DifferenceTreeNodeProvider.CreateNode(settings, rootNode, "Property1");

            Assert.AreEqual("Property1", treeNode.Member.Name);
            Assert.AreEqual(null, treeNode.Member.Info);
            Assert.IsTrue(treeNode.Member.GetType() == typeof(CustomDifferenceTreeNodeMember));
            Assert.IsTrue(treeNode.Ancestor == rootNode);
        }

        [Test]
        public void TestThrowDifferenceTreeBuilderNotImplementedException()
        {
            var factory = new CustomComparersFactory();
            var comparer = factory.GetObjectsComparer<string>();
            var rootNode = DifferenceTreeNodeProvider.CreateRootNode();
            Assert.Throws<DifferenceTreeBuilderNotImplementedException>(() => comparer.TryBuildDifferenceTree("hello", "hi", rootNode).ToArray());
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

        [Test]
        public void EnumerateConditional_Completed()
        {
            var list = new List<int> { 6, 8, 79, 3, 45, 9 };
            bool completed = false;
            int? lastElement = null;

            list.EnumerateConditional(
                element =>
                {
                    lastElement = element;
                    return true;
                },
                () => completed = true);

            Assert.AreEqual(9, lastElement);
            Assert.AreEqual(true, completed);
        }
        
        [Test]
        public void EnumerateConditional_FetchFirst_NotCompleted()
        {
            var list = new List<int> { 6, 8, 79, 3, 45, 9 };
            bool completed = false;
            int? firstElement = null;

            list.EnumerateConditional(
                element => 
                {
                    firstElement = element;
                    return false; 
                },
                () => completed = true);

            Assert.AreEqual(6, firstElement);
            Assert.AreEqual(false, completed);
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

    class CustomDifferenceTreeNode : DifferenceTreeNodeBase
    {
        public CustomDifferenceTreeNode(IDifferenceTreeNodeMember member = null, IDifferenceTreeNode ancestor = null) : base(member, ancestor)
        {

        }
    }

    class CustomDifferenceTreeNodeMember : IDifferenceTreeNodeMember
    {
        public CustomDifferenceTreeNodeMember(string memberName)
        {
            Name = memberName;
        }
        public MemberInfo Info => null;

        public string Name { get; }
    }

    //Test commit.
}
