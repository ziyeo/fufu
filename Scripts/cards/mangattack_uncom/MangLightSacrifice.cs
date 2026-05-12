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
public class MangLightSacrifice : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, ValueProp.Move)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Exhaust
    };


    public override string PortraitPath => "res://fufu/images/card_portraits/mang_light_sacrifice.png";

    public MangLightSacrifice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        if (cardPlay.Target == null) return;

        // 造成10点普通伤害
        await DamageCmd.Attack(DynamicVars.Damage.IntValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性判定
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            int element = player.GetPowerAmount<ElementEnergyPower>();
            // 需要至少10元素能量
            if (element >= 10)
            {
                // 扣除10元素
                await PowerCmd.Apply<ElementEnergyPower>(choiceContext,player, -10, player, this);

                // 每1名歌者回2血
                int singerCount = player.GetPowerAmount<BasicSingerPower>();
                int heal = singerCount * 2;

                if (heal > 0)
                {
                    await CreatureCmd.Heal(player, heal);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
    //5.芒光献祭（消耗牌）：耗 2 点卡牌能量，造 10 普通伤害 ；角色芒属性时，需要10元素能量，打出扣10元素能量，每有1歌者，回2血；升级+3伤害
}