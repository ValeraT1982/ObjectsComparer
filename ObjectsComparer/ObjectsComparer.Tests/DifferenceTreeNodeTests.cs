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
        public void XXX()
        {
            var a1 = new A { ClassB = new B { Property1 = "hello" } };
            var a2 = new A { ClassB = new B { Property1 = "hallo" } };

            var comparer = new Comparer<A>();
            var rootNode = comparer.CalculateDifferenceTree(a1, a2);
            var differences = rootNode.GetDifferences().ToList();
            var classB = rootNode.Descendants.Single(n => n.Member.Name == nameof(A.ClassB));
            var property1 = classB.Descendants.Single(n => n.Member.Name == nameof(B.Property1));

            Assert.AreEqual(1, differences.Count);
            Assert.AreEqual(differences[0].MemberPath, $"{classB.Member.Name}.{property1.Member.Name}");
        }

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
        public void TestThrowDifferenceTreeBuilderNotImplemented_ConfigureListComparison()
        {
            var settings = new ComparisonSettings();

            settings.ConfigureListComparison();

            var factory = new CustomComparersFactory();
            var comparer = factory.GetObjectsComparer<A>(settings);

            var a1 = new A { ClassB = new B() };
            var a2 = new A { ClassB = new B() };

            Assert.Throws<DifferenceTreeBuilderNotImplementedException>(() => comparer.CalculateDifferences(a1, a2).ToArray());
        }

        [Test]
        public void TestThrowDifferenceTreeBuilderNotImplemented_ConfigureDifference()
        {
            var settings = new ComparisonSettings();

            settings.ConfigureDifference(false);

            var factory = new CustomComparersFactory();
            var comparer = factory.GetObjectsComparer<A>(settings);

            var a1 = new A { ClassB = new B() };
            var a2 = new A { ClassB = new B() };

            Assert.Throws<DifferenceTreeBuilderNotImplementedException>(() => comparer.CalculateDifferences(a1, a2).ToArray());
        }

        [Test]
        public void TestThrowDifferenceTreeBuilderNotImplemented_ConfigureDifferenceTree()
        {
            var settings = new ComparisonSettings();

            settings.ConfigureDifferenceTree((_, options) => { });

            var factory = new CustomComparersFactory();
            var comparer = factory.GetObjectsComparer<A>(settings);

            var a1 = new A { ClassB = new B() };
            var a2 = new A { ClassB = new B() };

            Assert.Throws<DifferenceTreeBuilderNotImplementedException>(() => comparer.CalculateDifferences(a1, a2).ToArray());
        }

        [Test]
        public void TestThrowDifferenceTreeBuilderNotImplemented_ConfigureDifferencePath()
        {
            var settings = new ComparisonSettings();

            settings.ConfigureDifferencePath((_, options) => { });

            var factory = new CustomComparersFactory();
            var comparer = factory.GetObjectsComparer<A>(settings);

            var a1 = new A { ClassB = new B() };
            var a2 = new A { ClassB = new B() };

            Assert.Throws<DifferenceTreeBuilderNotImplementedException>(() => comparer.CalculateDifferences(a1, a2).ToArray());
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

        [Test]
        public void CompareIntArrayUnequalListEnabled()
        {
            var a1 = new int[] { 3, 2, 1 };
            var a2 = new int[] { 1, 2, 3, 4 };

            var settings = new ComparisonSettings();
            settings.ConfigureListComparison(compareUnequalLists: true);
            var comparer = new Comparer(settings);

            var diffs = comparer.CalculateDifferences(a1, a2);

            foreach (var item in diffs)
            {
                System.Diagnostics.Debug.WriteLine(item);
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
            if (typeof(T) == typeof(B))
            {
                return (IComparer<T>)new CustomClassBComparer(settings, parentComparer, this);
            }

            if (typeof(T) == typeof(string))
            {
                return (IComparer<T>)new CustomStringComparer(settings, parentComparer, this);
            }

            return base.GetObjectsComparer<T>(settings, parentComparer);
        }
    }

    class CustomStringComparer : AbstractComparer<string>
    {
        public CustomStringComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(string obj1, string obj2)
        {
            //var comparer = Factory.GetObjectsComparer(obj1.GetType());
            //return comparer.CalculateDifferences(obj1, obj2);
            throw new NotImplementedException();
        }
    }

    class CustomClassBComparer : AbstractComparer<B>
    {
        public CustomClassBComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory) : base(settings, parentComparer, factory)
        {
        }

        public override IEnumerable<Difference> CalculateDifferences(B obj1, B obj2)
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
