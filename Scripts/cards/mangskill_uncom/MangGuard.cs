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
public class MangGuard : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 基础格挡 10 点
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10, ValueProp.Move),
        new HealVar(5)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/mang_guard.png";

    public MangGuard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        // 基础效果：获得10格挡 + 恢复5血
        await CreatureCmd.GainBlock(player, DynamicVars.Block.IntValue, DynamicVars.Block.Props, cardPlay);
        await CreatureCmd.Heal(player, DynamicVars.Heal.IntValue);

        // 芒属性判定
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int currentAtmo = player.GetPowerAmount<AtmospherePower>();
            // 需气氛 ≥7
            if (currentAtmo >= 7)
            {
                // 扣除7气氛
                await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -7, player, this);
                
                // 歌者加成 +2
                var Power = await PowerCmd.Apply<SingerTempHealBuffPower>(choiceContext,player, 2, player, this);
                if (Power != null)
                {
                    Power.Duration = 1; // 只持续当前回合！
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：格挡+3 → 10→13
        DynamicVars.Block.UpgradeValueBy(3);
    }
    //1.芒性守护：耗 2 点卡牌能量，获 10 格挡，恢复 5 血；角色芒属性时需气氛≥7，打出扣 7气氛歌者加成 + 2；升级加3格挡
}