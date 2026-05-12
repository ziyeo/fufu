using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.Scripts.Power;
using Fufu.FufuModCode.Character;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public sealed class EnergyConvert : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    public override string PortraitPath => "res://fufu/images/card_portraits/energy_convert.png";

    public EnergyConvert() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) 
            return;

        // 获取当前元素能量
        int currentElement = player.GetPowerAmount<ElementEnergyPower>();

        // 元素能量翻倍
        if (currentElement > 0)
        {
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, currentElement, player, this);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：费用变为 0
        EnergyCost.UpgradeBy(-1);
    }

    
        // "你的元素能量翻倍。\n" +
        // "升级：能量消耗变为0。";
}