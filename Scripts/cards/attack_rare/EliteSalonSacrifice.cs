using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using Fufu.FufuModCode.Character;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class EliteSalonSacrifice : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25, ValueProp.Move),
        new EnergyVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/elite_salon_sacrifice.png";

    public EliteSalonSacrifice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        if (cardPlay.Target == null) return;

        // 造成25点基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 检查气氛 ≥4
        int currentAtmosphere = creature.GetPowerAmount<AtmospherePower>();
        if (currentAtmosphere >= 4)
        {
            // 扣除4气氛
            await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, -4, creature, this);

            // 恢复1点能量
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        }

        // 荒属性：召唤2名沙龙 + 伤害+2
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            bool hasSalonBond = creature.HasPower<SalonBondPower>();
            int salonBond = creature.GetPowerAmount<SalonBondPower>();
            await PowerCmd.Apply<ShadowSalonPower>(choiceContext,creature, 2, creature, this);
            if(hasSalonBond)
                await CardPileCmd.Draw(choiceContext, salonBond, player);
            var buff =await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,creature, 2, creature, this);
            if (buff != null){
                buff.Duration = 1;
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级只改变回能数量，无需修改变量
        DynamicVars.Energy.UpgradeValueBy(2);
    }
}