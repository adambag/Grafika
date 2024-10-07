using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CustomRectangle = Grafika.Tools.Rectangle;
using CustomCircle = Grafika.Tools.Circle;
using System.Collections.Generic;
using System.Windows.Controls;
using Grafika.Tools;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;

namespace Grafika
{
    public partial class MainWindow : Window
    {
        private enum Tool
        {
            Cursor,
            Line,
            Rectangle,
            Circle
        }

        private Tool selectedTool = Tool.Cursor;
        private bool isDrawing = false;
        private Point startPoint;
        private System.Windows.Shapes.Line currentLine;
        private System.Windows.Shapes.Rectangle currentWpfRectangle;
        private Ellipse currentWpfCircle;
        private CustomRectangle customRectangle;
        private CustomCircle customCircle;
        private List<MoveShape> shapes = new List<MoveShape>();
        private MoveShape selectedShape = null;
        private bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Cursor_Checked(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Cursor;
        }
        private void Line_Checked(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Line;
        }
        private void Rectangle_Checked(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Rectangle;
        }
        private void Circle_Checked(object sender, RoutedEventArgs e)
        {
            selectedTool = Tool.Circle;
        }
        private void DrawShape_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(XStart.Text, out double x1) &&
                double.TryParse(YStart.Text, out double y1) &&
                double.TryParse(XEnd.Text, out double x2) &&
                double.TryParse(YEnd.Text, out double y2))
            {
                if (selectedTool == Tool.Line) //linia
                {
                    System.Windows.Shapes.Line line = new System.Windows.Shapes.Line 
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        X1 = x1,
                        Y1 = y1,
                        X2 = x2,
                        Y2 = y2
                    };
                    DrawingArea.Children.Add(line);
                }
                else if (selectedTool == Tool.Rectangle) //prostokąt
                {
                    var width = Math.Abs(x2 - x1);
                    var height = Math.Abs(y2 - y1);

                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Width = width,
                        Height = height
                    };

