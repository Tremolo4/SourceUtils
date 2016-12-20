using System.IO;
using SourceUtils;
using Newtonsoft.Json.Linq;

namespace MapViewServer
{
    public class VmtServlet : ResourceServlet
    {
        private JToken PropertyGroupToJson(MaterialPropertyGroup props)
        {
            var obj = new JObject();
            
            foreach (var name in props.PropertyNames)
            {
                var value = props[name];
                
                int intValue;
                if (int.TryParse(value, out intValue))
                {
                    obj.Add(name, intValue);
                    continue;
                }
                
                double doubleValue;
                if (double.TryParse(value, out doubleValue))
                {
                    obj.Add(name, doubleValue);
                    continue;
                }
                
                obj.Add(name, value);
            }
            
            return obj;
        }
        
        protected override void OnService()
        {
            Response.ContentType = "application/json";
            
            var vmt = Program.Loader.Load<ValveMaterialFile>(FilePath);
            
            var response = new JObject();
            
            foreach (var shader in vmt.Shaders)
            {
                response.Add(shader, PropertyGroupToJson(vmt[shader]));
            }
            
            var writer = new StreamWriter(Response.OutputStream);
            
            writer.WriteLine(response.ToString());            
            writer.Flush();
        }
    }
}