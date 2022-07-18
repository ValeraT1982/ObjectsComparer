namespace ObjectsComparer
{
    /// <summary>
    /// Arguments for <see cref="DifferencePathOptions.UseInsertPathFactory(System.Func{InsertPathFactoryArgs, string})"/>.
    /// </summary>
    public class InsertPathFactoryArgs
    {
        public InsertPathFactoryArgs(string defaultRootElementPath, IDifferenceTreeNode childNode)
        {
            DefaultRootElementPath = defaultRootElementPath;
            ChildNode = childNode;
        }

        /// <summary>
        /// The path that will be inserted to the property by default.
        /// </summary>
        public string DefaultRootElementPath { get; }

        /// <summary>
        /// The property to which the path is inserted.
        /// </summary>
        public IDifferenceTreeNode ChildNode { get; }
    }
}
