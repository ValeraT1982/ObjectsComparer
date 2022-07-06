using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;

namespace ObjectsComparer.Tests

{
	[TestFixture]
	public class DifferenceConfigurationTests
	{
		[Test]
		public void PreserveRawValuesDefaultBehavior()
		{
			var a1 = new A() { IntProperty = 10 };
			var a2 = new A() { IntProperty = 11 };

			var comparer = new Comparer<A>();

			var differences = comparer.CalculateDifferences(a1, a2).ToArray();

			Assert.IsTrue(differences[0].RawValue1 == null);
			Assert.IsTrue(differences[0].RawValue2 == null);
		}

		[Test]
		public void PreserveRawValues()
		{
			var a1 = new A() { IntProperty = 10 };
			var a2 = new A() { IntProperty = 11 };

			var settings = new ComparisonSettings();
			settings.ConfigureDifference(includeRawValues: true);

			var comparer = new Comparer<A>(settings);
			
			var differences = comparer.CalculateDifferences(a1, a2).ToArray();

			Assert.IsTrue((int)differences[0].RawValue1 == 10);
			Assert.IsTrue((int)differences[0].RawValue2 == 11);
		}

		[Test]
		public void DontPreserveRawValues()
		{
			var a1 = new A() { IntProperty = 10 };
			var a2 = new A() { IntProperty = 11 };

			var settings = new ComparisonSettings();
			settings.ConfigureDifference(includeRawValues: false);

			var comparer = new Comparer<A>(settings);

			var differences = comparer.CalculateDifferences(a1, a2).ToArray();

			Assert.IsTrue(differences[0].RawValue1 == null);
			Assert.IsTrue(differences[0].RawValue2 == null);
		}

		[Test]
		public void PreserveRawValuesConditionaly()
		{
			var a1 = new A() { IntProperty = 10, TestProperty1 = "TestProperty1value" };
			var a2 = new A() { IntProperty = 11, TestProperty1 = "TestProperty2value" };

			var settings = new ComparisonSettings();

			settings.ConfigureDifference((currentProperty, options) => 
			{
				options.IncludeRawValues(currentProperty.Member.Name == "IntProperty");
			});

			var comparer = new Comparer<A>(settings);

			var differences = comparer.CalculateDifferences(a1, a2).ToArray();

			Assert.IsTrue(differences[0].MemberPath == "IntProperty");
			Assert.IsTrue((int)differences[0].RawValue1 == 10);
			Assert.IsTrue((int)differences[0].RawValue2 == 11);

			Assert.IsTrue(differences[1].MemberPath == "TestProperty1");
			Assert.IsTrue(differences[1].RawValue1 == null);
			Assert.IsTrue(differences[1].RawValue2 == null);
		}

		[Test]
		public void CustomizeMemberPath()
		{
			var a1 = new A() { ClassB = new B { Property1 = "value1 " }, IntProperty = 10, TestProperty2 = "value1", ListOfC = new List<C> { new C { Property1 = "property1" } } };
			var a2 = new A() { ClassB = new B { Property1 = "value2 " }, IntProperty = 11, TestProperty2 = "value2", ListOfC = new List<C> { new C { Property1 = "property2" } } };

			var settings = new ComparisonSettings();

			settings
				.ConfigureDifference((currentProperty, options) =>
				{
					if (currentProperty.Member.Name == nameof(A.IntProperty))
					{
						options.UseDifferenceFactory(args => new Difference("Integer property", args.DefaultDifference.Value1, args.DefaultDifference.Value2));
					}
					else if (currentProperty.Member.Name == nameof(C.Property1))
					{
                        if (currentProperty.Ancestor.Member.Name == "ClassB")
                        {
							options.UseDifferenceFactory(args => new Difference("First property of Class B", args.DefaultDifference.Value1, args.DefaultDifference.Value2));
						}
                        else
                        {
							options.UseDifferenceFactory(args => new Difference("First property of class A", args.DefaultDifference.Value1, args.DefaultDifference.Value2));
						}
					}
				})
				.ConfigureDifferencePath((parentProperty, options) => 
				{
					if (parentProperty.Member.Name == nameof(A.ListOfC))
					{
						options.UseInsertPathFactory(args => "Collection of C objects");
					}
				});

			var comparer = new Comparer<A>(settings);

			var differences = comparer.CalculateDifferences(a1, a2).ToArray();

			Assert.IsTrue(differences.Any(diff => diff.MemberPath == "Integer property"));
			Assert.IsTrue(differences.Any(diff => diff.MemberPath == "TestProperty2"));
			Assert.IsTrue(differences.Any(diff => diff.MemberPath == "Collection of C objects[0].First property of class A"));
			Assert.IsTrue(differences.Any(diff => diff.MemberPath == "ClassB.First property of Class B"));
		}
	}
}

