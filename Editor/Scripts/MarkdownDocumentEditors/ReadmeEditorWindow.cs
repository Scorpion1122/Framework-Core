using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ReadmeEditorWindow : EditorWindow
{
	#region Consts

	private static readonly string ReadmeFileName = "README.md";
	
	#endregion
	
	[MenuItem("Help/Read Me")]
	private static void CreateEditor()
	{
		ReadmeEditorWindow window = GetWindow<ReadmeEditorWindow>(false, "Read Me");
		window.name = "Read Me";
		window.Show();
	}

	private void OnEnable()
	{
		rootVisualElement.Add(new MarkdownDocumentEditor(GetReadMePath(), GetReadMeTemplate()));
	}

	private string GetReadMePath()
	{
		string assetPath = Application.dataPath;
		string folderPath = assetPath.Remove(assetPath.LastIndexOf("/Assets", StringComparison.Ordinal));
		return Path.Combine(folderPath, ReadmeFileName);
	}

	private string GetReadMeTemplate()
	{
		return @"# Readme
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
