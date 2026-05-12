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
using Fufu.Scripts.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class Defend_Fufu : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Basic;
    private const TargetType target = TargetType.Self;
    private const bool showInLibrary = true;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };
    // 基础格挡：4点
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];
    public override string PortraitPath => "res://fufu/images/card_portraits/Defend_Fufu.png";

    public Defend_Fufu() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;
        // 芒性状态下：格挡 +2
        decimal finalBlock = DynamicVars.Block.BaseValue;
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            finalBlock += 2;
        }
        // 3. 切换为芒属性
        if (FufuModCode.Character.FufuMod.CurrentAttr != FufuAttr.Mang)
        {
            FufuModCode.Character.FufuMod.CurrentAttr = FufuAttr.Mang;
            await HuangMangOriginSeal.SwitchToHuangAttribute(player,choiceContext);
        }
        await HuangMangOriginSeal.RefreshStateIcon(player,choiceContext);

        // 2. 获得格挡
        await CreatureCmd.GainBlock(
        player, 
        finalBlock, 
        DynamicVars.Block.Props, 
        cardPlay
        );

        
    }

    protected override void OnUpgrade()
    {
        // 升级后：基础格挡 +3（变成7点）
        DynamicVars.Block.UpgradeValueBy(3);
    }
}