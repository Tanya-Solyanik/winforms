﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

/// <summary>
///  Represents a Windows up-down control that displays string values.
/// </summary>
[DefaultProperty(nameof(Items))]
[DefaultEvent(nameof(SelectedItemChanged))]
[DefaultBindingProperty(nameof(SelectedItem))]
[SRDescription(nameof(SR.DescriptionDomainUpDown))]
public partial class DomainUpDown : UpDownBase
{
    private static readonly string s_defaultValue = string.Empty;

    /// <summary>
    ///  Allowable strings for the domain updown.
    /// </summary>
    private DomainUpDownItemCollection? _domainItems;

    private string _stringValue = s_defaultValue;      // Current string value
    private int _domainIndex = -1;                    // Index in the domain list
    private bool _sorted;                 // Sort the domain values

    private EventHandler? _onSelectedItemChanged;

    private bool _inSort;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DomainUpDown"/> class.
    /// </summary>
    public DomainUpDown() : base()
    {
        // this class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);
        Text = string.Empty;
    }

    // Properties

    /// <summary>
    ///  Gets the collection of objects assigned to the
    ///  up-down control.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [SRDescription(nameof(SR.DomainUpDownItemsDescr))]
    [Localizable(true)]
    [Editor($"System.Windows.Forms.Design.StringCollectionEditor, {Assemblies.SystemDesign}", typeof(UITypeEditor))]
    public DomainUpDownItemCollection Items => _domainItems ??= new DomainUpDownItemCollection(this);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the index value of the selected item.
    /// </summary>
    [Browsable(false)]
    [DefaultValue(-1)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.DomainUpDownSelectedIndexDescr))]
    public int SelectedIndex
    {
        get
        {
            if (UserEdit)
            {
                return -1;
            }
            else
            {
                return _domainIndex;
            }
        }

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, Items.Count);

            if (value != SelectedIndex)
            {
                SelectIndex(value);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the selected item based on the index value
    ///  of the selected item in the
    ///  collection.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.DomainUpDownSelectedItemDescr))]
    public object? SelectedItem
    {
        get
        {
            int index = SelectedIndex;
            return (index == -1) ? null : Items[index];
        }
        set
        {
            // Treat null as selecting no item
            if (value is null)
            {
                SelectedIndex = -1;
            }
            else
            {
                // Attempt to find the given item in the list of items
                for (int i = 0; i < Items.Count; i++)
                {
                    if (value.Equals(Items[i]))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the item collection is sorted.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.DomainUpDownSortedDescr))]
    public bool Sorted
    {
        get
        {
            return _sorted;
        }

        set
        {
            _sorted = value;
            if (_sorted)
            {
                SortDomainItems();
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Gets or sets a value indicating whether the collection of items continues to
    ///  the first or last item if the user continues past the end of the list.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.DomainUpDownWrapDescr))]
    public bool Wrap { get; set; }

    /// <summary>
    ///  Occurs when the <see cref="SelectedItem"/> property has
    ///  been changed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.DomainUpDownOnSelectedItemChangedDescr))]
    public event EventHandler? SelectedItemChanged
    {
        add => _onSelectedItemChanged += value;
        remove => _onSelectedItemChanged -= value;
    }

    /// <summary>
    ///  Displays the next item in the object collection.
    /// </summary>
    public override void DownButton()
    {
        // Make sure domain values exist, and there are >0 items
        if (_domainItems is null)
        {
            return;
        }

        if (_domainItems.Count <= 0)
        {
            return;
        }

        // If the user has entered text, attempt to match it to the domain list
        //
        int matchIndex = -1;
        if (UserEdit)
        {
            matchIndex = MatchIndex(Text, false, _domainIndex);
        }

        if (matchIndex != -1)
        {
            // Found a match, so select this value
            _domainIndex = matchIndex;
            SelectIndex(matchIndex);
        }
        else
        {
            // Otherwise, get the next string in the domain list
            if (_domainIndex < _domainItems.Count - 1)
            {
                SelectIndex(_domainIndex + 1);
            }
            else if (Wrap)
            {
                SelectIndex(0);
            }
        }
    }

    /// <summary>
    ///  Tries to find a match of the supplied text in the domain list.
    ///  If complete is true, a complete match is required for success
    ///  (i.e. the supplied text is the same length as the matched domain value)
    ///  Returns the index in the domain list if the match is successful,
    ///  returns -1 otherwise.
    /// </summary>
    internal int MatchIndex(string text, bool complete)
    {
        return MatchIndex(text, complete, _domainIndex);
    }

    internal int MatchIndex(string text, bool complete, int startPosition)
    {
        // Make sure domain values exist
        if (_domainItems is null)
        {
            return -1;
        }

        // Sanity check of parameters
        if (text.Length < 1)
        {
            return -1;
        }

        if (_domainItems.Count <= 0)
        {
            return -1;
        }

        if (startPosition < 0)
        {
            startPosition = _domainItems.Count - 1;
        }

        if (startPosition >= _domainItems.Count)
        {
            startPosition = 0;
        }

        // Attempt to match the supplied string text with
        // the domain list. Returns the index in the list if successful,
        // otherwise returns -1.
        int index = startPosition;
        int matchIndex = -1;
        bool found;

        if (!complete)
        {
            text = text.ToUpper(CultureInfo.InvariantCulture);
        }

        // Attempt to match the string with Items[index]
        do
        {
            found = complete
                ? Items[index]!.ToString()!.Equals(text)
                : Items[index]!.ToString()!.ToUpper(CultureInfo.InvariantCulture)
                    .StartsWith(text, StringComparison.Ordinal);

            if (found)
            {
                matchIndex = index;
            }

            // Calculate the next index to attempt to match
            index++;
            if (index >= _domainItems.Count)
            {
                index = 0;
            }
        }
        while (!found && index != startPosition);

        return matchIndex;
    }

    /// <summary>
    ///  In the case of a DomainUpDown, the handler for changing
    ///  values is called OnSelectedItemChanged - so just forward it to that
    ///  function.
    /// </summary>
    protected override void OnChanged(object? source, EventArgs e)
    {
        OnSelectedItemChanged(source, e);
    }

    /// <summary>
    ///  Handles the <see cref="Control.KeyPress"/>
    ///  event, using the input character to find the next matching item in our
    ///  item collection.
    /// </summary>
    protected override void OnTextBoxKeyPress(object? source, KeyPressEventArgs e)
    {
        if (ReadOnly)
        {
            char[] character = [e.KeyChar];
            UnicodeCategory uc = char.GetUnicodeCategory(character[0]);

            if (uc is UnicodeCategory.LetterNumber
                or UnicodeCategory.LowercaseLetter
                or UnicodeCategory.DecimalDigitNumber
                or UnicodeCategory.MathSymbol
                or UnicodeCategory.OtherLetter
                or UnicodeCategory.OtherNumber
                or UnicodeCategory.UppercaseLetter)
            {
                // Attempt to match the character to a domain item
                int matchIndex = MatchIndex(new string(character), false, _domainIndex + 1);
                if (matchIndex != -1)
                {
                    // Select the matching domain item
                    SelectIndex(matchIndex);
                }

                e.Handled = true;
            }
        }

        base.OnTextBoxKeyPress(source, e);
    }

    /// <summary>
    ///  Raises the <see cref="SelectedItemChanged"/> event.
    /// </summary>
    protected void OnSelectedItemChanged(object? source, EventArgs e)
    {
        // Call the event handler
        _onSelectedItemChanged?.Invoke(this, e);
    }

    /// <summary>
    ///  Selects the item in the domain list at the given index
    /// </summary>
    private void SelectIndex(int index)
    {
        // Sanity check index

        Debug.Assert(_domainItems is not null, "Domain values array is null");
        Debug.Assert(index < _domainItems.Count && index >= -1, "SelectValue: index out of range");
        if (_domainItems is null || index < -1 || index >= _domainItems.Count)
        {
            return;
        }

        // If the selected index has changed, update the text
        _domainIndex = index;
        if (_domainIndex >= 0)
        {
            _stringValue = _domainItems[_domainIndex]!.ToString()!;
            UserEdit = false;
            UpdateEditText();
        }
        else
        {
            UserEdit = true;
        }

        Debug.Assert(_domainIndex >= 0 || UserEdit, $"UserEdit should be true when domainIndex < 0 {UserEdit}");
    }

    /// <summary>
    ///  Sorts the domain values
    /// </summary>
    private void SortDomainItems()
    {
        if (_inSort)
        {
            return;
        }

        _inSort = true;
        try
        {
            // Sanity check
            Debug.Assert(_sorted, "Sorted == false");

            if (_domainItems is not null)
            {
                // Sort the domain values
                ArrayList.Adapter(_domainItems).Sort(new DomainUpDownItemCompare());

                // Update the domain index
                if (!UserEdit)
                {
                    int newIndex = MatchIndex(_stringValue, true);
                    if (newIndex != -1)
                    {
                        SelectIndex(newIndex);
                    }
                }
            }
        }
        finally
        {
            _inSort = false;
        }
    }

    /// <summary>
    ///  Provides some interesting info about this control in String form.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();

        if (Items is not null)
        {
            s = $"{s}, Items.Count: {Items.Count}, SelectedIndex: {SelectedIndex}";
        }

        return s;
    }

    /// <summary>
    ///  Displays the previous item in the collection.
    /// </summary>
    public override void UpButton()
    {
        // Make sure domain values exist, and there are >0 items
        if (_domainItems is null)
        {
            return;
        }

        if (_domainItems.Count <= 0)
        {
            return;
        }

        // If the user has entered text, attempt to match it to the domain list
        int matchIndex = -1;
        if (UserEdit)
        {
            matchIndex = MatchIndex(Text, false, _domainIndex);
        }

        if (matchIndex != -1)
        {
            // Found a match, so set the domain index accordingly
            _domainIndex = matchIndex;
            SelectIndex(matchIndex);
        }
        else
        {
            // Otherwise, get the previous string in the domain list
            if (_domainIndex > 0)
            {
                SelectIndex(_domainIndex - 1);
            }
            else if (Wrap)
            {
                SelectIndex(_domainItems.Count - 1);
            }
        }
    }

    /// <summary>
    ///  Updates the text in the up-down control to display the selected item.
    /// </summary>
    protected override void UpdateEditText()
    {
        UserEdit = false;
        ChangingText = true;
        Text = _stringValue;
    }

    // This is not a breaking change -- Even though this control previously autosized to height,
    // it didn't actually have an AutoSize property. The new AutoSize property enables the
    // smarter behavior.
    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        int height = PreferredHeight;
        int width = LayoutUtils.OldGetLargestStringSizeInCollection(Font, Items).Width;

        // AdjustWindowRect with our border, since textbox is borderless.
        width = SizeFromClientSizeInternal(new(width, height)).Width + _upDownButtons.Width;
        return new Size(width, height) + Padding.Size;
    }
}
