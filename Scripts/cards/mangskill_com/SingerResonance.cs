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
public class SingerResonance : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.Self;
    private const bool showInLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/singer_resonance.png";

    public SingerResonance() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;

        // 基础治疗量
        int healAmount = 2;

        // 芒属性时 +1 → 总共 3
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            healAmount = 3;
        }

        // 给歌者加治疗buff 2回合
        var buff = await PowerCmd.Apply<SingerTempHealBuffPower>(choiceContext,player, healAmount, player, this);
        if (buff != null)
            {
                buff.Duration = 1; // 只持续当前回合！
            }
        // 判断是否有歌者
        bool hasSinger = player.HasPower<BasicSingerPower>();

        if (!hasSinger)
        {
            // 无歌者 → 召唤基础歌者 2 回合
            await PowerCmd.Apply<BasicSingerPower>(choiceContext,player, 2, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：费用 -1
        EnergyCost.UpgradeBy(-1);
    }

   
        // "歌者治疗+2（2回合）。\n"
        // "芒属性时治疗+1。\n" +
        // "无歌者时，召唤基础歌者2回合。\n" +
        // "升级：能量消耗-1。";
}