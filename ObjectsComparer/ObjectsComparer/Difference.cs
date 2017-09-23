namespace ObjectsComparer
{
    public class Difference
    {
        public string MemberPath { get; }

        public string Value1 { get; }

        public string Value2 { get; }

        public DifferenceTypes DifferenceType { get; }

        public string AdditionalInfo { get; }

        public Difference(string memberPath, string value1, string value2, 
            DifferenceTypes differenceType = DifferenceTypes.ValueMismatch, 
            string additionalInfo = "")
        {
            MemberPath = memberPath;
            Value1 = value1;
            Value2 = value2;
            AdditionalInfo = additionalInfo;
            DifferenceType = differenceType;
        }

        public Difference InsertPath(string path)
        {
            var newPath = string.IsNullOrWhiteSpace(MemberPath) || MemberPath.StartsWith("[")
                ? path + MemberPath
                : path + "." + MemberPath;

            return new Difference(
                newPath,
                Value1,
                Value2,
                DifferenceType,
                AdditionalInfo);
        }

        public override string ToString()
        {
            return $"Difference: MemberPath='{MemberPath}', Value1='{Value1}', Value2='{Value2}'.";
        }
    }
}