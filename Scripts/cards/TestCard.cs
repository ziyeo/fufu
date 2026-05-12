// using BaseLib.Abstracts;
// using BaseLib.Utils;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using MegaCrit.Sts2.Core.Models.CardPools;
// using Fufu.FufuModCode.Character.CardPool;
// using MegaCrit.Sts2.Core.ValueProps;

// namespace Fufu.Scripts.Cards;

// [Pool(typeof(FufuCardPool))]
// public class TestCard : CustomCardModel
// {
//     private const int energyCost = 1;
//     private const CardType type = CardType.Attack;
//     private const CardRarity rarity = CardRarity.Common;
//     private const TargetType targetType = TargetType.AnyEnemy;
//     private const bool shouldShowInCardLibrary = true;

//     protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move)];

//     public override string PortraitPath => "res://fufu/images/cards/test-test_card.png";

//     public TestCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
//     {
//     }

//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         if (cardPlay.Target is not null)
//         {
//             await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
//                 .FromCard(this)
//                 .Targeting(cardPlay.Target)
//                 .Execute(choiceContext);
//         }
//     }

//     protected override void OnUpgrade()
//     {
//         DynamicVars.Damage.UpgradeValueBy(4);
//     }
// }


// 加入哪个卡池
// [Pool(typeof(VergilCardPool))]
// public class Rapioslash : CustomCardModel
// {
//     // 基础耗能
//     private const int energyCost = 2;
//     // 卡牌类型
//     private const CardType type = CardType.Attack;
//     // 卡牌稀有度
//     private const CardRarity rarity = CardRarity.Common;
//     // 目标类型（AnyEnemy表示任意敌人）
//     private const TargetType targetType = TargetType.AllEnemies;
//     // 是否在卡牌图鉴中显示
//     private const bool shouldShowInCardLibrary = true;

//     // 卡牌的基础属性（例如这里是12点伤害）
//     protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move)];

//     public override string PortraitPath => $"res://weijier/images/cards/{Id.Entry.ToLowerInvariant()}.png";
//     protected override bool ShouldGlowGoldInternal => IsPlayable;
//     protected override bool IsPlayable => CardPile.GetCards(Owner, PileType.Exhaust).Any((CardModel c) => c is SwitchYamato);
//     public Rapioslash() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
//     {
//     }

//     // 打出时的效果逻辑
//     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//     {
//         await DamageCmd.Attack(DynamicVars.Damage.BaseValue) // 造成伤害，数值来源于卡牌的基础伤害属性
//             .FromCard(this) // 伤害来源于这张卡牌
//             .TargetingAllOpponents(base.CombatState!) 
//             .Execute(choiceContext);
//     }

//     // 升级后的效果逻辑
//     protected override void OnUpgrade()
//     {
//         DynamicVars.Damage.UpgradeValueBy(4); // 升级后增加3点伤害
//     }
//     public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
//     {
//     // 只对自己玩家、只对技能牌生效
//     if (cardPlay.Card.Owner != base.Owner || cardPlay.Card.Type != CardType.Attack||!CardPile.GetCards(base.Owner, PileType.Exhaust).Any((CardModel c) => c is SwitchYamato)||base.Pile?.Type != PileType.Hand)
//         return Task.CompletedTask;

//     // 每打1张攻击，费用-1
//     ReduceCostBy(1);
//     return Task.CompletedTask;
//     }
//     private void ReduceCostBy(int amount)
// {
//     // 本回合临时减费（回合结束重置）
//     base.EnergyCost.AddThisTurnOrUntilPlayed(-amount);
// }
// }