﻿using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;

namespace NServiceBus.Profiler.FunctionalTests
{
    public interface IMainWindow : IUIItemContainer
    {
        void Close();
        Window ModalWindow(string title);
    }
}