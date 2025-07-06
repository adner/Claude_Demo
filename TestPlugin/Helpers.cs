using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Xrm.Sdk;

namespace PluginTest{


    public static class Helpers
    {
        public static string ToJson(this IPluginExecutionContext context)
        {
            var serializer = new DataContractJsonSerializer(typeof(RemoteExecutionContext), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH\\:mm\\:ss.ffFFFFFzzz")
            });
            using (MemoryStream ms = new MemoryStream())
            using (StreamReader sr = new StreamReader(ms))
            {
                serializer.WriteObject(ms, context);
                ms.Position = 0;
                return sr.ReadToEnd();
            }
        }
        
        public static RemoteExecutionContext ToRemoteExecutionContext(this IPluginExecutionContext context)
        {
            var destination = new RemoteExecutionContext();
            var destFields = destination.GetType()
                .GetFields(BindingFlags.NonPublic |
                           BindingFlags.Instance)
                .ToArray();
            foreach (var sourceProperty in context.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var destField in destFields)
                {
                    if (sourceProperty.Name == "PreEntityImages" && destField.Name == "_preImages")
                    {
                        destField.SetValue(destination, sourceProperty.GetValue(
                            context, new object[] { }));
                        break;
                    }
                    if (sourceProperty.Name == "PostEntityImages" && destField.Name == "_postImages")
                    {
                        destField.SetValue(destination, sourceProperty.GetValue(
                            context, new object[] { }));
                        break;
                    }
                    if (!destField.Name.ToLower().Contains(sourceProperty.Name.ToLower()) ||
                        !destField.FieldType.IsAssignableFrom(sourceProperty.PropertyType)) continue;
                    destField.SetValue(destination, sourceProperty.GetValue(
                        context, new object[] { }));
                    break;
                }
            }
            return destination;
        }
    }
}