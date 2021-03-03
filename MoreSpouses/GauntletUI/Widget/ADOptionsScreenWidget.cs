using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
    public class ADOptionsScreenWidget : OptionsScreenWidget
    {
        public ADOptionsScreenWidget(UIContext context) : base(context)
        {
        }

        protected override void OnUpdate(float dt)
        {
            if (!this._initialized)
            {
                using (IEnumerator<Widget> enumerator = base.AllChildren.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        OptionsItemWidget optionsItemWidget;
                        if ((optionsItemWidget = (enumerator.Current as OptionsItemWidget)) != null)
                        {
                            optionsItemWidget.SetCurrentScreenWidget(this);
                        }
                    }
                }
                this._initialized = true;
            }
        }

        private Widget _currentOptionWidget;

        private bool _initialized;
    }
}