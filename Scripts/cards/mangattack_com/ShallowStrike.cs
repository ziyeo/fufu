using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.CardPools;
using Fufu.FufuModCode.Character.CardPool;
using Fufu.FufuModCode.Character;
using Fufu.Scripts.Power;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Cards;

[Pool(typeof(FufuCardPool))]
public class ShallowStrike : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType target = TargetType.AnyEnemy;
    private const bool showInLibrary = true;

    // 基础伤害 5 + 抽牌变量
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new CardsVar(1)
    ];

    public override string PortraitPath => "res://fufu/images/card_portraits/ShallowStrike.png";

    public ShallowStrike() : base(energyCost, type, rarity, target, showInLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.OnPlay(choiceContext, cardPlay);

        Creature player = base.Owner.Creature;
        if (!player.IsPlayer || player.Player == null) return;
        Player realPlayer = player.Player;
        
        if (cardPlay.Target == null) return;

        // 造成基础伤害
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        // 芒属性效果：抽牌 + 回血
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            // 抽牌（升级后 2 张）
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, realPlayer);
            
            // 恢复 3 点生命
            await CreatureCmd.Heal(player, 3m);
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：抽牌数 +1（1→2）
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}