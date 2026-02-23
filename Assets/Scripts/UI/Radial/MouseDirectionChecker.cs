using UnityEngine;
public static class MouseDirectionChecker
{    
    public enum MouseDirection
    {
        Center,     
        Right,      
        Left,       
        Up,         
        Down        
    }
   
    public static MouseDirection GetMouseDirection(float threshold = 0.3f, float deadZoneRadius = 50f)
    {        
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        Vector2 mousePos = Input.mousePosition;
        
        float distanceFromCenter = Vector2.Distance(mousePos, screenCenter);
        if (distanceFromCenter < deadZoneRadius)
        {
            return MouseDirection.Center;
        }
       
        Vector2 direction = (mousePos - screenCenter).normalized;
        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {           
            if (direction.x > threshold)
                return MouseDirection.Right;
            else if (direction.x < -threshold)
                return MouseDirection.Left;
        }
        else
        {            
            if (direction.y > threshold)
                return MouseDirection.Up;
            else if (direction.y < -threshold)
                return MouseDirection.Down;
        }
       
        return MouseDirection.Center;
    }
   
    public static MouseDirection GetMouseDirection(Vector2 customCenter, float threshold = 0.3f, float deadZoneRadius = 50f)
    {
        Vector2 mousePos = Input.mousePosition;

        float distanceFromCenter = Vector2.Distance(mousePos, customCenter);
        if (distanceFromCenter < deadZoneRadius)
        {
            return MouseDirection.Center;
        }

        Vector2 direction = (mousePos - customCenter).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > threshold)
                return MouseDirection.Right;
            else if (direction.x < -threshold)
                return MouseDirection.Left;
        }
        else
        {
            if (direction.y > threshold)
                return MouseDirection.Up;
            else if (direction.y < -threshold)
                return MouseDirection.Down;
        }

        return MouseDirection.Center;
    }

    public static string GetMouseDirectionString(float threshold = 0.3f, float deadZoneRadius = 50f)
    {
        return GetMouseDirection(threshold, deadZoneRadius).ToString();
    }

    public static bool IsMouseInDirection(MouseDirection direction, float threshold = 0.3f, float deadZoneRadius = 50f)
    {
        return GetMouseDirection(threshold, deadZoneRadius) == direction;
    }

    public static Vector2 GetMouseDirectionVector()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mousePos = Input.mousePosition;
        return (mousePos - screenCenter).normalized;
    }
       
    public static Vector2 GetRawMouseDirection()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
       
        Vector2 mousePos = Input.mousePosition;

        return mousePos - screenCenter;
    }

    public static float GetDistanceFromCenter()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        return Vector2.Distance(Input.mousePosition, screenCenter);
    }

    public static float GetMouseAngle()
    {
        Vector2 direction = GetRawMouseDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;
        return angle;
    }

    public static bool IsInDeadZone(float deadZoneRadius = 50f)
    {
        return GetDistanceFromCenter() < deadZoneRadius;
    }

    public static float GetNormalizedDistance()
    {
        float distance = GetDistanceFromCenter();
        float maxDistance = Mathf.Sqrt(
            Mathf.Pow(Screen.width / 2f, 2) +
            Mathf.Pow(Screen.height / 2f, 2)
        );
        return Mathf.Clamp01(distance / maxDistance);
    }
}