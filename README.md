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

## Overriding Comparison Rules
To override comparison rules we need to create custom value comparer or provide function how to compare objects and how to convert these objects to string(optional) and filter function(optional).
Value Comparer  should be inherited from **AbstractValueComparer<T>** or should implement **IValueComparer<T>**.
```csharp
public class MyValueComparer: AbstractValueComparer<string>
{
    public override bool Compare(string obj1, string obj2, ComparisonSettings settings)
    {
        return obj1 == obj2; //Implement comparison logic here
    }
}
```
Override comparison rule for objects of particular type.
```csharp
//Use MyComparer to compare all members of type string 
comparer.AddComparerOverride<string>(new MyValueComparer());
comparer.AddComparerOverride(typeof(string), new MyValueComparer());
//Use MyComparer to compare all members of type string except members which name starts with "Xyz"
comparer.AddComparerOverride(typeof(string), new MyValueComparer(), member => !member.Name.StartsWith("Xyz"));
comparer.AddComparerOverride<string>(new MyValueComparer(), member => !member.Name.StartsWith("Xyz"));
```
Override comparison rule for particular member (Field or Property).
```csharp
//Use MyValueComparer to compare StringProperty of ClassA
comparer.AddComparerOverride(() => new ClassA().StringProperty, new MyValueComparer());
comparer.AddComparerOverride(
    typeof(ClassA).GetTypeInfo().GetMember("StringProperty").First(),
    new MyValueComparer());
//Compare StringProperty of ClassA by length. If length equal consider that values are equal
comparer.AddComparerOverride(
    () => new ClassA().StringProperty,
    (s1, s2, parentSettings) => s1?.Length == s2?.Length,
    s => s.ToString());
comparer.AddComparerOverride(
    () => new ClassA().StringProperty,
    (s1, s2, parentSettings) => s1?.Length == s2?.Length);
```
Override comparison rule for particular member(s) (Field or Property) by name.
```csharp
//Use MyValueComparer to compare all members with name equal to "StringProperty"
comparer.AddComparerOverride("StringProperty", new MyValueComparer());
```
Overrides by type have highest priority, than overrides by member and overrides by member name have lowest priority.
If more than one value comparers of the same type (by type/by name/by member name) could be applyed to the same member, exception **AmbiguousComparerOverrideResolutionException** will be thrown during comparison.

Examples:
```csharp
var a1 = new ClassA();
var a2 = new ClassA();
comparer.AddComparerOverride<string>(valueComparer1, member => member.Name.StartsWith("String"));
comparer.AddComparerOverride<string>(valueComparer2, member => member.Name.EndsWith("Property"));

var result = comparer.Compare(a1, a2);//Exception here
```

## Comparison Settings
Comparer has an optional settings parameter to configure comparison. 

**RecursiveComparison**

True by default. If true, all members which are not primitive types, do not have custom comparison rule and do not implement ICompareble will be compared as separate objects using the same rules as current objects.

**EmptyAndNullEnumerablesEqual**

False by default. If true, empty enumerables and null values will be considered as equal values.

**UseDefaultIfMemberNotExist**

If true and member does not exists, objects comparer will consider that this member is equal to default value of opposite member type. Applicable for dynamic types comparison only. False by default.

Comparison Settings class allows to store custom values that can be used in custom comparers.
```csharp
SetCustomSetting<T>(T value, string key = null)
GetCustomSetting<T>(string key = null)
```

## Factory
Factory provides a way to encapsulate comparers creeation and configuration. Factory should implement **IComparersFactory** or should be inherited from **ComparersFactory**. 
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

**DoNotCompareValueComparer**

Allows to to skip some fields/types. Has singleton implementation (DoNotCompareValueComparer.Instance).

**DynamicValueComparer<T>**

Receives comparison rule as a function.

**NulableStringsValueComparer**

Null and empty strings considered as equal values.

**DefaultValueValueComparer**

Allows to consider provided value and default value of specified type as equal values (see Example 3 below).

**IgnoreCaseStringsValueComparer**

Allows to compare string ignoring case.

## Examples
There are some examples how Objects Comparer can be used. 

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
Configuring comparer.
```csharp
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
```

```csharp
var expectedMessage = new Message
{
    DateCreated = DateTime.Now.AddDays(-1),
    MessageType = 1,
    Status = 0
};

var actualMessage = new Message
{
    Id = "M12345",
    DateCreated = DateTime.Now,
    MessageType = 1,
    Status = 0
};

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(expectedMessage, actualMessage, out differences);
```
> Objects are equal

```csharp
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

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(expectedMessage, actualMessage, out differences);
```
> Objects are equal

