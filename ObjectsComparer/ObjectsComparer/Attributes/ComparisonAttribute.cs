// ***********************************************************************
// Assembly         : Objects Comparer
// Author           : Ankur Kumar Gupta
// Created          : 24-Feb-2022
// ***********************************************************************

namespace ObjectsComparer.Attributes
{
  using System;
  using ObjectsComparer.Enums;

  /// <summary>
  /// Class is used to specify whether the element on which it is applied will have comparison effect
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class ComparisonAttribute : Attribute
  {
    #region Properties

    /// <summary>
    /// Gets or sets (privately) comparison status i.e. whether or not it is to be included in comparison
    /// </summary>
    public ComparisonStatus ComparisonStatus { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes an instance of the <see cref="ComparisonAttribute"/> class.
    /// </summary>
    /// <param name="comparisonStatus">Reference to type of comparison</param>
    public ComparisonAttribute(ComparisonStatus comparisonStatus)
    {
      // stores the value of comparison status
      this.ComparisonStatus = comparisonStatus;
    }

    #endregion
  }
}
