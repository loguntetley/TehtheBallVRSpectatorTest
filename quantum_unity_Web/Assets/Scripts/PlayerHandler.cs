using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using Cinemachine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] EntityView entityView;

    public void OnEntityInstatiated()
    {
        Debug.Log("Player Character controller OnEntityInstatiated");

        QuantumGame game = QuantumRunner.Default.Game;

        Frame frame = game.Frames.Verified;

        if (frame.TryGet(entityView.EntityRef, out PlayerLink playerLink))
        {
            //check player is local
            if (game.PlayerIsLocal(playerLink.Player))
            {
                CinemachineVirtualCamera virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
                virtualCamera.m_Follow = transform;
            }
        }
    }   
}
