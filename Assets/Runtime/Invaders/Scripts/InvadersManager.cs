using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class InvadersManager : MonoBehaviour
{
    [SerializeField] protected int groundYValue;
    [SerializeField] protected UnityEvent onInvadersToutchGound;

    #region CREAT INVADER
    [SerializeField] protected List<Invader> InvadersToCreate = new List<Invader>();
    [SerializeField] protected int collunsToCreate;

    [SerializeField] protected Vector2 startPositionToInstantiate;
    [SerializeField] protected float spaceBettewnInvadersColluns, spaceBettewnInvadersLines;

    [SerializeField] protected int invaderInstantiateDelay;

    protected async Task CreatInvaders()
    {
        lines = new List<InvadersLine>();

        Vector2 positionToInstantiate;
        positionToInstantiate = startPositionToInstantiate;

        for(int i = 0; i < InvadersToCreate.Count; i++)
        {
            InvadersLine lineCreated = new(); // creat a temp line of invaders
            lineCreated.invaders = new List<Invader>();

            for(int a = 0; a < collunsToCreate; a++)
            {
                GameObject @object = Instantiate(InvadersToCreate[i].gameObject, positionToInstantiate, Quaternion.identity, transform);
                lineCreated.invaders.Add(@object.GetComponent<Invader>());

                positionToInstantiate += Vector2.right * spaceBettewnInvadersColluns;
                await Task.Delay(invaderInstantiateDelay);
            }

            lines.Add(lineCreated);
            positionToInstantiate = new(x: startPositionToInstantiate.x, y: positionToInstantiate.y - spaceBettewnInvadersLines);
        }

        _ = MoveInvaders();
    }
    #endregion

    #region MOVE INVADERS
    [SerializeField] protected float invaderLimitDistance;
    [SerializeField] protected float invaderMovementSpeed;
    [SerializeField] protected int movementInvaderDelay;

    protected List<InvadersLine> lines;
    protected Vector2 directionToMove = Vector2.right;

    /// <summary>
    /// Create a asyncronuss task to movement evrey cell of invaders in grid
    /// </summary>
    protected async Task MoveInvaders()
    {
        bool changeMovementDirection = false;

        for(int currentLine = lines.Count - 1; currentLine >= 0; currentLine--)
        {
            for(int currentInvader = directionToMove.x > 0 ? lines[currentLine].invaders.Count - 1 : 0; currentInvader < lines[currentLine].invaders.Count && currentInvader >= 0; currentInvader -= (int)directionToMove.x)
            {
                var invaderSelected = lines[currentLine].invaders[currentInvader];

                if(invaderSelected.Life.LifeStatus == LifeStatus.alive && invaderSelected.gameObject.activeSelf == true)
                {
                    invaderSelected.Movement.Move(directionToMove, invaderMovementSpeed);
                    invaderSelected.AtualizeCurrentFrame();

                    if(invaderSelected.transform.position.x > invaderLimitDistance || invaderSelected.transform.position.x < -invaderLimitDistance)
                    {
                        changeMovementDirection = true;
                    }

                    if(invaderSelected.transform.position.y <= groundYValue)
                    {
                        onInvadersToutchGound.Invoke();
                    }

                    await Task.Delay(movementInvaderDelay);
                }
            }
        }

        directionToMove = Vector2.right * directionToMove.x;

        if(changeMovementDirection)
        {
            directionToMove = new(directionToMove.x * -1, -1);
        }

        _ = MoveInvaders();
    }
    #endregion
}