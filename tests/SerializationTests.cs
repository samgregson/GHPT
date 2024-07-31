using GHPT.Prompts;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;

namespace UnitTests
{
    public class SerializationTests
    {

        private static PromptData GetTestPrompt()
        {
            IEnumerable<Components> additions = new List<Components>
                {
                    new Components() {
                        Name = "Number Slider",
                        Id = 1,
                        Value = "0..50..100"
                    },
                    new Components() {
                        Name = "Number Slider",
                        Id = 2,
                        Value = "0..90..360"
                    },
                    new Components()
                    {
                        Name = "Addition",
                        Id = 3,
                    }
                };

            IEnumerable<ConnectionPairing> connections = new List<ConnectionPairing>
                {
                    new ConnectionPairing()
                    {
                        To = new Connection()
                        {
                            Id = 1,
                            ParameterName = "number"
                        },
                        From = new Connection()
                        {
                            Id = 3,
                            ParameterName = "A"
                        },
                    },
                    new ConnectionPairing()
                    {
                        To = new Connection()
                        {
                            Id = 2,
                            ParameterName = "number"
                        },
                        From = new Connection()
                        {
                            Id = 3,
                            ParameterName = "B"
                        },
                    },
                };

            var promptData = new PromptData()
            {
                Advice = "Don't eat yellow snow",
                Components = additions,
                Connections = connections
            };

            return promptData;
        }

        [Test]
        public void Test1()
        {
            PromptData promptData = GetTestPrompt();
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            string json = JsonSerializer.Serialize(promptData, options);

            Assert.Pass();
        }
    }
}