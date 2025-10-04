# TextEditor Component Documentation

## Overview
A professional, feature-rich text editor component built entirely in C# for Blazor Server applications. No JavaScript dependencies required.

## Features

### ✅ Complete C# Implementation
- 100% server-side rendering
- No JavaScript interop required
- Blazor Server compatible

### ✅ Rich Text Formatting
- **Bold** text (`**text**`)
- *Italic* text (`*text*`)
- <u>Underlined</u> text (`<u>text</u>`)
- Link insertion (`[text](url)`)

### ✅ List Support
- Unordered (bullet) lists
- Ordered (numbered) lists
- Auto-formatting for lists

### ✅ Multiple View Modes
- **Edit Mode**: Full editing capabilities
- **Preview Mode**: Rendered HTML preview
- **Split Mode**: Side-by-side edit and preview

### ✅ Professional Features
- Real-time statistics (lines, words, characters)
- Keyboard shortcuts (Ctrl+B, Ctrl+I, Ctrl+U)
- Tab indentation support
- Configurable toolbar
- Status messages
- Responsive design

## Usage

### Basic Usage
```razor
@using SpirithubCafe.Web.Components.Shared

<TextEditor @bind-Content="@myContent" />

@code {
    private string myContent = "";
}
```

### Advanced Usage
```razor
<TextEditor @bind-Content="@editorContent" 
           Placeholder="Start writing your content here..."
           MinHeight="400"
           DefaultMode="TextEditor.EditorMode.Split"
           ReadOnly="false" />

@code {
    private string editorContent = "# Welcome to TextEditor!";
}
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Content` | `string` | `""` | The text content (two-way binding) |
| `Placeholder` | `string` | `"Start writing..."` | Placeholder text when empty |
| `ReadOnly` | `bool` | `false` | Makes the editor read-only |
| `MinHeight` | `int` | `300` | Minimum height in pixels |
| `DefaultMode` | `EditorMode` | `Edit` | Initial display mode |

## Editor Modes

### EditorMode.Edit
- Full editing interface
- Toolbar with formatting buttons
- Text area for input

### EditorMode.Preview  
- Read-only preview of formatted content
- HTML rendering of markdown-like syntax
- Perfect for displaying content

### EditorMode.Split
- Side-by-side edit and preview
- Real-time preview updates
- Best for content creation

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl + B` | Bold text |
| `Ctrl + I` | Italic text |
| `Ctrl + U` | Underline text |
| `Tab` | Insert 4 spaces (indentation) |

## Supported Formatting

### Text Formatting
```
**Bold Text** → Bold Text
*Italic Text* → Italic Text  
<u>Underlined Text</u> → Underlined Text
```

### Links
```
[Link Text](https://example.com) → Clickable Link
```

### Lists
```
- Item 1
- Item 2
- Item 3

1. First item
2. Second item
3. Third item
```

## CSS Classes
The component uses Tailwind CSS for styling with the following class structure:
- `.text-editor-container`: Main container
- Responsive design with proper mobile support
- Professional color scheme with gray tones
- Hover effects and transitions

## Examples

### Read-Only Preview
```razor
<TextEditor Content="@staticContent" 
           ReadOnly="true"
           DefaultMode="TextEditor.EditorMode.Preview" />
```

### Compact Editor
```razor
<TextEditor @bind-Content="@notes" 
           MinHeight="150"
           Placeholder="Quick notes..." />
```

### Blog Post Editor
```razor
<TextEditor @bind-Content="@blogPost" 
           MinHeight="500"
           DefaultMode="TextEditor.EditorMode.Split"
           Placeholder="Write your blog post here..." />
```

## Status Bar Information
The status bar displays:
- **Lines**: Total number of lines
- **Words**: Word count
- **Characters**: Character count including spaces
- **Status Messages**: Temporary feedback messages

## Browser Compatibility
- Works in all modern browsers
- No JavaScript dependencies
- Server-side rendering ensures compatibility
- Mobile-responsive design

## Performance
- Lightweight implementation
- Efficient server-side processing
- Real-time updates without lag
- Optimized for large content

## Customization
The component can be extended by:
- Adding new formatting options
- Customizing the toolbar
- Extending keyboard shortcuts
- Adding custom CSS themes

## File Location
```
/SpirithubCafe.Web/Components/Shared/TextEditor.razor
```

## Demo Page
View the component in action at:
```
http://localhost:5000/text-editor-demo
```

## Dependencies
- Blazor Server (.NET 9.0)
- System.Text.RegularExpressions
- Tailwind CSS for styling

---

Created with ❤️ for SpirithubCafe
100% C# Implementation - No JavaScript Required