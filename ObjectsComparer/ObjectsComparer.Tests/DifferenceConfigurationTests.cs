using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using System.Reflection;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using System.Collections;

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

        [Test]
        public void CalculateCompletedDifferenceTree()
        {
            var student1 = new Student
            {
                Person = new Person
                {
                    FirstName = "Daniel",

                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Prague", Country = "Czech republic" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var student2 = new Student
            {
                Person = new Person
                {
                    FirstName = "Dan",

                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czechia" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var comparer = new Comparer<Student>();

            var rootNode = comparer.CalculateDifferenceTree(student1, student2);

            Assert.AreEqual(3, rootNode.GetDifferences(recursive: true).Count());

            var stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            var differenceTreeStr = stringBuilder.ToString();

            /*
             * differenceTreeStr:
            ?
              Person
                FirstName
                    Difference: DifferenceType=ValueMismatch, MemberPath='Person.FirstName', Value1='Daniel', Value2='Dan'.
                LastName
                Birthdate
                PhoneNumber
                ListOfAddress1
                  [0]
                    Id
                    City
                      Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].City', Value1='Prague', Value2='Olomouc'.
                    Country
                      Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].Country', Value1='Czech republic', Value2='Czechia'.
                    State
                    PostalCode
                    Street
                  [1]
                    Id
                    City
                    Country
                    State
                    PostalCode
                    Street
                ListOfAddress2
             */

            using (var sr = new StringReader(differenceTreeStr))
            {
                var expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "?");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Person");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "FirstName");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.FirstName', Value1='Daniel', Value2='Dan'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "LastName");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Birthdate");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "PhoneNumber");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "ListOfAddress1");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "[0]");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Id");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "City");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].City', Value1='Prague', Value2='Olomouc'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Country");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].Country', Value1='Czech republic', Value2='Czechia'.");
            }

            rootNode.Shrink();

            stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            differenceTreeStr = stringBuilder.ToString();

            /* differenceTreeStr (shrinked):
             ?
              Person
                  FirstName
                    Difference: DifferenceType=ValueMismatch, MemberPath='Person.FirstName', Value1='Daniel', Value2='Dan'.
                ListOfAddress1
                  [0]
                    City
                      Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].City', Value1='Prague', Value2='Olomouc'.
                    Country
                      Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].Country', Value1='Czech republic', Value2='Czechia'.
             */

            using (var sr = new System.IO.StringReader(differenceTreeStr))
            {
                var expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "?");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Person");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "FirstName");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.FirstName', Value1='Daniel', Value2='Dan'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "ListOfAddress1");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "[0]");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "City");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].City', Value1='Prague', Value2='Olomouc'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Country");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.ListOfAddress1[0].Country', Value1='Czech republic', Value2='Czechia'.");
            }
        }

        [Test]
        public void CalculateUncompletedDifferenceTree()
        {
            var student1 = new Student
            {
                Person = new Person
                {
                    FirstName = "Daniel",

                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Prague", Country = "Czech republic" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var student2 = new Student
            {
                Person = new Person
                {
                    FirstName = "Dan",

                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czechia" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var comparer = new Comparer<Student>();

            var rootNode = comparer.CalculateDifferenceTree(
                student1,
                student2,
                currentContext => currentContext.RootNode.GetDifferences(recursive: true).Any() == false);

            Assert.AreEqual(1, rootNode.GetDifferences(recursive: true).Count());

            rootNode.Shrink();

            var stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            var differenceTreeStr = stringBuilder.ToString();

            /* differenceTreeStr (shrinked):
             ?
              Person
                FirstName
                        Difference: DifferenceType=ValueMismatch, MemberPath='Person.FirstName', Value1='Daniel', Value2='Dan'.
             */

            using (var sr = new System.IO.StringReader(differenceTreeStr))
            {
                var expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "?");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Person");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "FirstName");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Person.FirstName', Value1='Daniel', Value2='Dan'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine, null);
            }
        }

        [Test]
        public void CalculateDifferencesTranslateMembers()
        {
            var student1 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Prague", Country = "Czech republic" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var student2 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czechia" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var settings = new ComparisonSettings();
            settings.ConfigureDifferences(defaultMember => TranslateToCzech(defaultMember?.Name));
            var comparer = new Comparer<Student>(settings);
            var differences = comparer.CalculateDifferences(student1, student2).ToArray();

            Assert.AreEqual(2, differences.Count());

            Assert.IsTrue(differences.Any(d =>
                d.DifferenceType == DifferenceTypes.ValueMismatch &&
                d.MemberPath == "Osoba.Seznam adres 1[0].Město" &&
                d.Value1 == "Prague" && d.Value2 == "Olomouc"));

            Assert.IsTrue(differences.Any(d =>
                d.DifferenceType == DifferenceTypes.ValueMismatch &&
                d.MemberPath == "Osoba.Seznam adres 1[0].Stát" &&
                d.Value1 == "Czech republic" && d.Value2 == "Czechia"));
        }

        [Test]
        public void CalculateDifferenceTreeTranslateMembersUsingAttributes()
        {
            var student1 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Prague", Country = "Czech republic" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var student2 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czechia" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var settings = new ComparisonSettings();
            settings.ConfigureDifferences(defaultMember => TranslateToCzech(defaultMember));
            var comparer = new Comparer<Student>(settings);
            var rootNode = comparer.CalculateDifferenceTree(student1, student2);
            rootNode.Shrink();

            Assert.AreEqual(2, rootNode.GetDifferences(recursive: true).Count());

            var stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            var differenceTreeStr = stringBuilder.ToString();

            /* differenceTreeStr (shrinked):
             ?
              Člověk
                Kolekce adres
                  [0]
                    Aglomerace (město)
                      Difference: DifferenceType=ValueMismatch, MemberPath='Člověk.Kolekce adres[0].Aglomerace (město)', Value1='Prague', Value2='Olomouc'.
                    Země (stát)
                      Difference: DifferenceType=ValueMismatch, MemberPath='Člověk.Kolekce adres[0].Země (stát)', Value1='Czech republic', Value2='Czechia'.
             */

            using (var sr = new StringReader(differenceTreeStr))
            {
                var expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "?");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Člověk");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Kolekce adres");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "[0]");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Aglomerace (město)");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Člověk.Kolekce adres[0].Aglomerace (město)', Value1='Prague', Value2='Olomouc'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Země (stát)");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Člověk.Kolekce adres[0].Země (stát)', Value1='Czech republic', Value2='Czechia'.");
            }
        }

        [Test]
        public void CalculateDifferenceTreeTranslateMembers()
        {
            var student1 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Prague", Country = "Czech republic" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var student2 = new Student
            {
                Person = new Person
                {
                    ListOfAddress1 = new List<Address>
                    {
                        new Address { City = "Olomouc", Country = "Czechia" },
                        new Address { City = "Pilsen", Country = "Czech republic" }
                    }
                }
            };

            var settings = new ComparisonSettings();
            settings.ConfigureDifferences(defaultMember => TranslateToCzech(defaultMember?.Name));
            var comparer = new Comparer<Student>(settings);
            var rootNode = comparer.CalculateDifferenceTree(student1, student2);
            rootNode.Shrink();

            Assert.AreEqual(2, rootNode.GetDifferences(recursive: true).Count());

            var stringBuilder = new StringBuilder();
            WalkDifferenceTree(rootNode, 0, stringBuilder);
            var differenceTreeStr = stringBuilder.ToString();

            /* differenceTreeStr (shrinked):
             ?
              Osoba
                Seznam adres 1
                  [0]
                    Město
                      Difference: DifferenceType=ValueMismatch, MemberPath='Osoba.Seznam adres 1[0].Město', Value1='Prague', Value2='Olomouc'.
                    Stát
                      Difference: DifferenceType=ValueMismatch, MemberPath='Osoba.Seznam adres 1[0].Stát', Value1='Czech republic', Value2='Czechia'.
             */

            using (var sr = new StringReader(differenceTreeStr))
            {
                var expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "?");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Osoba");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Seznam adres 1");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "[0]");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Město");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Osoba.Seznam adres 1[0].Město', Value1='Prague', Value2='Olomouc'.");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Stát");
                expectedLine = sr.ReadLine();
                Assert.AreEqual(expectedLine.Trim(), "Difference: DifferenceType=ValueMismatch, MemberPath='Osoba.Seznam adres 1[0].Stát', Value1='Czech republic', Value2='Czechia'.");
            }
        }

        string TranslateToCzech(MemberInfo member)
        {
            if (member == null)
            {
                return null;
            }

            var descriptionAttr = member.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttr != null)
            {
                return descriptionAttr.Description;
            }

            return TranslateToCzech(member.Name);
        }

        string TranslateToCzech(string original)
        {
            var translated = original;

            switch (original)
            {
                case "Person":
                    translated = "Osoba";
                    break;
                case "FirstName":
                    translated = "Křestní jméno";
                    break;
                case "LastName":
                    translated = "Příjmení";
                    break;
                case "Birthdate":
                    translated = "Datum narození";
                    break;
                case "PhoneNumber":
                    translated = "Číslo telefonu";
                    break;
                case "ListOfAddress1":
                    translated = "Seznam adres 1";
                    break;
                case "City":
                    translated = "Město";
                    break;
                case "Country":
                    translated = "Stát";
                    break;
                default:
                    break;
            }

            return translated;
        }

        void WalkDifferenceTree(IDifferenceTreeNode node, int level, StringBuilder stringBuilder)
        {
            var blankMemberName = "?";
            string indent = String.Concat(Enumerable.Repeat(" ", 2 * level));

            if (TreeNodeIsListItem(node) == false)
            {
                var memberName = node?.Member?.Name ?? blankMemberName;
                var line = indent + memberName;
                stringBuilder.AppendLine(line);
                Debug.WriteLine(line);
            }

            foreach (var diff in node.Differences)
            {
                var line = indent + String.Concat(Enumerable.Repeat(" ", 2)) + diff.ToString();
                stringBuilder.AppendLine(line);
                Debug.WriteLine(line);
            }

            level++;

            var descendants = node.Descendants.ToArray();

            for (int i = 0; i < descendants.Length; i++)
            {
                var desc = descendants[i];

                if (TreeNodeIsListItem(desc))
                {
                    var line = indent + String.Concat(Enumerable.Repeat(" ", 2)) + $"[{GetIndex(desc)}]";
                    stringBuilder.AppendLine(line);
                    Debug.WriteLine(line);
                }

                WalkDifferenceTree(desc, level, stringBuilder);
            }
        }

        int? GetIndex(IDifferenceTreeNode node)
        {
            var itemx = node.Ancestor.Descendants
                    .Select((descendant, index) => new { Index = index, Descendant = descendant }).Where(n => n.Descendant == node)
                    .FirstOrDefault();

            return itemx?.Index;
        }

        bool TreeNodeIsListItem(IDifferenceTreeNode node)
        {
            if (node.Ancestor?.Member?.Info is PropertyInfo pi && typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
            {
                return true;
            }

            return false;
        }
    }
}

