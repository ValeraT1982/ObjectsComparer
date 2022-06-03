using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectsComparer
{
    public class DifferenceOptions
    {
        DifferenceOptions()
        {
        }

        /// <summary>
        /// Default options.
        /// </summary>
        internal static DifferenceOptions Default() => new DifferenceOptions();

        public bool RawValuesIncluded { get; private set; } = false;

        public void IncludeRawValues(bool value)
        {
            RawValuesIncluded = value;
        }
    }

    //public class CreateDifferenceArgs
    //{
    //    public CreateDifferenceArgs(string memberPath, string value1, string value2, DifferenceTypes differenceType = DifferenceTypes.ValueMismatch, object rawValue1 = null, object rawValue2 = null)
    //    {
    //        MemberPath = memberPath;
    //        Value1 = value1;
    //        Value2 = value2;
    //        DifferenceType = differenceType;
    //        RawValue1 = rawValue1;
    //        RawValue2 = rawValue2;
    //    }

    //    public string MemberPath { get; }

    //    public string Value1 { get; }

    //    public string Value2 { get; }

    //    public DifferenceTypes DifferenceType { get; }

    //    public object RawValue1 { get; }

    //    public object RawValue2 { get; }
    //}

    public static class DifferenceProvider
    {
        public static Difference CreateDifference(
            ComparisonSettings settings, 
            IDifferenceTreeNode differenceTreeNode,
            string memberPath, 
            string value1, 
            string value2, 
            DifferenceTypes differenceType = DifferenceTypes.ValueMismatch, 
            object rawValue1 = null, 
            object rawValue2 = null)
        {
            var options = DifferenceOptions.Default();
            settings.DifferenceOptionsAction?.Invoke(differenceTreeNode, options);

            //var difference = new Difference();

            throw new NotImplementedException();

        }
    }
}
