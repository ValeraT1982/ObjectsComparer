using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectsComparer.Tests.Utils
{
    //TODO: Move ToJson to Core.

    internal static class ComparisonContextSerializationExtensions
    {
        /// <summary>
        /// Serializes <see cref="ComparisonContext"/> object to json string.
        /// </summary>
        /// <param name="comparisonContext"></param>
        /// <param name="skipEmptyList"></param>
        /// <param name="skipNullReference"></param>
        /// <returns></returns>
        public static string ToJson(this ComparisonContext comparisonContext, bool skipEmptyList = true, bool skipNullReference = true)
        {
            return SerializeComparisonContext(comparisonContext, skipEmptyList, skipNullReference);
        }

        static string SerializeComparisonContext(ComparisonContext context, bool skipEmptyList, bool skipNullReference)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new ComparisonContextContractResolver(skipEmptyList, skipNullReference),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = false });
            settings.Converters.Add(new MemberInfoConverter());

            return JsonConvert.SerializeObject(context, Formatting.Indented, settings);
        }

        /// <summary>
        /// Converts <see cref="MemberInfo"/> object to JSON.
        /// </summary>
        class MemberInfoConverter : JsonConverter<MemberInfo>
        {
            public override void WriteJson(JsonWriter writer, MemberInfo value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(nameof(MemberInfo.Name));
                writer.WriteValue(value.Name);
                writer.WritePropertyName(nameof(MemberInfo.DeclaringType));
                writer.WriteValue(value.DeclaringType.FullName);
                writer.WriteEndObject();
            }

            public override MemberInfo ReadJson(JsonReader reader, Type objectType, MemberInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanRead => false;
        }

        class ComparisonContextContractResolver : DefaultContractResolver
        {
            readonly bool _skipEmptyList;
            readonly bool _skipNullReference;

            public ComparisonContextContractResolver(bool skipEmptyList = true, bool skipNullReference = true)
            {
                _skipEmptyList = skipEmptyList;
                _skipNullReference = skipNullReference;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                if (property.DeclaringType == typeof(ComparisonContext))
                {
                    property.ShouldSerialize =
                        instance =>
                        {
                            ComparisonContext ctx = (ComparisonContext)instance;

                            if (property.PropertyName == nameof(ComparisonContext.Descendants))
                            {
                                return _skipEmptyList == false || ctx.Descendants.Any();
                            }

                            if (property.PropertyName == nameof(ComparisonContext.Differences))
                            {
                                return _skipEmptyList == false || ctx.Differences.Any();
                            }

                            if (property.PropertyName == nameof(ComparisonContext.Member))
                            {
                                return _skipNullReference == false || ctx.Member != null;
                            }

                            if (property.PropertyName == nameof(ComparisonContext.Ancestor))
                            {
                                return _skipNullReference == false || ctx.Ancestor != null;
                            }

                            return true;
                        };

                    //if (property.PropertyName == nameof(ComparisonContext.Ancestor))
                    //{
                    //    property.ValueProvider = new AncestorValueProvider();
                    //}
                }

                return property;
            }
        }

        //class AncestorValueProvider : IValueProvider
        //{
        //    public object GetValue(object target)
        //    {
        //        var ancestor = (target as ComparisonContext).Ancestor;
        //        var newAncestor = ComparisonContext.Create(member: ancestor?.Member);
        //        return newAncestor;
        //    }

        //    public void SetValue(object target, object value)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
