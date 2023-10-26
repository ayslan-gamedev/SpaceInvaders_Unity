using System;
using UnityEngine;

/// <summary>
/// A sampleficated movement system
/// </summary>
public class Movement
{
    /// <summary>
    /// Current Position in vector of gameobject
    /// </summary>
    private Vector2 currentLocalPosition = Vector2.zero;
    
    /// <summary>
    /// Object to movement
    /// </summary>
    public GameObject @object;

    /// <summary>
    /// Movement object by game scene
    /// </summary>
    /// <param name="MovementAtributts">All variables to movement</param>
    public void Move(MovementGemericAtributts MovementAtributts)
    {
        Move(MovementAtributts.direction, MovementAtributts.speed, MovementAtributts.suavizeMovement, MovementAtributts.maxMovementLimit, MovementAtributts.maxMovementLimit);
    }

    /// <summary>
    /// Movement object by game scene
    /// </summary>
    /// <param name="direction"> direction to move </param>
    /// <param name="speed"> speed of movement </param>
    /// <param name="suavizeMovement"> use time.deltatime or not </param>
    public void Move(Vector2 direction, float speed, bool suavizeMovement)
    {
        // get current object position to not have a error on first execute script
        GetObjectCurrentPosition();

        if(suavizeMovement)
        {
            // change position using time.deltatime
            currentLocalPosition += speed * Time.deltaTime * direction;

            // atualize position in game scene
            ChangeObjectPosition();
           
            return;
        }
        
        // case not use suavize movement, use normal movement
        Move(direction, speed);
    }

    /// <summary>
    /// Movement object by game scene
    /// </summary>
    /// <param name="direction"> direction to move </param>
    /// <param name="speed"> speed of movement </param>
    public void Move(Vector2 direction, float speed)
    {
        // get current object position to not have a error on first execute script
        GetObjectCurrentPosition();

        // change position using speed and direction
        currentLocalPosition += speed * direction;
        
        // atualize position in game scene
        ChangeObjectPosition();
    }

    /// <summary>
    /// Movement object by game scene
    /// </summary>
    /// <param name="direction"> direction to move </param>
    /// <param name="speed"> speed of movement </param>
    /// <param name="suavizeMovement"> use time.deltatime or not </param>
    /// <param name="minMovementLimit"> minimus distance to object move </param>
    /// <param name="maxMovementLimit"> maximus distance to object move </param>
    public void Move(Vector2 direction, float speed, bool suavizeMovement, Vector2 minMovementLimit, Vector2 maxMovementLimit)
    {
        // get current object position to not have a error on first execute script
        GetObjectCurrentPosition();

        if(suavizeMovement)
        {
            // creat a temp current local position value to not change position imediatally
            Vector2 _currentLocalPosition = currentLocalPosition;

            _currentLocalPosition += speed * Time.deltaTime * direction;

            // comparate the minimus and maximos values and set the values in current local position
            currentLocalPosition = new()
            {
                x = Math.Clamp(_currentLocalPosition.x, min: minMovementLimit.x, max: maxMovementLimit.x),
                y = Math.Clamp(_currentLocalPosition.y, min: minMovementLimit.y, max: maxMovementLimit.y)
            };

            // atualize position in game scene
            ChangeObjectPosition();

            return;
        }

        Move(direction, speed, minMovementLimit, maxMovementLimit);
    }

    /// <summary>
    /// Movement object by game scene
    /// </summary>
    /// <param name="direction"> direction to move </param>
    /// <param name="speed"> speed of movement </param>
    /// <param name="minMovementLimit"> minimus distance to object move </param>
    /// <param name="maxMovementLimit"> maximus distance to object move </param>
    public void Move(Vector2 direction, float speed, Vector2 minMovementLimit, Vector2 maxMovementLimit)
    {
        // get current object position to not have a error on first execute script
        GetObjectCurrentPosition();

        // creat a temp current local position value to not change position imediatally
        Vector2 _currentLocalPosition = currentLocalPosition;

        _currentLocalPosition += speed * Time.deltaTime * direction;

        // comparate the minimus and maximos values and set the values in current local position
        currentLocalPosition = new()
        {
            x = Math.Clamp(_currentLocalPosition.x, min: minMovementLimit.x, max: maxMovementLimit.x),
            y = Math.Clamp(_currentLocalPosition.y, min: minMovementLimit.y, max: maxMovementLimit.y)
        };

        // atualize position in game scene
        ChangeObjectPosition();
    }

    /// <summary>
    /// Atualize current gameobject position
    /// </summary>
    private void ChangeObjectPosition()
    {
        @object.transform.position = currentLocalPosition;
    }

    /// <summary>
    /// Get current object position
    /// </summary>
    private void GetObjectCurrentPosition()
    {
        // checks if current position value in script is equals of real position
        if(currentLocalPosition == Vector2.one * @object.transform.position)
        {
            return;
        }
        
        // get current position on script
        currentLocalPosition = @object.transform.position;
    }
}