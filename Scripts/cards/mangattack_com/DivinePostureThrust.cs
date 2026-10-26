using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.CardPools;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class DivinePostureThrust : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.AnyEnemy;
    private const bool showInLibrary = true;

    // 基础伤害7 + 基础格挡5
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new BlockVar(5, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/DivinePostureThrust.png";

    public DivinePostureThrust() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;
        if (cardPlay.Target == null) return;

        // 基础伤害
        decimal finalDamage = DynamicVars.Damage.BaseValue;

        // 气氛≥5 时 伤害+3
        int currentAtm = player.GetPowerAmount<AtmospherePower>();
        if (currentAtm >= 5)
        {
            finalDamage += 3;
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext, player, 3, player, this);
        }

        // 造成伤害
        await DamageCmd.Attack(finalDamage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        

        // 芒属性时获得格挡
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await CreatureCmd.GainBlock(
                player,
                DynamicVars.Block.BaseValue,
                DynamicVars.Block.Props,
                cardPlay
            );
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡 +5（5→10）
        DynamicVars.Block.UpgradeValueBy(5);
    }
}