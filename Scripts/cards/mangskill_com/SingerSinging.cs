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
public class SingerSinging : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.Self;
    private const bool showInLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override string PortraitPath => "res://fufu/images/card_portraits/SingerSinging.png";

    public SingerSinging() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;

        // 召唤 3 名基础歌者，持续3回合
        await PowerCmd.Apply<BasicSingerPower>(choiceContext,player, 3, player, this);

        // 芒属性效果
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int element = player.GetPowerAmount<ElementEnergyPower>();
            if (element >= 2)
            {
                bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
                // 没有掌控 → 正常扣费
                if (!hasElementMastery)
                {
                // 扣除2元素能量
                await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -2, player, this);
                }
                // 获得1层歌者治疗加成（1回合）
                var buff = await PowerCmd.Apply<SingerTempHealBuffPower>(choiceContext,player, 1, player, this);
                if (buff != null)
                {
                    buff.Duration = 1; // 只持续当前回合！
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级变为0费
        base.EnergyCost.UpgradeBy(-1);
    }
}