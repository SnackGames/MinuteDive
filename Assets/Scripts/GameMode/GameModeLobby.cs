using GameMode;

public class GameModeLobby : GameModeBase
{
  #region GameModeBase
  public override GameModeType GetGameModeType() => GameModeType.Lobby;
  public override void StartGameMode()
  {
    base.StartGameMode();

    GetComponent<FloorManager>()?.GenerateFloors();
  }
  public override void FinishGameMode()
  {
    base.FinishGameMode();
  }

  // Update is called once per frame
  override protected void Update()
  {
    base.Update();
  }
  #endregion
}