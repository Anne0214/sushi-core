using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.All.Infrastructure.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteIdAttribute : Attribute
    {
        public string Id { get; }
        public RouteIdAttribute(string id) => Id = id;
    }

}
