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
public class ProtectOrigin : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(15, ValueProp.Move),
        new HealVar(6),
        new EnergyVar(2),
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/protect_origin.png";

    public ProtectOrigin() : base(energyCost, type, rarity, targetType, shouldShowInLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;
        Player players = player.Player;

        // 格挡：基础15，芒属性+3
        int block = DynamicVars.Block.IntValue;
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
            block += 3;

        await CreatureCmd.GainBlock(player, block, DynamicVars.Block.Props, cardPlay);
        // 恢复6血
        await CreatureCmd.Heal(player, DynamicVars.Heal.IntValue);
        int atmos = player.GetPowerAmount<AtmospherePower>();
        int elem = player.GetPowerAmount<ElementEnergyPower>();

        // 条件：气氛≥8 且 元素≥15
        if (atmos < 8 || elem < 15)
            return;

        // 扣除资源
        await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -8, player, this);
        bool hasElementMastery = player.HasPower<ElementMasteryPower>(); 
        // 没有掌控 → 正常扣费
        if (!hasElementMastery)
        {
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -15, player, this);
        }
        // 获得2点能量
       await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, players);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5);
        DynamicVars.Heal.UpgradeValueBy(2);
    }

    //守护本源：耗3费，获15格挡（芒属性+3），恢复6血；需气氛≥8+元素≥15，打出扣8气氛+15元素，获2能量,升级+5格挡，+2回血
}