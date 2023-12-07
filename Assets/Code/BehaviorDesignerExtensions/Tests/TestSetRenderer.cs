using BehaviorDesigner.Runtime.Tasks;

using NUnit.Framework;

using SaloonSlingers.Unity.Tests;

using UnityEngine;
using UnityEngine.TestTools;

namespace SaloonSlingers.BehaviorDesignerExtensions.Tests
{
    [RequiresPlayMode]
    public class TestSetRendererColor
    {
        [Test]
        public void Fails_WhenNoRendererAvailable()
        {
            var task = new SetRendererColor();
            task.Color = Color.blue;

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void Fails_WhenMissingColor()
        {
            var task = new SetRendererColor();
            var renderer = new Renderer();
            task.Renderer = renderer;

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Failure));
        }

        [Test]
        public void Succeeds_WhenColorAndRendererAvailable()
        {
            var task = new SetRendererColor();
            var renderer = TestUtils.CreateComponent<LineRenderer>();
            var color = Color.red;
            task.Renderer = renderer;
            task.Color = color;

            Assert.That(task.OnUpdate(), Is.EqualTo(TaskStatus.Success));
        }

        [Test]
        public void SetsColor_WhenColorAndRendererAvailable()
        {
            var task = new SetRendererColor();
            var renderer = TestUtils.CreateComponent<LineRenderer>();
            var color = Color.red;
            task.Renderer = renderer;
            task.Color = color;
            task.OnUpdate();

            Assert.That(renderer.sharedMaterial.color, Is.EqualTo(color));
        }
    }
}
