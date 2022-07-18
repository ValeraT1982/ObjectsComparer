using System;
using System.Linq.Expressions;
using System.Reflection;
using ObjectsComparer.Utils;
using ObjectsComparer.DifferenceTreeExtensions;

namespace ObjectsComparer
{
    /// <summary>
    /// Provides base implementation to configure comparer.
    /// </summary>
    public abstract class BaseComparer: IBaseComparer
    {
        /// <summary>
        /// Comparison Settings.
        /// </summary>
        public ComparisonSettings Settings { get; }

        /// <summary>
        /// Default <see cref="IValueComparer"/>
        /// </summary>
        public IValueComparer DefaultValueComparer { get; private set; }

        protected IComparersFactory Factory { get; }

        internal ComparerOverridesCollection OverridesCollection { get;  } = new ComparerOverridesCollection();
        
        protected BaseComparer(ComparisonSettings settings, BaseComparer parentComparer, IComparersFactory factory)
        {
            Factory = factory ?? new ComparersFactory();
            Settings = settings ?? new ComparisonSettings();
            DefaultValueComparer = new DefaultValueComparer();
            // ReSharper disable once InvertIf
            if (parentComparer != null)
            {
                DefaultValueComparer = parentComparer.DefaultValueComparer;
                OverridesCollection.Merge(parentComparer.OverridesCollection);
            }
        }

        /// <summary>
        /// Adds Comparer Override by Type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true. Null by default.</param>
        public void AddComparerOverride(Type type, IValueComparer valueComparer, Func<MemberInfo, bool> filter = null)
        {
            OverridesCollection.AddComparer(type, valueComparer, filter);
        }

        /// <summary>
        /// Adds Comparer Override by member filter.
        /// </summary>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true.</param>
        public void AddComparerOverride(IValueComparer valueComparer, Func<MemberInfo, bool> filter)
        {
            OverridesCollection.AddComparer(valueComparer, filter);
        }

