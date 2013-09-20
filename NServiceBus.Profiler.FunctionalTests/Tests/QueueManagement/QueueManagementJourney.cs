﻿using System.Linq;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.FunctionalTests.Screens;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.FunctionalTests.Tests.QueueManagement
{
    public class QueueManagementJourney : ProfilerTests
    {
        public IQueueManager QueueManager { get; set; }
        public ShellScreen Shell { get; set; }
        public QueueCreationDialog QueueCreationDialog { get; set; }

        [Test]
        public void Can_create_new_queues_and_see_them_in_the_queue_explorer()
        {
            var queueName = GetUniqueName("MyQueue");
            var expectedAddress = new Address(queueName);

            Shell.MainMenu.ToolsMenu.Click();
            Shell.MainMenu.CreateQueue.Click();
            
            QueueCreationDialog.Activate();
            QueueCreationDialog.QueueName.Text = queueName;
            QueueCreationDialog.Okay.Click();

            QueueManager.GetQueues().ShouldContain(q => q.Address == expectedAddress);


        }
    }
}