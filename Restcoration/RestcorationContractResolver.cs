using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Restcoration
{
    public class RestcorationContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            var urlSegmentAttribute = member.GetCustomAttribute(typeof (UrlSegmentAttribute), true) as UrlSegmentAttribute;
            if (urlSegmentAttribute != null)
            {
                if (urlSegmentAttribute.Ignore)
                    prop.ShouldSerialize = o => false;
            }
            return prop;
        }
    }
}