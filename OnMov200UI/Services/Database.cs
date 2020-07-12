using System.Collections.Generic;
using avaTodo.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using onmov200;

namespace avaTodo.Services
{
    public class Database
    {
        private OnMov200 OnMov200;

        public Database(string root)
        {
            OnMov200 = new OnMov200(root, root);
        }
        
        private List<ActivityModel> Activities = null;

        public IEnumerable<ActivityModel> GetItems()
        {

            if (Activities == null)
            {
                Activities = OnMov200.GetHeaders().Select(x => new ActivityModel(x)).ToList();
            }
            return Activities;
        }

        
    }
}