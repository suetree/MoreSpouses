using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace SueMoreSpouses.Operation
{

    class DistinctSpouse<TModel> : IEqualityComparer<TModel>
    {
        public bool Equals(TModel x, TModel y)
        {
            Hero t = x as Hero;
            Hero tt = y as Hero;
            if (t != null && tt != null) return t.StringId == tt.StringId;
            return false;
        }

        public int GetHashCode(TModel obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}