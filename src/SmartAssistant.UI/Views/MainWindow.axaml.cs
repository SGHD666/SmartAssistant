// <copyright file="MainWindow.axaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SmartAssistant.UI.Views;

using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using SmartAssistant.UI.ViewModels;

public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ILogger<MainWindow> logger)
    {
        _logger = logger;
        _logger.LogDebug("MainWindow constructor started");
        InitializeComponent();
        _logger.LogDebug("MainWindow InitializeComponent completed");
        
        DataContextChanged += MainWindow_DataContextChanged;
        _logger.LogDebug("MainWindow DataContextChanged event handler registered");

        Opened += MainWindow_Opened;
        _logger.LogDebug("MainWindow Opened event handler registered");
        
        _logger.LogDebug("MainWindow constructor completed");
    }

    private void MainWindow_Opened(object? sender, EventArgs e)
    {
        _logger.LogDebug("MainWindow_Opened event fired");
        this.Show();
        this.Activate();
        this.Focus();
        _logger.LogDebug("MainWindow visibility methods called in Opened event");
    }

    private void MainWindow_DataContextChanged(object? sender, System.EventArgs e)
    {
        _logger.LogDebug("MainWindow_DataContextChanged event fired");
        if (DataContext is MainWindowViewModel vm)
        {
            _logger.LogDebug("DataContext is MainWindowViewModel");
            if (vm.Messages is INotifyCollectionChanged notifyCollection)
            {
                notifyCollection.CollectionChanged += Messages_CollectionChanged;
                _logger.LogDebug("Messages CollectionChanged event handler registered");
            }
        }
        else
        {
            _logger.LogWarning("DataContext is not MainWindowViewModel, it is: {DataContextType}", DataContext?.GetType().FullName ?? "null");
        }
    }

    private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _logger.LogDebug("Messages collection changed: {Action}", e.Action);
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            _logger.LogDebug("New message added, scrolling to end");
            ChatScrollViewer?.ScrollToEnd();
            _logger.LogDebug("Scrolled to end");
        }
    }
}
