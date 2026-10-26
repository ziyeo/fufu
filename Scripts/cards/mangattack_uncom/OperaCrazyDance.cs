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
public class OperaCrazyDance : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/opera_crazy_dance.png";

    public OperaCrazyDance() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        if (cardPlay.Target == null) return;

        // 造成5点普通伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性：检查气氛 ≥5
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int currentAtmo = player.GetPowerAmount<AtmospherePower>();
            if (currentAtmo >= 5)
            {
                // 扣除5气氛
                await PowerCmd.Apply<AtmospherePower>(choiceContext,player, -5, player, this);

                await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, player.Player);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
    //2.歌剧狂舞：耗 1 点卡牌能量，造 5 普通伤害；角色芒属性时需气氛≥5，打出扣 5 气氛， 抽 1 牌。升级抽2牌
}