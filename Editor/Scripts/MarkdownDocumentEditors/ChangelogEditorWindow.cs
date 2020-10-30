using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ChangelogEditorWindow : EditorWindow
{
	#region Consts

	private static readonly string ChangelogFileName = "CHANGELOG.md";

	#endregion

	[MenuItem("Help/Changelog")]
	private static void CreateEditor()
	{
		ChangelogEditorWindow window = GetWindow<ChangelogEditorWindow>(false, "Changelog");
		window.Show();
	}

	private void OnEnable()
	{
		rootVisualElement.Add(new MarkdownDocumentEditor(GetChangelogPath(), GetChangelogTemplate()));
	}

	private string GetChangelogPath()
	{
		string assetPath = Application.dataPath;
		string folderPath = assetPath.Remove(assetPath.LastIndexOf("/Assets", StringComparison.Ordinal));
		return Path.Combine(folderPath, ChangelogFileName);
	}

	private string GetChangelogTemplate()
	{
		return @"# Change log
## Unreleased
* [GOOGLE](https://www.google.com): This is a url with some text afterwards
* This feature was so **important** It was made bold
* However this one is _made_ italic
* Here we have a list item with
    * sub
		* items
        
> Here we test a block quote with one line

> Here we test a block quote with
> multiple lines

## v0.0.1:
* Some feature for this version;";
	}
}
