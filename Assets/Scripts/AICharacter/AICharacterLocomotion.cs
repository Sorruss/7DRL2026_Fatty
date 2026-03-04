using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace FG
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        private AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        private Stack<Vector3> movementSteps = new Stack<Vector3>();
        private Vector3 playerReferencePosition;
        private Coroutine moveEnemyRoutine;
        private float currentEnemyPathRebuildCooldown;
        private WaitForFixedUpdate waitForFixedUpdate;
        [HideInInspector] public float moveSpeed;
        private bool chasePlayer = false;

        private void Start()
        {
            // Create waitforfixed update for use in coroutine
            waitForFixedUpdate = new WaitForFixedUpdate();

            // Reset player reference position
            playerReferencePosition = GameManager.instance.player.transform.position;
        }

        protected override void Update()
        {
            base.Update();

            if (playerReferencePosition == null)
                return;

            HandleAimingDirection();
            MoveEnemy();
        }

        /// <summary>
        /// Use AStar pathfinding to build a path to the player - and then move the enemy to each grid location on the path
        /// </summary>
        private void MoveEnemy()
        {
            Vector3 playerPos = GameManager.instance.player.transform.position;

            // Movement cooldown timer
            currentEnemyPathRebuildCooldown -= Time.deltaTime;

            // Check distance to player to see if enemy should start chasing
            if (!chasePlayer && Vector3.Distance(transform.position, playerPos) > 3.0f)
            {
                chasePlayer = true;
            }

            // If not close enough to chase player then return
            if (!chasePlayer)
                return;

            // if the movement cooldown timer reached or player has moved more than required distance
            // then rebuild the enemy path and move the enemy
            if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, playerPos) > 4.0f))
            {
                // Reset path rebuild cooldown timer
                currentEnemyPathRebuildCooldown = 3.0f;

                // Reset player reference position
                playerReferencePosition = playerPos;

                // Move the enemy using AStar pathfinding - Trigger rebuild of path to player
                CreatePath();

                // If a path has been found move the enemy
                if (movementSteps != null)
                {
                    if (moveEnemyRoutine != null)
                    {
                        // Trigger idle event
                        //aiCharacter.idleEvent.CallIdleEvent();
                        StopCoroutine(moveEnemyRoutine);
                    }

                    // Move enemy along the path using a coroutine
                    moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
                }
            }
        }

        /// <summary>
        /// Coroutine to move the enemy to the next location on the path
        /// </summary>
        private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
        {
            SetIsMoving(true);
            while (movementSteps.Count > 0)
            {
                Vector3 nextPosition = movementSteps.Pop();

                // while not very close continue to move - when close move onto the next step
                while (Vector3.Distance(nextPosition, transform.position) > 0.8f)
                {
                    // Trigger movement event
                    Vector2 targetPosition = (nextPosition - transform.position).normalized * Time.fixedDeltaTime * speed;
                    aiCharacter.ridigBody.MovePosition(aiCharacter.ridigBody.position + targetPosition);
                    yield return waitForFixedUpdate;  // moving the enmy using 2D physics so wait until the next fixed update
                }

                aiCharacter.ridigBody.linearVelocity = Vector2.zero;
                yield return waitForFixedUpdate;
            }

            // End of path steps - trigger the enemy idle event
            //enemy.idleEvent.CallIdleEvent();
            SetIsMoving(false);
            yield return null;
        }

        /// <summary>
        /// Use the AStar static class to create a path for the enemy
        /// </summary>
        private void CreatePath()
        {
            Room currentRoom = GameManager.instance.currentRoom;

            Grid grid = currentRoom.instantiatedRoom.roomGrid;

            // Get players position on the grid
            Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

            // Get enemy position on the grid
            Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

            // Build a path for the enemy to move on
            movementSteps = AStarPathfinding.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

            // Take off first step on path - this is the grid square the enemy is already on
            if (movementSteps != null)
            {
                movementSteps.Pop();
            }
            else
            {
                // Trigger idle event - no path
                //enemy.idleEvent.CallIdleEvent();
            }
        }

        /// <summary>
        /// Get the nearest position to the player that isn't on an obstacle
        /// </summary>
        private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
        {
            Vector3 playerPosition = GameManager.instance.player.transform.position;
            Vector3Int playerCellPosition = currentRoom.instantiatedRoom.roomGrid.WorldToCell(playerPosition);
            Vector2Int adjustedPlayerCellPositon = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

            int obstacle = currentRoom.instantiatedRoom.movementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y];

            // if the player isn't on a cell square marked as an obstacle then return that position
            if (obstacle != 0)
            {
                return playerCellPosition;
            }
            // find a surounding cell that isn't an obstacle - required because with the 'half collision' tiles
            // and tables the player can be on a grid square that is marked as an obstacle
            else
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        try
                        {
                            obstacle = currentRoom.instantiatedRoom.movementPenalty[adjustedPlayerCellPositon.x + i, adjustedPlayerCellPositon.y + j];
                            if (obstacle != 0)
                                return new Vector3Int(playerCellPosition.x + i, playerCellPosition.y + j, 0);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return playerCellPosition;
        }

        protected override void HandleAimingDirection()
        {
            Vector2 aimVector = (Vector2)(playerReferencePosition - aiCharacter.transform.position);
            float degrees = Helpers.GetVectorAngle(aimVector);
            aimDirection = Helpers.Degrees2AimDirection(degrees);
        }
    }
}
