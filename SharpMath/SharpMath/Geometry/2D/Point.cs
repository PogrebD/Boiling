namespace SharpMath.Geometry._2D;

public record struct Point(double X, double Y)
{
    public double Norm => Math.Sqrt(X * X + Y * Y);

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }

    public static Point operator /(Point p, double value)
    {
        return new Point(p.X / value, p.Y / value);
    }
    
    public static Point operator *(Point p, double value)
    {
        return new Point(p.X * value, p.Y * value);
    }
    
    public Point Normalize()
    {
        return this / Norm;
    }
};
