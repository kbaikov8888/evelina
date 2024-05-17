﻿using Avalonia.Controls;
using Avalonia.Controls.Templates;
using evelina.ViewModels.Common;
using System;

namespace evelina;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var name = data?.GetType().FullName?.Replace("ViewModel", "View");

        if (name != null)
        {
            var type = Type.GetType(name);
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}