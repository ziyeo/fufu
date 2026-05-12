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
public class HuangOutbreak : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override string PortraitPath => "res://fufu/images/card_portraits/huang_outbreak.png";

    public HuangOutbreak() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null)
            return;
        Player players = player.Player;

        // 沙龙水伤 +3，持续1回合
        var buff = await PowerCmd.Apply<SalonTempDamageBuffPower>(choiceContext,player, 3, player, this);
        if (buff != null)
        {
            buff.Duration = 1;
        }

        // 荒属性：每10元素召唤1沙龙，并扣除对应元素
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            int element = player.GetPowerAmount<ElementEnergyPower>();
            int summonCount = element / 10;

            if (summonCount > 0)
            {
                // 召唤对应数量沙龙
                bool hasSalonBond = player.HasPower<SalonBondPower>();
                int salonBond = player.GetPowerAmount<SalonBondPower>();
                await PowerCmd.Apply<ShadowSalonPower>(choiceContext,player, summonCount, player, this);
                if(hasSalonBond)
                await CardPileCmd.Draw(choiceContext, salonBond, players);
                // 扣除 10 × 数量 的元素能量
                int cost = summonCount * 10;
                bool hasElementMastery = player.HasPower<ElementMasteryPower>();
    
                // 没有掌控 → 正常扣费
                if (!hasElementMastery)
                {
                    await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -cost, player, this);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级耗能 -1：2费 → 1费
        base.EnergyCost.UpgradeBy(-1);
    }
    //1.荒性爆发,沙龙水伤 + 3（1 回合），角色荒属性时每有10元素能量，召唤 1 名沙龙；打出扣 对应的元素能量
}