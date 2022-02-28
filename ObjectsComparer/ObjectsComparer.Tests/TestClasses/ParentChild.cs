namespace ObjectsComparer.Tests.TestClasses
{
  using ObjectsComparer.Attributes;
  using ObjectsComparer.Enums;

  public class ParentChild
  {
    #region Constructors

    public ParentChild(Parent myParent,
                       string property1)
    {
      this.MyParent = myParent;
      this.Property1 = property1;
    }

    #endregion

    #region Properties

    public string Property1 { get; set; }

    [Comparison(ComparisonStatus.IgnoreInComparison)]
    public Parent MyParent { get; set; }

    #endregion
  }
}