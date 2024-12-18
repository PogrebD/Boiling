namespace SharpMath.Geometry._2D;

public readonly record struct Rectangle(double LeftX, double BottomY, double Width, double Height)
{
    public bool Contains(Point point)
    {
        return LeftX <= point.X && point.X <= LeftX + Width
            && BottomY <= point.Y && point.Y <= BottomY + Height;  
    }
}