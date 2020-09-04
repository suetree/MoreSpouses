using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.setting
{
    class ValueNamePair
    {

        public int Value { set; get; }
        public String Name { set; get; }

        public ValueNamePair(int value, String name)
        {
            this.Value = value;
            this.Name = name;
        }
    }
}
