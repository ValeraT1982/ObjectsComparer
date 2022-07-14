using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace ObjectsComparer
{
    /// <summary>
    /// Configuration for Objects Comparer.
    /// </summary>
    public partial class ComparisonSettings
    {
        /// <summary>
        /// If true, all members which are not primitive types, do not have custom comparison rule and 
        /// do not implement <see cref="IComparable"/> will be compared as separate objects using the same rules as current objects. True by default.
        /// </summary>
        public bool RecursiveComparison { get; set; }

        /// <summary>
        /// If true, empty <see cref="IEnumerable"/>  and null values will be considered as equal values. False by default.
        /// </summary>
        public bool EmptyAndNullEnumerablesEqual { get; set; }

        /// <summary>
        /// If true and member does not exists, objects comparer will consider that this member is equal to default value of opposite member type. 
        /// Applicable for dynamic types comparison only. False by default.
        /// </summary>
        public bool UseDefaultIfMemberNotExist { get; set; }

        private readonly Dictionary<Tuple<Type, string>, object> _settings = new Dictionary<Tuple<Type, string>, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonSettings" /> class. 
        /// </summary>
        public ComparisonSettings()
        {
            RecursiveComparison = true;
            EmptyAndNullEnumerablesEqual = false;
            UseDefaultIfMemberNotExist = false;
        }

        /// <summary>
        /// Sets value of custom setting. Could be used to pass parameters to custom value comparers.
        /// </summary>
        /// <typeparam name="T">Setting Type.</typeparam>
        /// <param name="value">Setting Value.</param>
        /// <param name="key">Setting Key.</param>
        public void SetCustomSetting<T>(T value, string key = null)
        {
            var dictionaryKey = new Tuple<Type, string>(typeof(T), key);
            _settings[dictionaryKey] = value;
        }

        /// <summary>
        /// Gets value of custom setting. Could be used in custom value comparers.
        /// </summary>
        /// <typeparam name="T">Setting Type.</typeparam>
        /// <param name="key">Setting Key.</param>
        /// <returns>Setting Value.</returns>
        public T GetCustomSetting<T>(string key = null)
        {
            var dictionaryKey = new Tuple<Type, string>(typeof(T), key);

            if (_settings.TryGetValue(dictionaryKey, out var value))
            {
                return (T) value;
            }

            throw new KeyNotFoundException();
        }

        public Action<IDifferenceTreeNode, ListComparisonOptions> ListComparisonOptionsAction { get; private set; } = null;

        /// <summary>
        /// Configures list comparison behavior, especially the type of the comparison. For more info, see <see cref="ListComparisonOptions"/>.
        /// The term list has a general meaning here and includes almost all Enumerable objects.
        /// </summary>
        /// <param name="comparisonOptions">
        /// First parameter: Current list node.
        /// </param>
        public ComparisonSettings ConfigureListComparison(Action<IDifferenceTreeNode, ListComparisonOptions> comparisonOptions)
        {
            if (comparisonOptions is null)
            {
                throw new ArgumentNullException(nameof(comparisonOptions));
            }

            ListComparisonOptionsAction = comparisonOptions;

            return this;
        }

        /// <summary>
        /// Configures list comparison behavior, especially the type of comparison. For more info, see <see cref="ListComparisonOptions"/>.
        /// The term list has a general meaning here and includes almost all Enumerable objects.
        /// </summary>
        /// <param name="comparisonOptions">See <see cref="ListComparisonOptions"/>.</param>
        public ComparisonSettings ConfigureListComparison(Action<ListComparisonOptions> comparisonOptions)
        {
            ConfigureListComparison((_, options) => comparisonOptions(options));

            return this;
        }

        /// <summary>
        /// Configures the type of list comparison and whether to compare unequal lists. For more info, see <see cref="ListComparisonOptions"/>.
        /// The term list has a general meaning here and includes almost all Enumerable objects.
        /// </summary>
        /// <param name="compareElementsByKey">
        /// True value is shortcut for <see cref="ListComparisonOptions.CompareElementsByKey()"/> operation.
        /// False value is shortcut for <see cref="ListComparisonOptions.CompareElementsByIndex()"/> operation. Default value = false.
        /// </param>
        /// <param name="compareUnequalLists">
        /// Shortcut for <see cref="ListComparisonOptions.CompareUnequalLists(bool)"/> operation. Default value = false.
        /// </param>
        public ComparisonSettings ConfigureListComparison(bool compareElementsByKey = false, bool compareUnequalLists = false)
        {
            ConfigureListComparison(options =>
            {
                options.CompareUnequalLists(compareUnequalLists);

                if (compareElementsByKey)
                {
                    options.CompareElementsByKey();
                }
            });

            return this;
        }

        public Action<IDifferenceTreeNode, DifferenceTreeOptions> DifferenceTreeOptionsAction { get; private set; }

        /// <summary>
        /// Configures creation of the <see cref="IDifferenceTreeNode"/> instance, see <see cref="DifferenceTreeOptions"/>.
        /// </summary>
        /// <param name="options">
        /// First parameter: The ancestor member the tree node is configured for.
        /// </param>
        public ComparisonSettings ConfigureDifferenceTree(Action<IDifferenceTreeNode, DifferenceTreeOptions> options)
        {
            DifferenceTreeOptionsAction = options ?? throw new ArgumentNullException(nameof(options));

            return this;
        }

        public Action<IDifferenceTreeNode, DifferenceOptions> DifferenceOptionsAction;

        /// <summary>
        /// Configures creation of the <see cref="Difference"/> instance, see <see cref="DifferenceOptions"/>.
        /// </summary>
        /// <param name="differenceOptions">
        /// First parameter: The member the difference is configured for.
        /// </param>
        public ComparisonSettings ConfigureDifference(Action<IDifferenceTreeNode, DifferenceOptions> differenceOptions)
        {
            DifferenceOptionsAction = differenceOptions ?? throw new ArgumentNullException(nameof(differenceOptions));

            return this;
        }

        /// <summary>
        /// Configures creation of the <see cref="Difference"/> instance.
        /// </summary>
        public ComparisonSettings ConfigureDifference(bool includeRawValues)
        {
            ConfigureDifference((_, options) => options.IncludeRawValues(includeRawValues));

            return this;
        }

        /// <summary>
        /// Wraps <see cref="ConfigureDifferenceTree(Action{IDifferenceTreeNode, DifferenceTreeOptions})"/>, <see cref="ConfigureDifference(Action{IDifferenceTreeNode, DifferenceOptions})"/> and <see cref="ConfigureDifferencePath(Action{IDifferenceTreeNode, DifferencePathOptions})"/>
        /// by applying the <paramref name="memberNameProvider"/> argument to them.
        /// </summary>
        /// <param name="memberNameProvider">
        /// Customizes the <see cref="Difference.MemberPath"/>, <see cref="IDifferenceTreeNodeMember.Name"/>. <br/>
        /// First parameter: It can be null in some cases, for example, when it represents the root of the comparison.
        /// </param>
        /// <param name="includeRawValues">Whether raw values should be included in the <see cref="Difference"/> instance.</param>
        public ComparisonSettings ConfigureDifferences(Func<MemberInfo, string> memberNameProvider, bool includeRawValues = false)
        {
            ConfigureDifferenceTree((ancestor, options) =>
                options.UseDifferenceTreeNodeMemberFactory(defaultMember =>
                    new DifferenceTreeNodeMember(
                        defaultMember?.Info,
                        memberNameProvider(defaultMember?.Info))));

            ConfigureDifference((currentMember, options) =>
                options.UseDifferenceFactory(args =>
                    new Difference(
                        memberPath: memberNameProvider(currentMember.Member.Info),
                        args.DefaultDifference.Value1,
                        args.DefaultDifference.Value2,
                        args.DefaultDifference.DifferenceType,
                        rawValue1: includeRawValues ? args.RawValue1 : null,
                        rawValue2: includeRawValues ? args.RawValue2 : null)));

            ConfigureDifferencePath((ancestor, options) =>
                options.UseInsertPathFactory(args => memberNameProvider(ancestor.Member.Info)));

            return this;
        }

        public Action<IDifferenceTreeNode, DifferencePathOptions> DifferencePathOptionsAction;

        /// <summary>
        /// Configures the insertion into the difference path, see <see cref="DifferencePathOptions"/>.
        /// </summary>
        /// <param name="options">
        /// First parameter: The parent of the member to which the path is inserted.
        /// </param>
        public ComparisonSettings ConfigureDifferencePath(Action<IDifferenceTreeNode, DifferencePathOptions> options)
        {
            DifferencePathOptionsAction = options ?? throw new ArgumentNullException(nameof(options));

            return this;
        }
    }
}
