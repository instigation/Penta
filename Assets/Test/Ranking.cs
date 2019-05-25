using GooglePlayGames;
using UnityEngine;


public class Ranking : MonoBehaviour
{
    string leaderBoardId = "CgkIk8ykmvcBEAIQAQ";

    private void Start()
    {
        PlayGamesPlatform.Activate();
    }
    public void RankButtonClick()
    {

        Social.localUser.Authenticate(AuthenticateHandler);
        //Social.ShowLeaderboardUI();
    }

    void AuthenticateHandler(bool isSuccess)
    {
        if (isSuccess)
        {
            float bestScore = GlobalInformation.getInt("bestScore");
            Social.ReportScore((long)bestScore, leaderBoardId, (bool success) =>
             {
                 if (success)
                 {
                     PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderBoardId);
                 }
                 else
                 {
                    // return;
                 }
             });
            
        }
        else
        {
           // return;
        }
    }
}
