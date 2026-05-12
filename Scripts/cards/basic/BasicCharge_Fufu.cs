using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class BasicCharge_Fufu : CustomCardModel
{
    // 基础配置：1费，技能，普通，目标自己
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType target = TargetType.Self;
    private const bool showInLibrary = true;

    public override string PortraitPath => "res://fufu/images/card_portraits/BasicCharge_Fufu.png";

    public BasicCharge_Fufu() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);
        Creature player = base.Owner.Creature;
        // if (!Strike_Fufu._hasAppliedInitialBuffs)
        //     {
        //     Strike_Fufu._hasAppliedInitialBuffs = true;

        //     // 设置 元素能量
        //     await PowerCmd.SetAmount<ElementEnergyPower>(
        //         player, 
        //         Fufu.FufuModCode.Character.FufuMod.ElementEnergyValue, 
        //         player, 
        //         null
        //     );
        // }
        // 核心效果：获得 5 点 元素能量
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, 5, player, null);
    }

    // 升级效果：消耗变为 0 费
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }

}