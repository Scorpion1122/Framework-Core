using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKO.UI.Toolkit;
using UnityEngine.UIElements;

[StyleSheet("markdown-styles")]
[Template("markdown-template")]
public class MarkdownDocumentEditor : VisualElement
{
	#region Variables
#pragma warning disable 0649

	private readonly string _filePath;
	private readonly string _fileTemplate;
	private FileSystemWatcher _fileSystemWatcher;
	private DateTime _lastFileWriteTime;
		
	// Create File View
	[ChildView("scroll-view")] 
	private VisualElement _scrollView;
	[ChildView("file-missing")] 
	private VisualElement _missingChangelogContainer;
	[ChildView("create-file-button")] 
	private Button _createFileButton;
	
	// Preview View
	[ChildView("file-content")] 
	private VisualElement _previewContainer;
	[ChildView("edit-button")] 
	private Button _editButton;
	
#pragma warning restore 0649
	#endregion
		
	#region Lifecycle
		
	public MarkdownDocumentEditor(string filePath, string fileTemplate)
	{
		this.InitializeReferences();

		_filePath = filePath;
		_fileTemplate = fileTemplate;

		_createFileButton.clicked += OnCreateFileClicked;
		_editButton.clicked += OnEditClicked;
		
		schedule.Execute(RefreshFileIfNeeded).Every(1000);
	}

	#endregion
	
	#region Private Methods

	private void RefreshFileIfNeeded()
	{
		if(File.Exists(_filePath))
		{
			DateTime dateTime = File.GetLastWriteTimeUtc(_filePath);
			if(dateTime != _lastFileWriteTime)
			{
				ShowStyledDocumentView();
				_lastFileWriteTime = dateTime;
			}
		}
	}

	private void ShowStyledDocumentView()
	{
		_previewContainer.Clear();
		
		if(File.Exists(_filePath))
		{
			string[] lines = File.ReadAllLines(_filePath);
			ParseChangelogLines(lines);
			_previewContainer.visible = true;
			_scrollView.visible = true;
			_editButton.visible = true;
			_missingChangelogContainer.visible = false;
		}
		else
		{
			ShowMissingFileView();
		}
	}

	private void ShowMissingFileView()
	{
		_previewContainer.visible = false;
		_editButton.visible = false;
		_scrollView.visible = false;
		_missingChangelogContainer.visible = true;
	}

	private void OnEditClicked()
	{
		//I wish we could edit inside of unity, but there is a limit to how many characters can go in a text field
		System.Diagnostics.Process.Start(_filePath);
	}

	private void ParseChangelogLines(string[] lines)
	{
		for(int i = 0, length = lines.Length; i < length; i++)
		{
			if(TryParseEmptyLine(_previewContainer, lines[i]))
			{
				continue;
			}

			if(TryParseHeaderLine(_previewContainer, lines, i))
			{
				continue;
			}

			if(TryParseBlockQoute(_previewContainer, lines, ref i))
			{
				continue;
			}

            if(TryParseTable(_previewContainer, lines, ref i))
            {
                continue;
            }
            
            if(TryParseCodeBlock(_previewContainer, lines, ref i))
            {
	            continue;
            }

            TryParseTextLine(_previewContainer, lines[i]);
		}
	}

	private bool TryParseCodeBlock(VisualElement previewContainer, string[] lines, ref int i)
	{
		string line = lines[i].Trim();

		if(!line.Equals("```"))
		{
			return false;
		}
		
		VisualElement codeBlock = new VisualElement();
		codeBlock.AddToClassList("code-block");
		previewContainer.Add(codeBlock);

		for(i += 1; i < lines.Length; i++)
		{
			line = lines[i];
			if(line.Trim().Equals("```"))
			{
				break;
			}

			if(TryParseEmptyLine(codeBlock, lines[i]))
			{
				continue;
			}
			
			TryParseTextLine(codeBlock, lines[i]);
		}
		
		return true;
	}

