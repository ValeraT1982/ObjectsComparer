namespace ObjectsComparer.Tests.TestClasses
{
  using System.Collections.ObjectModel;

  public class Parent
    {
        public string Property1 { get; set; }

        public ObservableCollection <ParentChild> Child1 { get; set; } = new ObservableCollection <ParentChild>();
    }
}
