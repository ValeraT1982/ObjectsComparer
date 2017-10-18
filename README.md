![Logo](https://github.com/ValeraT1982/ObjectsComparer/blob/master/ObjectsDataComparer.png)

# Objects Comparer
## Introduction
It is quite common situation when complex objects should be compared. Sometimes objects can contain nested elements, or some members should be excluded from comparison (auto generated identifiers, create/update date etc.), or some members can have custom comparison rules (same data in different formats, like phone numbers). To solve such kind of problems I have developed this small framework to compare objects.

Briefly, Objects Comparer is an object-to-object comparer, which allows to compare objects recursively member by member and define custom comparison rules for certain properties, fields or types.

Objects comparer can be considered as ready to use framework or as an idea for similar solutions.
## Installation
>Install-Package ObjectsComparer
## Basic Examples
Let's suppose that we have 2 classes
```csharp
public class ClassA
{
    public string StringProperty { get; set; }

    public int IntProperty { get; set; }

    public SubClassA SubClass { get; set; }
}

public class SubClassA
{
    public bool BoolProperty { get; set; }
}
```

There are some examples below how Objects Comparer can be used to compare instances of these classes.

```csharp
//Initialize objects and comparer
var a1 = new ClassA { StringProperty = "String", IntProperty = 1 };
var a2 = new ClassA { StringProperty = "String", IntProperty = 1 };
var comparer = new Comparer<ClassA>();

//Compare objects
IEnumerable<Difference> differences;
var isEqual = comparer.Compare(a1, a2, out differences);

//Print results
Debug.WriteLine(isEqual ? "Objects are equal" : string.Join(Environment.NewLine, differenses));
```
>Objects are equal

In examples below **Compare objects** and **Print results** blocks will be skipped for brevity except some cases.

```csharp
var a1 = new ClassA { StringProperty = "String", IntProperty = 1 };
var a2 = new ClassA { StringProperty = "String", IntProperty = 2 };
var comparer = new Comparer<ClassA>();
```
>Difference: DifferenceType=ValueMismatch, MemberPath='IntProperty', Value1='1', Value2='2'.

```csharp
var a1 = new ClassA { SubClass = new SubClassA { BoolProperty = true } };
var a2 = new ClassA { SubClass = new SubClassA { BoolProperty = false } };
var comparer = new Comparer<ClassA>();
```
>Difference: DifferenceType=ValueMismatch, MemberPath='SubClass.BoolProperty', Value1='True', Value2='False'.

## Enumerables (arrays)
```csharp
var a1 = new[] { 1, 2, 3 };
var a2 = new[] { 1, 2, 3 };
var comparer = new Comparer<int[]>();
```
>Objects are equal

```csharp
var a1 = new[] { 1, 2 };
var a2 = new[] { 1, 2, 3 };
var comparer = new Comparer<int[]>();
```
>Difference: DifferenceType=ValueMismatch, MemberPath='Length', Value1='2', Value2='3'.

```csharp
var a1 = new[] { 1, 2, 3 };
var a2 = new[] { 1, 4, 3 };
var comparer = new Comparer<int[]>();
```
>Difference: DifferenceType=ValueMismatch, MemberPath='[1]', Value1='2', Value2='4'.

```csharp
var a1 = new ArrayList { "Str1", "Str2" };
var a2 = new ArrayList { "Str1", 5 };
var comparer = new Comparer<ArrayList>();
```
>Difference: DifferenceType=TypeMismatch, MemberPath='[1]', Value1='Str2', Value2='5'.

## Multidimensional arrays

```csharp
var a1 = new[] { new[] { 1, 2 } };
var a2 = new[] { new[] { 1, 3 } };
var comparer = new Comparer<int[][]>();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='[0][1]', Value1='2', Value2='3'.

```csharp
var a1 = new[] { new[] { 1, 2 } };
var a2 = new[] { new[] { 2, 2 }, new[] { 3, 5 } };
var comparer = new Comparer<int[][]>();
```
>Difference: DifferenceType=ValueMismatch, MemberPath='Length', Value1='1', Value2='2'.

```csharp
var a1 = new[] { new[] { 1, 2 }, new[] { 3, 5 } };
var a2 = new[] { new[] { 1, 2 }, new[] { 3, 5, 6 } };
var comparer = new Comparer<int[][]>();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='[1].Length', Value1='2', Value2='3'.

```csharp
var a1 = new[,] { { 1, 2 }, { 1, 3 } };
var a2 = new[,] { { 1, 3, 4 }, { 1, 3, 8 } };
var comparer = new Comparer<int[,]>();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='Dimension1', Value1='2', Value2='3'.

```csharp
var a1 = new[,] { { 1, 2 } };
var a2 = new[,] { { 1, 3 } };
var comparer = new Comparer<int[,]>();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='[0,1]', Value1='2', Value2='3'.

## Dynamic objects

C# supports several types of dynamic objects. 

### ExpandoObject

```csharp
dynamic a1 = new ExpandoObject();
a1.Field1 = "A";
a1.Field2 = 5;
a1.Field4 = 4;
dynamic a2 = new ExpandoObject();
a2.Field1 = "B";
a2.Field3 = false;
a2.Field4 = "C";
var comparer = new Comparer();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='Field1', Value1='A', Value2='B'.

>Difference: DifferenceType=MissedMemberInSecondObject, MemberPath='Field2', Value1='5', Value2=''.

>Difference: DifferenceType=TypeMismatch, MemberPath='Field4', Value1='4', Value2='C'.

>Difference: DifferenceType=MissedMemberInFirstObject, MemberPath='Field3', Value1='', Value2='False'.


```csharp
dynamic a1 = new ExpandoObject();
a1.Field1 = "A";
a1.Field2 = 5;
dynamic a2 = new ExpandoObject();
a2.Field1 = "B";
a2.Field3 = false;
var comparer = new Comparer();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='Field1', Value1='A', Value2='B'.

>Difference: DifferenceType=MissedMemberInSecondObject, MemberPath='Field2', Value1='5', Value2=''.

>Difference: DifferenceType=MissedMemberInFirstObject, MemberPath='Field3', Value1='', Value2='False'.

Behavior if member not exists could be changed by providing custom ComparisonSettings (see Comparison Settings below).

```csharp
dynamic a1 = new ExpandoObject();
a1.Field1 = "A";
a1.Field2 = 0;
dynamic a2 = new ExpandoObject();
a2.Field1 = "B";
a2.Field3 = false;
a2.Field4 = "S";
var comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });
```

>Difference: DifferenceType=ValueMismatch, MemberPath='Field1', Value1='A', Value2='B'.

>Difference: DifferenceType=ValueMismatch, MemberPath='Field4', Value1='', Value2='S'.

### DynamicObject
Let’s assume that we have such implementation of the **DynamicObject** class. It is necessary to have a correct implementation of the method **GetDynamicMemberNames**, otherwise Objects Comparer wouldn't work in a right way.

```csharp
private class DynamicDictionary : DynamicObject
{
    // ReSharper disable once UnusedMember.Local
    public int IntProperty { get; set; }

    private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        var name = binder.Name;

        return _dictionary.TryGetValue(name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        _dictionary[binder.Name] = value;

        return true;
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        return _dictionary.Keys;
    }
}
```

```csharp
dynamic a1 = new DynamicDictionary();
a1.Field1 = "A";
a1.Field3 = true;
dynamic a2 = new DynamicDictionary();
a2.Field1 = "B";
a2.Field2 = 8;
a2.Field3 = 1;
var comparer = new Comparer();
```

>Difference: DifferenceType=ValueMismatch, MemberPath='Field1', Value1='A', Value2='B'.

>Difference: DifferenceType=TypeMismatch, MemberPath='Field3', Value1='True', Value2='1'.

>Difference: DifferenceType=MissedMemberInFirstObject, MemberPath='Field2', Value1='', Value2='8'.

### Compiler generated objects

```csharp
dynamic a1 = new
{
    Field1 = "A",
    Field2 = 5,
    Field3 = true
};
dynamic a2 = new
{
    Field1 = "B",
    Field2 = 8
};
var comparer = new Comparer();

IEnumerable<Difference> differences;
var isEqual = comparer.Compare((object)a1, (object)a2, out differences);
```

>Difference: DifferenceType=ValueMismatch, MemberPath='Field1', Value1='A', Value2='B'.

>Difference: DifferenceType=TypeMismatch, MemberPath='Field2', Value1='5', Value2='8'.

>Difference: DifferenceType=MissedMemberInSecondObject, MemberPath='Field3', Value1='True', Value2=''.

This example requires some additional explanations. Types of the objects a1 and a2 were generated by compiler and are considered as the same type if and only if objects a1 and a2 have same set of fields and these fields have the same types. If casting to **(object)** is skipped in case of different set of fields/types **RuntimeBinderException** will be thrown.

## Overriding comparison rules
To override comparison rules we need to create custom value comparer. This class should be inherited from AbstractValueComparer<T> or should implement IValueComparer<T>.
```csharp
public class MyComparer: AbstractValueComparer<string>
{
    public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
    {
        return obj1 == obj2; //Implement comparison logic here
    }
}
```
Type comparison rule override.
```csharp
comparer.AddComparerOverride<string>(new MyComparer());
```
Field comparison rule override.
```csharp
comparer.AddComparerOverride(() => new ClassA().StringProperty, new MyComparer());
```
```csharp
comparer.AddComparerOverride(
    () => new ClassA().StringProperty, 
    (s1, s2, parentSettings) => s1 == s2,
    s == s.ToString());
```

## Comparison Settings
Comparer has an optional settings parameter to configure comparison. 

**RecursiveComparison.**

True by default. If true, all members which are not primitive types, do not have custom comparison rule and do not implement ICompareble will be compared as separate objects using the same rules as current objects.

**EmptyAndNullEnumerablesEqual.**

False by default. If true, empty enumerables and null values will be considered as equal values.

Comparison Settings class allows to store custom values that can be used in custom comparers.
```csharp
SetCustomSetting<T>(T value, string key = null)
GetCustomSetting<T>(string key = null)
```
## Factory
Factory provides a way to encapsulate comparers creeation and configuration. Factory should implement IComparersFactory or should be inherited from ComparersFactory.
```csharp
public class MyComparersFactory: ComparersFactory
{
    public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, IBaseComparer parentComparer = null)
    {
        if (typeof(T) == typeof(ClassA))
        {
            var comparer = new Comparer<ClassA>(settings, parentComparer, this);
            comparer.AddComparerOverride<Guid>(new MyCustomGuidComparer());

            return (IComparer<T>)comparer;
        }

        return base.GetObjectsComparer<T>(settings, parentComparer);
    }
}
```

## Non-generic comparer
```csharp
var comparer = new Comparer();
var isEqual = comparer.Compare(a1, a2);
```
This comparer creates generic implementation of comparer for each comparison.

## Useful Value Comparers
Framework contains several custom comparers that can be useful.

**DoNotCompareValueComparer.**

Allows to to skip some fields/types. Has singleton implementation (DoNotCompareValueComparer.Instance).

**DynamicValueComparer<T>.**

Receives comparison rule as a function.

**NulableStringsValueComparer.**

Null and empty strings considered as equal values.
## Examples
There are some examples how Objects Comparer can be used. 

NUnit is used for developing unit tests to show how examples work.

### Example 1: Expected Message
Challenge: Check if received message equal to the expected message.
```csharp
public class Error
{
    public int Id { get; set; }

    public string Messgae { get; set; }
}
```
```csharp
public class Message
{
    public string Id { get; set; }

    public DateTime DateCreated { get; set; }

    public int MessageType { get; set; }

    public int Status { get; set; }

    public List<Error> Errors { get; set; }

    public override string ToString()
    {
        return $"Id:{Id}, Date:{DateCreated}, Type:{MessageType}, Status:{Status}";
    }
}
```
```csharp
[TestFixture]
public class Example1Tests
{
    private IComparer<Message> _comparer;
        
    [SetUp]
    public void SetUp()
    {
        _comparer = new Comparer<Message>(
            new ComparisonSettings
            {
                //Null and empty error lists are equal
                EmptyAndNullEnumerablesEqual = true
            });
        //Do not compare DateCreated 
        _comparer.AddComparerOverride<DateTime>(DoNotCompareValueComparer.Instance);
        //Do not compare Id
        _comparer.AddComparerOverride(() => new Message().Id, DoNotCompareValueComparer.Instance);
        //Do not compare Message Text
        _comparer.AddComparerOverride(() => new Error().Messgae, DoNotCompareValueComparer.Instance);
    }

    [Test]
    public void EqualMessagesWithoutErrorsTest()
    {
        var expectedMessage = new Message
        {
            MessageType = 1,
            Status = 0,
        };

        var actualMessage = new Message
        {
            Id = "M12345",
            DateCreated = DateTime.Now,
            MessageType = 1,
            Status = 0,
        };

        var isEqual = _comparer.Compare(expectedMessage, actualMessage);

        Assert.IsTrue(isEqual);
    }

    [Test]
    public void EqualMessagesWithErrorsTest()
    {
        var expectedMessage = new Message
        {
            MessageType = 1,
            Status = 1,
            Errors = new List<Error>
            {
                new Error { Id = 2 },
                new Error { Id = 7 }
            }
        };

        var actualMessage = new Message
        {
            Id = "M12345",
            DateCreated = DateTime.Now,
            MessageType = 1,
            Status = 1,
            Errors = new List<Error>
            {
                new Error { Id = 2, Messgae = "Some error #2" },
                new Error { Id = 7, Messgae = "Some error #7" },
            }
        };

        var isEqual = _comparer.Compare(expectedMessage, actualMessage);

        Assert.IsTrue(isEqual);
    }
}
```
### Example 2: Persons comparison
Challenge: Compare persons from different sources.
```csharp
public class Person
{
    public Guid PersonId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string MiddleName { get; set; }

    public string PhoneNumber { get; set; }

    public override string ToString()
    {
        return $"{FirstName} {MiddleName} {LastName} ({PhoneNumber})";
    }
}
```
Phone number can have different formats. Let’s compare only digits.
```csharp
public class PhoneNumberComparer: AbstractValueComparer<string>
{
    public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
    {
        return ExtractDigits(obj1) == ExtractDigits(obj2);
    }

    private string ExtractDigits(string str)
    {
        return string.Join(
            string.Empty, 
            (str ?? string.Empty)
                .ToCharArray()
                .Where(char.IsDigit));
    }
}
```
Factory allows not to configure comparer every time we need to create it.
```csharp
public class MyComparersFactory: ComparersFactory
{
    public override IComparer<T> GetObjectsComparer<T>(ComparisonSettings settings = null, IBaseComparer parentComparer = null)
    {
        if (typeof(T) == typeof(Person))
        {
            var comparer = new Comparer<Person>(settings, parentComparer, this);
            //Do not compare PersonId
            comparer.AddComparerOverride<Guid>(DoNotCompareValueComparer.Instance);
            //Sometimes MiddleName can be skipped. Compare only if property has value.
            comparer.AddComparerOverride(
                () => new Person().MiddleName,
                (s1, s2, parentSettings) => string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2) || s1 == s2);
            comparer.AddComparerOverride(
                () => new Person().PhoneNumber,
                new PhoneNumberComparer());

            return (IComparer<T>)comparer;
        }

        return base.GetObjectsComparer<T>(settings, parentComparer);
    }
}
```
```csharp
[TestFixture]
public class Example2Tests
{
    private MyComparersFactory _factory;
    private IComparer<Person> _comparer;

    [SetUp]
    public void SetUp()
    {
        _factory = new MyComparersFactory();
        _comparer = _factory.GetObjectsComparer<Person>();
    }

    [Test]
    public void EqualPersonsTest()
    {
        var person1 = new Person
        {
            PersonId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "F",
            PhoneNumber = "111-555-8888"
        };
        var person2 = new Person
        {
            PersonId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "(111) 555 8888"
        };
            
        IEnumerable<Difference> differenses;
        var isEqual = _comparer.Compare(person1, person2, out differenses);

        Assert.IsTrue(isEqual);

        Debug.WriteLine($"Persons {person1} and {person2} are equal");
    }

    [Test]
    public void DifferentPersonsTest()
    {
        var person1 = new Person
        {
            PersonId = Guid.NewGuid(),
            FirstName = "Jack",
            LastName = "Doe",
            MiddleName = "F",
            PhoneNumber = "111-555-8888"
        };
        var person2 = new Person
        {
            PersonId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "L",
            PhoneNumber = "222-555-9999"
        };

        IEnumerable<Difference> differenses;
        var isEqual = _comparer.Compare(person1, person2, out differenses);

        var differensesList = differenses.ToList();
        Assert.IsFalse(isEqual);
        Assert.AreEqual(3, differensesList.Count);
        Assert.IsTrue(differensesList.Any(d => d.MemberPath == "FirstName" && d.Value1 == "Jack" && d.Value2 == "John"));
        Assert.IsTrue(differensesList.Any(d => d.MemberPath == "MiddleName" && d.Value1 == "F" && d.Value2 == "L"));
        Assert.IsTrue(differensesList.Any(d => d.MemberPath == "PhoneNumber" && d.Value1 == "111-555-8888" && d.Value2 == "222-555-9999"));

        Debug.WriteLine($"Persons {person1} and {person2}");
        Debug.WriteLine("Differences:");
        Debug.WriteLine(string.Join(Environment.NewLine, differensesList));
    }
}
```

>Persons John F Doe (111-555-8888) and John  Doe ((111) 555 8888) are equal


>Persons Jack F Doe (111-555-8888) and John L Doe (222-555-9999)

>Differences:

>Difference: MemberPath='FirstName', Value1='Jack', Value2='John'.

>Difference: MemberPath='MiddleName', Value1='F', Value2='L'.

>Difference: MemberPath='PhoneNumber', Value1='111-555-8888', Value2='222-555-9999'.


## Contributing
Any useful changes are welcomed. 

Feel free to report any defects or ideas how this framework can be improved. 

Create an issue, contact me directly or fork the code and submit a pull request!