	private bool TryParseTable(VisualElement root, string[] lines, ref int lineIndex)
    {
        List<List<string>> columns = new List<List<string>>();
        for(int length = lines.Length; lineIndex < length; lineIndex++)
        {
            string line = lines[lineIndex];
            if(!IsTableLine(line))
            {
                break;
            }
            
            string[] lineItems = line.Split('|');
            for(int j = 0, jLength = lineItems.Length; j < jLength; j++)
            {
                string lineItem = lineItems[j].Trim();

                if(columns.Count <= j)
                {
                    columns.Add(new List<string>() { lineItem });
                }
                else
                {
                    columns[j].Add(lineItem);
                }
            }
        }

        if(columns.Count == 0)
        {
            return false;
        }

        VisualElement table = new VisualElement();
        table.AddToClassList("table");
        root.Add(table);

        for(int i = 1, length = columns.Count - 1; i < length; i++)
        {
            VisualElement column = new VisualElement();
            column.AddToClassList("table-column");
            table.Add(column);

            bool didPassHeader = false;
            List<string> items = columns[i];
            for(int j = 0, jLength = items.Count; j < jLength; j++)
            {
	            string item = items[j];
	            if(!didPassHeader && string.IsNullOrEmpty(item.Replace("-", "").Trim()))
	            {
		            didPassHeader = true;
	            }
	            else if(!didPassHeader)
                {
                    Label headerElement = new Label(item);
                    headerElement.AddToClassList("table-header");
                    column.Add(headerElement);
                }
                else if(string.IsNullOrEmpty(item))
                {
	                VisualElement emptyLine = CreateLineElement(column);
	                emptyLine.Add(new Label(" "));
	                emptyLine.AddToClassList("table-item");
                }
                else
                {
                    TryParseTextLine(column, item, "table-item");
                }
            }
        }

        return true;
    }

    private bool IsTableLine(string line)
    {
        line = line.Trim();
        return line.StartsWith("|") && line.EndsWith("|");
    }

    private bool TryParseBlockQoute(VisualElement root, string[] lines, ref int lineIndex)
	{
		string line = lines[lineIndex];
		if(line[0] != '>')
		{
			return false;
		}
		
		VisualElement blockQuote = new VisualElement();
		blockQuote.AddToClassList("block-quote");
		
		while(!string.IsNullOrEmpty(line) && line[0] == '>')
		{
			line = line.Substring(1, line.Length - 1);
			if(!TryParseEmptyLine(blockQuote, line))
			{
				TryParseTextLine(blockQuote, line);
			}
			
			lineIndex++;
			if(lineIndex >= lines.Length)
			{
				break;
			}
			line = lines[lineIndex];
		}
		
		lineIndex--;
		root.Add(blockQuote);
		return true;
	}

	private bool TryParseEmptyLine(VisualElement root, string line)
	{
		if(string.IsNullOrEmpty(line))
		{
			VisualElement lineElement = new VisualElement();
			lineElement.AddToClassList("line-empty");
			root.Add(lineElement);

			return true;
		}

		return false;
	}

	private bool TryParseHeaderLine(VisualElement root, string[] lines, int lineIndex)
	{
		string line = lines[lineIndex];
		
		int headerIndex = 0;
		while(line[headerIndex] == '#')
		{
			headerIndex++;
		}

		if(headerIndex == 0)
		{
			return false;
		}
		
		VisualElement lineElement = CreateLineElement(root);
		Label label = new Label(line.Substring(headerIndex).Trim());
		label.AddToClassList($"header-{headerIndex}");
		lineElement.Add(label);
		return true;
	}

	private bool TryParseTextLine(VisualElement root, string line, string additionalClass = null)
	{
		line = line.Replace("\t", "    ");
		VisualElement lineElement = CreateLineElement(root);
        if(!string.IsNullOrEmpty(additionalClass))
        {
            lineElement.AddToClassList(additionalClass);
        }
        
		StringBuilder builder = new StringBuilder();
        for(int i = 0, length = line.Length; i < length; i++)
		{
			char character = line[i];
			if(character == '[' && TryParseLink(lineElement, builder, line, ref i))
			{
				continue;
			}

			if(character == '*' && (TryParseBoldText(lineElement, builder, line, ref i) || TryParseUnorderedListItem(lineElement, builder, line, ref i)))
			{
				continue;
			}

			// _italic_
			if(TryParseSingleQuoteSyntax(lineElement, builder, '_', "text-italic", line, ref i) )
			{
				continue;
			}

			// `code`
			if(TryParseSingleQuoteSyntax(lineElement, builder, '`', "code-line", line, ref i))
			{
				continue;
			}
			
			if(i == length - 1)
			{
				builder.Append(character);
				AddNormalText(lineElement, builder.ToString());
				builder.Clear();
			}
			else
			{
				builder.Append(character);
			}
		}

		return true;
	}

