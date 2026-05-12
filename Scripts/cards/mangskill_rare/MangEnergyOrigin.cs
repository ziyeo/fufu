using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class MangEnergyOrigin : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_energy_origin.png";

    public MangEnergyOrigin() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;

        // 获得8元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, 8, player, this);

        // 芒属性：歌者临时治疗+2（1回合）
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            var buff = await PowerCmd.Apply<SingerHealBuffPower>(choiceContext,player, 2, player, this);
            if (buff != null)
                {}
        }

        // 解锁被动：4气氛 ↔ 回1血
        int atmos = player.GetPowerAmount<AtmospherePower>();
        int heal = atmos / 4;

        if (heal > 0)
        {
            await CreatureCmd.Heal(player,heal);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：2费 → 1费
        base.EnergyCost.UpgradeBy(-1);
    }

    //芒能本源：耗2点卡牌能量，获8元素；芒属性时歌者治疗+2；解锁4气氛↔回1血，升级1能量。
}