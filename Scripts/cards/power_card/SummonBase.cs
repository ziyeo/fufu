using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.Models.Powers;
using Fufu.Scripts.Power;


namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class SummonBase : CustomCardModel
{
    // private const int energyCost = 1;
    // private const CardType type = CardType.Power;
    // private const CardRarity rarity = CardRarity.Uncommon;
    // private const TargetType targetType = TargetType.Self;
    // private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/summon_base.png";
    // "永久。回合结束时，每3名召唤物获得1点元素能量。";

    public SummonBase() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature c = Owner.Creature;
        await PowerCmd.Apply<SummonBasePower>(choiceContext,new[] { c }, 1, c, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}