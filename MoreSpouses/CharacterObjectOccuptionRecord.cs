using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses
{
    class CharacterObjectOccuptionRecord
    {

        public String StringId;

        public String CurrOccuption;

        public String OriginalOccuption;

        public List<CharacterObjectOccuptionTemplate> OccuptionTemplates = new List<CharacterObjectOccuptionTemplate>();
    }

    class CharacterObjectOccuptionTemplate {

        public String Occuption;
        public String Template;
    }
}
