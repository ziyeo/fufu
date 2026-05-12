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
using MegaCrit.Sts2.Core.Entities.Players;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class SingerBlessing : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(4),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/singer_blessing.png";

    public SingerBlessing() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;
        Player players = player.Player;
        // 基础效果：歌者治疗 +3，持续1回合
        var healBuff = await PowerCmd.Apply<SingerTempHealBuffPower>(choiceContext,player, 3, player, this);
        if (healBuff != null)
        {
            healBuff.Duration = 1;
        }
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            await CreatureCmd.Heal(player, DynamicVars.Heal.IntValue);

        }
        // 检查元素能量是否 ≥5
        int element = player.GetPowerAmount<ElementEnergyPower>();
        if (element < 5)
            return;

        // 扣除 5 元素能量
        bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -5, player, this);
        }
        
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, players);
        
    }

    protected override void OnUpgrade()
    {
       DynamicVars.Cards.UpgradeValueBy(1);
    }
}