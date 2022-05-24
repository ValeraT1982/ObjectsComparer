namespace ObjectsComparer.Tests.TestClasses
{
  using ObjectsComparer.Attributes;

  public class Child
  {
    #region Constructors

    public Child(string property1,
                 string property2)
    {
      this.Property1 = property1;
      this.Property2 = property2;
    }

    #endregion

    #region Properties

    public string Property1 { get; set; }

    [IgnoreInComparison]
    public string Property2 { get; set; }

    #endregion
  }
}