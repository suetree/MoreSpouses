using SueMoreSpouses.setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace SueMoreSpouses.view.setting
{
    class SpouseSettingsProperty
    {
        public SpouseSettingsType SettingsType { get; set; }
        public String DisplayName { get; set; }
        public String FieldName { get; set; }

        public List<ValueName> SelectItems { get; set; }


        object _propertyValue;


        public delegate void OnValueChange(object value);

    

        public object PropertyValue {
            get {
                return this._propertyValue;
            }
            set {
                if (value != this._propertyValue )
                {
                    this._propertyValue = value;
                    SetPropertyByMothen(value);
                }
            }
        }

        public SpouseSettingsProperty(string identifyKey, SpouseSettingsType settingsType, string displayName,  float minValue = 0, float maxValue = 0)
        {
            SettingsType = settingsType;
            DisplayName = displayName;
            FieldName = identifyKey;
            MaxValue = maxValue;
            MinValue = minValue;
        }

        public SpouseSettingsProperty DefaultValue(object value)
        {
            this.PropertyValue = value;
            return this;
        }

        public SpouseSettingsProperty SetSelectItems(List<ValueName> list)
        {
            this.SelectItems = list;
            return this;
        }

        

        private void SetPropertyByMothen(object value)
        {

            Type Ts = MoreSpouseSetting.Instance.SettingData.GetType();
            object v = Convert.ChangeType(value, Ts.GetProperty(FieldName).PropertyType);
            Ts.GetProperty(FieldName).SetValue(MoreSpouseSetting.Instance.SettingData, v, null);

        }
      



        public float MaxValue { get; set; }
        public float MinValue { get; set; }


    }
}
