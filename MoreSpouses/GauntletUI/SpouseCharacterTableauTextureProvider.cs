using SueMoreSpouses.Tableau;
using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.TwoDimension;

namespace SueMoreSpouses.GauntletUI
{
    class SpouseCharacterTableauTextureProvider : TextureProvider
    {
		private SpouseCharacterTableau _characterTableau;

		private TaleWorlds.Engine.Texture _texture;

		private TaleWorlds.TwoDimension.Texture _providedTexture;

		public string BannerCodeText
		{
			set
			{
				this._characterTableau.SetBannerCode(value);
			}
		}

		public string BodyProperties
		{
			set
			{
				this._characterTableau.SetBodyProperties(value);
			}
		}

		public int StanceIndex
		{
			set
			{
				this._characterTableau.SetStanceIndex(value);
			}
		}

		public bool IsFemale
		{
			set
			{
				this._characterTableau.SetIsFemale(value);
			}
		}

		public bool IsEquipmentAnimActive
		{
			set
			{
				this._characterTableau.SetIsEquipmentAnimActive(value);
			}
		}

		public string EquipmentCode
		{
			set
			{
				this._characterTableau.SetEquipmentCode(value);
			}
		}

		public bool CurrentlyRotating
		{
			set
			{
				this._characterTableau.RotateCharacter(value);
			}
		}

		public string MountCreationKey
		{
			set
			{
				this._characterTableau.SetMountCreationKey(value);
			}
		}

		public uint ArmorColor1
		{
			set
			{
				this._characterTableau.SetArmorColor1(value);
			}
		}

		public uint ArmorColor2
		{
			set
			{
				this._characterTableau.SetArmorColor2(value);
			}
		}

		public SpouseCharacterTableauTextureProvider()
		{
			this._characterTableau = new SpouseCharacterTableau();
		}

		public override void Clear()
		{
			this._characterTableau.OnFinalize();
			base.Clear();
		}

		private void CheckTexture()
		{
			if (this._texture != this._characterTableau.Texture)
			{
				this._texture = this._characterTableau.Texture;
				if (this._texture != null)
				{
					EngineTexture platformTexture = new EngineTexture(this._texture);
					this._providedTexture = new TaleWorlds.TwoDimension.Texture(platformTexture);
					return;
				}
				this._providedTexture = null;
			}
		}

		public override TaleWorlds.TwoDimension.Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._characterTableau.SetTargetSize(width, height);
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._characterTableau.OnTick(dt);
		}
	}
}
