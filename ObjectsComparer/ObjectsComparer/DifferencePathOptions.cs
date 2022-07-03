using System;

namespace ObjectsComparer
{
    public class DifferencePathOptions
    {
        DifferencePathOptions()
        {
        }

        /// <summary>
        /// Default options.
        /// </summary>
        internal static DifferencePathOptions Default() => new DifferencePathOptions();

        public Func<InsertPathFactoryArgs, string> InsertPathFactory { get; private set; } = null;

        /// <summary>
        /// Factory for insertion into the <see cref="Difference.MemberPath"/> property.
        /// </summary>
        /// <param name="factory">
        /// Null value is allowed here and means that a default behavior of the insertion into the difference path is required.<br/>
        /// Func first parameter: The args for the path insertion.<br/>
        /// Returns: Transformed root path element or the not transformed root path element itself.
        /// </param>
        public void UseInsertPathFactory(Func<InsertPathFactoryArgs, string> factory)
        {
            InsertPathFactory = factory;
        }

    }
}
