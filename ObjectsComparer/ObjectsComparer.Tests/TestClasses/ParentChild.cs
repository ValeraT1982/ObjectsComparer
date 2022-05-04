namespace ObjectsComparer.Tests.TestClasses
{
  using System.Collections.ObjectModel;

  using NUnit.Framework;

  using ObjectsComparer.Attributes;

  public class ParentChild
  {
    #region Constructors

    public ParentChild(Parent myParent,
                       string property1)
    {
      this.MyParent = myParent;
      this.Property1 = property1;
    }



    public ParentChild(Parent myParent,
                       string property1,
                       ObservableCollection <Child> children)
    {
      this.MyParent = myParent;
      this.Property1 = property1;
      this.Children = children;
    }

    #endregion

    #region Properties

    public string Property1 { get; set; }

    [IgnoreInComparison]
    public Parent MyParent { get; set; }

    public ObservableCollection <Child> Children { get; set; } = new ObservableCollection <Child>();

    #endregion
  }
}