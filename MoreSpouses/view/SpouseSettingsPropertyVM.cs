using SueMoreSpouses.view.setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view
{
     class SpouseSettingsPropertyVM : ViewModel
    {

       SpouseSettingsProperty _spouseSettingsProperty;

      public SpouseSettingsPropertyVM(SpouseSettingsProperty spouseSettingsProperty)
        {
            this._spouseSettingsProperty = spouseSettingsProperty;
            this.RefreshValues();
        }

         [DataSourceProperty]
          public string DisplayName
          {
              get => new TextObject(this._spouseSettingsProperty.DisplayName, null).ToString();
          }

           [DataSourceProperty]
           public bool IsBoolProperty {
               get => this._spouseSettingsProperty.SettingsType == SpouseSettingsType.BoolProperty;
           }

           [DataSourceProperty]
           public bool IsFloatProperty
           {
               get => (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.FloatProperty || this._spouseSettingsProperty.SettingsType == SpouseSettingsType.IntegerProperty);
           }

           [DataSourceProperty]
           public bool BoolValue {
               get {
                   bool val = false;
                   if(this._spouseSettingsProperty.SettingsType == SpouseSettingsType.BoolProperty)
                   {
                    if(null != this._spouseSettingsProperty.PropertyValue)
                    {
                        val = (bool)this._spouseSettingsProperty.PropertyValue;
                    }
                      
                   }
                   return val;
               }
               set {
                   if (!this._spouseSettingsProperty.PropertyValue.Equals(value))
                   {
                       this._spouseSettingsProperty.PropertyValue = value;
                       base.OnPropertyChanged("BoolValue");
                   }
               }
           }

           [DataSourceProperty]
           public float NumberValue
           {
               get {
                   float val = 0f;
                  if (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.FloatProperty || this._spouseSettingsProperty.SettingsType == SpouseSettingsType.IntegerProperty)
                   {
                    if (null != this._spouseSettingsProperty.PropertyValue)
                    {
                        if (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.FloatProperty)
                        {
                            val = (float)this._spouseSettingsProperty.PropertyValue;
                        }
                        else
                        {
                            val = Convert.ToInt32(this._spouseSettingsProperty.PropertyValue);
                        }
                    }
                   }
                   return val;
               }
               set
               {
                   if (!this._spouseSettingsProperty.PropertyValue.Equals(value))
                   {
                    if (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.IntegerProperty)
                    {
                        this._spouseSettingsProperty.PropertyValue = Convert.ToInt32(value);
                    }
                    else {
                        this._spouseSettingsProperty.PropertyValue = value;
                    }
                     
                       base.OnPropertyChanged("NumberValue");
                       base.OnPropertyChanged("ShowValueText");
                       //InformationManager.DisplayMessage(new InformationMessage("newValue " + this._spouseSettingsProperty.PropertyValue));
                   }
               }
           }

           [DataSourceProperty]
           public float MaxValue {
               get => (float)this._spouseSettingsProperty.MaxValue;
           }


           [DataSourceProperty]
           public float MinValue
           {
               get => (float)this._spouseSettingsProperty.MinValue;
           }


        [DataSourceProperty]
        public string ShowValueText
        {
            get {
                String text = "";
                if (null != this._spouseSettingsProperty.PropertyValue)
                {
                    if (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.FloatProperty)
                    {
                        float val = (float)this._spouseSettingsProperty.PropertyValue;
                        text = val.ToString("0.00");
                    }
                    else if (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.IntegerProperty)
                    {
                        int val = Convert.ToInt32(this._spouseSettingsProperty.PropertyValue);
                        text = val + "";
                    }
                }
                return text;
            }
        }

    }
}