        /// <summary>
        /// Adds Comparer Override by Type.
        /// </summary>
        /// <typeparam name="TType">Type.</typeparam>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true. Null by default.</param>
        public void AddComparerOverride<TType>(IValueComparer valueComparer, Func<MemberInfo, bool> filter = null)
        {
            AddComparerOverride(typeof(TType), valueComparer, filter);
        }

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        /// <param name="memberLambda">Lambda to get member.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        public void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer valueComparer)
        {
            OverridesCollection.AddComparer(PropertyHelper.GetMemberInfo(memberLambda), valueComparer);
        }

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        /// <param name="memberLambda">Lambda to get member.</param>
        /// <param name="compareFunction">Function to compare objects.</param>
        /// <param name="toStringFunction">Function to convert objects to string.</param>
        public void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda, 
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction, 
            Func<TProp, string> toStringFunction)
        {
            OverridesCollection.AddComparer(
                PropertyHelper.GetMemberInfo(memberLambda),
                new DynamicValueComparer<TProp>(
                    compareFunction,
                    toStringFunction));
        }

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        /// <param name="memberLambda">Lambda to get member.</param>
        /// <param name="compareFunction">Function to compare objects.</param>
        public void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda,
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction)
        {
            OverridesCollection.AddComparer(
                PropertyHelper.GetMemberInfo(memberLambda),
                new DynamicValueComparer<TProp>(
                    compareFunction,
                    obj => obj?.ToString()));
        }

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <param name="memberInfo">Member Info.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        public void AddComparerOverride(MemberInfo memberInfo, IValueComparer valueComparer)
        {
            OverridesCollection.AddComparer(memberInfo, valueComparer);
        }

        /// <summary>
        /// Adds Comparer Override by Member name.
        /// </summary>
        /// <param name="memberName">Member Name.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true. Null by default.</param>
        public void AddComparerOverride(string memberName, IValueComparer valueComparer, Func<MemberInfo, bool> filter = null)
        {
            OverridesCollection.AddComparer(memberName, valueComparer, filter);
        }

        /// <summary>
        /// Sets <see cref="IBaseComparer.DefaultValueComparer"/>.
        public void SetDefaultComparer(IValueComparer valueComparer)
        /// </summary>
        /// <param name="valueComparer">Value Comparer.</param>
        {
            DefaultValueComparer = valueComparer ?? throw new ArgumentNullException(nameof(valueComparer));
        }

        /// <summary>
        /// Ignors comparison for Type.
        /// </summary>
        /// <typeparam name="TType">Type.</typeparam>
        public void IgnoreMember<TType>()
        {
            OverridesCollection.AddComparer(typeof(TType), DoNotCompareValueComparer.Instance);
        }

        /// <summary>
        /// Ignors comparison for Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        public void IgnoreMember<TProp>(Expression<Func<TProp>> memberLambda)
        {
            OverridesCollection.AddComparer(
                PropertyHelper.GetMemberInfo(memberLambda),
                DoNotCompareValueComparer.Instance);
        }

        /// <summary>
        /// Ignors comparison for Member by Member name.
        /// </summary>
        /// <param name="memberName">Member Name.</param>
        public void IgnoreMember(string memberName)
        {
            OverridesCollection.AddComparer(memberName, DoNotCompareValueComparer.Instance);
        }

        /// <summary>
        /// Ignors comparison by Member filter.
        /// </summary>
        /// <param name="filter">Member will be ignored if filter(memberInfo) == true.</param>
        public void IgnoreMember(Func<MemberInfo, bool> filter)
        {
            OverridesCollection.AddComparer(DoNotCompareValueComparer.Instance, filter);
        }


        /// <summary>
        /// Adds an <paramref name="difference"/> to the end of the <paramref name="differenceTreeNode"/>'s <see cref="IDifferenceTreeNode.Differences"/>.
        /// </summary>
        /// <returns>The <see cref="DifferenceLocation"/> instance.</returns>
        [Obsolete("Use 2. method", true)]
        protected virtual DifferenceLocation AddDifferenceToTree(Difference difference, IDifferenceTreeNode differenceTreeNode)
        {
            if (difference is null)
            {
                throw new ArgumentNullException(nameof(difference));
            }

            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            differenceTreeNode.AddDifference(difference);

            return new DifferenceLocation(difference, differenceTreeNode);
        }

        protected virtual DifferenceLocation AddDifferenceToTree(IDifferenceTreeNode differenceTreeNode, string memberPath, string value1, string value2,
            DifferenceTypes differenceType = DifferenceTypes.ValueMismatch, object rawValue1 = null, object rawValue2 = null)
        {
            var difference = CreateDifference(differenceTreeNode, memberPath, value1, value2, differenceType, rawValue1, rawValue2);
            differenceTreeNode.AddDifference(difference);

            return new DifferenceLocation(difference, differenceTreeNode);
        }

        protected virtual Difference CreateDifference(IDifferenceTreeNode differenceTreeNode, string memberPath, string value1, string value2,
            DifferenceTypes differenceType = DifferenceTypes.ValueMismatch, object rawValue1 = null, object rawValue2 = null)
        {
            if (differenceTreeNode is null)
            {
                throw new ArgumentNullException(nameof(differenceTreeNode));
            }

            var differenceOptions = DifferenceOptions.Default();
            var defaultDifference = new Difference(memberPath, value1, value2, differenceType);

            Settings.DifferenceOptionsAction?.Invoke(differenceTreeNode, differenceOptions);

            if (differenceOptions.DifferenceFactory == null)
            {
                return defaultDifference;
            }

            var customDifference = differenceOptions.DifferenceFactory(new CreateDifferenceArgs(defaultDifference, rawValue1, rawValue2));

            if (customDifference == null)
            {
                throw new NullReferenceException("DifferenceFactory returned null.");
            }

            return customDifference;
        }

        protected virtual void InsertPathToDifference(Difference difference, string defaultRootElementPath, IDifferenceTreeNode rootNode, IDifferenceTreeNode differenceNode)
        {
            var differencePathOptions = DifferencePathOptions.Default();
            
            Settings.DifferencePathOptionsAction?.Invoke(rootNode, differencePathOptions);

            if (differencePathOptions.InsertPathFactory != null)
            {
                defaultRootElementPath = differencePathOptions.InsertPathFactory(new InsertPathFactoryArgs(defaultRootElementPath, differenceNode));
            }

            difference.InsertPath(defaultRootElementPath);
        }
    }
}