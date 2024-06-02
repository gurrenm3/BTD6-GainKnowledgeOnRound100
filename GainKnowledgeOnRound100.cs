using MelonLoader;
using BTD_Mod_Helper;
using GainKnowledgeOnRound100;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models.Store.Loot;
using Il2CppAssets.Scripts.Models.Store;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using System.Linq;

[assembly: MelonInfo(typeof(GainKnowledgeOnRound100.GainKnowledgeOnRound100), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace GainKnowledgeOnRound100;

/// <summary>
/// [Overview of the mod]
/// This mod gives the player extra Monkey Knowledge on certain rounds.
/// By default you will gain the reward on every 25 rounds from round 100 to 300.
/// The purpose of the mod is intended to make late game more worthwhile and it does not work in Sandbox.
/// 
/// [Summary of the code]
/// Whenever the player joins a game, the mod checks if it's sandbox or not and stores it for later to avoid checking more than once.
/// Then, each time the player beats a round the mod checks the previously stored value to see if they are in sandbox mode or not.
/// If they are then it will stop running code and not give them any rewards.
/// Otherwise it will check what round they just beat. They will earn knowledge points if the round they just
/// completed is one they're supposed to earn extra knowledge on. If it is not then they earn nothing, otherwise they will be
/// awarded the points and an in-game popup will notify them that they extra knowledge points.
/// </summary>
public class GainKnowledgeOnRound100 : BloonsTD6Mod
{
    /// <summary>
    /// The mod setting that controls how many knowledge points the player earns by beating one of the reward rounds.
    /// </summary>
    public ModSettingInt knowledgePointsToGain = new ModSettingInt(1)
    {
        min = 1,
        max = 5,
        slider = true
    };

    /// <summary>
    /// The round numbers that the player earns knowledge points from.
    /// <br/>Must be 1 less than the desired round number.
    /// </summary>
    public readonly int[] roundNumbersToEarnReward = new[]
    {
        99,
        124,
        149,
        174,
        199,
        224,
        249,
        274,
        299
    };

    private bool isSandboxMode;

    /// <summary>
    /// Runs code when the game is starting. 
    /// <br/>Tells the user that the mod has loaded.
    /// </summary>
    public override void OnApplicationStart()
    {
        ModHelper.Msg<GainKnowledgeOnRound100>("GainKnowledgeOnRound100 loaded!");
    }

    /// <summary>
    /// Runs code when the match is first joined.
    /// <br/>Caches whether or not the player is in sandbox mode at the beginning of the game to avoid checking it repeatedly.
    /// </summary>
    public override void OnMatchStart()
    {
        isSandboxMode = IsSandboxMode();
    }

    /// <summary>
    /// Runs code at the end of each round. 
    /// <br/>Will award knowledge points to the player if the round that just ended is one of the reward rounds.
    /// </summary>
    public override void OnRoundEnd()
    {
        TryAddKnowledgePoints();
    }

    /// <summary>
    /// Attempts to add the knowledge points if one of the reward rounds was beaten and you are not in sandbox mode.
    /// </summary>
    public void TryAddKnowledgePoints()
    {
        // doesn't work in sandbox mode!
        if (isSandboxMode)
            return;

        int currentRound = InGame.instance.GetSimulation().GetCurrentRound();

        // don't run if it's not the correct round
        if (!CanEarnRewardThisRound(currentRound))
            return;

        // apply knowledge points.
        int amountToGain = knowledgePointsToGain;
        GiveKnowledgePoints(amountToGain);

        // show popup and debug message.
        NotifyKnowledgePointsGained(amountToGain, currentRound);
    }

    /// <summary>
    /// Returns whether or not the game is in sandbox mode.
    /// </summary>
    /// <returns></returns>
    private bool IsSandboxMode()
    {
        return InGame.instance.mapEditorInSandboxMode || InGame.instance.GetSimulation().sandbox;
    }

    /// <summary>
    /// Returns whether or not the reward can be earned on the current round.
    /// </summary>
    /// <returns></returns>
    private bool CanEarnRewardThisRound(int currentRound)
    {
        bool isDesiredRound = roundNumbersToEarnReward.Contains(currentRound);

        // can only earn rewards if the player beat the desired round.
        return isDesiredRound;
    }

    /// <summary>
    /// Gives knowledge points to the player.
    /// </summary>
    /// <param name="amount">Number of knowledge points to give to the player.</param>
    private void GiveKnowledgePoints(int amount)
    {
        KnowledgePointsLoot knowledgePointsLoot = new KnowledgePointsLoot(amount);
        knowledgePointsLoot.Apply(LootFrom.round100);
    }

    /// <summary>
    /// Notifies the player that they have gained knowledge points.
    /// </summary>
    /// <param name="amount">The number of knowledge points that the user gained.</param>
    private void NotifyKnowledgePointsGained(int amount, int currentRoundNumber)
    {
        string rewardText = $"Congratulations on beating round {currentRoundNumber + 1}! You gained {amount} more knowledge point(s)!!";
        ModHelper.Msg<GainKnowledgeOnRound100>(rewardText);
        Game.instance.GetPopupScreen().ShowOkPopup(rewardText);
    }
}