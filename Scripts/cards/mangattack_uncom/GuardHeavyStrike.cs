using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class GuardHeavyStrike : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new BlockVar(4, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/guard_heavy_strike.png";

    public GuardHeavyStrike() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        if (cardPlay.Target == null) return;

        // 造成伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性时获得格挡
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await CreatureCmd.GainBlock(player, DynamicVars.Block.IntValue, DynamicVars.Block.Props, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        // 伤害+3，格挡+2
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}