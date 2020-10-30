using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounts
{
    [Serializable]
    class PackageClass
    {
        string app_path = AppDomain.CurrentDomain.BaseDirectory;
        string packName;
        string _Parameter;
        string _Pvalue;
        public PackageClass()
        {

        }
       public PackageClass(string name, string parameter, string pvalue)
        {
            packName = name;
            _Parameter = parameter;
            _Pvalue = pvalue;

        }
        public string PName { get { return packName; }
            set { packName = value; } }
        public string Parameter {get{ return _Parameter ;} set { _Parameter = value; } }
        public string Pvalue {get{ return _Pvalue ;} set { _Pvalue = value; } }

        
       
    }
    
}
