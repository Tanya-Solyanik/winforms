// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    private readonly TreeView _treeView1;

    public Form1()
    {
        InitializeComponent();
        _treeView1 = new()
        {
            Size = new Size(80, 200),
            Location = new Point(10,10)
        };

        Controls.Add(_treeView1);

        _treeView1.BeginUpdate();
        _treeView1.Nodes.Add(new TreeNode("root"));
        _treeView1.Nodes[0].Nodes.Add(new TreeNode("child"));
        _treeView1.Nodes[0].Expand();
        _treeView1.EndUpdate();

        Repro();
    }

    private void Repro()
    {
        Load += (s, e) =>
        {
            NoRepro();
        };
    }

    private void NoRepro()
    {
        TreeNode a0 = new("a0");
        TreeNode b0 = new("b0");
        TreeNode c0 = new("c0");
        _treeView1.Nodes[0].Nodes.AddRange([a0, b0, c0]);

        TreeNode a1 = new("a1");
        TreeNode b1 = new("b1");
        TreeNode c1 = new("c1");
        _treeView1.Nodes[0].Nodes.AddRange([a1, b1, c1]);
    }
}
