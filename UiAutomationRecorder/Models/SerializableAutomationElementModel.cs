using System;

namespace UiAutomationRecorder.Models;

/// <summary>
/// Represents a serializable model for automation elements.
/// </summary>
public record SerializableAutomationElementModel
{
    /// <summary>
    /// Gets or sets the name of the automation element.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the control type of the automation element.
    /// </summary>
    public string ControlType { get; set; }

    /// <summary>
    /// Gets or sets the automation ID of the automation element.
    /// </summary>
    public string AutomationId { get; set; }

    /// <summary>
    /// Gets or sets the bounding rectangle of the automation element.
    /// </summary>
    public string BoundingRectangle { get; set; }

    /// <summary>
    /// Gets or sets the class name of the automation element.
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// Gets or sets the action description (e.g., "ButtonClick", "TextEntry").
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the action.
    /// </summary>
    public DateTime Timestamp { get; set; }
}

