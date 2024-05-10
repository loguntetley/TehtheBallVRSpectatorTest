using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>, ISignalOnPlayerDataSet, ISignalOnPlayerDisconnected
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* CharacterController;
            public Transform3D* Transform;
            public PlayerLink* Link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var input = f.GetPlayerInput(filter.Link->Player);

            var inputVector = new FPVector2((FP)input->DirectionX/10, (FP)input->DirectionY/10);

            //Anti cheat
            if (inputVector.SqrMagnitude > 1)
                inputVector = inputVector.Normalized;

            //Move
            filter.CharacterController->Move(f, filter.Entity, inputVector.XOY);

            //Jump
            if (input->Jump.WasPressed)
                filter.CharacterController->Jump(f);

            //Look in the movement direction
            if (inputVector.SqrMagnitude != default)
            {
                filter.Transform->Rotation = FPQuaternion.Lerp(filter.Transform->Rotation, FPQuaternion.LookRotation(inputVector.XOY), f.DeltaTime * 10);
            }

            //Player respawn if too far
            if (filter.Transform->Position.Y < -7)
            {
                Log.Debug("Quantum: Player went too far, respawning player");
                filter.Transform->Position = GetSpawnPosition(filter.Link->Player, f.PlayerCount);
            }
        }

        public void OnPlayerDataSet(Frame f, PlayerRef player) 
        {
            var data = f.GetPlayerData(player);

            var prototypeEntity = f.FindAsset<EntityPrototype>(data.characterPrototype.Id);
            var createEntity = f.Create(prototypeEntity);

            if (f.Unsafe.TryGetPointer<PlayerLink>(createEntity, out var playerLink))
            {
                playerLink->Player = player;
            }

            if (f.Unsafe.TryGetPointer<Transform3D>(createEntity, out var transform))
            {
                transform->Position = GetSpawnPosition(player, f.PlayerCount);
            }
        }

        private FPVector3 GetSpawnPosition(int playerNumber, int playerCount)
        {
            int widthOfAllPlayers = playerCount * 2;
            return new FPVector3((playerNumber + 2) + 1 - widthOfAllPlayers/2, 0 , 0);
        }

        void ISignalOnPlayerDisconnected.OnPlayerDisconnected(Frame f, PlayerRef player)
        {
            //Find the player entity that left and destroy it
            foreach (var playerLink in f.GetComponentIterator<PlayerLink>())
            {
                if (playerLink.Component.Player != player)
                    continue;

                Log.Debug("Quantum: OnPlayerDisconnected player destroyed");

                f.Destroy(playerLink.Entity);
            }
        }
    }
}
