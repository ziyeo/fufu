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
public class SingerGuardSacrifice : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new BlockVar(8, ValueProp.Move)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/singer_guard_sacrifice.png";

    public SingerGuardSacrifice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature creature = Owner.Creature;
        if (!creature.IsPlayer || creature.Player == null) return;
        Player player = creature.Player;

        if (cardPlay.Target == null) return;

        // 造成12点基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性时：格挡 +8
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await CreatureCmd.GainBlock(
            creature, 
            DynamicVars.Block.BaseValue, 
            DynamicVars.Block.Props, 
            cardPlay
        );
        }
        
        // 检查条件：元素≥10 + 气氛≥4
        int element = creature.GetPowerAmount<ElementEnergyPower>();
        int atmosphere = creature.GetPowerAmount<AtmospherePower>();
        if (element < 10 || atmosphere < 4)
            return;

        // 扣除 10元素 + 4气氛
        bool hasElementMastery = creature.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,creature, -10, creature, this);
        }
        await PowerCmd.Apply<AtmospherePower>(choiceContext,creature, -4, creature, this);
        // 歌者治疗 +2（1回合）
        var healBuff = await PowerCmd.Apply<SingerHealBuffPower>(choiceContext,creature, 2, creature, this);
        if (healBuff != null)
        {
            
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：伤害 +5
        DynamicVars.Damage.UpgradeValueBy(5);
    }

  // 歌者守护献祭：耗 2 点卡牌能量，造 12 普通伤害 ；芒属性时格挡 + 8；需元素≥10 + 气氛≥4，打出扣 10 元素 + 4 气氛， 歌者治疗+2。升级+5伤害

}