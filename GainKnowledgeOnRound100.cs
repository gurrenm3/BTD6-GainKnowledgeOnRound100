using MelonLoader;
using BTD_Mod_Helper;
using GainKnowledgeOnRound100;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models.Store.Loot;
using Il2CppAssets.Scripts.Models.Store;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;

[assembly: MelonInfo(typeof(GainKnowledgeOnRound100.GainKnowledgeOnRound100), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace GainKnowledgeOnRound100;

public class GainKnowledgeOnRound100 : BloonsTD6Mod
{
    public ModSettingInt knowledgePointsToGain = new ModSettingInt(1); // mod setting to track reward amount.
    const int roundNumberForReward = 2;  // number is 1 less than the round we want.

    public override void OnApplicationStart()
    {
        ModHelper.Msg<GainKnowledgeOnRound100>("GainKnowledgeOnRound100 loaded!");
    }

    public override void OnRoundEnd()
    {
        base.OnRoundEnd();

        // doesn't work in sandbox mode!
        if (InGame.instance.mapEditorInSandboxMode || InGame.instance.GetSimulation().sandbox)
            return;        

        // don't run if it's not the correct round
        int currentRound = InGame.instance.GetSimulation().GetCurrentRound();
        if (currentRound != roundNumberForReward)
            return;

        // apply knowledge points.
        KnowledgePointsLoot knowledgePointsLoot = new KnowledgePointsLoot(knowledgePointsToGain);
        knowledgePointsLoot.Apply(LootFrom.round100);

        // show popup and debug message.
        string rewardText = $"Congratulations on beating round {roundNumberForReward + 1}! You gained {knowledgePointsToGain.GetValue()} more knowledge point!!";
        ModHelper.Msg<GainKnowledgeOnRound100>(rewardText);
        Game.instance.GetPopupScreen().ShowOkPopup(rewardText);
    }
}