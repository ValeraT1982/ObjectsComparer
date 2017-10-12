using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectsComparer
{
    /// <summary>
    /// Defines properties and methods to configure comparer.
    /// </summary>
    public interface IBaseComparer
    {
        /// <summary>
        /// Default <see cref="IValueComparer"/>
        /// </summary>
        IValueComparer DefaultValueComparer { get; }

        /// <summary>
        /// Comparison Settings.
        /// </summary>
        ComparisonSettings Settings { get; }

        /// <summary>
        /// Sets <see cref="DefaultValueComparer"/>.
        /// </summary>
        /// <param name="valueComparer">Value Comparer.</param>
        void SetDefaultComparer(IValueComparer valueComparer);

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        /// <param name="memberLambda">Lambda to get member.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        void AddComparerOverride<TProp>(Expression<Func<TProp>> memberLambda, IValueComparer valueComparer);

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <param name="memberInfo">Member Info.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        void AddComparerOverride(MemberInfo memberInfo, IValueComparer valueComparer);

        /// <summary>
        /// Adds Comparer Override by Type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true. Null by default.</param>
        void AddComparerOverride(Type type, IValueComparer valueComparer, Func<MemberInfo, bool> filter = null);

        /// <summary>
        /// Adds Comparer Override by Type.
        /// </summary>
        /// <typeparam name="TType">Type.</typeparam>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true. Null by default.</param>
        void AddComparerOverride<TType>(IValueComparer valueComparer, Func<MemberInfo, bool> filter = null);

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        /// <param name="memberLambda">Lambda to get member.</param>
        /// <param name="compareFunction">Function to compare objects.</param>
        /// <param name="toStringFunction">Function to convert objects to string.</param>
        void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda, 
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction, 
            Func<TProp, string> toStringFunction);

        /// <summary>
        /// Adds Comparer Override by Member.
        /// </summary>
        /// <typeparam name="TProp">Type of the member.</typeparam>
        /// <param name="memberLambda">Lambda to get member.</param>
        /// <param name="compareFunction">Function to compare objects.</param>
        void AddComparerOverride<TProp>(
            Expression<Func<TProp>> memberLambda,
            Func<TProp, TProp, ComparisonSettings, bool> compareFunction);

        /// <summary>
        /// Adds Comparer Override by Member name.
        /// </summary>
        /// <param name="memberName">Member Name.</param>
        /// <param name="valueComparer">Value Comparer.</param>
        /// <param name="filter">Value Comparer will be used only if filter(memberInfo) == true. Null by default.</param>
        void AddComparerOverride(string memberName, IValueComparer valueComparer, Func<MemberInfo, bool> filter = null);
    }
}