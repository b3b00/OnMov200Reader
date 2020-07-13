using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using onmov200;
using onmov200.Models;

namespace onmov200.Services
{
    public class Database
    {
        public OnMov200 OnMov200 { get; private set; }

        public Database(string root)
        {
            OnMov200 = new OnMov200(root, root);
        }
        
        private List<ActivityModel> Activities = null;

        public IEnumerable<ActivityModel> GetActivities()
        {

            if (Activities == null)
            {
                Activities = OnMov200.GetHeaders().Select(x => new ActivityModel(x)).ToList();
            }
            return Activities;
        }

        
    }
}