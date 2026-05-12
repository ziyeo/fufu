using BaseLib.Abstracts;
using Fufu.Scripts.Cards;
using Fufu.Scripts.Relics;
using Fufu.Scripts.Power;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character.RelicPool;
using Fufu.FufuModCode.Character.PotionPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;


namespace Fufu.FufuModCode.Character;

public enum FufuAttr
{
	Huang,
	Mang
}
public class FufuMod : PlaceholderCharacterModel
{
	public static int AtmosphereValue { get; set; }=0;
	public static int ElementEnergyValue { get; set; }=0;
	
	// 资源上限（可通过能力牌提升，贴合核心规则）
	public static int AtmosphereMax { get; set; } = int.MaxValue;
	public static int ElementEnergyMax { get; set; } = int.MaxValue; // 元素能量无基础上限
	
	// 当前荒/芒属性（初始荒属性，贴合核心规则）
	public static FufuAttr CurrentAttr { get; set; } = FufuAttr.Huang;
	public const string CharacterId = "Fufu";
	
	public static readonly Color Color = new("#73d9ff");
	public override Color DialogueColor => Colors.Blue;

	// 地图上该角色绘制连线时使用的颜色
	public override Color MapDrawingColor => Colors.Blue;

	public override Color NameColor => Color;
	public override CharacterGender Gender => CharacterGender.Neutral;
	public override int StartingHp => 80;
	public override string CustomCharacterSelectBg => "res://fufu/images/charui/bg.tscn";
	public override string CustomVisualPath => "res://fufu/scenes/creature_visuals/fufu.tscn";
	public override string CustomRestSiteAnimPath => "res://fufu/scenes/rest_site/fufu_rest_site.tscn";
	public override string CustomMerchantAnimPath => "res://fufu/scenes/merchant/fufu_merchant.tscn";
	public override string CustomIconPath => "res://fufu/scenes/ui/character_icons/fufusmall.tscn";
	public override string CustomEnergyCounterPath => "res://fufu/images/charui/test_energy_counter.tscn";
	// 初始卡组
	public override IEnumerable<CardModel> StartingDeck => new List<CardModel>
	{
		ModelDb.Card<Strike_Fufu>(),
		ModelDb.Card<Strike_Fufu>(),
		ModelDb.Card<Strike_Fufu>(),
		ModelDb.Card<Strike_Fufu>(),
		ModelDb.Card<Defend_Fufu>(),
		ModelDb.Card<Defend_Fufu>(),
		ModelDb.Card<Defend_Fufu>(),
		ModelDb.Card<Defend_Fufu>(),
		ModelDb.Card<BasicCharge_Fufu>(),
		ModelDb.Card<BasicDrawCards>(),
		ModelDb.Card<BeckonShadow>(),
		ModelDb.Card<BeckonShadow>(),
	};

	// 初始遗物
	public override IReadOnlyList<RelicModel> StartingRelics => [
		ModelDb.Relic<HuangMangOriginSeal>(),
	];
	
	
	public override CardPoolModel CardPool => ModelDb.CardPool<FufuCardPool>();
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<FufuRelicPool>();
	public override PotionPoolModel PotionPool => ModelDb.PotionPool<FufuPotionPool>();

	public override string CustomIconTexturePath => "res://fufu/images/charui/character_icon_char_name.png";
	public override string CustomCharacterSelectIconPath => "res://fufu/images/charui/char_select_char_name.png";
	public override string CustomCharacterSelectLockedIconPath => "res://fufu/images/charui/char_select_char_name.png";
	public override string CustomMapMarkerPath => "res://fufu/images/charui/character_icon_char_name.png";
}
