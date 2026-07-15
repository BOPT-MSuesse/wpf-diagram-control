# WPF Diagram Control with Zoom and Panning

A reusable WPF control for displaying and interacting with diagrams, featuring smooth zooming and panning capabilities.

## Features

- **Zoom Controls**: Use toolbar buttons or Ctrl+MouseWheel to zoom (10%-500%)
- **Panning**: Hold Space and drag to pan across the diagram
- **Fit to View**: Auto-fit diagram to visible area
- **Reset View**: Return to original zoom level and position
- **Real-time Feedback**: Position and zoom level display in status bar
- **Sample Diagram**: Pre-loaded with example shapes and connections
- **Extensible API**: Public methods for adding custom shapes

## System Requirements

- **Visual Studio**: 2022 (v17.0 or later)
- **.NET Framework**: 4.5.1 or higher
- **Windows**: 7 SP1 or later
- **Platform**: x86, x64, or ARM64

## Usage

### Basic Usage in XAML

```xml
<Window xmlns:views="clr-namespace:DiagramControl.Views">
    <views:DiagramControl x:Name="MyDiagram"/>
</Window>
```

### Programmatic Usage in C#

```csharp
// Add custom shapes
var rectangle = new Rectangle 
{ 
    Width = 100, 
    Height = 50, 
    Fill = Brushes.LightBlue, 
    Stroke = Brushes.Black 
};
DiagramControl.AddShape(rectangle, 50, 50);

// Set zoom level
DiagramControl.SetZoomLevel(1.5);

// Get current zoom
double currentZoom = DiagramControl.GetZoomLevel();

// Clear diagram
DiagramControl.ClearDiagram();
```

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Ctrl + Mouse Wheel** | Zoom in/out |
| **Space + Drag** | Pan the diagram |
| **Zoom In Button** | Increase zoom by 10% |
| **Zoom Out Button** | Decrease zoom by 10% |
| **Fit to View** | Auto-fit all elements |
| **Reset** | Return to default view (100% zoom, position 0,0) |

## Zoom Limits

- **Minimum**: 10% (0.1x)
- **Maximum**: 500% (5.0x)
- **Default**: 100% (1.0x)
- **Increment**: 10% per zoom action

## Architecture

### DiagramControl.xaml / DiagramControl.xaml.cs
The main reusable user control containing:
- **Toolbar**: Zoom control buttons
- **Canvas**: Drawing surface for shapes (2000x2000 default)
- **ScrollViewer**: Viewport management with automatic scrollbars
- **Status Bar**: Real-time position and zoom feedback

### Key Public Methods

```csharp
// Add a UIElement at specified coordinates
public void AddShape(UIElement shape, double x, double y)

// Remove all shapes from diagram
public void ClearDiagram()

// Get current zoom level (0.1 to 5.0)
public double GetZoomLevel()

// Set zoom to specific level
public void SetZoomLevel(double zoom)
```

### Transform System

The control uses a `TransformGroup` with:
- **ScaleTransform**: Handles zoom in/out
- **TranslateTransform**: Handles panning (via ScrollViewer)

## Customization

### Extend the Control

```csharp
public class CustomDiagramControl : DiagramControl
{
    public void AddDiamondShape(double x, double y, string label)
    {
        var polygon = new Polygon
        {
            Points = new PointCollection 
            { 
                new Point(50, 0), 
                new Point(100, 50), 
                new Point(50, 100), 
                new Point(0, 50) 
            },
            Fill = Brushes.LightGreen,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        AddShape(polygon, x, y);
    }
}
```

### Modify Canvas Size

Edit `DiagramControl.xaml`:
```xml
<Canvas x:Name="DiagramCanvas" 
        Width="4000" 
        Height="4000"/>
```

### Change Styling

Update colors in `DiagramControl.xaml`:
```xml
<!-- Toolbar background -->
<StackPanel Background="#E8E8E8" .../>

<!-- Canvas background -->
<Canvas Background="#F9F9F9" .../>
```

### Adjust Zoom Increment

Modify in `DiagramControl.xaml.cs`:
```csharp
private const double ZoomIncrement = 0.15; // 15% per action
```

## Sample Diagram

The control includes a demo diagram with:
- 3 process boxes (blue, green, red)
- 1 decision diamond (yellow)
- 1 output box (gray)
- Connecting lines showing flow

## Performance Considerations

- **Small diagrams** (<100 shapes): Excellent performance
- **Medium diagrams** (100-1000 shapes): Good performance
- **Large diagrams** (1000+ shapes): Consider using `VisualCollection` with custom rendering for better performance

For very large diagrams:
```csharp
// Use DrawingVisual for better performance
var drawingVisual = new DrawingVisual();
var drawingContext = drawingVisual.RenderOpen();
// Draw multiple items at once
drawingContext.Close();
```

## Building & Running

### Prerequisites

- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET Framework 4.5.1 Developer Pack** (if not already installed)

### Build

Open the `.csproj` file in Visual Studio 2022 and build using:
- **Build** → **Build Solution** (or press Ctrl+Shift+B)

Or from command line:
```bash
msbuild DiagramControl.csproj /p:Configuration=Release
```

### Run

- Press **F5** in Visual Studio to run with debugging
- Press **Ctrl+F5** to run without debugging
- Or double-click the executable in `bin\Debug\` or `bin\Release\`

## Project Structure

```
wpf-diagram-control/
├── DiagramControl.xaml              # UI layout
├── DiagramControl.xaml.cs           # Control logic
├── MainWindow.xaml                  # Application window
├── MainWindow.xaml.cs               # Window code-behind
├── App.xaml                         # Application resources
├── App.xaml.cs                      # Application setup
├── Properties/
│   └── AssemblyInfo.cs              # Assembly metadata
├── DiagramControl.csproj            # Project configuration (VS2022)
└── README.md                        # This file
```

## Visual Studio 2022 Compatibility

- **ToolsVersion**: 17.0 (Visual Studio 2022)
- **LangVersion**: latest (C# 11 language features available)
- **Target Framework**: .NET Framework 4.5.1
- **Platform Target**: AnyCPU (supports x86, x64, and ARM64)

The project file is fully compatible with Visual Studio 2022 while maintaining .NET Framework 4.5.1 as the target runtime.

## License

Free to use and modify for your projects.

## Contributing

Feel free to fork, modify, and enhance this control for your needs!
