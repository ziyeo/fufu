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
public class HuangFlameSacrifice : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_flame_sacrifice.png";

    public HuangFlameSacrifice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;
         if (cardPlay.Target == null) return;

        // 对敌人造成18点普通伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 荒属性时获得4元素能量
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, 4, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级伤害+4：25 → 29
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}