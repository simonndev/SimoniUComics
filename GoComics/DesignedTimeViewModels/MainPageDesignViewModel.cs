using GoComics.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoComics.DesignedTimeViewModels
{
    public class MainPageDesignViewModel
    {
        public MainPageDesignViewModel()
        {
            var allTimeFeatures = JsonConvert.DeserializeObject<List<Featured>>(File.ReadAllText(@"../Json/AllTimeFeatures.json"));
            this.AllTimeFeatureCollection = new ReadOnlyCollection<Featured>(allTimeFeatures);
        }

        public IReadOnlyCollection<Featured> AllTimeFeatureCollection
        {
            get; private set;
        }
    }
}
