using Messenger.Tools;
using NUnit.Framework;

namespace NUnitTests;

[TestFixture]
public class TransliteratorTests
{
    [TestCase("Bulba", "Бульба")]
    [TestCase("Schaste", "Счастье")]
    [TestCase("Dozhdlivaia pogoda", "Дождливая погода")]
    [TestCase("I have read a great book. — Ia prochital zamechatelnuiu knigu", "I have read a great book. — Я прочитал замечательную книгу")]
    [TestCase("Lednik Eiiafiadlaiekiudl", "Ледник Эйяфьядлайёкюдль")]
    public void RunTest(string expected_result, string message_in)
    {
        // Assert
        Assert.AreEqual(expected_result, Transliterator.ToLatin(message_in));
    }
}