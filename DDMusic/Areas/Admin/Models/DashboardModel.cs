using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class DashboardModel
    {
        public List<string> labels { get; set; }
        public List<DataSet> datasets { get; set; }

        public DashboardModel()
        {
            labels = new List<string>();
            datasets = new List<DataSet>();
        }
    }
    public class DataSet
    {
        public List<int> data { get; set; }
        public List<string> backgroundColor { get; set; }


        public DataSet()
        {
            data = new List<int>();
            backgroundColor = new List<string>();
            
        }
    }

    //public class DashboardBarModel
    //{
    //    public List<string> labels { get; set; }
    //    public List<DataSet> datasets { get; set; }

    //    public DashboardBarModel()
    //    {
    //        labels = new List<string>();
    //        datasets = new List<DataSet>();
    //    }
    //}
    //public class DataBarSet
    //{
    //    public List<int> data { get; set; }
    //    public string backgroundColor { get; set; }
    //    public string label { get; set; }


    //    public DataBarSet()
    //    {
    //        data = new List<int>();
            
    //    }
    //}
}
