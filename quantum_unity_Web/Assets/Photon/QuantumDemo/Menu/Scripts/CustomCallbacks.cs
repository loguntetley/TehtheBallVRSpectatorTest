using Quantum;
using UnityEngine;
using UnityEngine.UI;

public class CustomCallbacks : QuantumCallbacks 
{
    public RuntimePlayer runtimePlayer;
    public RuntimePlayer runtimePlayer2;
    public Toggle toggle;

    public override void OnGameStart(Quantum.QuantumGame game) 
    {
        // paused on Start means waiting for Snapshot
        if (game.Session.IsPaused) return;

        foreach (var lp in game.GetLocalPlayers()) {
          Debug.Log("CustomCallbacks - sending player: " + lp);
            if (toggle.isOn)
            {
                game.SendPlayerData(lp, runtimePlayer2);
            }
            else 
            {
                game.SendPlayerData(lp, runtimePlayer);
            }
          
        }
        toggle.gameObject.SetActive(false);
    }

    public override void OnGameResync(Quantum.QuantumGame game)
    {
      Debug.Log("Detected Resync. Verified tick: " + game.Frames.Verified.Number);
    }
}

