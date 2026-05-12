using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class HuangMangFeast : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool showInLibrary = true;

    // 标记为【消耗】
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(4),
        new CardsVar(4)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_mang_feast.png";

    public HuangMangFeast() : base(energyCost, type, rarity, targetType, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        // 荒属性：抽4张牌
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
        }

        // 芒属性：获得4点能量
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：取消消耗
        RemoveKeyword(CardKeyword.Exhaust);
    }
}