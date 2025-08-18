using Messenger.Tools;
using NUnit.Framework;

namespace NUnitTests;

[TestFixture]
public class TransliterationTests
{
    [TestCase("Bulba", "Бульба")]
    [TestCase("Schaste", "Счастье")]
    [TestCase("Dozhdlivaia pogoda", "Дождливая погода")]
    [TestCase("I have read a great book. — IA prochital zamechatelnuiu knigu", "I have read a great book. — Я прочитал замечательную книгу")]
    [TestCase("Lednik Eiiafiadlaiekiudl", "Ледник Эйяфьядлайёкюдль")]
    [Description("Testing Transliteration.Run method")]
    public void RunTest(string expected_result, string message_in)
    {
        // Assert
        Assert.AreEqual(expected_result, Transliteration.Run(message_in));
    }
}