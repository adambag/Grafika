using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

[Serializable]
public class MoveShape
{
    // typ figury 
    public string ShapeType { get; set; }

    // współrzędne
    public double X { get; set; }
    public double Y { get; set; }

    // wymiary 
    public double Width { get; set; }
    public double Height { get; set; }

    // obiekt Shape jest ignorowany bo to element graficzny
    [NonSerialized]
    public Shape Shape;

    // używane do przesuwania figury
    public Point Offset { get; set; }

    // konstruktor wymagany do deserializacji
    public MoveShape() { }

    // konstruktor dla linii
    public MoveShape(System.Windows.Shapes.Line line)
    {
        ShapeType = "Line";
        X = line.X1;
        Y = line.Y1;
        Width = line.X2;
        Height = line.Y2;
        Shape = line;
    }

    // konstruktor dla prostokąta
    public MoveShape(System.Windows.Shapes.Rectangle rectangle)
    {
        ShapeType = "Rectangle"; 
        X = Canvas.GetLeft(rectangle);
        Y = Canvas.GetTop(rectangle);
        Width = rectangle.Width;
        Height = rectangle.Height;
        Shape = rectangle; 
    }

    // konstruktor dla okręgu
    public MoveShape(Ellipse ellipse)
    {
        ShapeType = "Circle";
        X = Canvas.GetLeft(ellipse);
        Y = Canvas.GetTop(ellipse);
        Width = ellipse.Width;
        Height = ellipse.Height;
        Shape = ellipse;
    }

    //przywracania figury
    public Shape RestoreShape()
    {
        if (ShapeType == "Line")
        {
            var line = new System.Windows.Shapes.Line
            {
                X1 = X,  
                Y1 = Y,  
                X2 = Width,  
                Y2 = Height, 
                Stroke = Brushes.Black, 
                StrokeThickness = 2
            };
            return line;
        }
        else if (ShapeType == "Rectangle")
        {
            var rect = new System.Windows.Shapes.Rectangle
            {
                Width = Width,  
                Height = Height, 
                Stroke = Brushes.Black, 
                StrokeThickness = 2 
            };
            Canvas.SetLeft(rect, X); 
            Canvas.SetTop(rect, Y);  
            return rect;
        }
        else if (ShapeType == "Circle")
        {
            var ellipse = new Ellipse
            {
                Width = Width, 
                Height = Height, 
                Stroke = Brushes.Black, 
                StrokeThickness = 2
            };
            Canvas.SetLeft(ellipse, X); 
            Canvas.SetTop(ellipse, Y);  
            return ellipse;
        }
        return null; 
    }

    // aktualna pozycja  
    public Point GetPosition()
    {
        return new Point(Canvas.GetLeft(Shape), Canvas.GetTop(Shape));
    }

    // nowa pozycja
    public void SetPosition(double x, double y)
    {
        Canvas.SetLeft(Shape, x);
        Canvas.SetTop(Shape, y); 
    }
}
