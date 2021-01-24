using SueMoreSpouses.setting;
using SueMoreSpouses.view.setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view
{
     class SpouseSettingsPropertyVM : ViewModel
    {

        SpouseSettingsProperty _spouseSettingsProperty;
        SelectorVM<SelectorItemVM> _selectorVM;

      public SpouseSettingsPropertyVM(SpouseSettingsProperty spouseSettingsProperty)
        {
            this._spouseSettingsProperty = spouseSettingsProperty;
            if (this.IsDropdownProperty)
            {
                ValueNamePair  valueName = (ValueNamePair) this._spouseSettingsProperty.PropertyValue;
                List<TextObject> texts = new List<TextObject>();
                int currentIndex = 0;
                int k = 0;
                this._spouseSettingsProperty.SelectItems.ForEach((obj) => {
                    if (valueName.Value == obj.Value) { currentIndex = k; }
                    texts.Add(new TextObject(obj.Name, null));
                    k++;
                }  );
                this._selectorVM = new SelectorVM<SelectorItemVM>(texts, currentIndex, OnselectItem);
            }
            this.RefreshValues();
        }

        public void OnselectItem(SelectorVM<SelectorItemVM> selectorVM)
        {
            // selectorVM.ItemList = this.DropdownValue.ItemList;
            // selectorVM.SelectedIndex = this.DropdownValue.SelectedIndex;
            this._spouseSettingsProperty.PropertyValue = this._spouseSettingsProperty.SelectItems[selectorVM.SelectedIndex];
            base.OnPropertyChanged("DropdownValue");
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
        public bool IsTextInputProperty
        {
            get => this._spouseSettingsProperty.SettingsType == SpouseSettingsType.InputTextProperty;
        }

        [DataSourceProperty]
           public bool IsFloatProperty
           {
               get => (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.FloatProperty || this._spouseSettingsProperty.SettingsType == SpouseSettingsType.IntegerProperty);
           }

        [DataSourceProperty]
        public bool IsDropdownProperty
        {
            get => (this._spouseSettingsProperty.SettingsType == SpouseSettingsType.SelectProperty );
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM>  DropdownValue
        {
            get {
                if (this.IsDropdownProperty)
                {
                    return this._selectorVM; 
                }
                else
                {
                    return new SelectorVM<SelectorItemVM>(0, null);
                }
            }
        }

        [DataSourceProperty]
        public String TextValue
        {
            get  {
                if (IsTextInputProperty && null != this._spouseSettingsProperty.PropertyValue)
                {
                    return this._spouseSettingsProperty.PropertyValue.ToString();
                }
                return "";
            }
        }

        public void OnTextValueClick()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData(DisplayName, string.Empty, true, true, 
                GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), 
                new Action<string>(this.OnChangeNameDone), null, false, new Func<string, bool>(this.IsNewNameApplicable), ""), false);
        }
        private bool IsNewNameApplicable(string input)
        {
            return input.Length <= 10 && input.Length >= 0;
        }

        private void OnChangeNameDone(string value)
        {
            if (null != this._spouseSettingsProperty.PropertyValue &&!this._spouseSettingsProperty.PropertyValue.Equals(value))
            {
                this._spouseSettingsProperty.PropertyValue = value;

            }
            else
            {
                this._spouseSettingsProperty.PropertyValue = value;
            }

            base.OnPropertyChanged("TextValue");
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