```csharp
var expectedMessage = new Message
{
    MessageType = 1,
    Status = 1,
    Errors = new List<Error>
    {
        new Error { Id = 2, Messgae = "Some error #2" },
        new Error { Id = 8, Messgae = "Some error #8" }
    }
};

var actualMessage = new Message
{
    Id = "M12345",
    DateCreated = DateTime.Now,
    MessageType = 1,
    Status = 2,
    Errors = new List<Error>
    {
        new Error { Id = 2, Messgae = "Some error #2" },
        new Error { Id = 7, Messgae = "Some error #7" }
    }
};

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(expectedMessage, actualMessage, out differences);
```
> Difference: DifferenceType=ValueMismatch, MemberPath='Status', Value1='1', Value2='2'.

> Difference: DifferenceType=ValueMismatch, MemberPath='Errors[1].Id', Value1='8', Value2='7'.
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
Configuring comparer.
```csharp
_factory = new MyComparersFactory();
_comparer = _factory.GetObjectsComparer<Person>();
```

```csharp
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

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(person1, person2, out differences);
```
> Objects are equal

```csharp
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

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(person1, person2, out differences);
```
> Difference: DifferenceType=ValueMismatch, MemberPath='FirstName', Value1='Jack', Value2='John'.

> Difference: DifferenceType=ValueMismatch, MemberPath='MiddleName', Value1='F', Value2='L'.

> Difference: DifferenceType=ValueMismatch, MemberPath='PhoneNumber', Value1='111-555-8888', Value2='222-555-9999'.

### Example 3: Comparing JSON configuration files
Challenge: There are files with settings with some differences and we should find these differences. If some fields do not exist in configuration file default values will be used. URLs can have and cannot have http prefix. Some fields should be ignored.
Json.NET is used to deserialize JSON data.

Settings0
```json
{
  "ConnectionString": "USER ID=superuser;PASSWORD=superpassword;DATA SOURCE=localhost:1111",
  "Email": {
    "Port": 25,
    "Host": "MyHost.com",
    "EmailAddress": "test@MyHost.com"
  },
  "Settings": {
    "DataCompression": "On",
    "DataSourceType": "MultiDataSource",
    "SomeUrl": "http://MyHost.com/VeryImportantData",
    "SomeOtherUrl": "http://MyHost.com/NotSoImportantData/",
    "CacheMode": "Memory",
    "MaxCacheSize": "1GB",
    "SuperModes": {
      "SmartMode1": "Enabled",
      "SmartMode2": "Disabled",
      "SmartMode3": "Enabled"
    }
  },
  "Timeouts": {
    "TotalProcessTimeout": 500,
    "ProcessTaskTimeout": 100
  },
  "BackupSettings": {
    "BackupIntervalUnit": "Day",
    "BackupInterval": 100
  },
  "Notifications": [
    {
      "Phone": "111-222-3333"
    },
    {
      "Phone": "111-222-4444"
    },
    {
      "EMail": "support@MyHost.com"
    }
  ],
  "Logging": {
    "Enabled": true,
    "Pattern": "Logs\\MyApplication.%data{yyyyMMdd}.log",
    "MaximumFileSize": "20MB",
    "Level": "ALL"
  }
}
```
Settings1
```json
{
  "ConnectionString": "USER ID=admin;PASSWORD=*****;DATA SOURCE=localhost:22222",
  "Email": {
    "Port": 25,
    "Host": "MyHost.com",
    "EmailAddress": "test@MyHost.com"
  },
  "Settings": {
    "DataCompression": "On",
    "DataSourceType": "MultiDataSource",
    "SomeUrl": "MyHost.com/VeryImportantData",
    "SomeOtherUrl": "MyHost.com/NotSoImportantData/",
    "CacheMode": "Memory",
    "MaxCacheSize": "1GB",
    "SuperModes": {
      "SmartMode1": "enabled",
      "SmartMode3": "enabled"
    }
  },
  "BackupSettings": {
    "BackupIntervalUnit": "Day",
    "BackupInterval": 100
  },
  "Notifications": [
    {
      "Phone": "111-222-3333"
    },
    {
      "EMail": "support@MyHost.com"
    }
  ],
  "Logging": {
    "Enabled": true,
    "Pattern": "Logs\\MyApplication.%data{yyyyMMdd}.log",
    "MaximumFileSize": "20MB",
    "Level": "ALL"
  }
}
```
Settings2
```json
{
  "ConnectionString": "USER ID=superuser;PASSWORD=superpassword;DATA SOURCE=localhost:1111",
  "Email": {
    "Port": 25,
    "Host": "MyHost.com",
    "EmailAddress": "test@MyHost.com"
  },
  "Settings": {
    "DataSourceType": "MultiDataSource",
    "SomeUrl": "http://MyHost.com/VeryImportantData",
    "SomeOtherUrl": "http://MyHost.com/NotSoImportantData/",
    "CacheMode": "Memory",
    "MaxCacheSize": "1GB",
    "SuperModes": {
      "SmartMode3": "Enabled"
    }
  },
  "Timeouts": {
    "TotalProcessTimeout": 500,
    "ProcessTaskTimeout": 200
  },
  "BackupSettings": {
    "BackupIntervalUnit": "Week",
    "BackupInterval": 2
  },
  "Notifications": [
    {
      "EMail": "support@MyHost.com"
    }
  ],
  "Logging": {
    "Enabled": false,
    "Pattern": "Logs\\MyApplication.%data{yyyyMMdd}.log",
    "MaximumFileSize": "40MB",
    "Level": "ERROR"
  }
}
```
Configuring comparer.
```csharp
_comparer = new Comparer(new ComparisonSettings { UseDefaultIfMemberNotExist = true });
//Some fields should be ignored
_comparer.AddComparerOverride("ConnectionString", DoNotCompareValueComparer.Instance);
_comparer.AddComparerOverride("Email", DoNotCompareValueComparer.Instance);
_comparer.AddComparerOverride("Notifications", DoNotCompareValueComparer.Instance);
//Smart Modes are disabled by default. These fields are not case sensitive
var disabledByDefaultComparer = new DefaultValueValueComparer<string>("Disabled", IgnoreCaseStringsValueComparer.Instance);
_comparer.AddComparerOverride("SmartMode1", disabledByDefaultComparer);
_comparer.AddComparerOverride("SmartMode2", disabledByDefaultComparer);
_comparer.AddComparerOverride("SmartMode3", disabledByDefaultComparer);
//http prefix in URLs should be ignored
var urlComparer = new DynamicValueComparer<string>(
    (url1, url2, settings) => url1.Trim('/').Replace(@"http://", string.Empty) == url2.Trim('/').Replace(@"http://", string.Empty));
_comparer.AddComparerOverride("SomeUrl", urlComparer);
_comparer.AddComparerOverride("SomeOtherUrl", urlComparer);
//DataCompression is Off by default.
_comparer.AddComparerOverride("DataCompression", new DefaultValueValueComparer<string>("Off", NulableStringsValueComparer.Instance));
//ProcessTaskTimeout and TotalProcessTimeout fields have default values.
_comparer.AddComparerOverride("ProcessTaskTimeout", new DefaultValueValueComparer<long>(100, DefaultValueComparer.Instance));
_comparer.AddComparerOverride("TotalProcessTimeout", new DefaultValueValueComparer<long>(500, DefaultValueComparer.Instance));
```

