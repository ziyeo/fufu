using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class OperaStab : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/opera_stab.png";

    public OperaStab() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        if (cardPlay.Target == null)
            return;

        Creature player = base.Owner.Creature;
        Creature target = cardPlay.Target;

        // 基础伤害
        int damage = DynamicVars.Damage.IntValue;

        // 荒属性时额外 +4 伤害
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            damage += 4;
        }

        // 造成伤害
        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +4
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}