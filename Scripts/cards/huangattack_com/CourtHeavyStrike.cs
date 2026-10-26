using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class CourtHeavyStrike : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/court_heavy_strike.png";

    public CourtHeavyStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        if (cardPlay.Target == null) return;

        // 造成基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 荒属性效果
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player);
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, 2, creature, this);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}