                    Canvas.SetLeft(rectangle, Math.Min(x1, x2));
                    Canvas.SetTop(rectangle, Math.Min(y1, y2));
                    DrawingArea.Children.Add(rectangle);
                }
                else if (selectedTool == Tool.Circle) //okrąg
                {
                    var radiusX = Math.Abs(x2 - x1);
                    var radiusY = Math.Abs(y2 - y1);
                    var radius = Math.Max(radiusX, radiusY);

                    Ellipse ellipse = new Ellipse
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Width = radius * 2,
                        Height = radius * 2
                    };

                    Canvas.SetLeft(ellipse, x1 - radius);
                    Canvas.SetTop(ellipse, y1 - radius);
                    DrawingArea.Children.Add(ellipse);
                }
            }
            else
            {
                MessageBox.Show("Proszę wprowadzić poprawne współrzędne.");
            }
        }

        private void DrawingArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DrawingArea);

            if (selectedTool == Tool.Line)
            {
                isDrawing = true;
                currentLine = new System.Windows.Shapes.Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = startPoint.X,
                    Y1 = startPoint.Y,
                    X2 = startPoint.X,
                    Y2 = startPoint.Y
                };
                DrawingArea.Children.Add(currentLine);
            }
            else if (selectedTool == Tool.Rectangle)
            {
                isDrawing = true;
                customRectangle = new CustomRectangle(startPoint.X, startPoint.Y, 0, 0);
                currentWpfRectangle = new System.Windows.Shapes.Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };
                Canvas.SetLeft(currentWpfRectangle, customRectangle.X);
                Canvas.SetTop(currentWpfRectangle, customRectangle.Y);
                DrawingArea.Children.Add(currentWpfRectangle);
            }
            else if (selectedTool == Tool.Circle)
            {
                isDrawing = true;
                customCircle = new CustomCircle(startPoint.X, startPoint.Y, 0);
                currentWpfCircle = new Ellipse
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };
                Canvas.SetLeft(currentWpfCircle, customCircle.CenterX);
                Canvas.SetTop(currentWpfCircle, customCircle.CenterY);
                DrawingArea.Children.Add(currentWpfCircle);
            }
            else if (selectedTool == Tool.Cursor)
            {
                //sprawdza czy figura jest pod kursorem 
                foreach (var shape in shapes)
                {
                    if (shape.Shape.IsMouseOver)
                    {
                        selectedShape = shape;
                        var position = selectedShape.GetPosition();
                        selectedShape.Offset = new Point(startPoint.X - position.X, startPoint.Y - position.Y);
                        isDragging = true;
                        break;
                    }
                }
            }
        }
        //aktualizacja 
        private void DrawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.GetPosition(DrawingArea);

            if (isDrawing)
            {
                if (selectedTool == Tool.Line && currentLine != null) //linia
                {
                    currentLine.X2 = currentPosition.X;
                    currentLine.Y2 = currentPosition.Y;
                }
                else if (selectedTool == Tool.Rectangle && currentWpfRectangle != null) //prostokąt
                {
                    double width = Math.Abs(currentPosition.X - startPoint.X);
                    double height = Math.Abs(currentPosition.Y - startPoint.Y);

                    customRectangle.Width = width;
                    customRectangle.Height = height;

                    currentWpfRectangle.Width = width;
                    currentWpfRectangle.Height = height;

                    Canvas.SetLeft(currentWpfRectangle, Math.Min(currentPosition.X, startPoint.X));
                    Canvas.SetTop(currentWpfRectangle, Math.Min(currentPosition.Y, startPoint.Y));
                }
                else if (selectedTool == Tool.Circle && currentWpfCircle != null) //okrąg
                {
                    double radiusX = Math.Abs(currentPosition.X - startPoint.X);
                    double radiusY = Math.Abs(currentPosition.Y - startPoint.Y);

                    customCircle.Radius = Math.Max(radiusX, radiusY);

                    currentWpfCircle.Width = customCircle.Radius * 2;
                    currentWpfCircle.Height = customCircle.Radius * 2;

                    Canvas.SetLeft(currentWpfCircle, Math.Min(currentPosition.X, startPoint.X));
                    Canvas.SetTop(currentWpfCircle, Math.Min(currentPosition.Y, startPoint.Y));
                }
            }
            //przesunięcie obiektu 
            else if (isDragging && selectedShape != null)
            {
                double newX = currentPosition.X - selectedShape.Offset.X;
                double newY = currentPosition.Y - selectedShape.Offset.Y;
                selectedShape.SetPosition(newX, newY);
            }
        }

        private void DrawingArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                if (selectedTool == Tool.Line) // zakończenie linii
                {
                    shapes.Add(new MoveShape(currentLine));
                    currentLine = null;
                }
                else if (selectedTool == Tool.Rectangle) //zakończenie protokąta
                {
                    shapes.Add(new MoveShape(currentWpfRectangle));
                    currentWpfRectangle = null;
                }
                else if (selectedTool == Tool.Circle) //zakończenie okręgu
                {
                    shapes.Add(new MoveShape(currentWpfCircle));
                    currentWpfCircle = null;
                }
            }

            if (isDragging)
            {
                isDragging = false;
                selectedShape = null;
            }
        }
        //zapis do pliku json
        private void SaveShapesToFile(string fileName)
        {
            var shapesToSave = shapes.Select(s =>
            {
                if (s.Shape is System.Windows.Shapes.Line line)
                    return new MoveShape(line);
                else if (s.Shape is System.Windows.Shapes.Rectangle rectangle)
                    return new MoveShape(rectangle);
                else if (s.Shape is Ellipse ellipse)
                    return new MoveShape(ellipse);
                return null;
            }).Where(s => s != null).ToList();

            var json = JsonConvert.SerializeObject(shapesToSave, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        //wczytanie z pliku json
        private void LoadShapesFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                var loadedShapes = JsonConvert.DeserializeObject<List<MoveShape>>(json);

                if (loadedShapes != null)
                {
                    DrawingArea.Children.Clear();
                    shapes.Clear();

                    foreach (var moveShape in loadedShapes)
                    {
                        var shape = moveShape.RestoreShape();
                        if (shape != null)
                        {
                            moveShape.Shape = shape;
                            shapes.Add(moveShape);
                            DrawingArea.Children.Add(shape);
                        }
                    }
                }
            }
        }

        //obsługa przycisku zapisz
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveShapesToFile(saveFileDialog.FileName);
            }
        }
        //obsługa przycisku wczytaj
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                LoadShapesFromFile(openFileDialog.FileName);
            }
        }
    }
}
