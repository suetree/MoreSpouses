using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.setting
{
    class ValueName
    {

        public int Value { set; get; }
        public String Name { set; get; }

        public ValueName(int value, String name)
        {
            this.Value = value;
            this.Name = name;
        }
    }
}
