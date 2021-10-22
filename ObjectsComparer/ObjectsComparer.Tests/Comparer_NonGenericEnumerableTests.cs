using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using ObjectsComparer.Tests.TestClasses;
using ObjectsComparer.Utils;

namespace ObjectsComparer.Tests
{
    [TestFixture]
    public class ComparerNonGenericEnumerableTests
    {
        [Test]
        public void Equality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void InequalityCount()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("2", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
        }

        [Test]
        public void InequalityProperty()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str3" } } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[1].Property1", differences.First().MemberPath);
            Assert.AreEqual("Str2", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void InequalityCount_InequalityProperty_CompareByKey()
        {
            var a1 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str2" },
                    new B { Property1 = "Str1" } ,
                    null
                }
            };

            var a2 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str1" }
                }
            };

            //ComparisonContext.BelongsTo(): Returns true if current context's Member belongs to the member or if it is the member itself.
            //var r = PropertyHelper.GetMemberInfo<string>(() => new B().Property1);

            var settings = new ComparisonSettings();

            settings.List.Configure((ctx, options) =>
            {
                options.CompareUnequalLists = true;
                options.CompareElementsByKey(keyOptions => keyOptions.UseKey(nameof(B.Property1)));
            });

            var comparer = new Comparer<A>(settings);
            var rootContext = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(a1, a2, rootContext).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("3", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[1].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Str2]", differences[1].MemberPath);
            Assert.AreEqual(false, string.IsNullOrWhiteSpace(differences[1].Value1));
            Assert.AreEqual(true, string.IsNullOrWhiteSpace(differences[1].Value2));
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[2].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[NULL]", differences[2].MemberPath);
            Assert.AreEqual(true, differences[2].Value2 == string.Empty);
        }

        [Test]
        public void InequalityCount_InequalityProperty_CompareByKey2()
        {
            var a1 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str1" }
                }
            };

            var a2 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str2" },
                    new B { Property1 = "Str1" } ,
                    null
                }
            };
                        
            //ComparisonContext.BelongsTo(): Returns true if current context's Member belongs to the member or if it is the member itself.
            //var r = PropertyHelper.GetMemberInfo<string>(() => new B().Property1);

            var settings = new ComparisonSettings();

            settings.List.Configure((ctx, options) =>
            {
                options.CompareUnequalLists = true;
                options.CompareElementsByKey(keyOptions => keyOptions.UseKey(nameof(B.Property1)));
            });

            var comparer = new Comparer<A>(settings);
            var rootContext = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(a1, a2, rootContext).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("1", differences.First().Value1);
            Assert.AreEqual("3", differences.First().Value2);
            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[1].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[Str2]", differences[1].MemberPath);
            Assert.AreEqual(true, string.IsNullOrWhiteSpace(differences[1].Value1));
            Assert.AreEqual(false, string.IsNullOrWhiteSpace(differences[1].Value2));
            Assert.AreEqual(DifferenceTypes.MissedElementInFirstObject, differences[2].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[NULL]", differences[2].MemberPath);
            Assert.AreEqual(true, differences[2].Value2 == string.Empty);
            Assert.AreEqual(true, differences[2].Value2 == string.Empty);
        }

        [Test]
        public void RightComparisonContextGraph()
        {
            var p1 = new Person
            {
                FirstName = "FirstName1",
                LastName = "LastName1",
                NonGenericLiveAddresses = new ArrayList
                {
                    new Address
                    {
                        Id = 1,
                        City = "City1"
                    },

                    new Address{}
                },
                NonGenericStayAddresses = new ArrayList
                {
                    new Address
                    {
                        Id = 2,
                        City = "City3"
                    },
                }
            };

            var p2 = new Person
            {
                FirstName = "FirstName2",
                LastName = "LastName2",
                NonGenericLiveAddresses = new ArrayList
                {
                    null,

                    new Address
                    {
                        Id = 1,
                        City = "City2"
                    },

                    null
                }
            };

            var settings = new ComparisonSettings();

            settings.List.Configure((_, options) => 
            {
                options.CompareUnequalLists = true;
                options.CompareElementsByKey(keyOptions =>
                {
                    keyOptions.KeyPrefix = "Key: ";
                    keyOptions.NullElementIdentifier = "Null-ref";
                });
            });

            var comparer = new Comparer<Person>(settings);
            var rootContext = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(p1, p2, rootContext).ToList();
            var hasDiffs = rootContext.HasDifferences(false);
            var scontextBeforeShrink = SerializeComparisonContext(rootContext);
            rootContext.Shrink();
            var scontextAfterShrink = SerializeComparisonContext(rootContext);
        }

        string SerializeComparisonContext(ComparisonContext context)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = ShouldSerializeContractResolver.Instance,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

            return JsonConvert.SerializeObject(context, Formatting.None, settings);
        }

        [Test]
        public void InequalityCount_InequalityProperty_CompareByIndex()
        {
            var a1 = new A
            {
                NonGenericEnumerable = new ArrayList
                {
                    new B { Property1 = "Str2" },
                    new B { Property1 = "Str1" } ,
                    null
                }
            };

            var a2 = new A 
            {
                NonGenericEnumerable = new ArrayList 
                {
                    new B { Property1 = "Str1" } 
                } 
            };

            var settings = new ComparisonSettings();

            settings.List.Configure((ctx, options) =>
            {
                options.CompareUnequalLists = true;
            });

            var comparer = new Comparer<A>(settings);
            var rootContext = ComparisonContext.Create();
            var differences = comparer.CalculateDifferences(a1, a2, rootContext).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable", differences.First().MemberPath);
            Assert.AreEqual(DifferenceTypes.NumberOfElementsMismatch, differences.First().DifferenceType);
            Assert.AreEqual("3", differences.First().Value1);
            Assert.AreEqual("1", differences.First().Value2);
            Assert.AreEqual(DifferenceTypes.ValueMismatch, differences[1].DifferenceType);
            Assert.AreEqual("NonGenericEnumerable[0].Property1", differences[1].MemberPath);
            Assert.AreEqual("Str2", differences[1].Value1);
            Assert.AreEqual("Str1", differences[1].Value2);
            Assert.AreEqual(DifferenceTypes.MissedElementInSecondObject, differences[2].DifferenceType);
            Assert.AreEqual(true, differences[2].Value1 != string.Empty);
            Assert.AreEqual(true, differences[2].Value2 == string.Empty);
        }

        [Test]
        public void NullElementsEquality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { null } };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void NullAndNotNullElementsInequality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { null, "Str1" } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { "Str2", null } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            Assert.AreEqual(2, differences.Count);
            Assert.AreEqual("NonGenericEnumerable[0]", differences[0].MemberPath);
            Assert.AreEqual(string.Empty, differences[0].Value1);
            Assert.AreEqual("Str2", differences[0].Value2);
            Assert.AreEqual("NonGenericEnumerable[1]", differences[1].MemberPath);
            Assert.AreEqual("Str1", differences[1].Value1);
            Assert.AreEqual(string.Empty, differences[1].Value2);
        }

        [Test]
        public void InequalityType()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } } };
            var a2 = new A { NonGenericEnumerable = new ArrayList { new B { Property1 = "Str1" }, "Str3" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[1]", differences.First().MemberPath);
            Assert.AreEqual("ObjectsComparer.Tests.TestClasses.B", differences.First().Value1);
            Assert.AreEqual("Str3", differences.First().Value2);
        }

        [Test]
        public void DerivedClassEquality()
        {
            var a1 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) };
            var a2 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) };
            var comparer = new Comparer<A>();

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }

        [Test]
        public void DerivedClassInequalityProperty()
        {
            var a1 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) { Property1 = "Str3" } };
            var a2 = new A { NonGenericEnumerableImplementation = new EnumerableImplementation(new ArrayList { new B { Property1 = "Str1" }, new B { Property1 = "Str2" } }) { Property1 = "Str4" } };
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerableImplementation.Property1", differences.First().MemberPath);
            Assert.AreEqual("Str3", differences.First().Value1);
            Assert.AreEqual("Str4", differences.First().Value2);
        }

        [Test]
        public void NullAndEmptyInequality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList() };
            var a2 = new A();
            var comparer = new Comparer<A>();

            var differences = comparer.CalculateDifferences(a1, a2).ToList();

            CollectionAssert.IsNotEmpty(differences);
            Assert.AreEqual("NonGenericEnumerable[]", differences.First().MemberPath);
            Assert.AreEqual("System.Collections.ArrayList", differences.First().Value1);
            Assert.AreEqual(string.Empty, differences.First().Value2);
        }

        [Test]
        public void NullAndEmptyEquality()
        {
            var a1 = new A { NonGenericEnumerable = new ArrayList() };
            var a2 = new A();
            var comparer = new Comparer<A>(new ComparisonSettings { EmptyAndNullEnumerablesEqual = true });

            var isEqual = comparer.Compare(a1, a2);

            Assert.IsTrue(isEqual);
        }
    }

    class ShouldSerializeContractResolver : DefaultContractResolver
    {
        /*
         * https://stackoverflow.com/questions/46977905/overriding-a-property-value-in-custom-json-net-contract-resolver
         * https://dotnetfiddle.net/PAZULK
         * https://stackoverflow.com/questions/2441290/javascriptserializer-json-serialization-of-enum-as-string
         * 
         */
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(ComparisonContext) && property.PropertyName == nameof(ComparisonContext.Member))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        //ComparisonContext ctx = (ComparisonContext)instance;
                        return false;
                    };
            }

            //if (property.DeclaringType == typeof(Difference) && property.PropertyName == nameof(Difference.DifferenceType))
            //{
            //    property.ValueProvider = new ValueProvider(differenceObj =>
            //    {
            //        var difference = (Difference)differenceObj;
            //        return difference.DifferenceType.ToString();
            //    });
            //}

            return property;
        }
    }

    class ValueProvider : IValueProvider
    {
        readonly Func<object, object> _getValueProviderImpl;

        public ValueProvider(Func<object, object> valueProviderImpl)
        {
            _getValueProviderImpl = valueProviderImpl;
        }


        //Ser
        public object GetValue(object target)
        {
            return _getValueProviderImpl(target);
        }

        //Deser
        public void SetValue(object target, object value)
        {
            throw new NotImplementedException();
        }
    }
}