```csharp
var settings0Json = LoadJson("Settings0.json");
var settings0 = JsonConvert.DeserializeObject<ExpandoObject>(settings0Json);
var settings1Json = LoadJson("Settings1.json");
var settings1 = JsonConvert.DeserializeObject<ExpandoObject>(settings1Json);

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(settings0, settings1, out differences);
```
> Objects are equal

```csharp
var settings0Json = LoadJson("Settings0.json");
var settings0 = JsonConvert.DeserializeObject<ExpandoObject>(settings0Json);
var settings2Json = LoadJson("Settings2.json");
var settings2 = JsonConvert.DeserializeObject<ExpandoObject>(settings2Json);

IEnumerable<Difference> differences;
var isEqual = _comparer.Compare(settings0, settings2, out differences);
```
> Difference: DifferenceType=ValueMismatch, MemberPath='Settings.DataCompression', Value1='On', Value2='Off'.

> Difference: DifferenceType=ValueMismatch, MemberPath='Settings.SuperModes.SmartMode1', Value1='Enabled', Value2='Disabled'.

> Difference: DifferenceType=ValueMismatch, MemberPath='Timeouts.ProcessTaskTimeout', Value1='100', Value2='200'.

> Difference: DifferenceType=ValueMismatch, MemberPath='BackupSettings.BackupIntervalUnit', Value1='Day', Value2='Week'.

> Difference: DifferenceType=ValueMismatch, MemberPath='BackupSettings.BackupInterval', Value1='100', Value2='2'.

> Difference: DifferenceType=ValueMismatch, MemberPath='Logging.Enabled', Value1='True', Value2='False'.

> Difference: DifferenceType=ValueMismatch, MemberPath='Logging.MaximumFileSize', Value1='20MB', Value2='40MB'.

> Difference: DifferenceType=ValueMismatch, MemberPath='Logging.Level', Value1='ALL', Value2='ERROR'.


## Contributing
Any useful changes are welcomed.

Feel free to report any defects or ideas how this framework can be improved. 

Create an issue, contact me directly or fork the code and submit a pull request!