	// [name](url)
	private bool TryParseLink(VisualElement root, StringBuilder builder, string line, ref int characterIndex)
	{
		int urlNameEndIndex = line.IndexOf(']', characterIndex);
		int urlNameLength = urlNameEndIndex - characterIndex;

		if(urlNameLength < 2)
		{
			return false;
		}

		string urlName = line.Substring(characterIndex + 1, urlNameLength - 1);

		int urlStartIndex = line.IndexOf('(', urlNameEndIndex);
		if(urlStartIndex < 0 || urlStartIndex - urlNameEndIndex > 2)
		{
			return false;
		}

		int urlEndIndex = line.IndexOf(')', urlStartIndex);
		int urlLength = urlEndIndex - urlStartIndex;
		string url = line.Substring(urlStartIndex + 1, urlLength - 1);
		
		AddNormalText(root, builder);
		root.Add(new UrlText(urlName, url));

		characterIndex = urlEndIndex;
		
		return true;
	}

	// **bold**
	private bool TryParseBoldText(VisualElement root, StringBuilder builder, string line, ref int characterIndex)
	{
		int remainingLength = line.Length - characterIndex;
		if(remainingLength < 5 || line[characterIndex + 1] != '*')
		{
			return false;
		}

		int endIndex = line.IndexOf('*', characterIndex + 2);
		if(line[endIndex + 1] != '*')
		{
			return false;
		}

		int boldTextLength = endIndex - (characterIndex + 1) - 1;
		if(boldTextLength <= 0)
		{
			return false;
		}

		AddNormalText(root, builder);
		
		TextElement textElement = new TextElement();
		textElement.text = line.Substring(characterIndex + 2, boldTextLength);
		textElement.AddToClassList("text-bold");
		root.Add(textElement);

		characterIndex = endIndex + 1;
		return true;
	}

	//* some text
	//	* some indented text
	private bool TryParseUnorderedListItem(VisualElement root, StringBuilder builder, string line, ref int characterIndex)
	{
		if(line.Length - characterIndex < 2)
		{
			return false;
		}

		if(line[characterIndex + 1] != ' ')
		{
			return false;
		}

		AddNormalText(root, builder);
		VisualElement element = new VisualElement();

		int indentLevel = GetIndentLevel(line, characterIndex);
		if(indentLevel > 0)
		{
			element.AddToClassList($"unsorted-list-indent-{indentLevel}");
		}
		else
		{
			element.AddToClassList("unsorted-list");
		}
		root.Add(element);
		characterIndex += 1;
		
		return true;
	}

	private bool TryParseSingleQuoteSyntax(VisualElement root, StringBuilder builder, char key, string style, string line, ref int characterIndex)
	{
		if(line[characterIndex] != key)
		{
			return false;
		}
		
		int remainingLength = line.Length - characterIndex;
		if(remainingLength < 3 )
		{
			return false;
		}

		int endIndex = line.IndexOf(key, characterIndex + 1);
		int textLength = endIndex - (characterIndex + 1);
		if(textLength <= 0)
		{
			return false;
		}

		AddNormalText(root, builder);
		
		TextElement textElement = new TextElement();
		textElement.text = line.Substring(characterIndex + 1, textLength);
		textElement.AddToClassList(style);
		root.Add(textElement);

		characterIndex = endIndex;
		return true;
	}

	private int GetIndentLevel(string line, int characterIndex)
	{
		if(characterIndex == 0)
		{
			return 0;
		}

		int spaceCount = 0;
		int tabCount = 0;
		for(int i = characterIndex - 1; i >= 0; i--)
		{
			if(line[i] == '\t')
			{
				tabCount++;
			}

			if(line[i] == ' ')
			{
				spaceCount++;
			}
		}

		return spaceCount / 4 + tabCount;
	}
	
	private void AddNormalText(VisualElement root, StringBuilder builder)
	{
		string text = builder.ToString();
		if(!string.IsNullOrEmpty(text))
		{
			AddNormalText(root, text);
		}
		builder.Clear();
	}

	private void AddNormalText(VisualElement root, string text)
	{
		TextElement textElement = new TextElement();
		textElement.text = text;
		textElement.AddToClassList("text-normal");
		root.Add(textElement);
	}

	private VisualElement CreateLineElement(VisualElement root)
	{
		VisualElement lineElement = new VisualElement();
		lineElement.AddToClassList("line");
		root.Add(lineElement);
		return lineElement;
	}

	private void OnCreateFileClicked()
	{
		if(!File.Exists(_filePath))
		{
			FileStream fileStream = File.Create(_filePath);
			fileStream.Close();
			File.WriteAllText(_filePath, _fileTemplate);
		}
		ShowStyledDocumentView();
	}
	
	#endregion
}
