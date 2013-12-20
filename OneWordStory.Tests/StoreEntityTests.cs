using System;
using NUnit.Framework;
using OneWordStory.Domain.Entities;


namespace OneWordStory.Tests
{
    [TestFixture]
    public class StoreEntityTests
    {
        [Test]
        public void HasEditor()
        {

            Story story = new Story();
            story.CurrentEditorId = "blahs";

            Assert.IsTrue(story.HasEditor);

            story = new Story();

            Assert.IsFalse(story.HasEditor);

        }

    }
}

