// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class DataGridViewForm : Form
{
    public DataGridViewForm()
    {
        InitializeComponent();

        dataGridView1.Rows.Add("aaa", "aa", true);
        dataGridView1.Rows.Add("bbb", "bb", false);
        dataGridView1.Rows.Add("ccc", "cc", false);

        var linkCell = dataGridView1.Rows[0].Cells[5] as DataGridViewLinkCell;
        if (linkCell is not null)
        {
            linkCell.Value = new Uri("http://www.bing.com");
        }

        linkCell = dataGridView1.Rows[1].Cells[5] as DataGridViewLinkCell;
        if (linkCell is not null)
        {
            linkCell.Value = new Uri("http://www.bing.com");
        }

        linkCell = dataGridView1.Rows[2].Cells[5] as DataGridViewLinkCell;
        if (linkCell is not null)
        {
            linkCell.Value = new Uri("http://www.bing.com");
        }

        var imageCell = dataGridView1.Rows[0].Cells[4] as DataGridViewImageCell;
        var bitmap = new Bitmap(8, 8);
        for(int x=0;x< 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                bitmap.SetPixel(x, y, (x + y) % 2 == 0 ? Color.Black : Color.White);
            }
        }

        if (imageCell is not null)
        {
            imageCell.Value = bitmap;
        }

        imageCell = dataGridView1.Rows[1].Cells[4] as DataGridViewImageCell;
        if (imageCell is not null)
        {
            imageCell.Value = bitmap;
        }

        imageCell = dataGridView1.Rows[2].Cells[4] as DataGridViewImageCell;
        if (imageCell is not null)
        {
            imageCell.Value = bitmap;
        }
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true or == false
// 0:014> !DumpHeap -type DataGridView
//         Address MT     Size
// 000001f9800e6858 00007ffc37d74fe8       24     
// 000001f9800e6e58 00007ffc37e12548       24     
// Statistics:
//              MT Count    TotalSize Class Name
// 00007ffc37e12548        1           24 System.Windows.Forms.DataGridViewColumnCollection+ColumnOrderComparer
// 00007ffc37d74fe8        1           24 System.Windows.Forms.DataGridViewElementStates[]
// Total 2 objects
//
// DataGridView DataGridView+DataGridViewTopRowAccessibleObject InternalAccessibleObject [RefCount Handle, Count: 1]
//
// DataGridView DataGridViewTopLeftHeaderCell DataGridViewTopLeftHeaderCell+DataGridViewTopLeftHeaderCellAccessibleObject InternalAccessibleObject [RefCount Handle, Count: 1]
//
// DataGridView DataGridViewTextBoxEditingControl TextBoxBase+TextBoxBaseUiaTextProvider UiaTextRange [RefCount Handle, Count: 1]
//
// DataGridViewRowHeaderCell DataGridViewRowHeaderCell+DataGridViewRowHeaderCellAccessibleObject [RefCount Handle, Count: 4]
//

// DataGridView DataGridViewComboBoxEditingControl ComboBox+ComboBoxItemAccessibleObject [RefCount Handle, Count: 3]
