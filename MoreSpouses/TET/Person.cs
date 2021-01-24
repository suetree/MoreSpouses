using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.TET
{
    class Person
    {
        public string name;

        private int age;

        private int gender;

        public Person(string name, int age)
        {
            this.name = name;
            this.age = age;
            this.gender = 1;//1=男
        }


    }
}
