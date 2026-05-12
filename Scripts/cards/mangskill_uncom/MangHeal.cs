using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Fufu.FufuModCode.Character.CardPool;
using MegaCrit.Sts2.Core.Entities.Players;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class MangHeal : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(8)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_heal.png";

    public MangHeal() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;
        Player players = player.Player;

        // 基础效果：恢复8血
        await CreatureCmd.Heal(player, DynamicVars.Heal.IntValue);

        // 芒属性判定：气氛≥10 → 治疗+4，召唤歌者+2
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {          
            int atmos = player.GetPowerAmount<AtmospherePower>();
            if (atmos >= 10)
            {
                await CreatureCmd.Heal(player, 4);
                await PowerCmd.Apply<BasicSingerPower>(choiceContext,player, 2, player, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：2费 → 1费
       base.EnergyCost.UpgradeBy(-1);
    }

    //6.芒光治愈：耗 2 点卡牌能量，恢复 8 血；角色芒属性时气氛≥10 则恢复 + 4，召唤歌者+ 2。升级1能量
}